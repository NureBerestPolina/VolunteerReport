namespace EnRoute.Infrastructure.Commands
{
    public record RegisterCommand
    {
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
    }
}
