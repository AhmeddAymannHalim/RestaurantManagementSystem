using FluentValidation;
using RestaurantManageSystem.Application.DTOs.Order;

namespace RestaurantManageSystem.Application.Validators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.TableId)
                .GreaterThan(0).WithMessage("Table ID must be greater than 0");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be greater than 0");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item")
                .Must(items => items.All(i => i.MenuItemId > 0))
                .WithMessage("All menu item IDs must be valid")
                .Must(items => items.All(i => i.Quantity > 0))
                .WithMessage("All item quantities must be greater than 0");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");
        }
    }
}
