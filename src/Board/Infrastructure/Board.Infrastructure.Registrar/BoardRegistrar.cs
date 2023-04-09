using AutoMapper;
using Board.Application.AppData.Contexts.Adverts.Services;
using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Application.AppData.Contexts.Categories.Services;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Application.AppData.Contexts.Comments.Services;
using Board.Application.AppData.Contexts.AdvertImages.Repositories;
using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.Users.Repositories;
using Board.Infrastructure.DataAccess;
using Board.Infrastructure.DataAccess.Contexts.Categories.Repositories;
using Board.Infrastructure.DataAccess.Contexts.Comments.Repositories;
using Board.Infrastructure.DataAccess.Contexts.AdvertImages.Repositories;
using Board.Infrastructure.DataAccess.Contexts.Adverts.Repositories;
using Board.Infrastructure.DataAccess.Interfaces;
using Board.Infrastructure.Registrar.MapProfiles;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Board.Application.AppData.Contexts.Users.Services;
using Board.Infrastructure.DataAccess.Contexts.Users.Repositories;

namespace Board.Infrastructure.Registrar
{
    public static class BoardRegistrar
    {
        public static IServiceCollection AddServiceRegistrationModule(this IServiceCollection services)
        {
            // Регистрация сервисов работы с БД
            services.AddSingleton<IDbContextOptionsConfigurator<BoardDbContext>, BoardDbContextConfiguration>();
            services.AddDbContext<BoardDbContext>((Action<IServiceProvider, DbContextOptionsBuilder>)
                ((sp, dbOptions) => sp.GetRequiredService<IDbContextOptionsConfigurator<BoardDbContext>>()
                   .Configure((DbContextOptionsBuilder<BoardDbContext>)dbOptions)));
            services.AddScoped((Func<IServiceProvider, DbContext>)(sp => sp.GetRequiredService<BoardDbContext>()));


            //// Регистрация сервисов работы с обьектным хранилищем
            //services.AddScoped<IObjectStorage, MinioStorage>();

            #region Регистрация репозиториев
            // Регистрация репозиториев
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //services.AddScoped(typeof(ICachedRepository<>), typeof(CachedRepository<>));
            services.AddScoped<IAdvertRepository, AdvertRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IAdvertImageRepository, AdvertImageRepository>();
            #endregion

            //// Регистрация application-сервисов
            services.AddScoped<IAdvertService, AdvertService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IFileService, FileService>();
            services.AddScoped<ICommentService, CommentService>();
            //services.AddScoped<INotifierService, EmailService>();

            //// Регистрация вспомогательных сервисов
            //services.AddSingleton<IDateTimeService, DateTimeService>();
            //services.AddTransient(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
            //services.AddScoped<IClaimsAccessor, HttpContextClaimsAccessor>();
            //services.AddTransient<IValidator<UserRegisterDto>, UserRegisterValidator>();
            //services.AddTransient<IValidator<UserLoginDto>, UserLoginValidator>();
            //services.AddTransient<IValidator<UserChangePasswordDto>, UserChangePasswordValidator>();
            //services.AddTransient<IValidator<UserChangeEmailDto>, UserChangeEmailValidator>();
            //services.AddTransient<IValidator<UserUpdateRequestDto>, UserUpdateValidator>();
            //services.AddTransient<IValidator<UserEmailDto>, UserEmailValidator>();
            //services.AddTransient<IValidator<UserResetPasswordDto>, UserResetPasswordValidator>();
            //services.AddTransient<IValidator<AdvertisementRequestDto>, AdvertisementValidator>();
            //services.AddTransient<IValidator<AdvertisementUpdateRequestDto>, AdvertisementUpdateValidator>();
            //services.AddTransient<IValidator<CommentRequestDto>, CommentValidator>();
            //services.AddTransient<IValidator<CommentUpdateRequestDto>, CommentUpdateValidator>();


            //services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration
            //    (a =>
            //    {
            //        a.AddMaps(Assembly.GetExecutingAssembly());
            //    })));

            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));



            return services;
        }


        private static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                // TODO: доделать кфг маппера
                cfg.AddMaps(Assembly.GetExecutingAssembly());
                //var profiles = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(Profile).IsAssignableFrom(t));

                //cfg.AddProfiles(profiles);
                //var result3 = result2.
                //cfg.AddProfile<AdvertProfile>();
                //cfg.AddProfile<AdvertImageProfile>();
                //cfg.AddProfile<CategoryProfile>();
                //cfg.AddProfile<CommentProfile>();
            });
            configuration.AssertConfigurationIsValid();

            return configuration;
        }
    }
}
