using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260927001, "修改BusinessCodeSeed编号字段名称")]
    public class BusinessCodeSeed_20260927001(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 BusinessCodeSeed 编号字段名称修改 [{GetType().Name}] ......\x1b[0m");
            Rename.Column("CurrentValue").OnTable("BusinessCodeSeed").To("CurrentNumber");
            Rename.Column("CurrentDate").OnTable("BusinessCodeSeed").To("NumberDate");
            Logger.LogInformation($"\x1b[32mERP_DB 数据库 BusinessCodeSeed 编号字段名称修改完成 [{GetType().Name}]\x1b[0m");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在回滚 ERP_DB 数据库 BusinessCodeSeed 编号字段名称修改 [{GetType().Name}] ......\x1b[0m");
            Rename.Column("CurrentNumber").OnTable("BusinessCodeSeed").To("CurrentValue");
            Rename.Column("NumberDate").OnTable("BusinessCodeSeed").To("CurrentDate");
            Logger.LogInformation($"\x1b[32mERP_DB 数据库 BusinessCodeSeed 编号字段名称修改回滚完成 [{GetType().Name}]\x1b[0m");
        }
    }
}