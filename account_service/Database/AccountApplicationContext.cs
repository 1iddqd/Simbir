using System;
using Microsoft.EntityFrameworkCore;

namespace account_service.Database
{
    public class AccountApplicationContext : DbContext
    {
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<AccountsRoles> AccountsRoles { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }

        public AccountApplicationContext()
        {
            Database.EnsureCreated();

            if (!Accounts.Any())
            {

                AccountsRoles.AddRange(
                    new AccountsRoles { 
                        id = 1, 
                        accounts = new Accounts { id = 1, username = "admin", firstName = "Admin", lastName = "Admin", password = "admin" }, 
                        roles = new Roles { id = 1, roleName = "admin" }
                    },
                    new AccountsRoles
                    { 
                        id = 2, 
                        accounts = new Accounts { id = 2, username = "manager", firstName = "Manager", lastName = "Manager", password = "manager" }, 
                        roles = new Roles { id = 2, roleName = "manager" }
                    },
                    new AccountsRoles
                    { 
                        id = 3, 
                        accounts = new Accounts { id = 3, username = "doctor", firstName = "Doctor", lastName = "Doctor", password = "doctor" }, 
                        roles = new Roles { id = 3, roleName = "doctor" }
                    },
                    new AccountsRoles
                    { 
                        id = 4, 
                        accounts = new Accounts { id = 4, username = "user", firstName = "User", lastName = "User", password = "user" }, 
                        roles = new Roles { id = 4, roleName = "user" }
                    }
                );
            }

            SaveChanges();
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
