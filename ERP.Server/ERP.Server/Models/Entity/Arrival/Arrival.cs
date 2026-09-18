using ERP.Server.Models.Enums;

namespace ERP.Server.Models.Entity
{
    public class Arrival
    {
        public int ArrivalID { get; set; }//到货单ID
        public string ArrivalCode { get; set; } = string.Empty;//到货单编码
        public int SupplierID { get; set; }//供应商ID
        public int CompanyID { get; set; }//公司ID
        public int LocationID { get; set; }//到货站点ID
        public ArrivalStatus ArrivalStatus { get; set; }//到货单状态
        public DateTime? ArrivalTime { get; set; }//实际到货时间
        public decimal TotalAmount { get; set; }//到货总金额
        public int? AuditUserID { get; set; }//审核人ID
        public DateTime? AuditTime { get; set; }//审核时间
        public int? CancelAuditUserID { get; set; } // 反审核人ID
        public DateTime? CancelAuditTime { get; set; } // 反审核时间
        public string? Remark { get; set; }//备注
        public int CreateUserID { get; set; }//创建人ID
        public DateTime CreateTime { get; set; }//创建时间
        public int? UpdateUserID { get; set; }//最后修改人ID
        public DateTime? UpdateTime { get; set; }//最后修改时间
        public int? StockInUserID { get; set; }//入库人ID
        public DateTime? StockInTime { get; set; }//入库时间
        public int? CancelStockInUserID { get; set; } // 撤销入库人ID
        public DateTime? CancelStockInTime { get; set; } // 撤销入库时间
        public int? VoidUserID { get; set; }//作废人ID
        public DateTime? VoidTime { get; set; }//作废时间
    }
}