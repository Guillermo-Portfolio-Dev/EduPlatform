namespace Academic.Domain.Entities
{
    public class Student
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Faculty { get; private set; } = string.Empty;
        public DateTime RegisteredAt { get; private set; }

        // Constructor vacío requerido por Dapper para mapear consultas (ej. QueryFirstOrDefaultAsync)
        public Student() { }

        // Constructor con parámetros para crear nuevas instancias desde el negocio
        public Student(Guid id, string firstName, string lastName, string email, string faculty)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Faculty = faculty;
            RegisteredAt = DateTime.UtcNow;
        }
    }
}
