using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._10
{
    [Migration(20261016001, "InventoryTransaction新增撤销流水关联字段")]
    public class InventoryTransaction_20261016001(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 InventoryTransaction 新增撤销流水关联字段 [{GetType().Name}] ......\x1b[0m");
            Alter.Table("InventoryTransaction").AddColumn("ReversalOfTransactionID").AsInt32().Nullable();
            Execute.Sql(@"CREATE UNIQUE NONCLUSTERED INDEX UX_InventoryTransaction_ReversalOfTransactionID
                          ON InventoryTransaction (ReversalOfTransactionID)
                          WHERE ReversalOfTransactionID IS NOT NULL");
            Logger.LogInformation($"\x1b[32mERP_DB 数据库 InventoryTransaction 新增撤销流水关联字段执行完成 [{GetType().Name}]\x1b[0m");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 InventoryTransaction 撤销流水关联字段 [{GetType().Name}] ......\x1b[0m");
            Delete.Index("UX_InventoryTransaction_ReversalOfTransactionID").OnTable("InventoryTransaction");
            Delete.Column("ReversalOfTransactionID").FromTable("InventoryTransaction");
            Logger.LogInformation($"\x1b[32mERP_DB 数据库 InventoryTransaction 撤销流水关联字段回滚完成 [{GetType().Name}]\x1b[0m");
        }
    }
}