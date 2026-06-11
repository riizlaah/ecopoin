using EcoPoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace EcoPoinAPI.Controllers
{
    [Route("ecopoin-api-v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly EcoPoinContext dbc;
        private readonly IConfiguration conf;

        public UsersController(EcoPoinContext ctx, IConfiguration c)
        {
            dbc = ctx;
            conf = c;
        }

        [HttpPost("login")]
        public ActionResult Login(LoginDTO input)
        {
            var user = dbc.Users.FirstOrDefault(u => u.Username == input.username);
            if (user == null) return Helper.err("User not found", 404);
            if (!Helper.VerifySha256(input.password, user.Password)) return Helper.err("Wrong username or password");
            return Helper.json(new
            {
                id = user.Id,
                fullName = user.FullName,
                username = user.Username,
                role = user.Role,
                token = GenerateToken(user.Id, user.Role)
            }, "Login successful");
        }

        [HttpPost("register")]
        public ActionResult Register(RegisterDTO input)
        {
            if (input.password.Length < 8) return Helper.err("Password length must be 8 letters or more");
            var hasLetter = input.password.Any(Char.IsLetter);
            var hasUpper = input.password.Any(Char.IsUpper);
            var hasLower = input.password.Any(Char.IsLower);
            var hasDigit = input.password.Any(Char.IsDigit);
            var hasSymbol = input.password.Any(c => !Char.IsLetterOrDigit(c));
            if(!hasLetter || !hasUpper || !hasLower || !hasDigit || !hasSymbol) {
                return Helper.err("Password must contain uppercase and lowercase letter, digit and symbols");
            }
            if(!Regex.IsMatch(input.phone, @"\+?\d{9,}"))
            {
                return Helper.err("Phone number not valid");
            }
            if (dbc.Users.Any(u => u.Username == input.username)) return Helper.err("Username has been taken");
            if (dbc.Users.Any(u => u.Email == input.email)) return Helper.err("Email has been taken");
            if (dbc.Users.Any(u => u.Phone == input.phone)) return Helper.err("Phone has been taken");
            dbc.Users.Add(new User
            {
                Username = input.username,
                Password = Helper.Sha256(input.password),
                Email = input.email,
                Phone = input.phone,
                FullName = input.fullName,
                Role = "resident"
            });
            dbc.SaveChanges();
            return Helper.msg("Register successful");
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult Me()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = dbc.Users.Include(u => u.RedemptionPoints).Include(u => u.DepositPoints).FirstOrDefault(u => u.Id == userId);
            if (user == null) return Helper.err("User not found", 404);
            return Helper.json(new
            {
                id = userId,
                username = user.Username,
                fullName = user.FullName,
                email = user.Email,
                phone = user.Phone,
                role = user.Role,
                balance = user.Balance,
                environmentalImpact = user.EnvironmentalImpact,
            }, "Profile fetched successfully");
        }

        [HttpGet("points")]
        [Authorize]
        public ActionResult Points()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = dbc.Users.Include(u => u.RedemptionPoints).Include(u => u.DepositPoints).FirstOrDefault(u => u.Id == userId);
            if (user == null) return Helper.err("User not found", 404);
            return Helper.json(new
            {
                username = user.Username,
                totalPoints = user.TotalPoints,
                balance = user.Balance,
                environmentalImpact = user.EnvironmentalImpact,
                redeemedPoints = user.RedeemedPoints,
            }, "Points fetched successfully");
        }

        private string GenerateToken(int id, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(conf["Jwt:Issuer"], conf["Jwt:Audience"], claims, expires: DateTime.Now.AddHours(8), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        // Admins

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult GetAll(int page = 1, int size = 10, string search = "")
        {
            var query = dbc.Users.AsQueryable();
            if(search.Trim() != "")
            {
                query = query.Where(u => EF.Functions.Like(u.Username, $"%{search}%") || EF.Functions.Like(u.FullName, $"%{search}%") || EF.Functions.Like(u.Email, $"%{search}%") || EF.Functions.Like(u.Phone, $"%{search}%"));
            }
            var (error, result, paging) = Helper.Paginate(query, u => new
            {
                id = u.Id,
                username = u.Username,
                fullName = u.FullName,
                email = u.FullName,
                phone = u.Phone,
                role = u.Role
            }, page, size);
            if (error != "") return Helper.err(error);
            return Helper.paginate(result, page, paging?.items ?? 0, paging?.totalPage ?? 1, "Users fetched successfully");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Create(UserDTO input)
        {
            if (input.password.Length < 8) return Helper.err("Password length must be 8 letters or more");
            var hasLetter = input.password.Any(Char.IsLetter);
            var hasUpper = input.password.Any(Char.IsUpper);
            var hasLower = input.password.Any(Char.IsLower);
            var hasDigit = input.password.Any(Char.IsDigit);
            var hasSymbol = input.password.Any(c => !Char.IsLetterOrDigit(c));
            if (!hasLetter || !hasUpper || !hasLower || !hasDigit || !hasSymbol)
            {
                return Helper.err("Password must contain uppercase and lowercase letter, digit and symbols");
            }
            if (Regex.IsMatch(input.phone, @"\+?\d{9,}"))
            {
                return Helper.err("Phone number not valid");
            }
            if (dbc.Users.Any(u => u.Username == input.username)) return Helper.err("Username has been taken");
            if (dbc.Users.Any(u => u.Email == input.email)) return Helper.err("Email has been taken");
            if (dbc.Users.Any(u => u.Phone == input.phone)) return Helper.err("Phone has been taken");
            dbc.Users.Add(new User
            {
                Username = input.username,
                Password = Helper.Sha256(input.password),
                FullName = input.fullName,
                Email = input.email,
                Phone = input.phone,
                Role = input.role,
            });
            dbc.SaveChanges();
            return Helper.msg("User created successfully");
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public ActionResult Get(int id)
        {
            var user = dbc.Users.AsNoTrackingWithIdentityResolution().FirstOrDefault(u => u.Id == id);
            if (user == null) return Helper.err("User not found", 404);
            return Helper.json(new
            {
                id = user.Id,
                username = user.Username,
                fullName = user.FullName,
                email = user.FullName,
                phone = user.Phone,
                role = user.Role
            }, "User fetched successfully");
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult Update(int id, UserDTO input)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "resident";
            if (role != "admin" && id != userId) return Helper.err("Forbidden");
            if(input.password != "")
            {
                if (input.password.Length < 8) return Helper.err("Password length must be 8 letters or more");
                var hasLetter = input.password.Any(Char.IsLetter);
                var hasUpper = input.password.Any(Char.IsUpper);
                var hasLower = input.password.Any(Char.IsLower);
                var hasDigit = input.password.Any(Char.IsDigit);
                var hasSymbol = input.password.Any(c => !Char.IsLetterOrDigit(c));
                if (!hasLetter || !hasUpper || !hasLower || !hasDigit || !hasSymbol)
                {
                    return Helper.err("Password must contain uppercase and lowercase letter, digit and symbols");
                }
            }
            if (Regex.IsMatch(input.phone, @"\+?\d{9,}"))
            {
                return Helper.err("Phone number not valid");
            }
            var roles = new[] { "resident", "officer", "admin" };
            if (!roles.Contains(input.role)) return Helper.err("Role invalid");
            var user = dbc.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return Helper.err("User not found", 404);
            if (dbc.Users.Any(u => u.Username == input.username && u.Id != userId)) return Helper.err("Username has been taken");
            if (dbc.Users.Any(u => u.Email == input.email && u.Id != userId)) return Helper.err("Email has been taken");
            if (dbc.Users.Any(u => u.Phone == input.phone && u.Id != userId)) return Helper.err("Phone has been taken");
            user.Username = input.username; 
            user.Email = input.email; 
            user.Phone = input.phone;
            user.FullName = input.fullName;
            if(role == "admin") user.Role = input.role;
            if (input.password != "") user.Password = Helper.Sha256(input.password);
            dbc.SaveChanges();
            return Helper.msg("User updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var user = dbc.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return Helper.err("User not found", 404);
            dbc.Users.Remove(user);
            dbc.SaveChanges();
            return Helper.msg("User removed successfully");
        }

    }

    public class LoginDTO
    {
        [Required] public string username { get; set; } = null!;
        [Required] public string password { get; set; } = null!;
    }

    public class RegisterDTO
    {
        [Required] public string username { get; set; } = null!;
        [Required] public string fullName { get; set; } = null!;
        [Required][EmailAddress] public string email { get; set; } = null!;
        [Required] public string phone { get; set; } = null!;
        [Required] public string password { get; set; } = null!;
    }

    public class UserDTO
    {
        [Required] public string username { get; set; } = null!;
        [Required] public string fullName { get; set; } = null!;
        [Required][EmailAddress] public string email { get; set; } = null!;
        [Required] public string phone { get; set; } = null!;
        [Required(AllowEmptyStrings = true)] public string password { get; set; } = null!;
        [Required] public string role { get; set; } = null!;
    }
}
