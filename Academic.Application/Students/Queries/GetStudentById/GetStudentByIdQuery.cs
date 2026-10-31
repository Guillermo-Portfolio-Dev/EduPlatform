using EduPlatform.BuildingBlocks.Results;
using MediatR;

namespace Academic.Application.Students.Queries.GetStudentById
{
    public record GetStudentByIdQuery(Guid Id) : IRequest<Result<StudentResponseDto>>;

    // DTO para estructurar la respuesta de la lectura
    public record StudentResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Faculty,
    DateTime RegisteredAt
);
}
