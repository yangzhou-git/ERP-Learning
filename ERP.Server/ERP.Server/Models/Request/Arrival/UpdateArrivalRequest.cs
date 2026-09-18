namespace ERP.Server.Models.Request.Arrival
{
    public class UpdateArrivalRequest
    {
        public int ArrivalID { get; set; }
        public int SupplierID { get; set; }
        public int LocationID { get; set; }
        public DateTime? ArrivalTime {  get; set; }
        public string? Remark { get; set; }
        public List<UpdateArrivalDetailRequest> ArrivalDetails { get; set; } = new();
    }
}
