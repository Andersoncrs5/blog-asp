using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.DTOs.UserConfig;
using blog.entities;
using blog.utils.Responses;
using Blog.entities;
using Blog.SetUnitOfWork;
using Blog.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace blog.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserConfigController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public UserConfigController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> Get()
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? user = await _uow.UserRepository.Get(userId);
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

            UserConfigEntity? config = await _uow.UserConfigRepository.GetAsync(user);
            if (config == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Config User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            return Ok(new ResponseBody<UserConfigEntity>
            {
                Status = true,
                Message = "User config founded!!!",
                Code = 200,
                Body = config,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpDelete]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Delete()
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? user = await _uow.UserRepository.Get(userId);

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

            UserConfigEntity? config = await _uow.UserConfigRepository.GetAsync(user);
            if (config == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Config User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }
            
            await _uow.UserConfigRepository.DeleteAsync(config);

            return Ok(new ResponseBody<UserConfigEntity>
            {
                Status = true,
                Message = "User config deleted!!!",
                Code = 200,
                Body = null,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Create([FromBody] CreateUserConfigDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            bool check = await _uow.UserConfigRepository.Exists(userId);
            if (check)
            {
                return Conflict(new ResponseBody<string>
                {
                    Body = null,
                    Code = 409,
                    Message = "User already have a config",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? user = await _uow.UserRepository.Get(userId);
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

            UserConfigEntity config = await _uow.UserConfigRepository.CreateAsync(dto, user);

            return Ok(new ResponseBody<UserConfigEntity>
            {
                Status = true,
                Message = "User config created!!!",
                Code = 200,
                Body = config,
                Datetime = DateTimeOffset.Now
            });
        }

        [HttpPut]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> Update([FromBody] UpdateUserConfigDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ResponseBody<string>
                {
                    Body = null,
                    Code = 401,
                    Message = "You are not authorizetion",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            ApplicationUser? user = await _uow.UserRepository.Get(userId);
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

            UserConfigEntity? config = await _uow.UserConfigRepository.GetAsync(user);
            if (config == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Message = "Config User not found",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            UserConfigEntity configUpdated = await _uow.UserConfigRepository.UpdateAsync(dto, user, config);

            return Ok(new ResponseBody<UserConfigEntity>
            {
                Status = true,
                Message = "User config updated!!!",
                Code = 200,
                Body = configUpdated,
                Datetime = DateTimeOffset.Now
            });
        }

    }
}