using ERP.Server.Common;
using ERP.Server.Models.Enums;

namespace ERP.Server.Models.Response.Arrival
{
    public class ArrivalDetailResponse
    {
        public int ArrivalID { get; set; } // 到货单ID
        public string ArrivalCode { get; set; } = string.Empty; // 到货单号
        public ArrivalStatus ArrivalStatus { get; set; } // 到货单状态
        public int CompanyID { get; set; } // 公司ID
        public string CompanyName { get; set; } = string.Empty; // 公司名称
        public int SupplierID { get; set; } // 供应商ID
        public string SupplierName { get; set; } = string.Empty; // 供应商名称
        public int LocationID { get; set; } // 到货站点ID
        public string LocationName { get; set; } = string.Empty; // 到货站点名称
        public DateTime? ArrivalTime { get; set; } // 实际到货时间
        public int GoodsCount { get; set; } // 商品品种数
        public decimal TotalQuantity { get; set; } // 到货总数量
        public decimal TotalAmount { get; set; } // 到货总金额
        public string? Remark { get; set; } // 备注
        public string CreateUserName { get; set; } = string.Empty; // 录单人
        public DateTime CreateTime { get; set; } // 录单时间
        public string? AuditUserName { get; set; } // 审核人
        public DateTime? AuditTime { get; set; } // 审核时间
        public string? StockInUserName { get; set; } // 入库人
        public DateTime? StockInTime { get; set; } // 入库时间
        public string? VoidUserName { get; set; } // 作废人
        public DateTime? VoidTime { get; set; } // 作废时间
        public PageResponse<ArrivalDetailItemResponse> ArrivalDetails { get; set; } = new PageResponse<ArrivalDetailItemResponse>(); // 到货单明细列表

    }
}
