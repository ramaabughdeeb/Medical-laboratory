namespace LaboratoryDesktopApp.Models;

public class TestParameter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string DataType { get; set; } = "Numeric";
    public string Unit { get; set; } = "";
    public string ReferenceInfo { get; set; } = "Demo only — not a clinical reference";
    public bool IsRequired { get; set; } = true;
    public int DisplayOrder { get; set; } = 1;
    public bool IsActive { get; set; } = true;
}
