using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._08
{
    [Migration(20260822002, "创建Unit单位表")]
    public class Unit_20260822002(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 Unit 表创建 [{GetType().Name}] ......\x1b[0m");
            Create.Table("Unit").WithColumn("UnitID").AsInt32().NotNullable().PrimaryKey().Identity()
                                .WithColumn("UnitName").AsString(32).NotNullable()
                                .WithColumn("UnitCode").AsString(32).NotNullable()
                                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                                .WithColumn("Sort").AsInt32().NotNullable().WithDefaultValue(0)
                                .WithColumn("CreateUserID").AsInt32().NotNullable().ForeignKey("Users", "UserID")
                                .WithColumn("CreateTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                                .WithColumn("UpdateUserID").AsInt32().Nullable().ForeignKey("Users", "UserID")
                                .WithColumn("UpdateTime").AsDateTime().Nullable();
            Create.Index("UX_Unit_UnitName").OnTable("Unit").OnColumn("UnitName").Ascending().WithOptions().Unique();
            Create.Index("UX_Unit_UnitCode").OnTable("Unit").OnColumn("UnitCode").Ascending().WithOptions().Unique();
            Logger.LogInformation($"ERP_DB 数据库 Unit 表创建过程已执行完成 [{GetType().Name}]");
        }
        public override void Down()
        {
            Delete.Index("UX_Unit_UnitCode").OnTable("Unit");
            Delete.Index("UX_Unit_UnitName").OnTable("Unit");
            Delete.Table("Unit");
        }
    }
}
