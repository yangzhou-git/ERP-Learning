using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260912006, "Inventory库存表新增SupplierID")]
    public class Inventory_20260912006(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 Inventory 库存表新增 SupplierID [{GetType().Name}] ......\x1b[0m");
            Alter.Table("Inventory").AddColumn("SupplierID").AsInt32().NotNullable().ForeignKey("FK_Inventory_SupplierID_Supplier", "Supplier", "SupplierID");
            Delete.Index("UX_Inventory_CompanyID_LocationID_GoodsID").OnTable("Inventory");
            Create.Index("UX_Inventory_CompanyID_LocationID_GoodsID_SupplierID").OnTable("Inventory").OnColumn("CompanyID").Ascending().OnColumn("LocationID").Ascending().OnColumn("GoodsID").Ascending().OnColumn("SupplierID").Ascending().WithOptions().Unique();
            Logger.LogInformation($"ERP_DB 数据库 Inventory 库存表新增 SupplierID 已执行完成 [{GetType().Name}]");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 Inventory 库存表 SupplierID [{GetType().Name}] ......\x1b[0m");
            Delete.Index("UX_Inventory_CompanyID_LocationID_GoodsID_SupplierID").OnTable("Inventory");
            Delete.ForeignKey("FK_Inventory_SupplierID_Supplier").OnTable("Inventory");
            Delete.Column("SupplierID").FromTable("Inventory");
            Create.Index("UX_Inventory_CompanyID_LocationID_GoodsID").OnTable("Inventory").OnColumn("CompanyID").Ascending().OnColumn("LocationID").Ascending().OnColumn("GoodsID").Ascending().WithOptions().Unique();
            Logger.LogInformation($"ERP_DB 数据库 Inventory 库存表 SupplierID 回滚完成 [{GetType().Name}]");
        }
    }
}
