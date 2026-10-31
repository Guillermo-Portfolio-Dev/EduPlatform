using Academic.Domain.Entities;
using Academic.Domain.Repositories;
using Academic.Infrastructure.Persistence;
using Academic.Infrastructure.Resilience;
using Dapper;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using System.Data;

namespace Academic.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DbConnectionFactory _connectionFactory;
        private readonly AsyncRetryPolicy _retryPolicy;

        public StudentRepository(DbConnectionFactory connectionFactory, ILogger<StudentRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _retryPolicy = PollyPolicies.GetSqlRetryPolicy(logger);
        }

        public async Task AddAsync(Student student, CancellationToken cancellationToken)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                using IDbConnection conn = _connectionFactory.CrearConexion();

                const string sql = @"
                    INSERT INTO Students (Id, FirstName, LastName, Email, Faculty, RegisteredAt)
                    VALUES (@Id, @FirstName, @LastName, @Email, @Faculty, @RegisteredAt);";

                var param = new DynamicParameters();
                param.Add("@Id", student.Id);
                param.Add("@FirstName", student.FirstName);
                param.Add("@LastName", student.LastName);
                param.Add("@Email", student.Email);
                param.Add("@Faculty", student.Faculty);
                param.Add("@RegisteredAt", student.RegisteredAt);

                await conn.ExecuteAsync(
                    new CommandDefinition(sql, param, commandType: CommandType.Text, cancellationToken: cancellationToken));
            });
        }

        public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            using IDbConnection conn = _connectionFactory.CrearConexion();

            const string sql = @"
            SELECT Id, FirstName, LastName, Email, Faculty, RegisteredAt
            FROM Students
            WHERE Id = @Id;";

            var param = new DynamicParameters();
            param.Add("@Id", id);

            var result = await conn.QueryFirstOrDefaultAsync<Student>(
                new CommandDefinition(sql, param, commandType: CommandType.Text, cancellationToken: cancellationToken));

            return result;
        }
    }
}
