namespace ERP.Server.Models.Request
{
    public class UpdateSupplierStatusRequest
    {
        public int SupplierID { get; set; }
        public bool IsActive { get; set; }
    }
}
