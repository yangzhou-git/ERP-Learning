namespace ERP.Server.Models.Entity
{
    public class Supplier
    {
        public int SupplierID { get; set; }//供应商ID
        public string SupplierName { get; set; } = string.Empty;//供应山名称
        public string? Contact { get; set; }//联系人
        public string? Phone { get; set; }//电话
        public string? Address { get; set; }//地址
        public string? Remark { get; set; }//备注
        public bool IsActive { get; set; }//是否启用
        public DateTime CreateTime { get; set; }//创建时间
        public DateTime? UpdateTime { get; set; }//修改时间
    }
}
