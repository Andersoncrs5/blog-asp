using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using blog.utils.Responses;
using Blog.DTOs.Category;
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
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public CategoryController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet("{Id:long}")]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> Get(long Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Datetime = DateTimeOffset.Now,
                    Message = "Category Id is required",
                    Status = false
                });
            }

            CategoryEntity? category = await _uow.CategoryRepository.Get(Id);

            if (category == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "Category not found",
                    Status = false
                });
            }

            return Ok(new Response(
                "success",
                "Category found with successfully!",
                200,
                category
            ));
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        [Authorize(Roles = "AdminRole, SuperAdminRole")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userId = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (string.IsNullOrWhiteSpace(userId)) {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Message = "Id is required",
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

            bool checkName = await _uow.CategoryRepository.ExistsByName(dto.Name);

            if (checkName == true) 
            {
                return Conflict(new ResponseBody<string>
                {
                    Body = null,
                    Code = 409,
                    Message = "Name exists",
                    Status = false,
                    Datetime = DateTimeOffset.Now
                });
            }

            CategoryEntity categoryCreated = await _uow.CategoryRepository.Create(dto, user);

            return Ok(new Response(
                "success",
                "Category created with successfully!",
                201,
                categoryCreated
            ));
        }

        [HttpGet]
        [EnableRateLimiting("SlidingWindowLimiterPolicy")]
        public async Task<IActionResult> GetAll()
        {
            List<CategoryEntity> categories = await _uow.CategoryRepository.GetAll(true);

            return Ok(new Response(
                "success",
                "All category listed with successfully",
                200,
                categories
            ));
        }
        
        [HttpDelete("{Id:long}")]
        [EnableRateLimiting("DeleteItemPolicy")]
        [Authorize(Roles = "AdminRole, SuperAdminRole")]
        public async Task<IActionResult> Delete(long Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Datetime = DateTimeOffset.Now,
                    Message = "Category Id is required",
                    Status = false
                });
            }

            CategoryEntity? category = await _uow.CategoryRepository.Get(Id);

            if (category == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "Category not found",
                    Status = false
                });
            }

            await _uow.CategoryRepository.Delete(category);

            return Ok(new Response(
                "success",
                "Category deleted with successfully!",
                200,
                null
            ));
        }

        [HttpPut("{Id:long}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        [Authorize(Roles = "AdminRole, SuperAdminRole")]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryDTO dto, long Id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (Id <= 0)
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Datetime = DateTimeOffset.Now,
                    Message = "Category Id is required",
                    Status = false
                });
            }

            CategoryEntity? category = await _uow.CategoryRepository.Get(Id);

            if (category == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "Category not found",
                    Status = false
                });
            }

            CategoryEntity result = await _uow.CategoryRepository.Update(category, dto);

            return Ok(new Response(
                "success",
                "Category updated!!",
                200,
                result
            ));
        }
        
        [HttpGet("change-status/{Id:long}")]
        [EnableRateLimiting("UpdateItemPolicy")]
        [Authorize(Roles = "AdminRole, SuperAdminRole")]
        public async Task<IActionResult> ChangeStatusActive(long Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new ResponseBody<string>
                {
                    Body = null,
                    Code = 400,
                    Datetime = DateTimeOffset.Now,
                    Message = "Category Id is required",
                    Status = false
                });
            }

            CategoryEntity? category = await _uow.CategoryRepository.Get(Id);

            if (category == null)
            {
                return NotFound(new ResponseBody<string>
                {
                    Body = null,
                    Code = 404,
                    Datetime = DateTimeOffset.Now,
                    Message = "Category not found",
                    Status = false
                });
            }

            CategoryEntity result = await _uow.CategoryRepository.ChangeStatusActive(category);

            return Ok(new Response(
                "success",
                "Status changed!",
                200,
                result
            ));
        }

    }
}