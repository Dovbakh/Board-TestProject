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
    /// Валидатор модели генерации токена подтверждения почты пользователя.
    /// </summary>
    public class UserGenerateEmailConfirmationTokenValidator : AbstractValidator<UserGenerateEmailConfirmationTokenRequest>
    {
        /// <summary>
        /// Правила валидации модели генерации токена подтверждения пользователя.
        /// </summary>
        public UserGenerateEmailConfirmationTokenValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Электронная почта обязательна для заполнения.")
                .EmailAddress().WithMessage("Некорректный формат электронный почты.");
        }
    }
}
