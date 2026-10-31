using Dapper;
using EduPlatform.BuildingBlocks.Events;
using MassTransit;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Billing.Worker.Consumers
{
    public class StudentRegisteredBillingConsumer : IConsumer<StudentRegisteredEvent>
    {
        private readonly ILogger<StudentRegisteredBillingConsumer> _logger;
        private readonly string _connectionString;

        public StudentRegisteredBillingConsumer(ILogger<StudentRegisteredBillingConsumer> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("DataBaseBilling")!;
        }

        public async Task Consume(ConsumeContext<StudentRegisteredEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation("=================================================");
            _logger.LogInformation("[Billing.Worker] Procesando evento para facturación...");
            _logger.LogInformation(" Alumno ID: {StudentId}", message.StudentId);
            _logger.LogInformation("=================================================");

            var invoiceId = Guid.NewGuid();
            const decimal initialFee = 350.00m; // Monto de matricula simulado

            var sql = @"INSERT INTO Invoices (Id, StudentId, FullName, Email, Faculty, Amount, Status, CreatedAt) 
                    VALUES (@Id, @StudentId, @FullName, @Email, @Faculty, @Amount, @Status, @CreatedAt)";

            var param = new DynamicParameters();
            param.Add("@Id", invoiceId);
            param.Add("@StudentId", message.StudentId);
            param.Add("@FullName", $"{message.FirstName} {message.LastName}");
            param.Add("@Email", message.Email);
            param.Add("@Faculty", message.Faculty);
            param.Add("@Amount", initialFee);
            param.Add("@Status", "Pending");
            param.Add("@CreatedAt", DateTime.UtcNow);

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, param, commandType: CommandType.Text);

            _logger.LogInformation(
                "[Billing.Worker] Factura {InvoiceId} generada exitosamente para el alumno {StudentId}.", invoiceId, message.StudentId);
        }
    }
}
