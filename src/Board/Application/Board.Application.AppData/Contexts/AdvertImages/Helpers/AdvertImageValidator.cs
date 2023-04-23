using Board.Contracts.Contexts.AdvertImages;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Board.Application.AppData.Contexts.AdvertImages.Helpers
{
    public class AdvertImageValidator : AbstractValidator<AdvertImageAddRequest>
    {
        public AdvertImageValidator()
        {
    //        RuleFor(x => x.Images)
    //.NotEmpty().WithMessage("Прикрепите хотя бы одно изображение.")
    //.Must(x => x.All(x => x.Length < 25 * 1024 * 1024)).WithMessage("Размер файла должен быть меньше 25 Мб.")
    //.Must(x => x.All(x => x.ContentType == "image/png" || x.ContentType == "image/jpeg")).WithMessage("Неподдерживаемый формат изображения.")
    //.Must(x => x.All(x => {
    //    using var stream = x.OpenReadStream();

    //    var imageInfo = Image.Identify(stream);
    //    if (imageInfo == null)
    //    {
    //        throw new ValidationException("Неподдерживаемый формат изображения.");
    //    }
    //    if (imageInfo.Width < 600) return false;
    //    if (imageInfo.Height < 300) return false;

    //    stream.Position = 0;
    //    var imageFormat = Image.DetectFormat(stream);
    //    if (imageFormat.DefaultMimeType != "image/png" && imageFormat.DefaultMimeType != "image/jpeg") return false;
    //    return true;
    //})).WithMessage("Размер изображения должен быть минимум 600х300.");
        }
    }
}
