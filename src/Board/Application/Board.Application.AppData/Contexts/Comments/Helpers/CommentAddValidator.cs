using Board.Contracts.Contexts.Comments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Application.AppData.Contexts.Comments.Helpers
{
    public class CommentAddValidator : AbstractValidator<CommentAddRequest>
    {
        public CommentAddValidator()
        {
            RuleFor(c => c.Text)
                    .NotEmpty().WithMessage("Введите текст комментария к отзыву.")
                    .MaximumLength(200).WithMessage("Размер текста не должен превышать 200 символов.");
            RuleFor(c => c.Rating)
                    .NotEmpty().WithMessage("Укажите оценку к отзыву.")
                    .InclusiveBetween(1, 5).WithMessage("Оценка должна быть от 1 до 5.");
            RuleFor(c => c.UserId)
                    .NotEmpty().WithMessage("Укажите автора отзыва.");
            RuleFor(c => c.AdvertId)
                    .NotEmpty().WithMessage("Укажите к какому обьявлению относится отзыв.");
        }
    }
}
