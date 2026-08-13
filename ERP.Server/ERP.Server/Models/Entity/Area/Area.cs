namespace ERP.Server.Models.Entity
{
    public class Area
    {
        public int AreaID { get; set; }//地区ID
        public string AreaCode { get; set; }//地区编码
        public string AreaName { get; set; }//地区名称
        public int? ParentID { get; set; }//父级地区ID
        public int AreaLevel { get; set; }//地区级别
        public string? ZipCode { get; set; }//邮政编码
        public bool IsActive { get; set; }//是否启用
        public DateTime CreateTime { get; set; }//创建时间
        public DateTime? UpdateTime { get; set; }//修改时间
        public int Sort { get; set; }//排序
    }
}
