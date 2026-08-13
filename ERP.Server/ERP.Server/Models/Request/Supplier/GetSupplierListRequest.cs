namespace ERP.Server.Models.Request
{
    public class GetSupplierListRequest
    {
        public string? SupplierName { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
