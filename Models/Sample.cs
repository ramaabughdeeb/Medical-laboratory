namespace LaboratoryDesktopApp.Models;
public class Sample { public int Id {get;set;} public int OrderId {get;set;} public string SampleType {get;set;}=""; public string Barcode {get;set;}=""; public DateTime CollectedAt {get;set;}=DateTime.Now; public string Status {get;set;}="Collected"; public string SampleCode => $"SMP-{Id:0000}"; }
