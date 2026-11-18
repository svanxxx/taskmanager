namespace TaskManager.Api.Models;

/// <summary>
/// Default field value in TT_RES.dbo.FLDDFNTN.
/// </summary>
public sealed class FieldDefinition
{
    public int EntityType { get; set; }

    public string FieldCode { get; set; } = "";

    public string? DefaultValue { get; set; }
}
