using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260902004, "创建Inventory库存表")]
    public class Inventory_20260902004(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 Inventory 库存表创建 [{GetType().Name}] ......\x1b[0m");
            Create.Table("Inventory").WithColumn("InventoryID").AsInt32().NotNullable().PrimaryKey().Identity()
                                     .WithColumn("CompanyID").AsInt32().NotNullable().ForeignKey("FK_Inventory_CompanyID_Company", "Company", "CompanyID")
                                     .WithColumn("LocationID").AsInt32().NotNullable().ForeignKey("FK_Inventory_LocationID_Location", "Location", "LocationID")
                                     .WithColumn("GoodsID").AsInt32().NotNullable().ForeignKey("FK_Inventory_GoodsID_Goods", "Goods", "GoodsID")
                                     .WithColumn("Quantity").AsDecimal(18, 6).NotNullable().WithDefaultValue(0)
                                     .WithColumn("InventoryAmount").AsDecimal(18, 6).NotNullable().WithDefaultValue(0)
                                     .WithColumn("AverageCost").AsDecimal(18, 6).NotNullable().WithDefaultValue(0)
                                     .WithColumn("CreateUserID").AsInt32().NotNullable().ForeignKey("FK_Inventory_CreateUserID_Users", "Users", "UserID")
                                     .WithColumn("CreateTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                                     .WithColumn("UpdateUserID").AsInt32().Nullable().ForeignKey("FK_Inventory_UpdateUserID_Users", "Users", "UserID")
                                     .WithColumn("UpdateTime").AsDateTime().Nullable();
            Create.Index("UX_Inventory_CompanyID_LocationID_GoodsID").OnTable("Inventory").OnColumn("CompanyID").Ascending().OnColumn("LocationID").Ascending().OnColumn("GoodsID").Ascending().WithOptions().Unique();
            Execute.Sql(@"ALTER TABLE Inventory ADD CONSTRAINT CK_Inventory_Quantity CHECK (Quantity >= 0)");
            Execute.Sql(@"ALTER TABLE Inventory ADD CONSTRAINT CK_Inventory_InventoryAmount CHECK (InventoryAmount >= 0)");
            Execute.Sql(@"ALTER TABLE Inventory ADD CONSTRAINT CK_Inventory_AverageCost CHECK (AverageCost >= 0)");
            Logger.LogInformation($"ERP_DB 数据库 Inventory 库存表创建过程已执行完成 [{GetType().Name}]");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 Inventory 库存表 [{GetType().Name}] ......\x1b[0m");
            Delete.Table("Inventory");
            Logger.LogInformation($"ERP_DB 数据库 Inventory 库存表回滚完成 [{GetType().Name}]");
        }
    }
}