namespace LaboratoryDesktopApp.Models;

public class LabTest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string SampleType { get; set; } = "";
    public decimal Price { get; set; }
    public int TurnaroundHours { get; set; } = 24;
    public bool IsActive { get; set; } = true;
    public string Description { get; set; } = "Demo description";
    public List<TestParameter> Parameters { get; set; } = new();
}
