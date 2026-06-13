namespace SaptariX.Plugin.Abstractions.Workflows;

public sealed record WorkflowActivityDefinition(string Name, string DisplayName, string ModuleName, Type ActivityType);
