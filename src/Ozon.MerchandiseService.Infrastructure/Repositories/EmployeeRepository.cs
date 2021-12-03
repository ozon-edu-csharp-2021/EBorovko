using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using OpenTracing;
using Ozon.MerchandiseService.Domain.Aggregates.Employee;
using Ozon.MerchandiseService.Domain.Aggregates.Employee.ValueObjects;
using Ozon.MerchandiseService.Domain.ValueObjects;
using Ozon.MerchandiseService.Infrastructure.Repositories.Infrastructure;
using Employee = Ozon.MerchandiseService.Domain.Aggregates.Employee.Employee;

namespace Ozon.MerchandiseService.Infrastructure.Repositories
{
    public class EmployeeRepository: IEmployeeRepository
    {
        private readonly IDbConnectionFactory<NpgsqlConnection> _dbConnectionFactory;
        private readonly IChangeTracker _changeTracker;
        private readonly ITracer _tracer;
        private const int Timeout = 5;

        public EmployeeRepository(IDbConnectionFactory<NpgsqlConnection> dbConnectionFactory, IChangeTracker changeTracker, ITracer tracer)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        public async Task<bool> ExistsAsync(long employeeId, CancellationToken cancellationToken)
        {
            string name = $"{nameof(EmployeeRepository)}.{nameof(ExistsAsync)}";
            using var scope = _tracer.BuildSpan(name).StartActive(true);
            
            const string sql = @"
                SELECT Count(1)
                FROM employees
                WHERE employees.id = @EmployeeId;";
            
            var parameters = new
            {
                EmployeeId = employeeId
            };

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            int count =  await connection.ExecuteScalarAsync<int>(sql, param: parameters);
            
            return count != 0;
        }

        public async Task<Employee> GetWithAllMerchPacksAsync(long employeeId, CancellationToken cancellationToken)
        {
            string name = $"{nameof(EmployeeRepository)}.{nameof(GetWithAllMerchPacksAsync)}";
            using var scope = _tracer.BuildSpan(name).StartActive(true);
            
            const string sql = @"
                SELECT employees.id,employees.email,
                    merchandise_requests.employee_id,
                    merchandise_requests.merch_pack_type_id
                FROM employees
                INNER JOIN merchandise_requests ON merchandise_requests.employee_id = employees.id
                WHERE employees.id = @EmployeeId;";
            
            var parameters = new
            {
                EmployeeId = employeeId
            };

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            var command = new CommandDefinition(sql, parameters: parameters, commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var employees = new Dictionary<long, Models.Employee>();

            var models = await connection.QueryAsync<Models.Employee,Models.MerchandiseProvidingRequest,  Models.Employee>(
                    command,
                    (e, r) =>
                    {
                        if (!employees.TryGetValue(e.Id, out var employee))
                        {
                            employee = e;
                            employee.MerchandiseProvidingRequests = new List<Models.MerchandiseProvidingRequest>();
                            employees.Add(employee.Id, employee);
                        }

                        employee.MerchandiseProvidingRequests.Add(r);
                        return employee;
                    }, "employee_id");
            
            var model = models.FirstOrDefault();
            if (model != null)
            {
                var employee = new Employee
                (
                    model.Id,
                    Email.Create(model.Email),
                    model.MerchandiseProvidingRequests.Select(r => new MerchandisePack(MerchandisePackType.Parse(r.MerchPackTypeId)))
                );
                _changeTracker.Track(employee);
                return employee;
            }

            return null;
        }

        public async Task<Employee> GetWithAllMerchPacksAsync(string email, CancellationToken cancellationToken)
        {
            string name = $"{nameof(EmployeeRepository)}.{nameof(GetWithAllMerchPacksAsync)}";
            using var scope = _tracer.BuildSpan(name).StartActive(true);
            
            const string sql = @"
                SELECT employees.id,employees.email,
                    merchandise_requests.employee_id,
                    merchandise_requests.merch_pack_type_id
                FROM employees
                INNER JOIN merchandise_requests ON merchandise_requests.employee_id = employees.id
                WHERE employees.email = @Email;";
            
            var parameters = new
            {
                Email = email
            };

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            var command = new CommandDefinition(sql, parameters: parameters, commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var employees = new Dictionary<long, Models.Employee>();

            var models = await connection.QueryAsync<Models.Employee,Models.MerchandiseProvidingRequest,  Models.Employee>(
                    command,
                    (e, r) =>
                    {
                        if (!employees.TryGetValue(e.Id, out var employee))
                        {
                            employee = e;
                            employee.MerchandiseProvidingRequests = new List<Models.MerchandiseProvidingRequest>();
                            employees.Add(employee.Id, employee);
                        }

                        employee.MerchandiseProvidingRequests.Add(r);
                        return employee;
                    }, "employee_id");
            
            var model = models.FirstOrDefault();
            if (model != null)
            {
                var employee = new Employee
                (
                    model.Id,
                    Email.Create(model.Email),
                    model.MerchandiseProvidingRequests.Select(r => new MerchandisePack(MerchandisePackType.Parse(r.MerchPackTypeId)))
                );
                _changeTracker.Track(employee);
                return employee;
            }

            return null;
        }

        public async Task CreateAsync(Employee employee, CancellationToken cancellationToken)
        {
            using var scope = _tracer.BuildSpan(nameof(CreateAsync)).StartActive(true);
            
            const string query = @"
                INSERT INTO employees (id, email)
                VALUES (@Id, @Email)";

            var parameters = new
            {
                Id = employee.Id,
                Email = employee.Email.Value
            };
            var commandDefinition = new CommandDefinition(
                query,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);
            
            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            
            await connection.ExecuteScalarAsync<long>(commandDefinition);
            
            _changeTracker.Track(employee);
        }


        public Task<Employee> UpdateAsync(Employee employee, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}