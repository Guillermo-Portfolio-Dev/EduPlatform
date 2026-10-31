using EduPlatform.BuildingBlocks.Events;
using MassTransit;

namespace Notifications.Worker.Consumers
{
    public class StudentRegisteredEventConsumer : IConsumer<StudentRegisteredEvent>
    {
        private readonly ILogger<StudentRegisteredEventConsumer> _logger;

        public StudentRegisteredEventConsumer(ILogger<StudentRegisteredEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StudentRegisteredEvent> context)
        {
            var message = context.Message;

            // Aquí simularías el envío de un correo electrónico o una notificación push
            _logger.LogInformation(" [x] Evento recibido exitosamente en Notifications.Worker");
            _logger.LogInformation("--------------------------------------------------");
            _logger.LogInformation(" Destinatario: {Email}", message.Email);
            _logger.LogInformation(" Alumno: {FirstName} {LastName}", message.FirstName, message.LastName);
            _logger.LogInformation(" Facultad: {Faculty}", message.Faculty);
            _logger.LogInformation(" Fecha de Registro: {RegisteredAt}", message.RegisteredAt);
            _logger.LogInformation("--------------------------------------------------");

            await Task.CompletedTask;
        }
    }
}
