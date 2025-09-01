using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.utils.Responses;
using Blog.DTOs.User;
using Blog.entities;
using Blog.SetUnitOfWork;
using Blog.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Blog.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public UserController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet("me")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> Me()
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

            UserResponseDTO response = new UserResponseDTO(user.Id, user.UserName!, user.Email!, null);

            return Ok(new ResponseBody<UserResponseDTO>
            {
                Body = response,
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Message = "User found with successfully",
                Status = true,
            });
        }

        [HttpGet("{userId:required}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetUser(string userId)
        {
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

            await _uow.UserMetricRepository.SumOrRedProfileViews(metric, utils.enums.SumOrRedEnum.SUM);

            UserResponseDTO response = new UserResponseDTO(user.Id, user.UserName!, user.Email!, null);

            return Ok(new ResponseBody<UserResponseDTO>
            {
                Body = response,
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Message = "User found",
                Status = true,
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

            await _uow.UserRepository.Delete(user);

            return Ok(new ResponseBody<string>
            {
                Body = null,
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Message = "User deleted with successfully",
                Status = true,
            });
        }

        [HttpPut]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<ActionResult> Update([FromBody] UpdateUserDto userDto)
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

            ApplicationUser result = await _uow.UserRepository.Update(user, userDto);

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

            await _uow.UserMetricRepository.SumOrRedEditedCount(metric, utils.enums.SumOrRedEnum.SUM);

            return Ok(new ResponseBody<UserResponseDTO>
            {
                Body = new UserResponseDTO(result.Id, result.UserName!, result.Email!, null),
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Message = "User found",
                Status = true,
            });
        }

        [HttpGet("get-metric")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetMetric()
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

            return Ok(new ResponseBody<UserMetricEntity>
            {
                Body = metric,
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Message = "User metric found",
                Status = true,
            });
        }

        [HttpGet("{userId:required}/get-metric")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetMetric(string userId)
        {
            UserMetricEntity? metric = await this._uow.UserMetricRepository.Get(userId);
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

            return Ok(new ResponseBody<UserMetricEntity>
            {
                Body = metric,
                Code = 200,
                Datetime = DateTimeOffset.Now,
                Message = "User metric found",
                Status = true,
            });
        }

    }
}