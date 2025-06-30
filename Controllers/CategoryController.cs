using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
            CategoryEntity category = await _uow.CategoryRepository.Get(Id);
            return Ok(new Response(
                "success",
                "Category found with successfully!",
                200,
                category
            ));
        }

        [HttpPost]
        [EnableRateLimiting("CreateItemPolicy")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string? id = User.FindFirst(ClaimTypes.Sid)?.Value;
            ApplicationUser user = await _uow.UserRepository.Get(id);
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
        public async Task<IActionResult> Delete(long Id)
        {
            CategoryEntity category = await _uow.CategoryRepository.Get(Id);
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
        public async Task<IActionResult> Update([FromBody] UpdateCategoryDTO dto, long Id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CategoryEntity category = await _uow.CategoryRepository.Get(Id);
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
        public async Task<IActionResult> ChangeStatusActive(long Id)
        {
            CategoryEntity category = await _uow.CategoryRepository.Get(Id);
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