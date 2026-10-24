namespace ERP.Server.Models.Response
{
    public class SupplierListResponse
    {
        public int SupplierID { get; set; }
        public string SupplierCode { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string? Contact { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Remark { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
