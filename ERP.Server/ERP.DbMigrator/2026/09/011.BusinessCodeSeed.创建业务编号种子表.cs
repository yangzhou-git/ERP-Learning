using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260921011, "创建BusinessCodeSeed业务编号种子表")]
    public class BusinessCodeSeed_20260921011(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 BusinessCodeSeed 业务编号种子表创建 [{GetType().Name}] ......\x1b[0m");
            Create.Table("BusinessCodeSeed").WithColumn("BusinessCodeSeedID").AsInt32().NotNullable().PrimaryKey().Identity()
                                            .WithColumn("CompanyID").AsInt32().NotNullable().ForeignKey("FK_BusinessCodeSeed_CompanyID_Company", "Company", "CompanyID")
                                            .WithColumn("BusinessType").AsInt32().NotNullable()
                                            .WithColumn("Prefix").AsString(20).NotNullable()
                                            .WithColumn("NumberLength").AsInt32().NotNullable()
                                            .WithColumn("ResetType").AsInt32().NotNullable()
                                            .WithColumn("CurrentValue").AsInt32().NotNullable().WithDefaultValue(0)
                                            .WithColumn("CurrentDate").AsDate().Nullable()
                                            .WithColumn("CreateUserID").AsInt32().NotNullable().ForeignKey("FK_BusinessCodeSeed_CreateUserID_Users", "Users", "UserID")
                                            .WithColumn("CreateTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                                            .WithColumn("UpdateUserID").AsInt32().Nullable().ForeignKey("FK_BusinessCodeSeed_UpdateUserID_Users", "Users", "UserID")
                                            .WithColumn("UpdateTime").AsDateTime().Nullable();
            Create.Index("UX_BusinessCodeSeed_CompanyID_BusinessType").OnTable("BusinessCodeSeed").OnColumn("CompanyID").Ascending().OnColumn("BusinessType").Ascending().WithOptions().Unique();
            Logger.LogInformation($"\x1b[32mERP_DB 数据库 BusinessCodeSeed 业务编号种子表创建完成 [{GetType().Name}]\x1b[0m");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 BusinessCodeSeed 业务编号种子表 [{GetType().Name}] ......\x1b[0m");
            Delete.Table("BusinessCodeSeed");
            Logger.LogInformation($"\x1b[32mERP_DB 数据库 BusinessCodeSeed 业务编号种子表回滚完成 [{GetType().Name}]\x1b[0m");
        }
    }
}