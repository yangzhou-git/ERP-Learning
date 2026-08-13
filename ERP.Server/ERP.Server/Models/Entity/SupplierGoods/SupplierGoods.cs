namespace ERP.Server.Models.Entity
{
    public class SupplierGoods
    {
        public int SupplierGoodsID { get; set; } //商品供应商关系ID
        public int SupplierID { get; set; } //供应商ID
        public int GoodsID { get; set; } //商品ID
        public decimal PurchasePrice { get; set; } //采购价格
        public bool IsDefault { get; set; } = false; //是否默认供应商
        public bool IsActive { get; set; } = true; //是否启用
        public DateTime CreateTime { get; set; } //创建时间
        public DateTime? UpdateTime { get; set; } //修改时间
    }
}
