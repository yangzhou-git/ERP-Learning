using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._08
{
    [Migration(20260822003, "创建GoodsUnit商品单位关系表")]
    public class GoodsUnit_20260822003(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 GoodsUnit 表创建 [{GetType().Name}] ......\x1b[0m");
            Create.Table("GoodsUnit").WithColumn("GoodsUnitID").AsInt32().NotNullable().PrimaryKey().Identity()
                                     .WithColumn("GoodsID").AsInt32().NotNullable().ForeignKey("Goods", "GoodsID")
                                     .WithColumn("UnitID").AsInt32().NotNullable().ForeignKey("Unit", "UnitID")
                                     .WithColumn("ConversionRate").AsDecimal(18, 6).NotNullable()
                                     .WithColumn("IsBaseUnit").AsBoolean().NotNullable().WithDefaultValue(false)
                                     .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                                     .WithColumn("Sort").AsInt32().NotNullable().WithDefaultValue(0)
                                     .WithColumn("CreateUserID").AsInt32().NotNullable().ForeignKey("Users", "UserID")
                                     .WithColumn("CreateTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                                     .WithColumn("UpdateUserID").AsInt32().Nullable().ForeignKey("Users", "UserID")
                                     .WithColumn("UpdateTime").AsDateTime().Nullable();
            Create.Index("UX_GoodsUnit_GoodsID_UnitID").OnTable("GoodsUnit").OnColumn("GoodsID").Ascending().OnColumn("UnitID").Ascending().WithOptions().Unique();
            Execute.Sql(@" ALTER TABLE GoodsUnit ADD CONSTRAINT CK_GoodsUnit_ConversionRate CHECK (ConversionRate > 0)");
            Execute.Sql(@"ALTER TABLE GoodsUnit ADD CONSTRAINT CK_GoodsUnit_BaseUnitConversionRate CHECK (IsBaseUnit = 0 OR ConversionRate = 1)");
            Execute.Sql(@"CREATE UNIQUE INDEX UX_GoodsUnit_BaseUnit ON GoodsUnit(GoodsID) WHERE IsBaseUnit = 1");
            Logger.LogInformation($"ERP_DB 数据库 GoodsUnit 表创建过程已执行完成 [{GetType().Name}]");
        }
        public override void Down()
        {
            Delete.Index("UX_GoodsUnit_BaseUnit").OnTable("GoodsUnit");
            Delete.Table("GoodsUnit");
        }
    }
}
