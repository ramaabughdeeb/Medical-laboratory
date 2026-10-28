namespace LaboratoryDesktopApp.Models;
public class Patient { public int Id {get;set;} public string FullName {get;set;}=""; public string Phone {get;set;}=""; public string Gender {get;set;}=""; public DateTime DateOfBirth {get;set;}=DateTime.Today; public string Notes {get;set;}=""; public string PatientCode => $"PAT-{Id:0000}"; }
