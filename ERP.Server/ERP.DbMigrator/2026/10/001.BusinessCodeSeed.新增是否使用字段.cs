using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._10
{
    [Migration(20261010001, "BusinessCodeSeed新增是否使用字段")]
    public class BusinessCodeSeed_20261010001(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 BusinessCodeSeed 新增HasBeenUsed字段 [{GetType().Name}] ......\x1b[0m");
            Alter.Table("BusinessCodeSeed")
                .AddColumn("HasBeenUsed")
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
            Logger.LogInformation($"\x1b[32mERP_DB 数据库 BusinessCodeSeed 新增HasBeenUsed字段完成 [{GetType().Name}]\x1b[0m");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 BusinessCodeSeed 新增HasBeenUsed字段 [{GetType().Name}] ......\x1b[0m");
            Delete.Column("HasBeenUsed").FromTable("BusinessCodeSeed");
            Logger.LogInformation($"\x1b[32mERP_DB 数据库 BusinessCodeSeed 新增HasBeenUsed字段回滚完成 [{GetType().Name}]\x1b[0m");
        }
    }
}