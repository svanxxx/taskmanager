namespace TaskManager.Api.Infrastructure;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "";

    public string Audience { get; set; } = "";

    public string SigningKey { get; set; } = "";

    /// <summary>
    /// Optional path to a file whose contents are the symmetric signing key.
    /// Used when <see cref="SigningKey"/> is empty.
    /// </summary>
    public string SigningKeyPath { get; set; } = "";

    /// <summary>
    /// Maximum accepted token lifetime, measured from iat to exp when both claims are present.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;

    public int ClockSkewSeconds { get; set; } = 60;
}

public sealed class TaskDatabaseOptions
{
    public const string SectionName = "TaskDatabase";

    /// <summary>
    /// Catalog that contains DEFECTS, REPORTBY, FLDDISPO, FLDDFNTN, and USERS.
    /// The connection string database should be this catalog so EF Core queries the same tables.
    /// </summary>
    public string TrackerCatalog { get; set; } = "TT_RES";

    /// <summary>
    /// Catalog that contains PERSONS. The legacy data access uses Initial Catalog=TASKS for this table.
    /// </summary>
    public string PersonsCatalog { get; set; } = "TASKS";
}
