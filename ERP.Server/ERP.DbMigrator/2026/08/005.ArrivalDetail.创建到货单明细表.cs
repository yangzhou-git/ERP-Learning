using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._08
{
    [Migration(20260822005, "创建ArrivalDetail到货单明细表")]
    public class ArrivalDetail_20260822005(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 ArrivalDetail 表创建 [{GetType().Name}] ......\x1b[0m");
            Create.Table("ArrivalDetail")
                .WithColumn("ArrivalDetailID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("ArrivalID").AsInt32().NotNullable().ForeignKey("Arrival", "ArrivalID")
                .WithColumn("GoodsID").AsInt32().NotNullable().ForeignKey("Goods", "GoodsID")
                .WithColumn("GoodsUnitID").AsInt32().NotNullable().ForeignKey("GoodsUnit", "GoodsUnitID")
                .WithColumn("Quantity").AsInt32().NotNullable()
                .WithColumn("UnitCost").AsDecimal(18, 6).NotNullable()
                .WithColumn("Amount").AsDecimal(18, 6).NotNullable()
                .WithColumn("Remark").AsString(500).Nullable()
                .WithColumn("CreateUserID").AsInt32().NotNullable().ForeignKey("Users", "UserID")
                .WithColumn("CreateTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("UpdateUserID").AsInt32().Nullable().ForeignKey("Users", "UserID")
                .WithColumn("UpdateTime").AsDateTime().Nullable();
            Execute.Sql(@"ALTER TABLE ArrivalDetail ADD CONSTRAINT CK_ArrivalDetail_Quantity CHECK (Quantity > 0)");
            Execute.Sql(@"ALTER TABLE ArrivalDetail ADD CONSTRAINT CK_ArrivalDetail_UnitCost CHECK (UnitCost >= 0)");
            Execute.Sql(@"ALTER TABLE ArrivalDetail ADD CONSTRAINT CK_ArrivalDetail_Amount CHECK (Amount >= 0)");
            Logger.LogInformation($"ERP_DB 数据库 ArrivalDetail 表创建过程已执行完成 [{GetType().Name}]");
        }

        public override void Down()
        {
            Delete.Table("ArrivalDetail");
        }
    }
}