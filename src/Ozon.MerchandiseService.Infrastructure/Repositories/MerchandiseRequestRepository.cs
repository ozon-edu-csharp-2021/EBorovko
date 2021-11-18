using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest;
using Ozon.MerchandiseService.Domain.Aggregates.MerchandiseProvidingRequest.ValueObjects;
using Ozon.MerchandiseService.Domain.ValueObjects;
using Ozon.MerchandiseService.Infrastructure.Repositories.Infrastructure;
using Employee = Ozon.MerchandiseService.Domain.Aggregates.Employee.Employee;

namespace Ozon.MerchandiseService.Infrastructure.Repositories
{
    public class MerchandiseRequestRepository: IMerchandiseProvidingRequestRepository
    {
        private readonly IDbConnectionFactory<NpgsqlConnection> _dbConnectionFactory;
        private readonly IChangeTracker _changeTracker;
        private const int Timeout = 5;

        public MerchandiseRequestRepository(IDbConnectionFactory<NpgsqlConnection> dbConnectionFactory, IChangeTracker changeTracker)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
        }
        
        public async Task<MerchandiseProvidingRequest> FindByMerchPackIdAndEmployeeIdAsync(int merchPackId, long employeeId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT merchandise_requests.id, merch_pack_type_id, status, created_at, completed_at
                FROM merchandise_requests
                WHERE merch_pack_type_id = @MerchPackId AND employee_id = @EmployeeId
                ORDER BY completed_at DESC;";
            
            var parameters = new
            {
                MerchPackId = merchPackId,
                EmployeeId = employeeId
            };
            
            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            
            var models = await connection.QueryAsync<Models.MerchandiseProvidingRequest>(
                sql,
                parameters,
                null,
                Timeout,
                CommandType.Text
            );
            
            var first = models.FirstOrDefault();
            if (first != null)
            {
                var merchandiseProvidingRequest = new MerchandiseProvidingRequest
                (
                    first.Id,
                    new Employee(first.EmployeeId),
                    MerchandisePackType.Parse(first.MerchPackTypeId),
                    Status.Parse(first.Status),
                    first.CreatedAt,
                    first.CompletedAt
                );
                _changeTracker.Track(merchandiseProvidingRequest);
                return merchandiseProvidingRequest;
            }
            else
                return new MerchandiseProvidingRequest();
        }

        public async Task<long> CreateAsync(MerchandiseProvidingRequest merchandiseRequest, CancellationToken cancellationToken)
        {
            const string query = @"
                INSERT INTO merchandise_requests (employee_id, merch_pack_type_id, status, created_at, completed_at)
                VALUES (@EmployeeId, @MerchPackTypeId, @Status, @CreatedAt, @CompletedAt)
                RETURNING id";

            var parameters = new
            {
                EmployeeId = merchandiseRequest.Employee.Id,
                MerchPackTypeId = merchandiseRequest.MerchandisePackType.Id,
                Status = merchandiseRequest.CurrentStatus.Id,
                CreatedAt = merchandiseRequest.CreatedAt.ToUniversalTime(),
                CompletedAt = merchandiseRequest.CompletedAt?.ToUniversalTime()
            };
            var commandDefinition = new CommandDefinition(
                query,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);
            
            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            
            long id = await connection.ExecuteScalarAsync<long>(commandDefinition);
            
            merchandiseRequest.Id = id;
            _changeTracker.Track(merchandiseRequest);
            
            return id;
        }

        public Task<long> UpdateAsync(MerchandiseProvidingRequest merchandiseRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}