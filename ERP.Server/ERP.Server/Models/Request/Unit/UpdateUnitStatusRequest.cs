namespace ERP.Server.Models.Request
{
    public class UpdateUnitStatusRequest
    {
        public int UnitID { get; set; }//单位ID
        public bool IsActive { get; set; }//是否启用
    }
}
