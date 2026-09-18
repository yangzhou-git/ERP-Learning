using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260914008, "Arrival新增撤销入库审计字段")]
    public class Arrival_20260914008(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 Arrival 新增撤销入库审计字段 [{GetType().Name}] ......\x1b[0m");
            Alter.Table("Arrival").AddColumn("CancelStockInUserID").AsInt32().Nullable()
                                  .AddColumn("CancelStockInTime").AsDateTime().Nullable();
            Logger.LogInformation($"\x1b[32m执行 ERP_DB 数据库 Arrival 新增撤销入库审计字段完成 [{GetType().Name}] ......\x1b[0m");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 Arrival 撤销入库审计字段 [{GetType().Name}] ......\x1b[0m");
            Delete.Column("CancelStockInUserID").FromTable("Arrival");
            Delete.Column("CancelStockInTime").FromTable("Arrival");
            Logger.LogInformation($"\x1b[32m回滚 ERP_DB 数据库 Arrival 撤销入库审计字段完成 [{GetType().Name}] ......\x1b[0m");
        }
    }
}