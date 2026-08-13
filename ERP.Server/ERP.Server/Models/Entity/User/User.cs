namespace ERP.Server.Models.Entity
{
    public class User
    {
        public int UserID { get; set; }// 用户ID 唯一主键
        public string UserName { get; set; }//用户名
        public string Password { get; set; }//密码
        public string Email { get; set; }//邮箱
        public string Phone { get; set; }//电话
        public int CompanyID { get; set; }//所属公司ID
        public bool IsActive { get; set; }// 是否在职/可用状态
        public DateTime CreateTime { get; set; } // 账号创建时间
        public DateTime? UpdateTime { get; set; }//修改时间
    }
}
