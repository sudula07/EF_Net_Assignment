using FluentValidation;
using ProductApi.Application.DTOs;

namespace ProductApi.Application.Validators;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ModifiedBy)
            .NotEmpty()
            .MaximumLength(100);

        RuleForEach(x => x.Items)
            .SetValidator(new UpdateItemRequestValidator());
    }
}
