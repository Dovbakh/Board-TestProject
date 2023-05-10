using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Images.Helpers
{
    public class ImageUploadValidator : AbstractValidator<IFormFile>
    {
        public ImageUploadValidator() 
        {
            RuleFor(x => x.ContentType).NotNull().Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"))
                .WithMessage("Неподдерживаемый тип файла.");
            RuleFor(x => x.Length).NotNull().LessThanOrEqualTo(5242880)
                .WithMessage("Размер файла не должен превышать 5 Мб.");
        }
    }
}
