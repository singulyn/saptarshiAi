namespace SaptariX.Platform.AccessControl.EffectivePermissions;

public sealed class EffectivePermissionSetDto
{
    public bool HasAssignments { get; init; }
    public IReadOnlySet<string> Permissions { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}
