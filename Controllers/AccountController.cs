using EmployeeTimesheet.Database;
using EmployeeTimesheet.Database.Entities;
using EmployeeTimesheet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace EmployeeTimesheet.Controllers
{
	[AllowAnonymous]
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly EmployeeTimesheetDbContext _dbContext;
		private readonly IConfiguration _configuration;

		public AccountController(EmployeeTimesheetDbContext dbContext,
			IConfiguration configuration)
		{
			_dbContext = dbContext;
			_configuration = configuration;
		}

		[HttpPost("SignIn")]
		public IActionResult SignIn([FromBody] AuthenticationModel model)
		{
			if (model == null)
				return BadRequest(new { message ="Please enter username and password!" });

			var dbUser = _dbContext.Users
				.Include(x => x.Role)
				.Where(x => x.UserName == model.Username && x.Password == model.Password)
				.SingleOrDefault();

			if (dbUser != null)
			{
				var tokenString = GenerateJWT(dbUser);
				return Ok(new { token = tokenString });
			}
			else
				return Unauthorized("Username and password are incorrect!");
		}

		#region Private methods

		private string GenerateJWT(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
					new Claim(ClaimTypes.Name, user.UserName.ToString()),
					new Claim(ClaimTypes.Role, user.Role.Name.ToString()),
				}),
				Expires = DateTime.UtcNow.AddDays(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var stringToken = tokenHandler.WriteToken(token);

			return stringToken;
		}

		#endregion
	}
}