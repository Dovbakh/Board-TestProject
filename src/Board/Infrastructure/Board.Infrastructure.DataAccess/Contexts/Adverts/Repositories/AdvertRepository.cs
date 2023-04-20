using AutoMapper;
using AutoMapper.Configuration.Annotations;
using AutoMapper.QueryableExtensions;
using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Contracts.Contexts.Adverts;
using Board.Contracts.Contexts.Categories;
using Board.Domain;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Board.Infrastructure.DataAccess.Contexts.Adverts.Repositories
{
    public class AdvertRepository : IAdvertRepository
    {
        private readonly IRepository<Advert> _repository;
        private readonly IMapper _mapper;

        public AdvertRepository(IRepository<Advert> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<AdvertSummary>> GetAllAsync(int offset, int limit, CancellationToken cancellation)
        {
            var existingDtoList = await _repository.GetAll()
                .Include(a => a.AdvertImages)
                .ProjectTo<AdvertSummary>(_mapper.ConfigurationProvider)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellation);

            return existingDtoList;
        }

        public async Task<IReadOnlyCollection<AdvertSummary>> GetAllFilteredAsync(AdvertFilterRequest filterRequest, int offset, int limit, CancellationToken cancellation)
        {
            var query = _repository.GetAll();

            if (filterRequest.UserId.HasValue)
            {
                query = query.Where(a => a.UserId == filterRequest.UserId);
            }
            if (filterRequest.CategoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == filterRequest.CategoryId);
            }

            if (!string.IsNullOrWhiteSpace(filterRequest.Text))
            {
                query = query.Where(p => p.Name.ToLower().Contains(filterRequest.Text.ToLower()) || p.Description.ToLower().Contains(filterRequest.Text.ToLower()));
            }

            if (filterRequest.minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filterRequest.minPrice);
            }

            if (filterRequest.maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filterRequest.maxPrice);
            }
            if (filterRequest.highRating.HasValue)
            {
                query = query.Where(p => (p.User.CommentsFor.Sum(u => u.Rating) / p.User.CommentsFor.Count) >= 4);
            }

            if (!string.IsNullOrWhiteSpace(filterRequest.SortBy))
            {
                switch (filterRequest.SortBy)
                {
                    case "date":
                        query = filterRequest.OrderDesc == 1 ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
                        break;
                    case "price":
                        query = filterRequest.OrderDesc == 1 ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                        break;
                    default:
                        break;
                }
            }

            var existingDtoList = await query
                .Include(a => a.AdvertImages)
                .ProjectTo<AdvertSummary>(_mapper.ConfigurationProvider)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellation);


            return existingDtoList;
        }

        public async Task<AdvertDetails> GetByIdAsync(Guid advertId, CancellationToken cancellation)
        {
            var existingDto = await _repository.GetAll()
                .Where(a => a.Id == advertId)
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.AdvertImages)
                .ProjectTo<AdvertDetails>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellation);

            if (existingDto == null)
            {
                throw new KeyNotFoundException();
            }

            return existingDto;
        }

        public async Task<Guid> AddAsync(AdvertAddRequest addRequest, CancellationToken cancellation)
        {
            var newEntity = _mapper.Map<AdvertAddRequest, Advert>(addRequest);
            await _repository.AddAsync(newEntity, cancellation);

            return newEntity.Id;
        }

        public async Task<AdvertDetails> UpdateAsync(Guid advertId, AdvertUpdateRequest updateRequest, CancellationToken cancellation)
        {
            var existingEntity = await _repository.GetByIdAsync(advertId, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException();
            }

            var updatedEntity = _mapper.Map<AdvertUpdateRequest, Advert>(updateRequest, existingEntity);
            await _repository.UpdateAsync(updatedEntity, cancellation);
            var updatedDto = _mapper.Map<Advert, AdvertDetails>(updatedEntity);

            return updatedDto;
        }

        public async Task DeleteAsync(Guid advertId, CancellationToken cancellation)
        {
            var existingEntity = await _repository.GetByIdAsync(advertId, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException();
            }

            await _repository.DeleteAsync(existingEntity, cancellation);
        }

        public async Task SoftDeleteAsync(Guid advertId, CancellationToken cancellation)
        {
            var existingEntity = await _repository.GetByIdAsync(advertId, cancellation);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException();
            }

            existingEntity.isActive = false;
            await _repository.UpdateAsync(existingEntity, cancellation);
        }
    }
}
