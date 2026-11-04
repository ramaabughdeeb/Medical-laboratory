using Microsoft.EntityFrameworkCore;
using LaboratoryDesktopApp.Models;

namespace LaboratoryDesktopApp.Data;

public class AppDbContext : DbContext
{
    
    public DbSet<Patient> Patients { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "LaboratoryDesktopApp"
        );

        Directory.CreateDirectory(folder);

        string databasePath = Path.Combine(
            folder,
            "laboratory.db"
        );

        optionsBuilder.UseSqlite(
            $"Data Source={databasePath}"
        );
    }
}