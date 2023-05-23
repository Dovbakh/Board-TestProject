using FluentValidation;
using Identity.Contracts.Contexts.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.AppData.Helpers
{
    public class UserGenerateEmailConfirmationTokenValidator : AbstractValidator<UserGenerateEmailConfirmationTokenRequest>
    {
        public UserGenerateEmailConfirmationTokenValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Электронная почта обязательна для заполнения.")
                .EmailAddress().WithMessage("Некорректный формат электронный почты.");
        }
    }
}
