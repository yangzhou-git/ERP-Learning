using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260902003, "ArrivalDetail表补充历史快照字段")]
    public class ArrivalDetail_20260902003(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 ArrivalDetail 表补充历史快照字段 [{GetType().Name}] ......\x1b[0m");
            Alter.Table("ArrivalDetail").AddColumn("BarCode").AsString(20).Nullable()
                                        .AddColumn("GoodsCode").AsString(50).Nullable()
                                        .AddColumn("GoodsTitle").AsString(200).NotNullable()
                                        .AddColumn("GoodsSubTitle").AsString(200).Nullable()
                                        .AddColumn("UnitID").AsInt32().NotNullable().ForeignKey("FK_ArrivalDetail_UnitID_Unit", "Unit", "UnitID")
                                        .AddColumn("UnitCode").AsString(32).NotNullable()
                                        .AddColumn("UnitName").AsString(32).NotNullable()
                                        .AddColumn("ConversionRate").AsDecimal(18, 6).NotNullable()
                                        .AddColumn("BaseQuantity").AsDecimal(18, 6).NotNullable();
            Execute.Sql(@"ALTER TABLE ArrivalDetail DROP CONSTRAINT CK_ArrivalDetail_Quantity");
            Alter.Column("Quantity").OnTable("ArrivalDetail").AsDecimal(18, 6).NotNullable();
            Execute.Sql(@"ALTER TABLE ArrivalDetail ADD CONSTRAINT CK_ArrivalDetail_Quantity CHECK (Quantity > 0)");
            Execute.Sql(@"ALTER TABLE ArrivalDetail ADD CONSTRAINT CK_ArrivalDetail_ConversionRate CHECK (ConversionRate > 0)");
            Execute.Sql(@"ALTER TABLE ArrivalDetail ADD CONSTRAINT CK_ArrivalDetail_BaseQuantity CHECK (BaseQuantity > 0)");
            Logger.LogInformation($"ERP_DB 数据库 ArrivalDetail 表补充历史快照字段过程已执行完成 [{GetType().Name}]");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 ArrivalDetail 表历史快照字段 [{GetType().Name}] ......\x1b[0m");
            Delete.ForeignKey("FK_ArrivalDetail_UnitID_Unit").OnTable("ArrivalDetail");
            Execute.Sql(@"ALTER TABLE ArrivalDetail DROP CONSTRAINT CK_ArrivalDetail_Quantity");
            Execute.Sql(@"ALTER TABLE ArrivalDetail DROP CONSTRAINT CK_ArrivalDetail_ConversionRate");
            Execute.Sql(@"ALTER TABLE ArrivalDetail DROP CONSTRAINT CK_ArrivalDetail_BaseQuantity");
            Alter.Column("Quantity").OnTable("ArrivalDetail").AsInt32().NotNullable();
            Execute.Sql(@"ALTER TABLE ArrivalDetail ADD CONSTRAINT CK_ArrivalDetail_Quantity CHECK (Quantity > 0)");
            Delete.Column("BarCode").FromTable("ArrivalDetail");
            Delete.Column("GoodsCode").FromTable("ArrivalDetail");
            Delete.Column("GoodsTitle").FromTable("ArrivalDetail");
            Delete.Column("GoodsSubTitle").FromTable("ArrivalDetail");
            Delete.Column("UnitID").FromTable("ArrivalDetail");
            Delete.Column("UnitCode").FromTable("ArrivalDetail");
            Delete.Column("UnitName").FromTable("ArrivalDetail");
            Delete.Column("ConversionRate").FromTable("ArrivalDetail");
            Delete.Column("BaseQuantity").FromTable("ArrivalDetail");
            Logger.LogInformation($"ERP_DB 数据库 ArrivalDetail 表历史快照字段回滚完成 [{GetType().Name}]");
        }
    }
}
