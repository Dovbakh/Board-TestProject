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
    /// <summary>
    /// Валидатор модели добавления обьявления.
    /// </summary>
    public class AdvertAddValidator : AbstractValidator<AdvertAddRequest>
    {
        /// <summary>
        /// Правила валидации модели добавления обьявления.
        /// </summary>
        public AdvertAddValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя обязательно для заполнения.")
                .Matches(@"([A-ZА-Я0-9]([a-zA-Z0-9а-яА-Я]|[- @\.#&!№;%:?*()_])*)").WithMessage("Неправильный формат названия обьявления.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание обязательно для заполнения.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адрес обязателен для заполнения.");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Цена обязательна для заполнения.")
                .GreaterThanOrEqualTo(0).WithMessage("Цена не может быть отрицательной.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Категория обязательна к выбору.");

            RuleFor(x => x.ImagesId)
                .NotEmpty().WithMessage("Прикрепите хотя бы одно изображение.")
                .NotNull().WithMessage("Прикрепите хотя бы одно изображение.");
               
        }
    }
}
