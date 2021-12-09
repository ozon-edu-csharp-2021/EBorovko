using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using OpenTracing;
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
        private readonly ITracer _tracer;
        private const int Timeout = 5;

        public MerchandiseRequestRepository(IDbConnectionFactory<NpgsqlConnection> dbConnectionFactory, IChangeTracker changeTracker, ITracer tracer)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }
        
        public async Task<MerchandiseProvidingRequest> FindByMerchPackIdAndEmployeeIdAsync(int merchPackId, long employeeId, CancellationToken cancellationToken)
        {
            string name = $"{nameof(MerchandiseRequestRepository)}.{nameof(FindByMerchPackIdAndEmployeeIdAsync)}";
            using var scope = _tracer.BuildSpan(name).StartActive(true);
            
            const string sql = @"
                SELECT merchandise_requests.id, merch_pack_type_id, status, clothing_size, created_at, completed_at
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
                    ClothingSize.Parse(first.ClothingSize), 
                    first.CreatedAt,
                    first.CompletedAt
                );
                _changeTracker.Track(merchandiseProvidingRequest);
                return merchandiseProvidingRequest;
            }
            else
                return new MerchandiseProvidingRequest();
        }

        public async Task<MerchandiseProvidingRequest> FindByMerchPackIdAndEmployeeEmailAsync(int merchPackId, string email, CancellationToken cancellationToken)
        {
            string name = $"{nameof(MerchandiseRequestRepository)}.{nameof(FindByMerchPackIdAndEmployeeEmailAsync)}";
            using var scope = _tracer.BuildSpan(name).StartActive(true);
            
            const string sql = @"
                SELECT merchandise_requests.id, employee_id, merch_pack_type_id, status, clothing_size, created_at, completed_at
                FROM merchandise_requests
                LEFT JOIN employees ON employees.id = merchandise_requests.employee_id
                WHERE merch_pack_type_id = @MerchPackId AND employees.email = @EmployeeEmail
                ORDER BY completed_at DESC;";
            
            var parameters = new
            {
                MerchPackId = merchPackId,
                EmployeeEmail = email
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
                    ClothingSize.Parse(first.ClothingSize),
                    first.CreatedAt,
                    first.CompletedAt
                );
                _changeTracker.Track(merchandiseProvidingRequest);
                return merchandiseProvidingRequest;
            }
            else
                return new MerchandiseProvidingRequest();
        }

        /*public async Task<long> CreateAsync(MerchandiseProvidingRequest merchandiseRequest, CancellationToken cancellationToken)
        {
            string name = $"{nameof(MerchandiseRequestRepository)}.{nameof(CreateAsync)}";
            using var scope = _tracer.BuildSpan(name).StartActive(true);
            
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
        }*/
        
        public async Task<long> CreateAsync(MerchandiseProvidingRequest merchandiseRequest, CancellationToken cancellationToken)
        {
            string name = $"{nameof(MerchandiseRequestRepository)}.{nameof(CreateAsync)}";
            using var scope = _tracer.BuildSpan(name).StartActive(true);
            
            const string queryExistEmployee = @"
                SELECT id
                FROM employees
                WHERE employees.email = @EmployeeEmail;";
            
            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            long employeeId =  await connection.QueryFirstOrDefaultAsync<long>(queryExistEmployee, param: new
            {
                EmployeeEmail = merchandiseRequest.Employee.Email.Value
            });
            if (employeeId == 0)
            {
                const string queryCreateEmployee = @"
                INSERT INTO employees (email)
                VALUES (@EmployeeEmail)
                RETURNING id";
                
                employeeId = await connection.ExecuteScalarAsync<long>(
                    new CommandDefinition(
                        queryCreateEmployee,
                        parameters: new { EmployeeEmail = merchandiseRequest.Employee.Email.Value},
                        commandTimeout: Timeout,
                        cancellationToken: cancellationToken));
            }
            
            const string query = @"
                INSERT INTO merchandise_requests (employee_id, merch_pack_type_id, status, clothing_size, created_at, completed_at)
                VALUES (@EmployeeId, @MerchPackTypeId, @Status, @ClothingSize, @CreatedAt, @CompletedAt)
                RETURNING id";

            var parameters = new
            {
                EmployeeId = employeeId,
                MerchPackTypeId = merchandiseRequest.MerchandisePackType.Id,
                Status = merchandiseRequest.CurrentStatus.Id,
                ClothingSize = merchandiseRequest.ClothingSize.Id,
                CreatedAt = merchandiseRequest.CreatedAt.ToUniversalTime(),
                CompletedAt = merchandiseRequest.CompletedAt?.ToUniversalTime()
            };
            var commandDefinition = new CommandDefinition(
                query,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);
            
            long id = await connection.ExecuteScalarAsync<long>(commandDefinition);
            
            merchandiseRequest.Id = id;
            merchandiseRequest.Employee.Id = employeeId;
            _changeTracker.Track(merchandiseRequest);
            
            return id;
        }

        public async Task<int> UpdateAsync(MerchandiseProvidingRequest merchandiseRequest, CancellationToken cancellationToken)
        {
            string name = $"{nameof(MerchandiseRequestRepository)}.{nameof(UpdateAsync)}";
            using var scope = _tracer.BuildSpan(name).StartActive(true);
            
            const string sql = @"
                UPDATE merchandise_requests
                SET status = @Status, completed_at = @CompletedAt
                WHERE id = @Id";
            
            var parameters = new
            {
                Status = merchandiseRequest.CurrentStatus.Id,
                CompletedAt = merchandiseRequest.CompletedAt?.ToUniversalTime(),
                Id = merchandiseRequest.Id
            };
            
            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            
            int num =  await connection.ExecuteAsync(
                sql,
                parameters,
                null,
                Timeout,
                CommandType.Text
            );
            
            return num;
        }

        public async Task<IEnumerable<MerchandiseProvidingRequest>> GetAllWithStatus(int status, CancellationToken cancellationToken)
        {
            string name = $"{nameof(MerchandiseRequestRepository)}.{nameof(GetAllWithStatus)}";
            using var scope = _tracer.BuildSpan(name).StartActive(true);
            
            const string sql = @"
                SELECT merchandise_requests.id, merch_pack_type_id, status, clothing_size, created_at, completed_at
                FROM merchandise_requests
                WHERE status = @Status
                ORDER BY created_at ASC;";
            
            var parameters = new
            {
                Status = status,
            };
            
            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            
            var models = await connection.QueryAsync<Models.MerchandiseProvidingRequest>(
                sql,
                parameters,
                null,
                Timeout,
                CommandType.Text
            );
            
            var requests = models.Select(model => new MerchandiseProvidingRequest
            (
                model.Id,
                new Employee(model.EmployeeId),
                MerchandisePackType.Parse(model.MerchPackTypeId),
                Status.Parse(model.Status),
                ClothingSize.Parse(model.ClothingSize), 
                model.CreatedAt,
                model.CompletedAt
            ));
            return requests;
        }
    }
}