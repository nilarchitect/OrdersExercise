using FluentValidation;
using OrdersAPI.Models;

namespace OrdersAPI.Validators
{
    public class CreateOrderRequestValidator: AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.Items)
                .NotNull()
                .WithMessage("Items cannot be null")
                .Must(items => items != null && items.Count > 0)
                .WithMessage("Items must contain at least one item");
        }
    }
}
