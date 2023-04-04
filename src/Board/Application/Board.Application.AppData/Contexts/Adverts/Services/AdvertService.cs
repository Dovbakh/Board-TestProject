using AutoMapper;
using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.Adverts.Services;
using Board.Contracts.Contexts.Adverts;
using Board.Domain;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Adverts.Services
{
    public class AdvertService : IAdvertService
    {
        private readonly IAdvertRepository _advertRepository;
        private readonly IMapper _mapper;

        public AdvertService(IAdvertRepository advertRepository, IMapper mapper)
        {
            _advertRepository = advertRepository;
            _mapper = mapper;
        }

        public Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int? offset, int? count, CancellationToken cancellation)
        {
            if (count == null)
            {
                count = 10;
            }

            return _advertRepository.GetAllAsync(offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        public Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest request, int? offset, int? count, CancellationToken cancellation)
        {
            if (count == null)
            {
                count = 10;
            }

            return _advertRepository.GetAllFilteredAsync(request, offset.GetValueOrDefault(), count.GetValueOrDefault(), cancellation);
        }

        public Task<AdvertDetails> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            return _advertRepository.GetByIdAsync(id, cancellation);
        }

        public async Task<Guid> CreateAsync(AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            var newAdvertId = await _advertRepository.AddAsync(addRequest, cancellation);

            return newAdvertId;
        }

        public Task<AdvertDetails> PatchAsync(Guid id, JsonPatchDocument<AdvertUpdateRequest> updateRequest, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public async Task<AdvertDetails> UpdateAsync(Guid id, AdvertUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var updatedDto = await _advertRepository.UpdateAsync(id, updateRequest, cancellation);

            return updatedDto;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            await _advertRepository.DeleteAsync(id, cancellation);
        }
    }
}
