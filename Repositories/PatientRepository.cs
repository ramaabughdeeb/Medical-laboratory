using LaboratoryDesktopApp.Data;
using LaboratoryDesktopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LaboratoryDesktopApp.Repositories;

public class PatientRepository
{
    public List<Patient> GetAll()
    {
        using var db = new AppDbContext();

        return db.Patients
            .AsNoTracking()
            .OrderByDescending(p => p.Id)
            .ToList();
    }

    public Patient? GetById(int id)
    {
        using var db = new AppDbContext();

        return db.Patients
            .FirstOrDefault(p => p.Id == id);
    }

    public void Add(Patient patient)
    {
        using var db = new AppDbContext();

        db.Patients.Add(patient);
        db.SaveChanges();
    }

    public void Update(Patient patient)
    {
        using var db = new AppDbContext();

        db.Patients.Update(patient);
        db.SaveChanges();
    }

    public void Delete(int id)
    {
        using var db = new AppDbContext();

        var patient = db.Patients.Find(id);

        if (patient == null)
            return;

        db.Patients.Remove(patient);
        db.SaveChanges();
    }

    public List<Patient> Search(string keyword)
    {
        using var db = new AppDbContext();

        keyword = keyword.Trim();

        return db.Patients
            .AsNoTracking()
            .Where(p =>
                p.FullName.Contains(keyword) ||
                p.Phone.Contains(keyword))
            .ToList();
    }
}