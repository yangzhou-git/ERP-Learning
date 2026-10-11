using ERP.Server.Models.Request.BusinessCodeSeed;
using FluentValidation;

namespace ERP.Server.Validators.BusinessCodeSeed
{
    public class AddBusinessCodeSeedRequestValidator : AbstractValidator<AddBusinessCodeSeedRequest>
    {
        public AddBusinessCodeSeedRequestValidator() 
        {
            RuleFor(x => x.NumberLength).InclusiveBetween(1, 10).WithMessage("业务编号数字长度必须在1到10之间");
            RuleFor(x => x.BusinessType).IsInEnum().WithMessage("业务编号业务类型不合法");
            RuleFor(x => x.ResetType).IsInEnum().WithMessage("业务编号重置类型不合法");
            RuleFor(x => x.Prefix).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("业务编号前缀不能为空")
                .Must(x => x.Trim().Length <= 20).WithMessage("业务编号前缀长度不能超过20个字符");
        }
    }
}
