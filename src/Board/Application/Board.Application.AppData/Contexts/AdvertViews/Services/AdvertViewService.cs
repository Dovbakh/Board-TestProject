using Board.Application.AppData.Contexts.AdvertViews.Repositories;
using RedLockNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.AdvertViews.Services
{
    public class AdvertViewService : IAdvertViewService
    {
        private readonly IAdvertViewRepository _advertViewRepository;
        private readonly IDistributedLockFactory _distributedLockFactory;

        public AdvertViewService(IAdvertViewRepository advertViewRepository, IDistributedLockFactory distributedLockFactory)
        {
            _advertViewRepository = advertViewRepository;
            _distributedLockFactory = distributedLockFactory;
        }

        public Task<int> GetCount(Guid advertId, CancellationToken cancellation)
        {
            return _advertViewRepository.GetCount(advertId, cancellation);
        }

        public async Task IncreaseCount(Guid advertId, CancellationToken cancellation)
        {
            var resource = "AdvertCountKey_" + advertId;
            var expiry = TimeSpan.FromSeconds(30);
            var wait = TimeSpan.FromSeconds(10);
            var retry = TimeSpan.FromSeconds(1);
            await using (var redLock = await _distributedLockFactory.CreateLockAsync(resource, expiry, wait, retry, cancellation))
            {
                if (redLock.IsAcquired)
                {
                    await _advertViewRepository.IncreaseCount(advertId, cancellation);
                }
            }
        }

    }
}
