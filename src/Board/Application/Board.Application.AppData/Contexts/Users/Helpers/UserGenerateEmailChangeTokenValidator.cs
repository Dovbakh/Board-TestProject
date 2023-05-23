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
    /// Валидатор модели генерации токена изменения почты.
    /// </summary>
    public class UserGenerateEmailChangeTokenValidator : AbstractValidator<UserGenerateEmailTokenRequest>
    {
        /// <summary>
        /// Правила валидации модели генерации токена изменения пользователя.
        /// </summary>
        public UserGenerateEmailChangeTokenValidator()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("Электронная почта обязательна для заполнения.")
                .EmailAddress().WithMessage("Некорректный формат электронный почты.");
            RuleFor(x => x.CurrentEmail)
                .NotEmpty().WithMessage("Электронная почта обязательна для заполнения.")
                .EmailAddress().WithMessage("Некорректный формат электронный почты.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен для заполнения.")
                .Matches(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Пароль должен содержать цифры, латинские заглавные и строчные буквы.")
                .MinimumLength(8).WithMessage("Пароль должен состоять минимум из 8 символов.")
                .MaximumLength(100).WithMessage("Пароль должен состоять максимум из 100 символов.");
        }
    }
}
