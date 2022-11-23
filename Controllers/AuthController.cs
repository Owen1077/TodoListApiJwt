using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TodoListAPI.Data;
using TodoListAPI.Models;

namespace TodoListAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TodoAPIDbContext dbContext;

        //public AuthController(TodoAPIDbContext dbContext)
        //{
        //    this.dbContext = dbContext;
        //}

        public static User user = new User();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration, TodoAPIDbContext dbContext)
        {
            _configuration = configuration;
            this.dbContext = dbContext;
        }      


        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            //SqlConnection con = new SqlConnection(_configuration.GetConnectionString("TodoAPIConnectionString").ToString());
            //SqlCommand cmd = new SqlCommand("INSERT INTO RegistrationTodos(Username,Password) VALUES('" + request.Username + "','" + request.Password + "')", con);

            //var userd = new UserDto()
            //{
            //    Username = request.Username,
            //    Password = request.Password
            //    //ASK KACHI ABOUT THIS DATETIME

            //};


            //return Ok(todo);


            ////zzz
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;


            await dbContext.RegistrationTodos.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Ok(user);
            ////zzz
            //con.Open();
            //int i = cmd.ExecuteNonQuery();
            //con.Close();
            //if (i > 0)
            //{
            //    return Ok(user);
            //}
            //else
            //{
            //    return BadRequest("Error");
            //}
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var user = await dbContext.RegistrationTodos.FirstOrDefaultAsync(x => x.Username == request.Username);


            if (user.Username != request.Username)
            {
                return BadRequest("user not found");
            }

            if (!verifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(user);

            return Ok(token);

        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }
}
