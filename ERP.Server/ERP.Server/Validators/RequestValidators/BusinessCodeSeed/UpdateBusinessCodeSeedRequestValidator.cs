using FluentValidation;
using ERP.Server.Models.Request.BusinessCodeSeed;

namespace ERP.Server.Validators.RequestValidators.BusinessCodeSeed
{
    public class UpdateBusinessCodeSeedRequestValidator : AbstractValidator<UpdateBusinessCodeSeedRequest>
    {
        public UpdateBusinessCodeSeedRequestValidator()
        {
            RuleFor(x => x.BusinessCodeSeedID).GreaterThan(0).WithMessage("业务编号规则ID不合法");
            RuleFor(x => x.BusinessType).IsInEnum().WithMessage("业务编号业务类型不合法");
            RuleFor(x => x.ResetType).IsInEnum().WithMessage("业务编号重置类型不合法");
            RuleFor(x => x.Prefix).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("业务编号前缀不能为空")
                                  .Must(x => x.Trim().Length <= 20).WithMessage("业务编号前缀长度不能超过20个字符"); 
            RuleFor(x => x.NumberLength).InclusiveBetween(1,10).WithMessage("业务编号数字长度必须在1到10之间");
        }
    }
}
