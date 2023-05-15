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
    [Route("v1/advertviews")]
    [Produces("application/json")]
    [ApiConventionType(typeof(AppConventions))]
    [AllowAnonymous]
    public class AdvertViewController : ControllerBase
    {
        private readonly IAdvertViewService _advertViewService;

        public AdvertViewController(IAdvertViewService advertViewService)
        {
            _advertViewService = advertViewService;
        }

        [HttpGet("{advertId:Guid}")]
        public async Task<ActionResult<int>> GetCount(Guid advertId, CancellationToken cancellation)
        {
            var result = await _advertViewService.GetCountAsync(advertId, cancellation);

            return Ok(result);
        }

        [HttpPost("{advertId:Guid}")]
        public async Task<ActionResult<Guid>> Add(Guid advertId, CancellationToken cancellation)
        {
            var result = await _advertViewService.AddAsync(advertId, cancellation);

            return Ok(result);
        }

    }
}
