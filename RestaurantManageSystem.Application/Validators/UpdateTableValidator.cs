using FluentValidation;
using RestaurantManageSystem.Application.DTOs.Table;

namespace RestaurantManageSystem.Application.Validators
{
    public class UpdateTableValidator : AbstractValidator<UpdateTableDto>
    {
        public UpdateTableValidator()
        {
            RuleFor(x => x.TableNumber)
                .GreaterThan(0).WithMessage("Table number must be greater than 0")
                .LessThanOrEqualTo(999).WithMessage("Table number cannot exceed 999");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Table capacity must be at least 1")
                .LessThanOrEqualTo(20).WithMessage("Table capacity cannot exceed 20");
        }
    }
}
