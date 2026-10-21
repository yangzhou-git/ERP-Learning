using ERP.Server.Models.Enums;

namespace ERP.Server.Models.Entity.InventoryTransaction
{
    public class InventoryTransaction
    {
        public int InventoryTransactionID { get; set; } // 库存流水ID
        public int InventoryID { get; set; } // 库存ID
        public int CompanyID { get; set; } // 公司ID
        public int LocationID { get; set; } // 站点ID
        public int GoodsID { get; set; } // 商品ID
        public int SupplierID { get; set; } // 供应商ID

        public InventoryTransactionType TransactionType { get; set; } // 库存流水类型

        public int SourceID { get; set; } // 来源主单ID
        public int SourceDetailID { get; set; } // 来源明细ID
        public string SourceCode { get; set; } = string.Empty; // 来源单号

        public decimal BeforeQuantity { get; set; } // 变动前库存数量
        public decimal QuantityChange { get; set; } // 本次数量变化
        public decimal AfterQuantity { get; set; } // 变动后库存数量

        public decimal BeforeInventoryAmount { get; set; } // 变动前库存金额
        public decimal AmountChange { get; set; } // 本次金额变化
        public decimal AfterInventoryAmount { get; set; } // 变动后库存金额

        public decimal UnitCost { get; set; } // 本次基础单位成本
        public decimal BeforeAverageCost { get; set; } // 变动前平均成本
        public decimal AfterAverageCost { get; set; } // 变动后平均成本

        public int? ReversalOfTransactionID { get; set; } // 被撤销的原始库存流水ID，普通流水为NULL，撤销流水记录原始流水ID

        public string? Remark { get; set; } // 备注
        public int CreateUserID { get; set; } // 创建人ID
        public DateTime CreateTime { get; set; } // 创建时间
    }
}