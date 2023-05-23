using Board.Contracts.Contexts.Categories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Categories.Helpers
{
    /// <summary>
    /// Валидатор модели добавления категории.
    /// </summary>
    public class CategoryAddValidator : AbstractValidator<CategoryAddRequest>
    {
        /// <summary>
        /// Правила валидации модели добавления категории.
        /// </summary>
        public CategoryAddValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя обязательно для заполнения.")
                .Length(3, 20).WithMessage("Длина имени должна составлять от 3 до 20 символов.")
                .Matches(@"([A-ZА-Я0-9]([a-zA-Z0-9а-яА-Я]|[- @\.#&!№;%:?*()_])*)").WithMessage("Неправильный формат названия категории.");
        }
    }
}
