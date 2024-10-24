using Microsoft.EntityFrameworkCore;
using System.Data;

namespace document_service.Database
{
    public class HospitalApplicationContext : DbContext
    {
        public HospitalApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? db_port = Environment.GetEnvironmentVariable("POSTGRES_PORT");
            string? db_name = Environment.GetEnvironmentVariable("POSTGRES_DB");
            string? db_user = Environment.GetEnvironmentVariable("POSTGRES_USER");
            string? db_password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

            optionsBuilder.UseNpgsql($"Host=simbir_med_db;Port={db_port};Database={db_name};Username={db_user};password={db_password}");
        }
    }
}
