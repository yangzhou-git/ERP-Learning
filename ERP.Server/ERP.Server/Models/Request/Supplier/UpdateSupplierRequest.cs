namespace ERP.Server.Models.Request
{
    public class UpdateSupplierRequest
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? Contact { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Remark { get; set; }
    }
}
