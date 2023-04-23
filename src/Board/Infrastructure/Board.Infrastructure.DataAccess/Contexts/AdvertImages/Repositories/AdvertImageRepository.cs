using AutoMapper;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.AdvertImages.Repositories;
using Board.Contracts.Contexts.AdvertImages;
using Board.Domain;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.DataAccess.Contexts.AdvertImages.Repositories
{
    public class AdvertImageRepository : IAdvertImageRepository
    {
        private readonly IRepository<AdvertImage> _repository;
        private readonly IMapper _mapper;

        public AdvertImageRepository(IRepository<AdvertImage> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Guid> AddAsync(AdvertImageAddRequest addRequest, CancellationToken cancellation)
        {
            var newEntity = _mapper.Map<AdvertImageAddRequest, AdvertImage>(addRequest);
            await _repository.AddAsync(newEntity, cancellation);
            var newEntityId = newEntity.Id;

            return newEntityId;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellation)
        {
            var existingEntity = await _repository.GetByIdAsync(id, cancellation);
            if(existingEntity == null) 
            {
                throw new KeyNotFoundException();
            }

            await _repository.DeleteAsync(existingEntity, cancellation);
        }

        public async Task DeleteByFileIdAsync(Guid fileId, CancellationToken cancellation)
        {
            var existingEntity = await _repository.GetAll()
                .Where(a => a.FileId == fileId)
                .FirstOrDefaultAsync(cancellation);        
            
            if (existingEntity == null)
            {
                throw new KeyNotFoundException();
            }

            await _repository.DeleteAsync(existingEntity, cancellation);
        }

        public async Task<IReadOnlyCollection<AdvertImageDto>> GetAllByAdvertIdAsync(Guid advertId, CancellationToken cancellation)
        {
            var advertList = await _repository.GetAll()
                .Where(a => a.AdvertId == advertId)
                .ProjectTo<AdvertImageDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellation);

            return advertList;
        }

        public async Task<AdvertImageDto> GetByIdAsync(Guid id, CancellationToken cancellation)
        {
            var advert = await _repository.GetAll()
                .Where(a => a.Id == id)
                .ProjectTo<AdvertImageDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            return advert;
        }
    }
}
