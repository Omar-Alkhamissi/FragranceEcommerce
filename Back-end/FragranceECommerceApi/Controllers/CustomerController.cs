using FragEcom.DAL;
using FragEcom.DAL.DAO;
using FragEcom.DAL.DomainClasses;
using FragEcom.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FragEcom.Controllers
{
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        readonly AppDbContext? _ctx;
        readonly IConfiguration configuration;

        public CustomerController(AppDbContext context, IConfiguration config)
        {
            _ctx = context;
            this.configuration = config;
        }

        [HttpPost]
        [Route("api/[controller]/Register")]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerHelper>> Register(CustomerHelper helper)
        {
            CustomerDAO dao = new(_ctx!);
            Customer? already = await dao.GetByEmail(helper.Email);
            if (already == null)
            {
                HashSalt hashSalt = GenerateSaltedHash(64, helper.Password!);
                helper.Password = ""; 
                Customer dbCustomer = new()
                {
                    Firstname = helper.Firstname!,
                    Lastname = helper.Lastname!,
                    Email = helper.Email!,
                    Hash = hashSalt.Hash!,
                    Salt = hashSalt.Salt!
                };
                dbCustomer = await dao.Register(dbCustomer);
                if (dbCustomer.Id > 0)
                    helper.Token = "customer registered";
                else
                    helper.Token = "customer registration failed";
            }
            else
            {
                helper.Token = "customer registration failed - email already in use";
            }
            return helper;
        }

        [HttpPost]
        [Route("api/[controller]/Login")]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerHelper>> Login(CustomerHelper helper)
        {
            CustomerDAO dao = new(_ctx!);
            Customer? customer = await dao.GetByEmail(helper.Email);
            if (customer != null)
            {
                if (VerifyPassword(helper.Password, customer.Hash!, customer.Salt!))
                {
                    helper.Password = "";
                    var appSettings = configuration.GetSection("AppSettings").GetValue<string>("Secret");
                    // authentication successful so generate jwt token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(appSettings);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, customer.Id.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    string returnToken = tokenHandler.WriteToken(token);
                    helper.Token = returnToken;
                }
                else
                {
                    helper.Token = "username or password invalid - login failed";
                }
            }
            else
            {
                helper.Token = "no such customer - login failed";
            }
            return helper;
        }

        private static HashSalt GenerateSaltedHash(int size, string password)
        {
            var saltBytes = new byte[size];
            var provider = RandomNumberGenerator.Create();
            provider.GetNonZeroBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
            HashSalt hashSalt = new() { Hash = hashPassword, Salt = salt };
            return hashSalt;
        }

        public static bool VerifyPassword(string? enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword!, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }

        [HttpGet]
        [Route("api/diag/db")]
        [AllowAnonymous]
        public async Task<IActionResult> DbPing([FromServices] AppDbContext ctx)
        {
            try
            {
                await ctx.Database.OpenConnectionAsync();
                await ctx.Database.CloseConnectionAsync();
                return Ok("db ok");
            }
            catch (Exception ex)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine(ex.GetType().FullName + ": " + ex.Message);
                if (ex is SqlException sqlex && sqlex.Errors != null)
                {
                    foreach (SqlError err in sqlex.Errors)
                    {
                        sb.AppendLine($"SqlError {err.Number}: {err.Message}");
                    }
                }
                if (ex.InnerException != null)
                    sb.AppendLine("Inner: " + ex.InnerException.Message);
                return StatusCode(500, sb.ToString());
            }
        }
    }

    public class HashSalt
    {
        public string? Hash { get; set; }
        public string? Salt { get; set; }
    }
}