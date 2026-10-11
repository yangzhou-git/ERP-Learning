using ERP.Server.Models.Enums;

namespace ERP.Server.Models.Request.BusinessCodeSeed
{
    public class AddBusinessCodeSeedRequest
    {
        public BusinessCodeType BusinessType { get; set; }      // 业务类型
        public string Prefix { get; set; } = string.Empty;      // 编号前缀
        public int NumberLength { get; set; }                   // 流水号长度
        public BusinessCodeResetType ResetType { get; set; }    // 重置类型
    }
}
