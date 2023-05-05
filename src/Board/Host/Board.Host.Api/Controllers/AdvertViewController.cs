using Board.Application.AppData.Contexts.AdvertViews.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Board.Host.Api.Controllers
{
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
            var count = _advertViewService.GetCount(advertId, cancellation);

            return Ok(count);
        }

        [HttpPut("{advertId:Guid}")]
        public async Task<IActionResult> IncreaseCount(Guid advertId, CancellationToken cancellation)
        {
            var count = _advertViewService.IncreaseCount(advertId, cancellation);

            return Ok();
        }
    }
}
