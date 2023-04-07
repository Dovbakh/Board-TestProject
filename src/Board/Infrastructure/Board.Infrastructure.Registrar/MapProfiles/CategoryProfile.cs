using AutoMapper;
using Board.Contracts.Contexts.Categories;
using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Registrar.MapProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryAddRequest, Category>()
                .ForMember(s => s.Id, map => map.Ignore())
                .ForMember(s => s.Parent, map => map.Ignore())
                .ForMember(s => s.Children, map => map.Ignore())
                .ForMember(s => s.Adverts, map => map.Ignore())
                .ForMember(a => a.CreatedAt, map => map.MapFrom(d => DateTime.UtcNow))
                .ForMember(s => s.isActive, map => map.MapFrom(a => true));

            CreateMap<CategoryUpdateRequest, Category>()
                .ForMember(s => s.Id, map => map.Ignore())
                .ForMember(s => s.Parent, map => map.Ignore())
                .ForMember(s => s.Children, map => map.Ignore())
                .ForMember(s => s.Adverts, map => map.Ignore())
                .ForMember(s => s.CreatedAt, map => map.Ignore())
                .ForMember(s => s.isActive, map => map.Ignore());

            CreateMap<Category, CategorySummary>();

            CreateMap<Category, CategoryDetails>();


        }
    }
}
