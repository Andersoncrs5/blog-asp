using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.utils.Responses;
using Blog.DTOs.RecoverAccount;
using Blog.DTOs.User;
using Blog.entities;
using Blog.SetServices.IServices;
using Blog.SetUnitOfWork;
using Blog.utils;
using Blog.utils.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _uow;

        public AuthController(
            ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unit,
            IConfiguration configuration)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _uow = unit;
        }

        [HttpPost("login")]
        [EnableRateLimiting("authSystemPolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string email = dto.Email.Trim();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and Password must be provided.");

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 200,
                    Datetime = DateTimeOffset.Now,
                    Message = "Login invalid"
                });
            }

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 200,
                    Datetime = DateTimeOffset.Now,
                    Message = "Login invalid"
                });
            }

            IList<string>? userRoles = await _userManager.GetRolesAsync(user);

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrWhiteSpace(user.Email))
                return BadRequest("Email is null");

            List<Claim>? authClaim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Sid, user.Id!),
                new Claim(ClaimTypes.Email, user.Email.Trim()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaim.Add(new Claim(ClaimTypes.Role, userRole));
            }

            JwtSecurityToken? token = _tokenService.GenerateAccessToken(authClaim, _configuration);
            string? refreshToken = _tokenService.GenerateRefreshToken();

            if (token is null || string.IsNullOrWhiteSpace(refreshToken))
            {
                return StatusCode(500, new ResponseBody<string>
                {
                    Body = null,
                    Code = 200,
                    Datetime = DateTimeOffset.Now,
                    Message = "Error the geneate token o refresh token",
                    Status = false
                });
            }

            _ = int.TryParse(_configuration["jwt:RefreshTokenValidityInMinutes"], out int refreshTokenValidity);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidity);
            await _userManager.UpdateAsync(user);

            UserMetricEntity? metric = await _uow.UserMetricRepository.Get(user.Id);
            if (metric == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "User metric not found",
                    Status = false
                });
            }

            await _uow.UserMetricRepository.SetLastLogin(metric);

            ResponseTokens tokens = new ResponseTokens
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = user.RefreshToken,
                ExpiredAt = token.ValidTo,
                ExpiredAtRefreshToken = user.RefreshTokenExpiryTime
            };

            return Ok(new ResponseBody<ResponseTokens>
            {
                Body = tokens,
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Message = "Welcome",
                Status = false
            });
        }

        [HttpPost("register")]
        [EnableRateLimiting("authSystemPolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityRole? roleUser = await _roleManager.FindByNameAsync("UserRole");

            if (roleUser == null || string.IsNullOrWhiteSpace(roleUser.Name))
            {
                return StatusCode(500, new ResponseBody<string>
                {
                    Body = null,
                    Code = 500,
                    Message = "Role user not found",
                    Datetime = DateTimeOffset.Now,
                    Status = false
                });
            }

            string email = model.Email.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Email and Password must be provided.");

            ApplicationUser? checkEmailExists = await _userManager.FindByEmailAsync(email);
            if (checkEmailExists is not null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new ResponseBody<string>
                {
                    Body = null,
                    Code = 409,
                    Datetime = DateTimeOffset.Now,
                    Message = $"User already exists with email {email}",
                    Status = false
                });
            }

            ApplicationUser user = new ApplicationUser
            {
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Name
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                IEnumerable<string> errors = result.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseBody<IEnumerable<string>>
                {
                    Message = "Error creating new user",
                    Body = errors,
                    Code = 400,
                    Datetime = DateTimeOffset.Now,
                    Status = false
                });
            }

            await _uow.UserMetricRepository.Create(user.Id);

            ApplicationUser? userCreated = await _uow.UserRepository.Get(user.Id);
            if (userCreated == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "User not found",
                    Status = false
                });
            }

            var addRoleResult = await _userManager.AddToRoleAsync(userCreated, roleUser.Name);
            if (!addRoleResult.Succeeded)
            {
                IEnumerable<string> errors = addRoleResult.Errors.Select(e => e.Description);
                await _uow.UserRepository.Delete(userCreated);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBody<IEnumerable<string>>
                {
                    Message = "Error adding roles",
                    Body = errors,
                    Code = 500,
                    Datetime = DateTimeOffset.Now,
                    Status = false
                });
            }


            IList<string> userRoles = await _userManager.GetRolesAsync(userCreated);

            List<Claim> authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userCreated.UserName!),
                new Claim(ClaimTypes.Sid, userCreated.Id!),
                new Claim(ClaimTypes.Email, userCreated.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            JwtSecurityToken? token = _tokenService.GenerateAccessToken(authClaims, _configuration);
            string? refreshToken = _tokenService.GenerateRefreshToken();
            if (token is null || string.IsNullOrWhiteSpace(refreshToken))
            {
                return StatusCode(500, new ResponseBody<string>
                {
                    Body = null,
                    Code = 500,
                    Datetime = DateTimeOffset.Now,
                    Message = "Error generating token or refresh token",
                    Status = false
                });
            }

            _ = int.TryParse(_configuration["jwt:RefreshTokenValidityInMinutes"], out int refreshTokenValidity);

            userCreated.RefreshToken = refreshToken;
            userCreated.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidity);
            await _userManager.UpdateAsync(userCreated);

            ResponseTokens tokens = new ResponseTokens
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = userCreated.RefreshToken,
                ExpiredAt = token.ValidTo,
                ExpiredAtRefreshToken = userCreated.RefreshTokenExpiryTime
            };

            return Ok(new ResponseBody<ResponseTokens>
            {
                Body = tokens,
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Message = "User registered successfully",
                Status = true
            });
        }

        [HttpPost("refresh-token")]
        [EnableRateLimiting("authSystemPolicy")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (tokenModel is null || string.IsNullOrWhiteSpace(tokenModel.AcessToken) || string.IsNullOrWhiteSpace(tokenModel.RefreshToken))
                return BadRequest("Invalid client request");

            ClaimsPrincipal? main = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AcessToken, _configuration);

            if (main == null)
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "Invalid acess token ou refresh token",
                    Datetime = DateTimeOffset.Now,
                    Status = false
                });
            }

            string? username = main.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Message = "Invalid token data",
                    Code = 400,
                    Datetime = DateTimeOffset.Now,
                    Status = false
                });
            }

            ApplicationUser? user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "Invalid acess token ou refresh token",
                    Datetime = DateTimeOffset.Now,
                    Status = false
                });
            }

            JwtSecurityToken? newAccessToken = _tokenService.GenerateAccessToken(main.Claims.ToList(), _configuration);
            string? newRefreshToken = _tokenService.GenerateRefreshToken();

            if (newAccessToken is null || string.IsNullOrWhiteSpace(newRefreshToken))
            {
                return StatusCode(500, new ResponseBody<string>
                {
                    Message = "Error the geneate token o refresh token",
                    Body = null,
                    Code = 500,
                    Datetime = DateTimeOffset.Now,
                    Status = false
                });
            }

            _ = int.TryParse(_configuration["jwt:RefreshTokenValidityInMinutes"], out int refreshTokenValidity);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidity);
            await _userManager.UpdateAsync(user);

            ResponseTokens tokens = new ResponseTokens
            {
                Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken,
                ExpiredAt = newAccessToken.ValidTo,
            };

            return Ok(new ResponseBody<ResponseTokens>
            {
                Body = tokens,
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Message = "User registered successfully",
                Status = true
            });

        }

        [HttpPost]
        [Route("revoke")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(ResponseBody<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseBody<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseBody<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBody<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Revoke()
        {
            try
            {
                string? id = User.FindFirst(ClaimTypes.Sid)?.Value;
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Unauthorized(new ResponseBody<string>
                    {
                        Body = null,
                        Message = "you are not logged in",
                        Status = false,
                        Datetime = DateTimeOffset.Now,
                        Code = 401,
                    });
                }

                ApplicationUser? user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ResponseBody<string>
                    {
                        Body = null,
                        Message = "User not found",
                        Status = false,
                        Datetime = DateTimeOffset.Now,
                        Code = 404,
                    });
                }

                user.RefreshToken = null;

                await _userManager.UpdateAsync(user);

                return StatusCode(StatusCodes.Status200OK, new ResponseBody<string>
                {
                    Body = null,
                    Message = "Bye bye",
                    Status = true,
                    Datetime = DateTimeOffset.Now,
                    Code = 200,
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBody<string>
                {
                    Body = e.Message,
                    Message = "Error the to revoke! Please try again later",
                    Status = false,
                    Datetime = DateTimeOffset.Now,
                    Code = StatusCodes.Status500InternalServerError,
                });
            }
        }

        [HttpPost("request-password-reset")]
        [EnableRateLimiting("authSystemPolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDto requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string? userAgent = Request.Headers["User-Agent"].ToString();

            requestDto.RequestIpAddress = ipAddress;
            requestDto.RequestUserAgent = userAgent;

            string? resetPageUrl = _configuration["FrontendSettings:PasswordResetUrl"];
            if (string.IsNullOrWhiteSpace(resetPageUrl))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBody<string>
                {
                    Body = null,
                    Message = "you are not logged in",
                    Status = false,
                    Datetime = DateTimeOffset.Now,
                    Code = StatusCodes.Status500InternalServerError,
                });
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(requestDto.Email);
            if (user == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            await _uow.RecoverAccountRepository.RequestPasswordResetTokenAsync(requestDto, resetPageUrl, user);

            return Ok(new ResponseBody<string>
            {
                Message = "If an account exists with that email, a password reset link has been sent.",
                Body = null,
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Status = true,
            });
        }

        [HttpPost("reset-password")]
        [EnableRateLimiting("authSystemPolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (resetDto.NewPassword != resetDto.ConfirmPassword)
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "New Password and Confirm Password not match!",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? user = await _userManager.FindByIdAsync(resetDto.UserId);
            if (user == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            RecoverAccountEntity? recoveryEntry = await _uow.RecoverAccountRepository.GetAsync(user);
            if (recoveryEntry is null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "RecoverAccount Entity not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            bool success = await _uow.RecoverAccountRepository.ValidateAndResetPasswordAsync(resetDto, user, recoveryEntry);

            if (success)
            {
                return Ok(new ResponseBody<string>
                {
                    Message = "Password has been reset successfully.",
                    Body = null,
                    Code = 200,
                    Datetime = DateTimeOffset.Now,
                    Status = true,
                });
            }

            return Ok(new ResponseBody<string>
            {
                Message = "Password reset failed.",
                Body = null,
                Code = 400,
                Datetime = DateTimeOffset.Now,
                Status = false,
            });
        }
    }
}
