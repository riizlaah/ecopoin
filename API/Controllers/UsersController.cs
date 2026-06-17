using EcopoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace EcopoinAPI.Controllers
{
    [Route("ecopoin-v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly EcoPoinContext dbc;
        private readonly IConfiguration conf;

        public UsersController(EcoPoinContext dbc, IConfiguration c)
        {
            this.dbc = dbc;
            conf = c;
        }

        [HttpPost("login")]
        public ActionResult Login(LoginDTO input)
        {
            var user = dbc.Users.FirstOrDefault(u => u.Username == input.username);
            if (user == null) return Helper.err("User not found", 404);
            if (!Helper.isHashValid(input.password, user.Password)) return Helper.err("Wrong username or password");
            return Helper.json(new
            {
                id = user.Id,
                username = user.Username,
                fullName = user.FullName,
                email = user.Email,
                role = user.Role,
                token = GenToken(user.Id, user.Role)
            });
        }

        [HttpPost("register")]
        public ActionResult Register(RegisterDTO input)
        {
            var hasLetter = input.password.Any(Char.IsLetter);
            var hasDigit = input.password.Any(Char.IsDigit);
            var hasSymbol = input.password.Any(c => !Char.IsLetterOrDigit(c));
            var hasLower = input.password.Any(Char.IsLower);
            var hasUpper = input.password.Any(Char.IsUpper);
            if(!hasLetter || !hasDigit || !hasSymbol || !hasLower || !hasUpper)
            {
                return Helper.err("Password must have lowercase and uppercase letter, digit and symbol");
            }
            if(!Regex.IsMatch(input.phone, @"\+?\d{9,}"))
            {
                return Helper.err("Phone number not valid");
            }
            if (!dbc.Users.Any(u => u.Username == input.username)) return Helper.err("Username has been taken");
            if (!dbc.Users.Any(u => u.Email == input.email)) return Helper.err("Email has been taken");
            if (!dbc.Users.Any(u => u.Phone == input.phone)) return Helper.err("Phone number has been taken");
            dbc.Users.Add(new User
            {
                Username = input.username,
                FullName = input.fullName,
                Email = input.email,
                Phone = input.phone,
                Password = Helper.hash(input.password),
                Role = "resident"
            });
            dbc.SaveChanges();
            return Helper.json("Register successful");
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult Me()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = dbc.Users.Find(userId);
            if (user == null) return Helper.err("User not found", 404);
            return Helper.json(new
            {
                id = userId,
                fullName = user.FullName,
                username = user.Username,
                email = user.Email,
                phone = user.Phone,
                balance = user.TotalBalance,
                envImpact = user.TotalEnvImpact
            });
        }

        [HttpGet("points")]
        [Authorize]
        public ActionResult Points()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = dbc.Users.Find(userId);
            if (user == null) return Helper.err("User not found", 404);
            return Helper.json(new
            {
                id = userId,
                fullName = user.FullName,
                balance = user.TotalBalance,
                totalPoints = user.TotalPoints,
                submittedWeight = user.TotalSubmittedWeight,
                envImpact = user.TotalEnvImpact
            });
        }

        private string GenToken(int id, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["Jwt:Key"])), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(conf["Jwt:Issuer"], conf["Jwt:Audience"], claims, expires: DateTime.Now.AddHours(8), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
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
        [Required][MinLength(8, ErrorMessage = "Password length must be 8 characters or more")] public string password { get; set; } = null!;
    }
}
