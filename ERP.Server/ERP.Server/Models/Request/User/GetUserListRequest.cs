namespace ERP.Server.Models.Request
{
    public class GetUserListRequest
    {
        public string? UserName { get; set; }//查询用户名（可为空）
        public int Page {  get; set; }// 当前页码
        public int PageSize { get; set; }// 每页显示条数
    }
}
