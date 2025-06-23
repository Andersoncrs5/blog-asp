using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.DTOs.UserConfig;
using blog.entities;
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
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            UserConfigEntity config = await _uow.UserConfigRepository.GetAsync(user);

            return Ok(new Response(
                "success",
                "User config founded!!!",
                200,
                config
            ));
        }

        [HttpDelete]
        [EnableRateLimiting("DeleteItemPolicy")]
        public async Task<IActionResult> Delete()
        {
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            UserConfigEntity config = await _uow.UserConfigRepository.GetAsync(user);
            await _uow.UserConfigRepository.DeleteAsync(config);

            return Ok(new Response(
                "success",
                "User config deleted!!!",
                200,
                null
            ));
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Create([FromBody] CreateUserConfigDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            UserConfigEntity config = await _uow.UserConfigRepository.CreateAsync(dto, user);

            return Ok(new Response(
                "success",
                "User config created!!!",
                200,
                config
            ));
        }

        [HttpPut]
        [EnableRateLimiting("UpdateItemPolicy")]
        public async Task<IActionResult> Update([FromBody] UpdateUserConfigDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(userId);
            UserConfigEntity config = await _uow.UserConfigRepository.UpdateAsync(dto, user);

            return Ok(new Response(
                "success",
                "User config updated!!!",
                200,
                config
            ));
        }

    }
}