using EduPlatform.BuildingBlocks.Results;
using MediatR;

namespace Academic.Application.Students.Commands.RegisterStudent
{
    public record RegisterStudentCommand
        (
        string FirstName,
        string LastName,
        string Email,
        string Faculty
        ) : IRequest<Result<Guid>>;
}
