using Academic.Domain.Entities;
using Academic.Domain.Repositories;
using EduPlatform.BuildingBlocks.Events;
using EduPlatform.BuildingBlocks.Results;
using MassTransit;
using MediatR;

namespace Academic.Application.Students.Commands.RegisterStudent
{
    public class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, Result<Guid>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public RegisterStudentCommandHandler(IStudentRepository studentRepository, IPublishEndpoint publishEndpoint)
        {
            _studentRepository = studentRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result<Guid>> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
        {
            // 1. Validaciones de negocio (ej. verificar si el correo ya existe)

            // 2. Guardar en la base de datos (SQL Server)
            var studentId = Guid.NewGuid();
            var student = new Student(studentId, request.FirstName, request.LastName, request.Email, request.Faculty);


            await _studentRepository.AddAsync(student, cancellationToken);

            // 3. Publicar el evento a RabbitMQ

            await _publishEndpoint.Publish<StudentRegisteredEvent>(new
            {
                StudentId = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                Faculty = student.Faculty,
                RegisteredAt = student.RegisteredAt
            }, cancellationToken);

            return await Task.FromResult(Result<Guid>.Success(studentId));
        }
    }
}
