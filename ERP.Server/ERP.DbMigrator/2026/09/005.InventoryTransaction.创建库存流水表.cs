using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260902005, "创建InventoryTransaction库存流水表")]
    public class InventoryTransaction_20260902005(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 InventoryTransaction 库存流水表创建 [{GetType().Name}] ......\x1b[0m");

            Create.Table("InventoryTransaction").WithColumn("InventoryTransactionID").AsInt32().NotNullable().PrimaryKey().Identity()
                                                .WithColumn("InventoryID").AsInt32().NotNullable().ForeignKey("FK_InventoryTransaction_InventoryID_Inventory", "Inventory", "InventoryID")
                                                .WithColumn("CompanyID").AsInt32().NotNullable().ForeignKey("FK_InventoryTransaction_CompanyID_Company", "Company", "CompanyID")
                                                .WithColumn("LocationID").AsInt32().NotNullable().ForeignKey("FK_InventoryTransaction_LocationID_Location", "Location", "LocationID")
                                                .WithColumn("GoodsID").AsInt32().NotNullable().ForeignKey("FK_InventoryTransaction_GoodsID_Goods", "Goods", "GoodsID")
                                                .WithColumn("TransactionType").AsInt32().NotNullable()
                                                .WithColumn("SourceID").AsInt32().NotNullable()
                                                .WithColumn("SourceDetailID").AsInt32().NotNullable()
                                                .WithColumn("SourceCode").AsString(32).NotNullable()
                                                .WithColumn("BeforeQuantity").AsDecimal(18, 6).NotNullable()
                                                .WithColumn("QuantityChange").AsDecimal(18, 6).NotNullable()
                                                .WithColumn("AfterQuantity").AsDecimal(18, 6).NotNullable()
                                                .WithColumn("BeforeInventoryAmount").AsDecimal(18, 6).NotNullable()
                                                .WithColumn("AmountChange").AsDecimal(18, 6).NotNullable()
                                                .WithColumn("AfterInventoryAmount").AsDecimal(18, 6).NotNullable()
                                                .WithColumn("UnitCost").AsDecimal(18, 6).NotNullable()
                                                .WithColumn("BeforeAverageCost").AsDecimal(18, 6).NotNullable()
                                                .WithColumn("AfterAverageCost").AsDecimal(18, 6).NotNullable()
                                                .WithColumn("Remark").AsString(500).Nullable()
                                                .WithColumn("CreateUserID").AsInt32().NotNullable().ForeignKey("FK_InventoryTransaction_CreateUserID_Users", "Users", "UserID")
                                                .WithColumn("CreateTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            Create.Index("IX_InventoryTransaction_InventoryID_CreateTime")
                .OnTable("InventoryTransaction")
                .OnColumn("InventoryID").Ascending()
                .OnColumn("CreateTime").Descending();
            Create.Index("IX_InventoryTransaction_TransactionType_SourceID")
                .OnTable("InventoryTransaction")
                .OnColumn("TransactionType").Ascending()
                .OnColumn("SourceID").Ascending();

            Execute.Sql(@"ALTER TABLE InventoryTransaction ADD CONSTRAINT CK_InventoryTransaction_BeforeQuantity  CHECK (BeforeQuantity >= 0)");
            Execute.Sql(@"ALTER TABLE InventoryTransaction ADD CONSTRAINT CK_InventoryTransaction_AfterQuantity CHECK (AfterQuantity >= 0)");
            Execute.Sql(@"ALTER TABLE InventoryTransaction ADD CONSTRAINT CK_InventoryTransaction_QuantityChange CHECK (QuantityChange <> 0)");
            Execute.Sql(@"ALTER TABLE InventoryTransaction ADD CONSTRAINT CK_InventoryTransaction_BeforeInventoryAmount CHECK (BeforeInventoryAmount >= 0)");
            Execute.Sql(@"ALTER TABLE InventoryTransaction ADD CONSTRAINT CK_InventoryTransaction_AfterInventoryAmount CHECK (AfterInventoryAmount >= 0)");
            Execute.Sql(@"ALTER TABLE InventoryTransaction ADD CONSTRAINT CK_InventoryTransaction_UnitCost CHECK (UnitCost >= 0)");
            Execute.Sql(@"ALTER TABLE InventoryTransaction ADD CONSTRAINT CK_InventoryTransaction_BeforeAverageCost CHECK (BeforeAverageCost >= 0)");
            Execute.Sql(@"ALTER TABLE InventoryTransaction ADD CONSTRAINT CK_InventoryTransaction_AfterAverageCost CHECK (AfterAverageCost >= 0)");
            Logger.LogInformation($"ERP_DB 数据库 InventoryTransaction 库存流水表创建过程已执行完成 [{GetType().Name}]");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 InventoryTransaction 库存流水表 [{GetType().Name}] ......\x1b[0m");
            Delete.Table("InventoryTransaction");
            Logger.LogInformation($"ERP_DB 数据库 InventoryTransaction 库存流水表回滚完成 [{GetType().Name}]");
        }
    }
}