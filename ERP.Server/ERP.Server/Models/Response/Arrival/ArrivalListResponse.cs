using ERP.Server.Models.Enums;

namespace ERP.Server.Models.Response.Arrival
{
    public class ArrivalListResponse
    {
        public int ArrivalID { get; set; } // 到货单ID
        public string ArrivalCode { get; set; } = string.Empty; // 到货单号
        public ArrivalStatus ArrivalStatus { get; set; } // 到货单状态
        public string SupplierName { get; set; } = string.Empty; // 供应商名称
        public string LocationName { get; set; } = string.Empty; // 到货站点名称
        public decimal TotalAmount { get; set; } // 到货总金额
        public DateTime? ArrivalTime { get; set; } // 实际到货时间
        public string CreateUserName { get; set; } = string.Empty; // 录单人
        public DateTime CreateTime { get; set; } // 录单时间
    }
}
