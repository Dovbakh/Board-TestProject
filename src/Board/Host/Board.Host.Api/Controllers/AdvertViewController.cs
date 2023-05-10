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
    public class AdvertViewController : ControllerBase
    {
        private readonly IAdvertViewService _advertViewService;

        public AdvertViewController(IAdvertViewService advertViewService)
        {
            _advertViewService = advertViewService;
        }

        [HttpGet("{advertId:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetCount(Guid advertId, CancellationToken cancellation)
        {
            var totalCount = await _advertViewService.GetCountAsync(advertId, cancellation);

            return Ok(totalCount);
        }

        [HttpPut("{advertId:Guid}")]
        public async Task<IActionResult> UpdateCount(Guid advertId, int count, CancellationToken cancellation)
        {
            var totalCount = await _advertViewService.UpdateCountAsync(advertId, count, cancellation);

            return Ok(totalCount);
        }
    }
}
