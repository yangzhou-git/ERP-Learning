using ERP.Server.Models.Request.Arrival;
using FluentValidation;

namespace ERP.Server.Validators.RequestValidators.Arrival
{
    public class AddArrivalRequestValidator : AbstractValidator<AddArrivalRequest>
    {
        public AddArrivalRequestValidator() 
        {
            RuleFor(x => x.SupplierID).GreaterThan(0).WithMessage("供应商ID必须大于0");
            RuleFor(x => x.LocationID).GreaterThan(0).WithMessage("站点ID必须大于0");
            RuleFor(x => x.ArrivalDetails).NotEmpty().WithMessage("到货明细不能为空");
            RuleForEach(x => x.ArrivalDetails).NotNull().WithMessage("到货明细不能包含空项").SetValidator(new AddArrivalDetailRequestValidator());
            RuleFor(x => x.ArrivalTime).Must(arrivalTime => arrivalTime == null || arrivalTime <= DateTime.Now).WithMessage("到货时间不能晚于当前时间");
            RuleFor(x => x.ArrivalDetails).Must(details => details == null || !details.Where(x => x != null).GroupBy(x => x.GoodsUnitID).Any(g => g.Count() > 1)).WithMessage("到货明细中不能有重复的商品单位关系ID");
            RuleFor(x => x.Remark).MaximumLength(500).WithMessage("到货单备注不能超过500个字符");
        }
    }
}
