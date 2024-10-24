
namespace account_service.Database
{
    public class Accounts
    {
        public int id { get; set; }

        public required string username { get; set; }

        public required string firstName { get; set; }

        public required string lastName { get; set; }

        public required string password { get; set; }

    }

    public class Roles
    {
        public int id { get; set; }

        public required string roleName { get; set; }

    }

    public class AccountsRoles
    {
        public int id { get; set; }

        public required Accounts accounts { get; set; }

        public required Roles roles { get; set; }
    }

    public class RefreshTokens
    {
        public int id { get; set; }
        public required Accounts accounts { get; set; }

        public required string token { get; set; }
    }
}
