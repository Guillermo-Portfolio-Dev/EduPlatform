namespace EduPlatform.BuildingBlocks.Events
{
    // Este es el evento que publicará Academic.Api cuando un alumno se registre
    public record StudentRegisteredEvent
    {
        public Guid StudentId { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Faculty { get; init; } = string.Empty;
        public DateTime RegisteredAt { get; init; }
    }
}
