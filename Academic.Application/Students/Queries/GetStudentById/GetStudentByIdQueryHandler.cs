using Academic.Domain.Repositories;
using EduPlatform.BuildingBlocks.Results;
using MediatR;

namespace Academic.Application.Students.Queries.GetStudentById
{
    public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentResponseDto>>
    {
        private readonly IStudentRepository _studentRepository;

        public GetStudentByIdQueryHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<Result<StudentResponseDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

            if (student == null)
            {
                return Result<StudentResponseDto>.Failure("El estudiante no fue encontrado");
            }

            var response = new StudentResponseDto(
                    student.Id,
                    student.FirstName,
                    student.LastName,
                    student.Email,
                    student.Faculty,
                    student.RegisteredAt
                );

            return Result<StudentResponseDto>.Success(response);
        }
    }
}
