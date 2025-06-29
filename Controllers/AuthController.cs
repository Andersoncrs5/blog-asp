using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            string email = dto.Email.Trim();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and Password must be provided.");

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Unauthorized();
            
            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized();

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
                return StatusCode(500, new ResponseException("Error the geneate token o refresh token"));

            _ = int.TryParse(_configuration["jwt:RefreshTokenValidityInMinutes"], out int refreshTokenValidity);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidity);
            await _userManager.UpdateAsync(user);

            UserMetricEntity metric = await _uow.UserMetricRepository.Get(user.Id);
            await _uow.UserMetricRepository.SetLastLogin(metric);

            return Ok(new ResponseTokens(
                new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken,
                token.ValidTo,
                200 
            ));
        }

        [HttpPost("register")]
        [EnableRateLimiting("authSystemPolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityRole? roleUser = await _roleManager.FindByNameAsync("UserRole");

            if (roleUser == null)
                return StatusCode(500,new ResponseException("Role user not found", 500));

            if (string.IsNullOrEmpty(roleUser.Name) || string.IsNullOrWhiteSpace(roleUser.Name))
                return StatusCode(500,new ResponseException("Role user not found", 500));

            string email = model.Email.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Email and Password must be provided.");

            ApplicationUser? checkEmailExists = await _userManager.FindByEmailAsync(email);
            if (checkEmailExists is not null )
                return StatusCode(StatusCodes.Status409Conflict, new ResponseException($"User already exists with email {email}", 409));

            ApplicationUser? user = new ApplicationUser
            {
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Name
            };

            IdentityResult? result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseException(
                    "Error the create new user",
                    400,
                    "fail",
                    errors
                ));
            }

            await _uow.UserMetricRepository.Create(user.Id);

            ApplicationUser userCreated = await _uow.UserRepository.Get(user.Id);

            var addRoleResult = await _userManager.AddToRoleAsync(userCreated, roleUser.Name);

            if (!addRoleResult.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                await _uow.UserRepository.Delete(userCreated);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseException(
                    "Error the set role to user",
                    StatusCodes.Status500InternalServerError,
                    "fail",
                    errors
                ));
            }

            // await this._uow.EmailService.SendWelcomeEmailAsync(user.Email, user.UserName);

            return Ok(new Response(
                "success",
                "User created with success",
                201,
                user
            ));
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
                return BadRequest(new ResponseException("Invalid acess token ou refresh token",400,"fail"));

            string? username = main.Identity?.Name;

            if(string.IsNullOrWhiteSpace(username))
                return BadRequest(new ResponseException("Invalid token data",400,"fail"));

            ApplicationUser? user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest(new ResponseException("Invalid access token or refresh token",400,"fail"));

            JwtSecurityToken? newAccessToken = _tokenService.GenerateAccessToken(main.Claims.ToList(), _configuration);
            string? newRefreshToken = _tokenService.GenerateRefreshToken();

            if (newAccessToken is null || string.IsNullOrWhiteSpace(newRefreshToken))
                return StatusCode(500, new ResponseException("Error the geneate token o refresh token"));

            _ = int.TryParse(_configuration["jwt:RefreshTokenValidityInMinutes"], out int refreshTokenValidity);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidity);
            await _userManager.UpdateAsync(user);

            return Ok(new ResponseTokens(
                new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                newRefreshToken,
                newAccessToken.ValidTo,
                200 
            ));

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("revoke/{email}")]
        [EnableRateLimiting("authSystemPolicy")]
        public async Task<IActionResult> Revoke(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseException($"Error email is required", 400));

            ApplicationUser? user = await this._userManager.FindByEmailAsync(email);

            if (user == null) 
                return StatusCode(404, new ResponseException("Invalid email", 404));

            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            return NoContent();
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

            string resetPageUrl = _configuration["FrontendSettings:PasswordResetUrl"] ?? "https://frontend.com/reset-password";

            await this._uow.RecoverAccountRepository.RequestPasswordResetTokenAsync(requestDto, resetPageUrl);

            return Ok(new Response(
                "success",
                "If an account exists with that email, a password reset link has been sent.",
                200,
                null
            ));
        }

        [HttpPost("reset-password")]
        [EnableRateLimiting("authSystemPolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            bool success = await this._uow.RecoverAccountRepository.ValidateAndResetPasswordAsync(resetDto);

            if (success)
            {
                return Ok(new Response(
                    "success",
                    "Password has been reset successfully.",
                    200,
                    null
                ));
            }
            
            return BadRequest(new Response(
                "fail",
                "Password reset failed.",
                400,
                null
            ));
        }
    }
}
