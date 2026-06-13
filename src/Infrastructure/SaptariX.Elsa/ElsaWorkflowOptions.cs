namespace SaptariX.Elsa;

public sealed class ElsaWorkflowOptions
{
    public const string SectionName = "Elsa";

    public string ConnectionStringName { get; set; } = "SaptariX";
    public bool UseSqlServerPersistence { get; set; } = true;
    public bool RuntimeEnabled { get; set; } = true;
    public bool ManagementEnabled { get; set; } = true;
}
