using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog.DTOs.User;
using Blog.entities;
using Blog.SetUnitOfWork;
using Blog.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public UserController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            string? id = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(id);

            return Ok(new Response(
                "success",
                "User found",
                200,
                new UserResponseDTO(user.Id, user.UserName!,user.Email!)
            ));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            string? id = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(id);

            await this._uow.UserRepository.Delete(user);

            return Ok(new Response(
                "success",
                "User deleted with success",
                200,
                null
            ));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Update([FromBody] UpdateUserDto userDto) 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? id = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(id);
            ApplicationUser result = await _uow.UserRepository.Update(user, userDto);

            return Ok(new Response(
                "success",
                "User updated with success",
                200,
                new UserResponseDTO(result.Id, result.UserName!,result.Email!)
            ));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("get-metric")]
        public async Task<IActionResult> GetMetric()
        {
            string? id = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(id);
            UserMetricEntity metric = await this._uow.UserMetricRepository.Get(user.Id);

            return Ok(new Response(
                "success",
                "User metric found",
                200,
                metric
            ));
        }

    }
}