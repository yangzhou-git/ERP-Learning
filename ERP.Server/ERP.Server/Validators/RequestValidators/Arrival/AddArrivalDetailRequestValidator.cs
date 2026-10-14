using ERP.Server.Models.Request.Arrival;
using FluentValidation;

namespace ERP.Server.Validators.RequestValidators.Arrival
{
    public class AddArrivalDetailRequestValidator : AbstractValidator<AddArrivalDetailRequest>
    {
        public AddArrivalDetailRequestValidator()
        {
            RuleFor(x => x.GoodsUnitID).GreaterThan(0).WithMessage("商品单位关系ID必须大于0");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("到货数量必须大于0").PrecisionScale(14, 2, true).WithMessage("到货数量最多允许12位整数和2位小数");
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithMessage("到货金额必须大于等于0").PrecisionScale(14, 2, true).WithMessage("到货金额最多允许12位整数和2位小数");
            RuleFor(x => x.PurchaseUnitCost).GreaterThanOrEqualTo(0).WithMessage("采购单位单价必须大于等于0").PrecisionScale(18, 6, true).WithMessage("采购单位单价最多允许12位整数和6位小数");
            RuleFor(x => x.Remark).MaximumLength(500).WithMessage("到货明细备注不能超过500个字符");
        }
    }
}
