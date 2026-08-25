namespace ERP.Server.Models.Request
{
    public class UpdateUnitRequest
    {
        public int UnitID { get; set; }//单位ID
        public string UnitName { get; set; }//单位名称
        public string UnitCode { get; set; }//单位编码
    }
}
