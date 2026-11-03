using ERP.Server.Models.Request.Inventory;
using FluentValidation;

namespace ERP.Server.Validators.RequestValidators.Inventory
{
    public class GetInventorySummaryListRequestValidator : AbstractValidator<GetInventorySummaryListRequest>
    {
        public GetInventorySummaryListRequestValidator()
        {
            RuleFor(x => x.LocationID).GreaterThan(0).When(x => x.LocationID.HasValue).WithMessage("站点ID不合法");
        }
    }
}
