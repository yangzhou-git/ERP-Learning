namespace ERP.Server.Models.Enums
{
    public enum ArrivalStatus
    {
        Pending = 0, // 待审核
        Audited = 1, // 已审核
        Stocked = 2,// 已入库
        Voided = 3 // 已作废
    }
}
