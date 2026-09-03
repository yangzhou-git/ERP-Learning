namespace ERP.Server.Models.Request.Arrival
{
    public class AddArrivalRequest
    {
        public int SupplierID { get; set; }//供应商ID
        public int LocationID { get; set; }//到货站点ID
        public DateTime? ArrivalTime { get; set; }//实际到货时间
        public string? Remark { get; set; }//备注
        public List<AddArrivalDetailRequest> ArrivalDetails { get; set; } = new List<AddArrivalDetailRequest>();//到货明细列表
    }
}
