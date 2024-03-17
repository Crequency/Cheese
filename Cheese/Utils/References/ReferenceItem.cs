namespace Cheese.Utils.References;

public class ReferenceItem
{
    public string? Name { get; set; }

    public string? Location { get; set; }

    public string? Url { get; set; }
    
    public string? Branch { get; set; }

    public ReferenceType Type { get; set; } = ReferenceType.Unknown;

    public bool InSubmodule { get; set; } = false;
}

public enum ReferenceType
{
    Unknown = 0,
    GitRepo = 1,
    Binary = 2,
}
