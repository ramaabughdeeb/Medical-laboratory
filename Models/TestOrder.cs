namespace LaboratoryDesktopApp.Models;
public class TestOrder { public int Id {get;set;} public int PatientId {get;set;} public List<string> TestCodes {get;set;}=new(); public DateTime CreatedAt {get;set;}=DateTime.Now; public string Status {get;set;}="Pending sample"; public string OrderCode => $"ORD-{Id:0000}"; }
