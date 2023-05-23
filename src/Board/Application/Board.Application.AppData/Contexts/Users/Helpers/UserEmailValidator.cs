using Board.Contracts.Contexts.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Users.Helpers
{
    /// <summary>
    /// Валидатор модели почты пользователя.
    /// </summary>
    public class UserEmailValidator : AbstractValidator<UserEmail>
    {
        /// <summary>
        /// Правила валидации модели почты пользователя.
        /// </summary>
        public UserEmailValidator() 
        {
            RuleFor(x => x.Value).NotEmpty().WithMessage("Электронная почта обязательна для заполнения.")
                .EmailAddress().WithMessage("Некорректный формат электронный почты.");
        }

    }
}
