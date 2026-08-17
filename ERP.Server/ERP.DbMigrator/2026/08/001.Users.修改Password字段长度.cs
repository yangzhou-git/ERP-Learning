using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._08
{
    [Migration(20260817001, "Users表Password字段长度修改为255")]
    public class Users_20260817001(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 Users 表 Password 字段更新 [{GetType().Name}] ......\x1b[0m");
            Alter.Table("Users").AlterColumn("Password").AsString(255).NotNullable();
            Logger.LogInformation($"ERP_DB 数据库 Users 表 Password 字段更新过程已执行完成 [{GetType().Name}]");
        }

        public override void Down()
        {
            Alter.Table("Users").AlterColumn("Password").AsString(100).NotNullable();
        }
    }
}