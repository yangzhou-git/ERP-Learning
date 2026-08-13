namespace ERP.Server.Models.Entity;
    public class GoodsCategory
    {
        public int CategoryID { get; set; }//商品分类ID
        public int? ParentID { get; set; }//上级分类ID
        public string CategoryName { get; set; } = string.Empty;//商品分类名称
        public string CategoryCode { get; set; } = string.Empty;//商品分类编码
        public int Sort { get; set; }//排序
        public bool IsActive { get; set; }//是否启用
        public DateTime CreateTime { get; set; }//创建时间
        public DateTime? UpdateTime { get; set; }//修改时间
}
