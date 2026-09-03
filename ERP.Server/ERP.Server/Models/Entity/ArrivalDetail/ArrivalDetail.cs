namespace ERP.Server.Models.Entity
{
    public class ArrivalDetail
    {
        public int ArrivalDetailID { get; set; }//到货明细ID
        public int ArrivalID { get; set; }//到货单ID
        public int GoodsID { get; set; }//商品ID
        public int GoodsUnitID { get; set; }//商品单位关系ID
        public decimal Quantity { get; set; }//到货数量
        public decimal UnitCost { get; set; }//单位成本
        public decimal Amount { get; set; }//明细金额
        public string? Remark { get; set; }//备注
        public int CreateUserID { get; set; }//创建人ID
        public DateTime CreateTime { get; set; }//创建时间
        public int? UpdateUserID { get; set; }//最后修改人ID
        public DateTime? UpdateTime { get; set; }//最后修改时间
        public string? BarCode { get; set; }//商品条码
        public string? GoodsCode { get; set; }//商品编码
        public string GoodsTitle { get; set; } = string.Empty;//商品名称
        public string? GoodsSubTitle { get; set; }//商品副标题
        public int UnitID { get; set; }//单位ID
        public string UnitCode { get; set; } = string.Empty;//单位编码
        public string UnitName { get; set; } = string.Empty;//单位名称
        public decimal ConversionRate { get; set; }//单位换算率
        public decimal BaseQuantity { get; set; }//基础单位数量
    }
}
