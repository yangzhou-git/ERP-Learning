using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260912007, "InventoryTransaction库存流水表新增SupplierID")]
    public class InventoryTransaction_20260912007(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 InventoryTransaction 库存流水表新增 SupplierID [{GetType().Name}] ......\x1b[0m");
            Alter.Table("InventoryTransaction").AddColumn("SupplierID").AsInt32().NotNullable().ForeignKey("FK_InventoryTransaction_SupplierID_Supplier", "Supplier", "SupplierID");
            Logger.LogInformation($"ERP_DB 数据库 InventoryTransaction 库存流水表新增 SupplierID 已执行完成 [{GetType().Name}]");
        }
        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 InventoryTransaction 库存流水表 SupplierID [{GetType().Name}] ......\x1b[0m");
            Delete.ForeignKey("FK_InventoryTransaction_SupplierID_Supplier").OnTable("InventoryTransaction");
            Delete.Column("SupplierID").FromTable("InventoryTransaction");
            Logger.LogInformation($"ERP_DB 数据库 InventoryTransaction 库存流水表 SupplierID 回滚完成 [{GetType().Name}]");
        }
    }
}
