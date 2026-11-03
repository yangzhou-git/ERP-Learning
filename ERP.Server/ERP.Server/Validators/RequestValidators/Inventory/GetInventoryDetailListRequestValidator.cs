using ERP.Server.Models.Request.Inventory;
using FluentValidation;

namespace ERP.Server.Validators.RequestValidators.Inventory
{
    public class GetInventoryDetailListRequestValidator : AbstractValidator<GetInventoryDetailListRequest>
    {
        public GetInventoryDetailListRequestValidator()
        {
            RuleFor(x => x.LocationID).GreaterThan(0).WithMessage("站点ID不合法");
            RuleFor(x => x.GoodsID).GreaterThan(0).WithMessage("商品ID不合法");
        }
    }
}
