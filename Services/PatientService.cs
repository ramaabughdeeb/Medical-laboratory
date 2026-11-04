using LaboratoryDesktopApp.Models;
using LaboratoryDesktopApp.Repositories;

namespace LaboratoryDesktopApp.Services;

public class PatientService
{
    private readonly PatientRepository _repository;

    public PatientService()
    {
        _repository = new PatientRepository();
    }

    // Get all patients
    public List<Patient> GetAllPatients()
    {
        return _repository.GetAll();
    }

    // Get patient by ID
    public Patient? GetPatientById(int id)
    {
        if (id <= 0)
            return null;

        return _repository.GetById(id);
    }

    // Register new patient
    public void RegisterPatient(Patient patient)
    {
        if (string.IsNullOrWhiteSpace(patient.FullName))
            throw new ArgumentException("Patient name is required.");

        if (patient.DateOfBirth.Date > DateTime.Today)
            throw new ArgumentException("Date of birth cannot be in the future.");

        patient.FullName = patient.FullName.Trim();
        patient.Phone = patient.Phone.Trim();
        patient.Notes = patient.Notes.Trim();

        _repository.Add(patient);
    }

    // Update patient
    public void UpdatePatient(Patient patient)
    {
        if (patient.Id <= 0)
            throw new ArgumentException("Invalid patient ID.");

        if (string.IsNullOrWhiteSpace(patient.FullName))
            throw new ArgumentException("Patient name is required.");

        if (patient.DateOfBirth.Date > DateTime.Today)
            throw new ArgumentException("Invalid date of birth.");

        patient.FullName = patient.FullName.Trim();
        patient.Phone = patient.Phone.Trim();
        patient.Notes = patient.Notes.Trim();

        _repository.Update(patient);
    }

    // Delete patient
    public void DeletePatient(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid patient ID.");

        _repository.Delete(id);
    }

    // Search patients
    public List<Patient> SearchPatients(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return _repository.GetAll();

        return _repository.Search(keyword);
    }
}