using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace ERP.DbMigrator
{
    public abstract class DbMigration : Migration
    {
        protected readonly ILogger<DbMigration> Logger;

        protected DbMigration(ILogger<DbMigration> logger)
        {
            Logger = logger;
        }
    }
}