using Academic.Domain.Entities;

namespace Academic.Domain.Repositories
{
    public interface IStudentRepository
    {
        Task AddAsync(Student student, CancellationToken cancellationToken);
        Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
