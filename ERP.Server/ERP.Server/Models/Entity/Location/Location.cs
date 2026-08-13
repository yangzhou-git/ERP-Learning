namespace ERP.Server.Models.Entity
{
    public class Location
    {
        public int LocationID { get; set; }//站点ID
        public int CompanyID { get; set; }//公司ID
        public string? CompanyName { get; set; }//公司名称
        public string LocationCode { get; set; }//站点编号
        public string LocationName { get; set; }//站点名称
        public int? ParentID { get; set; }//父级站点ID
        public bool CanSale { get; set; }//是否允许销售
        public bool CanStock { get; set; }//是否允许库存
        public bool CanReceive { get; set; }//是否允许收货
        public string? ProvinceID { get; set; }//省份编码
        public string? CityID { get; set; }//城市编码
        public string? DistrictID { get; set; }//区县编码
        public string? Address { get; set; }//详细地址
        public string? Contact { get; set; }//联系人
        public string? Phone { get; set; }//联系电话
        public bool IsActive { get; set; }//是否启用
        public DateTime CreateTime { get; set; }//创建时间
        public DateTime? UpdateTime { get; set; }//修改时间
    }
}
