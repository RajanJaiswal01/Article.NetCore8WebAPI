using FluentValidation;

namespace Article.Application.Blog.Command.Create
{
    public class CreateCommandValidator:AbstractValidator<CreateCommand>
    {
        public CreateCommandValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .NotNull()
                .WithMessage("{PropertyName} Cannot be Empty")
                .MinimumLength(3)
                .WithMessage("{PropertyName} cannot have less than 3 letters.")
                .MaximumLength(20)
                .WithMessage("{PropertyName} cannot have more than 20 letters.");


            RuleFor(r => r.Description)
                .NotEmpty()
                .NotNull()
                .WithMessage("{PropertyName} Cannot be Empty")
                .MinimumLength(5)
                .WithMessage("{PropertyName} cannot have less than 5 letters.")
                .MaximumLength(30)
                .WithMessage("{PropertyName} cannot have more than 30 letters.");

        }
    }
}
