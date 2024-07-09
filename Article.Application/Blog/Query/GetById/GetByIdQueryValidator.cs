using FluentValidation;

namespace Article.Application.Blog.Query.GetById
{
    public class GetByIdQueryValidator : AbstractValidator<GetByIdQuery>
    {
        public GetByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull().GreaterThan(0).WithMessage("Id must be greater than 0");
        }
    }
}
