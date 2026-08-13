using System.ComponentModel.DataAnnotations;

namespace ERP.Server.Models.Request
{
    public class AddUserRequest
    {
        public string UserName { get; set; } = string.Empty;//用户名
        public string Password { get; set; } = string.Empty;//密码
        public string Email { get; set; } = string.Empty;//邮箱
        public string Phone {  get; set; } //电话
        public int CompanyID { get; set; }//所属公司ID
    }
}
