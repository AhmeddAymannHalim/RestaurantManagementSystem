using FluentValidation;
using RestaurantManageSystem.Application.DTOs.Order;

namespace RestaurantManageSystem.Application.Validators
{
    public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusDto>
    {
        public UpdateOrderStatusValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(status => status == "Pending" || status == "Preparing" || 
                               status == "Ready" || status == "Served" || status == "Cancelled")
                .WithMessage("Invalid order status. Must be Pending, Preparing, Ready, Served, or Cancelled");
        }
    }
}
