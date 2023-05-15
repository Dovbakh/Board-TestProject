using AutoMapper;
using Board.Contracts.Contexts.Categories;
using Board.Contracts.Contexts.Comments;
using Board.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Infrastructure.Registrar.MapProfiles.Contexts
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentDetails>();

            CreateMap<CommentAddRequest, Comment>()
                .ForMember(s => s.Id, map => map.Ignore())
                //.ForMember(s => s.User, map => map.Ignore())
                //.ForMember(s => s.Author, map => map.Ignore())
                .ForMember(s => s.Advert, map => map.Ignore())
                .ForMember(a => a.CreatedAt, map => map.MapFrom(d => DateTime.UtcNow))
                .ForMember(s => s.isActive, map => map.MapFrom(a => true));

            CreateMap<CommentUpdateRequest, Comment>()
                .ForMember(s => s.Id, map => map.Ignore())
                //.ForMember(s => s.User, map => map.Ignore())
                //.ForMember(s => s.Author, map => map.Ignore())
                .ForMember(s => s.Advert, map => map.Ignore())
                .ForMember(s => s.CreatedAt, map => map.Ignore())
                .ForMember(s => s.isActive, map => map.Ignore())
                .ForMember(s => s.UserId, map => map.Ignore())
                .ForMember(s => s.AdvertId, map => map.Ignore());

        }
    }
}
