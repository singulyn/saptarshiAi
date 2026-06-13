namespace SaptariX.Persistence.SqlServer;

public sealed class SqlServerOptions
{
    public const string SectionName = "SqlServer";

    public string ConnectionStringName { get; set; } = "SaptariX";
    public int DefaultCommandTimeoutSeconds { get; set; } = 30;
}
