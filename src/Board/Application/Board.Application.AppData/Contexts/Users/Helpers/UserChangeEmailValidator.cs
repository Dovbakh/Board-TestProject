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
    /// Валидатор модели изменения почты пользователя.
    /// </summary>
    public class UserChangeEmailValidator : AbstractValidator<UserChangeEmailRequest>
    {
        /// <summary>
        /// Правила валидации модели изменения почты пользователя.
        /// </summary>
        public UserChangeEmailValidator() 
        {
            RuleFor(x => x.CurrentEmail)
                .NotEmpty().WithMessage("Электронная почта обязательна для заполнения.")
                .EmailAddress().WithMessage("Некорректный формат электронный почты.");
            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("Электронная почта обязательна для заполнения.")
                .EmailAddress().WithMessage("Некорректный формат электронный почты.");
        }
    }
}
