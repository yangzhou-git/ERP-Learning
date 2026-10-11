using ERP.Server.Models.Enums;

namespace ERP.Server.Models.Entity.BusinessCodeSeed
{
    public class BusinessCodeSeed
    {
        public int BusinessCodeSeedID { get; set; } //业务编号种子ID
        public int CompanyID { get; set; } //公司ID
        public BusinessCodeType BusinessType { get; set; } //业务类型
        public string Prefix { get; set; } = string.Empty; //业务前缀
        public int NumberLength { get; set; } //流水号长度
        public BusinessCodeResetType ResetType { get; set; } //重置类型
        public int CurrentNumber { get; set; } //当前流水号
        public DateTime? NumberDate { get; set; } //当前编号日期
        public bool HasBeenUsed { get; set; } // 是否使用过
        public int CreateUserID { get; set; } //创建人ID
        public DateTime CreateTime { get; set; } //创建时间
        public int? UpdateUserID { get; set; } //修改人ID
        public DateTime? UpdateTime { get; set; } //修改时间
    }
}