using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._10
{
    [Migration(20261014001, "ArrivalDetail新增采购单位单价字段")]
    public class ArrivalDetail_20261014001(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation("\x1b[34m正在执行 ERP_DB 数据库 ArrivalDetail 新增采购单位单价字段 [{Migration}] ......\x1b[0m", GetType().Name);
            Alter.Table("ArrivalDetail").AddColumn("PurchaseUnitCost").AsDecimal(18, 6).NotNullable();
            Logger.LogInformation("\x1b[32mERP_DB 数据库 ArrivalDetail 新增采购单位单价字段完成 [{Migration}]\x1b[0m", GetType().Name);
        }

        public override void Down()
        {
            Logger.LogInformation("\x1b[34m正在回滚 ERP_DB 数据库 ArrivalDetail 采购单位单价字段 [{Migration}] ......\x1b[0m", GetType().Name);
            Delete.Column("PurchaseUnitCost").FromTable("ArrivalDetail");
            Logger.LogInformation("\x1b[32mERP_DB 数据库 ArrivalDetail 采购单位单价字段回滚完成 [{Migration}]\x1b[0m", GetType().Name);
        }
    }
}