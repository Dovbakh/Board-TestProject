using Board.Application.AppData.Contexts.AdvertViews.Services;
using Board.Contracts.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
    /// <summary>
    /// Работа с обьявлениями.
    /// </summary>
    [ApiController]
    [Route("v2/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    public class AdvertViewsController : ControllerBase
    {
        private readonly IAdvertViewService _advertViewService;

        public AdvertViewsController(IAdvertViewService advertViewService)
        {
            _advertViewService = advertViewService;
        }

        [HttpGet("{advertId:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetCount(Guid advertId, CancellationToken cancellation)
        {
            var result = await _advertViewService.GetCountAsync(advertId, cancellation);

            return Ok(result);
        }

        [HttpPost("{advertId:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<Guid>> Add(Guid advertId, CancellationToken cancellation)
        {
            var result = await _advertViewService.AddAsync(advertId, cancellation);

            return Ok(result);
        }

    }
}
