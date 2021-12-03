using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using FluentMigrator;

namespace OzonEdu.StockApi.Migrator.Migrations
{
    [Migration(1)]
    public class MerchandiseRequestTable: Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
                CREATE TABLE if not exists merchandise_requests(
                    id BIGSERIAL PRIMARY KEY,
                    employee_id BIGINT NOT NULL,
                    merch_pack_type_id INTEGER NOT NULL,
                    status INTEGER NOT NULL,
                    clothing_size INTEGER NOT NULL,
                    created_at TIMESTAMP NOT NULL,
                    completed_at TIMESTAMP 
                    );"
            );
        }

        public override void Down()
        {
            Execute.Sql("DROP TABLE if exists merchandise_requests;");
        }
    }
}