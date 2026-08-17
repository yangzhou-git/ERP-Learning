namespace ERP.Server.Models.Entity
{
    public class Goods
    {
        public int GoodsID { get; set; }//商品ID
        public string BarCode { get; set; } = string.Empty;//商品条码
        public string GoodsCode { get; set; } = string.Empty;//商家/系统商品编码
        public int CategoryID { get; set; }//分类ID
        public string GoodsTitle { get; set; } = string.Empty;//商品名称/书名
        public string? GoodsSubTitle { get; set; }//商品副名
        public decimal GoodsPrice { get; set; }//商品价格
        public string Unit { get; set; } = string.Empty;//商品单位
        public bool IsActive { get; set; }//是否启用
        public DateTime CreateTime { get; set; }//商品创建时间
        public DateTime? UpdateTime { get; set; }//商品修改时间
    }
}