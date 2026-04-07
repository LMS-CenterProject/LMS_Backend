namespace LMS.Domain.Errors;

public sealed record Error(string Code, string Description)
{
    public static Error None { get; } = new Error(string.Empty, string.Empty);
}
