using account_service.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace account_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private AccountApplicationContext _dbcontext = new AccountApplicationContext();
        private readonly ILogger<AccountsController> _logger;

        AccountApplicationContext db = new();

        public AccountsController(ILogger<AccountsController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet("Me")]
        public IActionResult Me()
        {
            try
            {
                var userName = User.Identity.Name;

                if (string.IsNullOrEmpty(userName))
                {
                    return Unauthorized("Username not found in the token.");
                }

                var user = _dbcontext.Accounts.FirstOrDefault(x => x.username == userName);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var accountInfo = new
                {
                    user.id,
                    user.firstName,
                    user.lastName,
                    user.username,
                };

                return Ok(accountInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize]
        [HttpPut("Update")]
        public IActionResult UpdateMyAccount([FromQuery] string lastname, [FromQuery] string firstname, [FromQuery] string password)
        {
            try
            {
                var userName = User.Identity.Name;

                if (string.IsNullOrEmpty(userName))
                {
                    return Unauthorized("Username not found in the token.");
                }

                var user = _dbcontext.Accounts.FirstOrDefault(x => x.username == userName);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                user.firstName = firstname;
                user.lastName = lastname;
                user.password = password;

                _dbcontext.Accounts.Update(user);
                _dbcontext.SaveChanges();

                return Ok("Account successfully updated.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [Authorize]
        [HttpGet()]
        public IActionResult GetAccounts()
        {
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("Username not found in the token.");
            }

            var user = _dbcontext.Accounts.FirstOrDefault(x => x.username == userName);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userRoles = _dbcontext.AccountsRoles.Where(x => x.accounts.id == user.id).ToList();
            foreach (var userRole in userRoles)
            {
                if (userRole.accounts.id == 1)
                {
                    return Ok(_dbcontext.Accounts.ToList());
                }
            }

            return BadRequest("You dont have permissions to see this resource.");
        }

        [Authorize]
        [HttpPost()]
        public IActionResult CreateAccount([FromBody] NewAccountRequest accountData)
        {
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("Username not found in the token.");
            }

            var user = _dbcontext.Accounts.FirstOrDefault(x => x.username == userName);
            if (user == null)
            {
                return NotFound("User  not found.");
            }

            var userRoles = _dbcontext.AccountsRoles.Where(x => x.accounts.id == user.id).ToList();
            foreach (var userRole in userRoles)
            {
                if (userRole.accounts.id == 1)
                {
                    var lastUser = _dbcontext.Accounts.OrderByDescending(x => x.id).FirstOrDefault();
                    int newId = (lastUser != null) ? lastUser.id + 1 : 1;

                    var newAccount = new Accounts
                    {
                        id = newId,
                        lastName = accountData.lastName,
                        firstName = accountData.firstName,
                        username = accountData.username,
                        password = accountData.password
                    };
                    _dbcontext.Accounts.Add(newAccount);
                    for (int i = 0; i < accountData.roles.Length; i++)
                    {
                        var role = _dbcontext.Roles.FirstOrDefault(x => x.id == accountData.roles[i]);
                        if (role == null)
                        {
                            return NotFound("Role not found.");
                        }
                        var newAccountRole = new AccountsRoles
                        {
                            accounts = newAccount,
                            roles = role,
                        };
                        _dbcontext.AccountsRoles.Add(newAccountRole);
                    }
                    _dbcontext.SaveChanges();
                    return Ok("Account successfully created.");
                }
            }

            return BadRequest("You don't have permissions to see this resource.");
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateAccount(int id, [FromBody] NewAccountRequest accountData)
        {
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("Username not found in the token.");
            }

            var user = _dbcontext.Accounts.FirstOrDefault(x => x.username == userName);
            if (user == null)
            {
                return NotFound("User  not found.");
            }

            var existingAccount = _dbcontext.Accounts.FirstOrDefault(x => x.id == id);
            if (existingAccount == null)
            {
                return NotFound("Account not found.");
            }

            var userRoles = _dbcontext.AccountsRoles.Where(x => x.accounts.id == user.id).ToList();
            bool hasPermission = userRoles.Any(role => role.accounts.id == 1);

            if (!hasPermission)
            {
                return BadRequest("You don't have permissions to update this account.");
            }

            existingAccount.lastName = accountData.lastName;
            existingAccount.firstName = accountData.firstName;
            existingAccount.username = accountData.username;
            existingAccount.password = accountData.password;

            var existingRoles = _dbcontext.AccountsRoles.Where(x => x.accounts.id == id).ToList();
            _dbcontext.AccountsRoles.RemoveRange(existingRoles);
            foreach (var roleId in accountData.roles)
            {
                var role = _dbcontext.Roles.FirstOrDefault(x => x.id == roleId);
                if (role == null)
                {
                    return NotFound($"Role with ID {roleId} not found.");
                }
                var newAccountRole = new AccountsRoles
                {
                    accounts = existingAccount,
                    roles = role,
                };
                _dbcontext.AccountsRoles.Add(newAccountRole);
            }

            _dbcontext.SaveChanges();
            return Ok("Account successfully updated.");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(int id)
        {
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("Username not found in the token.");
            }

            var user = _dbcontext.Accounts.FirstOrDefault(x => x.username == userName);
            if (user == null)
            {
                return NotFound("User  not found.");
            }

            var userRoles = _dbcontext.AccountsRoles.Where(x => x.accounts.id == user.id).ToList();
            bool hasPermission = userRoles.Any(role => role.accounts.id == 1);
            if (!hasPermission)
            {
                return BadRequest("You don't have permissions to delete this account.");
            }

            var accountToDelete = _dbcontext.Accounts.FirstOrDefault(x => x.id == id);
            if (accountToDelete == null)
            {
                return NotFound("Account not found.");
            }

            _dbcontext.Accounts.Remove(accountToDelete);
            _dbcontext.SaveChanges();

            return Ok("Account successfully deleted.");
        }
    }
    public class NewAccountRequest
    {
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int[] roles { get; set; }
    }
}
