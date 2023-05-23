using FluentValidation;
using Board.Contracts.Contexts.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Users.Helpers
{
    /// <summary>
    /// Валидатор модели изменения пользователя.
    /// </summary>
    public class UserUpdateValidator : AbstractValidator<UserUpdateRequest>
    {
        /// <summary>
        /// Правила валидации модели изменения пользователя.
        /// </summary>
        public UserUpdateValidator()
        {
            RuleFor(x => x.Name)
                        .NotEmpty().WithMessage("Название обьявления обязательно для заполнения.")
                        .Matches(@"([А-Я]{1}[а-яё]{1,23})").WithMessage("Неправильный формат имени");

            //RuleFor(x => x.Phone)
            //            .Matches(@"[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}").WithMessage("Неверный формат номера телефона.")
            //            .NotEmpty().WithMessage("Номер телефона обязателен для заполнения.");

            RuleFor(x => x.Address)
                        .NotEmpty().WithMessage("Адрес обязателен для заполнения.");
        }
    }
}
