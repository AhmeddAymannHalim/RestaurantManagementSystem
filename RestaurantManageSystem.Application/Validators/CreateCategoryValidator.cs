using FluentValidation;
using RestaurantManageSystem.Application.DTOs.Category;

namespace RestaurantManageSystem.Application.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Category name is required")
                .MinimumLength(3).WithMessage("Category name must be at least 3 characters")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");

            RuleFor(x => x.CategoryNameAr)
                .MinimumLength(3).WithMessage("Arabic category name must be at least 3 characters")
                .MaximumLength(100).WithMessage("Arabic category name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.CategoryNameAr));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}
