using Article.Application.Blog.Command.Create;
using Article.Application.Blog.Query.GetAll;
using Article.Application.Blog.Query.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Article.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BlogController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [Route("GetById/{Id}")]
        public async Task<IActionResult> GetById([FromRoute] GetByIdQuery query)
        {
            var data = await _mediator.Send(query);
            if (data.IsError)
            {
                if (data.ErrorsList.Any())
                {
                    return BadRequest(data);
                }
                else
                {
                    return NotFound(data);
                }
            }

            return Ok(data);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllQuery();
            var data = await _mediator.Send(query);
            return Ok(data);
        }


        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateCommand model)
        {
            var data = await _mediator.Send(model);
            if (data.IsError)
            {
                if (data.ErrorsList.Any())
                {
                    return BadRequest(data);
                }
                else
                {
                    return NotFound(data);
                }
            }

            return Ok(data);
        }
    }
}
