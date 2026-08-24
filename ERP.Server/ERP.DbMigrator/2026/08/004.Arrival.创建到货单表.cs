using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._08
{
    [Migration(20260822004, "创建Arrival到货单表")]
    public class Arrival_20260822004(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 Arrival 表创建 [{GetType().Name}] ......\x1b[0m");
            Create.Table("Arrival")
               .WithColumn("ArrivalID").AsInt32().NotNullable().PrimaryKey().Identity()
               .WithColumn("ArrivalCode").AsString(32).NotNullable()
               .WithColumn("SupplierID").AsInt32().NotNullable().ForeignKey("Supplier", "SupplierID")
               .WithColumn("CompanyID").AsInt32().NotNullable().ForeignKey("Company", "CompanyID")
               .WithColumn("LocationID").AsInt32().NotNullable().ForeignKey("Location", "LocationID")
               .WithColumn("ArrivalStatus").AsInt32().NotNullable().WithDefaultValue(0)
               .WithColumn("ArrivalTime").AsDateTime().Nullable()
               .WithColumn("TotalAmount").AsDecimal(18, 6).NotNullable().WithDefaultValue(0)
               .WithColumn("AuditUserID").AsInt32().Nullable().ForeignKey("Users", "UserID")
               .WithColumn("AuditTime").AsDateTime().Nullable()
               .WithColumn("Remark").AsString(500).Nullable()
               .WithColumn("CreateUserID").AsInt32().NotNullable().ForeignKey("Users", "UserID")
               .WithColumn("CreateTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
               .WithColumn("UpdateUserID").AsInt32().Nullable().ForeignKey("Users", "UserID")
               .WithColumn("UpdateTime").AsDateTime().Nullable();

            Create.Index("UX_Arrival_ArrivalCode").OnTable("Arrival").OnColumn("ArrivalCode").Ascending().WithOptions().Unique();
            Execute.Sql(@"ALTER TABLE Arrival ADD CONSTRAINT CK_Arrival_TotalAmount CHECK (TotalAmount >= 0)");
            Execute.Sql(@"ALTER TABLE Arrival ADD CONSTRAINT CK_Arrival_ArrivalStatus CHECK (ArrivalStatus IN (0, 1, 2, 3))");
            Logger.LogInformation( $"ERP_DB 数据库 Arrival 表创建过程已执行完成 [{GetType().Name}]");
        }
        public override void Down()
        {
            Delete.Table("Arrival");
        }
    }
}