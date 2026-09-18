using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260914009, "Arrival新增反审核审计字段")]
    public class Arrival_20260914009(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 Arrival 新增反审核审计字段 [{GetType().Name}] ......\x1b[0m");
            Alter.Table("Arrival").AddColumn("CancelAuditUserID").AsInt32().Nullable()
                                  .AddColumn("CancelAuditTime").AsDateTime().Nullable();
            Logger.LogInformation($"\x1b[32m执行 ERP_DB 数据库 Arrival 新增反审核审计字段完成 [{GetType().Name}] ......\x1b[0m");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 Arrival 反审核审计字段 [{GetType().Name}] ......\x1b[0m");
            Delete.Column("CancelAuditUserID").FromTable("Arrival");
            Delete.Column("CancelAuditTime").FromTable("Arrival");
            Logger.LogInformation($"\x1b[32m回滚 ERP_DB 数据库 Arrival 反审核审计字段完成 [{GetType().Name}] ......\x1b[0m");
        }
    }
}