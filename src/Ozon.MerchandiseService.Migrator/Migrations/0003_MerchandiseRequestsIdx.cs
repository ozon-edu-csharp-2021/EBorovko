using FluentMigrator;

namespace OzonEdu.StockApi.Migrator.Migrations
{
    [Migration(3)]
    public class MerchandiseRequestsIdx: ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                CREATE INDEX merchandise_requests_merch_pack_type_id_and_employee_id_idx ON merchandise_requests (merch_pack_type_id, employee_id)"
            );
        }
    }
}