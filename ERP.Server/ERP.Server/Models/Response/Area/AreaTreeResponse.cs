namespace ERP.Server.Models.Response
{ 
    public class AreaTreeResponse
    {
        public int AreaID { get; set; }//地区ID
        public string AreaCode { get; set; }//地区编码
        public string AreaName { get; set; }//地区名称
        public int? ParentID { get; set; }//父级地区ID
        public int AreaLevel { get; set; }//地区级别
    }
}
