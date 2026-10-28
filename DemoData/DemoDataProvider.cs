using LaboratoryDesktopApp.Models;
namespace LaboratoryDesktopApp.DemoData;
// All data is local in-memory demo data. It is reset when the application closes.
public static class DemoDataProvider
{
 public static List<TestCategory> Categories {get;}=new(){new(){Name="Hematology",Description="Demo category"},new(){Name="Biochemistry",Description="Demo category"},new(){Name="Urinalysis",Description="Demo category"},new(){Name="Immunology",Description="Demo category"},new(){Name="Microbiology",Description="Demo category"}};
 public static List<LabTest> Tests {get;}=new(){new(){Code="CBC",Name="Complete Blood Count",Category="Hematology",SampleType="Whole blood",Price=15m,TurnaroundHours=4,Description="Demonstration test record.",Parameters=new(){new(){Name="Hemoglobin",Code="HGB",Unit="g/dL",DisplayOrder=1},new(){Name="WBC",Code="WBC",Unit="10^9/L",DisplayOrder=2},new(){Name="Platelets",Code="PLT",Unit="10^9/L",DisplayOrder=3}}},new(){Code="GLU",Name="Glucose",Category="Biochemistry",SampleType="Serum",Price=8m,TurnaroundHours=2,Description="Demonstration test record.",Parameters=new(){new(){Name="Glucose",Code="GLU",Unit="mg/dL",DisplayOrder=1}}},new(){Code="LFT",Name="Liver Function Test",Category="Biochemistry",SampleType="Serum",Price=25m,TurnaroundHours=8,Description="Demonstration test record."},new(){Code="UA",Name="Urinalysis",Category="Urinalysis",SampleType="Urine",Price=10m,TurnaroundHours=3,Description="Demonstration test record."}};
 public static List<Patient> Patients {get;}=new(); public static List<TestOrder> Orders {get;}=new(); public static List<Sample> Samples {get;}=new(); public static List<TestResult> Results {get;}=new();
 public static int NextPatientId=1,NextOrderId=1,NextSampleId=1,NextResultId=1;
 public static Patient? PatientFor(int id)=>Patients.FirstOrDefault(x=>x.Id==id); public static TestOrder? OrderFor(int id)=>Orders.FirstOrDefault(x=>x.Id==id);
}
