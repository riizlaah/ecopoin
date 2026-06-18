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
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ExtBaseController
    {
        private readonly EcoPoinContext dbc;
        private readonly IConfiguration conf;

        public UsersController(EcoPoinContext dbc, IConfiguration conf)
        {
            this.dbc = dbc;
            this.conf = conf;
        }

        [HttpPost("login")]
        public ActionResult Login(LoginDTO input)
        {
            var user = dbc.Users.FirstOrDefault(u => u.Username == input.username);
            if (user == null) return err("User not found", 404);
            if (!isHashValid(input.password, user.Password)) return err("Wrong username or password");
            return json(new
            {
                id = user.Id,
                username = user.Username,
                fullName = user.FullName,
                role = user.Role,
                token = GenToken(user.Id, user.Role)
            }, "Login success");
        }

        [HttpPost("register")]
        public ActionResult Register(RegisterDTO input)
        {
            if(!Regex.IsMatch(input.phone, @"^\+?\d{9,}$"))
            {
                return err("Phone number not valid");
            }
            var hasLetter = input.password.Any(Char.IsLetter);
            var hasDigit = input.password.Any(Char.IsDigit);
            var hasSymbol = input.password.Any(c => !Char.IsLetterOrDigit(c));
            var hasLower = input.password.Any(Char.IsLower);
            var hasUpper = input.password.Any(Char.IsUpper);
            if(!hasLetter || !hasDigit || !hasSymbol || !hasLower || !hasUpper)
            {
                return err("Password must contains uppercase and lowercase letters, digit and symbol");
            }
            if (dbc.Users.Any(u => u.Username == input.username)) return err("Username has been taken");
            if (dbc.Users.Any(u => u.Email == input.email)) return err("Email has been taken");
            if (dbc.Users.Any(u => u.Phone == input.phone)) return err("Phone has been taken");
            dbc.Users.Add(new User
            {
                Username = input.username,
                FullName = input.fullName,
                Email = input.email,
                Phone = input.phone,
                Password = hash(input.password),
                Role = "resident"
            });
            dbc.SaveChanges();
            return msg("Register successful");
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult Me()
        {
            var userId = getUserId();
            var user = dbc.Users.Include(u => u.DepositResidents).Include(u => u.RedemptionPoints).Include(u => u.DepositResidents).ThenInclude(d => d.WasteType).FirstOrDefault(u => u.Id == userId);
            if (user == null) return err("User not found");
            return json(new { 
                id = user.Id,
                username = user.Username,
                fullName = user.FullName,
                email = user.Email,
                phone = user.Phone,
                balance = user.TotalBalance,
                envImpact = user.TotalEnvImpact,
                submittedWeight = user.TotalSubmittedWeight,
            }, "Profile fetched successfully");
        }

        [HttpGet("points")]
        [Authorize]
        public ActionResult Points()
        {
            var userId = getUserId();
            var user = dbc.Users.Include(u => u.DepositResidents).Include(u => u.RedemptionPoints).Include(u => u.DepositResidents).ThenInclude(d => d.WasteType).FirstOrDefault(u => u.Id == userId);
            if (user == null) return err("User not found");
            return json(new
            {
                id = user.Id,
                username = user.Username,
                fullName = user.FullName,
                balance = user.TotalBalance,
                envImpact = user.TotalEnvImpact,
                submittedWeight = user.TotalSubmittedWeight,
                totalPoints = user.TotalPoints,
            }, "Profile fetched successfully");
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
        [Required] public string username { get; set; }
        [Required] public string password { get; set; }
    }

    public class RegisterDTO
    {
        [Required] public string username { get; set; }
        [Required] public string fullName { get; set; }
        [Required][EmailAddress] public string email { get; set; }
        [Required] public string phone { get; set; }
        [Required][MinLength(8, ErrorMessage ="Password length must be 8 characters or more")] public string password { get; set; }
    }
}
