using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260915010, "ArrivalDetail新增软删除字段")]
    public class ArrivalDetail_20260915010(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 ArrivalDetail 新增软删除字段 [{GetType().Name}] ......\x1b[0m");
            Alter.Table("ArrivalDetail").AddColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);
            Logger.LogInformation($"\x1b[32m执行 ERP_DB 数据库 ArrivalDetail 新增软删除字段 [{GetType().Name}] 完成 ......\x1b[0m");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[31m正在回滚 ERP_DB 数据库 ArrivalDetail 删除软删除字段 [{GetType().Name}] ......\x1b[0m");
            Delete.Column("IsDeleted").FromTable("ArrivalDetail");
            Logger.LogInformation($"\x1b[32m回滚 ERP_DB 数据库 ArrivalDetail 删除软删除字段 [{GetType().Name}] 完成 ......\x1b[0m");
        }
    }
}