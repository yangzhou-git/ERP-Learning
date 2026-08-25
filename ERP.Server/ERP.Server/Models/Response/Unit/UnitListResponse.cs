namespace ERP.Server.Models.Response
{
    public class UnitListResponse
    {
        public int UnitID { get; set; }//单位ID
        public string UnitName { get; set; }//单位名称
        public string UnitCode { get; set; }//单位编码
        public bool IsActive { get; set; }//是否启用
        public int Sort { get; set; }//排序
        public int CreateUserID { get; set; }//创建人ID
        public DateTime CreateTime { get; set; }//创建时间
        public int? UpdateUserID { get; set; }//修改人ID
        public DateTime? UpdateTime { get; set; }//修改时间
    }
}