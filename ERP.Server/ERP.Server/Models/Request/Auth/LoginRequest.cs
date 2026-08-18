namespace ERP.Server.Models.Request
{
    public class LoginRequest
    {
        public string UserName { get; set; } = string.Empty;//用户名
        public string Password { get; set; } = string.Empty;//密码
    }
}
