using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator._2026._09
{
    [Migration(20260901001, "调整Goods商品表，移除Unit字段并补充审计字段")]
    public class Goods_20260901001(ILogger<DbMigration> logger) : DbMigration(logger)
    {
        public override void Up()
        {
            Logger.LogInformation($"\x1b[34m正在执行 ERP_DB 数据库 Goods 表调整 [{GetType().Name}] ......\x1b[0m");
            Delete.Column("Unit").FromTable("Goods");
            Alter.Table("Goods").AddColumn("CreateUserID").AsInt32().NotNullable()
                                .AddColumn("UpdateUserID").AsInt32().Nullable();
            Create.ForeignKey("FK_Goods_CreateUserID_Users")
                    .FromTable("Goods").ForeignColumn("CreateUserID")
                    .ToTable("Users").PrimaryColumn("UserID");
            Create.ForeignKey("FK_Goods_UpdateUserID_Users")
                    .FromTable("Goods").ForeignColumn("UpdateUserID")
                    .ToTable("Users").PrimaryColumn("UserID");
            Logger.LogInformation($"ERP_DB 数据库 Goods 表调整过程已执行完成 [{GetType().Name}]");
        }

        public override void Down()
        {
            Logger.LogInformation($"\x1b[33m正在还原 ERP_DB 数据库 Goods 表调整 [{GetType().Name}] ......\x1b[0m");
            Delete.ForeignKey("FK_Goods_UpdateUserID_Users").OnTable("Goods");
            Delete.ForeignKey("FK_Goods_CreateUserID_Users").OnTable("Goods");
            Delete.Column("UpdateUserID").FromTable("Goods");
            Delete.Column("CreateUserID").FromTable("Goods");
            Alter.Table("Goods").AddColumn("Unit").AsString(20).NotNullable();
            Logger.LogInformation($"ERP_DB 数据库 Goods 表调整还原过程已执行完成 [{GetType().Name}]");
        }
    }
}
