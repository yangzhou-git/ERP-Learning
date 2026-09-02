using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260902002, "Arrival表补充入库和作废审计字段")]
    public class Arrival_20260902002(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 Arrival 表补充入库和作废审计字段 [{GetType().Name}] ......\x1b[0m");
            Alter.Table("Arrival").AddColumn("StockInUserID").AsInt32().Nullable().ForeignKey("FK_Arrival_StockInUserID_Users", "Users", "UserID")
                                  .AddColumn("StockInTime").AsDateTime().Nullable()
                                  .AddColumn("VoidUserID").AsInt32().Nullable().ForeignKey("FK_Arrival_VoidUserID_Users", "Users", "UserID")
                                  .AddColumn("VoidTime").AsDateTime().Nullable();
            Logger.LogInformation($"ERP_DB 数据库 Arrival 表补充入库和作废审计字段过程已执行完成 [{GetType().Name}]");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 Arrival 表入库和作废审计字段 [{GetType().Name}] ......\x1b[0m");
            Delete.ForeignKey("FK_Arrival_StockInUserID_Users").OnTable("Arrival");
            Delete.ForeignKey("FK_Arrival_VoidUserID_Users").OnTable("Arrival");
            Delete.Column("StockInUserID").FromTable("Arrival");
            Delete.Column("StockInTime").FromTable("Arrival");
            Delete.Column("VoidUserID").FromTable("Arrival");
            Delete.Column("VoidTime").FromTable("Arrival");
            Logger.LogInformation($"ERP_DB 数据库 Arrival 表入库和作废审计字段回滚完成 [{GetType().Name}]");
        }
    }
}
