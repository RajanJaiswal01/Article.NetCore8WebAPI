using FluentValidation;

namespace Article.Application.Blog.Command.Update
{
    public class UpdateCommandValidation:AbstractValidator<UpdateCommand>
    {
        public UpdateCommandValidation()
        {
            RuleFor(r => r.Id)
                .NotEmpty()
                .NotNull()
                .WithMessage("Id cannot be Null.")
                .GreaterThanOrEqualTo(1)
                .WithMessage("Id Must be greater than 0");


        }
    }
}
