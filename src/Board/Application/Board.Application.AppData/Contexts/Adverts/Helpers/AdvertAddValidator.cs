using Board.Contracts.Contexts.Adverts;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Board.Application.AppData.Contexts.Adverts.Helpers
{
    public class AdvertAddValidator : AbstractValidator<AdvertAddRequest>
    {
        public AdvertAddValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя обязательно для заполнения.")
                .Matches(@"([A-ZА-Я0-9]([a-zA-Z0-9а-яА-Я]|[- @\.#&!№;%:?*()_])*)").WithMessage("Неправильный формат названия обьявления.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание обязательно для заполнения.");

            //RuleFor(x => x.Phone)
            //    .Matches(@"[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}").WithMessage("Неверный формат номера телефона.")
            //    .NotEmpty().WithMessage("Номер телефона обязателен для заполнения.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адрес обязателен для заполнения.");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Цена обязательна для заполнения.")
                .GreaterThanOrEqualTo(0).WithMessage("Цена не может быть отрицательной.");

            //RuleFor(x => x.UserName)
            //    .NotEmpty().WithMessage("Имя обязательно для заполнения.")
            //    .Matches(@"([А-Я]{1}[а-яё]{1,23})").WithMessage("Неправильный формат имени");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Категория обязательна к выбору.");

            RuleFor(x => x.AdvertImagesId)
                .NotEmpty().WithMessage("Прикрепите хотя бы одно изображение.")
                .NotNull().WithMessage("Прикрепите хотя бы одно изображение.");
               
        }
    }
}
