using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.SetUnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace blog.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class FavoritePostController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public FavoritePostController(IUnitOfWork uow)
        {
            _uow = uow;
        }
    }
}