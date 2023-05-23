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
    /// Валидатор модели подтверждения почты пользователя.
    /// </summary>
    public class UserConfirmEmailValidator : AbstractValidator<UserConfirmEmailRequest>
    {
        /// <summary>
        /// Правила валидации модели подтверждения почты пользователя.
        /// </summary>
        public UserConfirmEmailValidator() 
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Электронная почта обязательна для заполнения.")
                .EmailAddress().WithMessage("Некорректный формат электронный почты.");
        }
    }
}
