using EmployeeTimesheet.Database;
using EmployeeTimesheet.Database.Entities;
using EmployeeTimesheet.Models;
using EmployeeTimesheet.Shared;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EmployeeTimesheet.Controllers
{
	[Authorize]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly EmployeeTimesheetDbContext _dbContext;
		private readonly UserContext _userContext;

		public UserController(EmployeeTimesheetDbContext dbContext,
			UserContext userContext)
		{
			_dbContext = dbContext;
			_userContext = userContext;
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpGet("api/User")]
		public ActionResult<List<UserModel>> GetUsers()
		{
			var query = _dbContext.Users
				.AsQueryable();

			if (_userContext.GetUserRole() == RoleEnum.Manager)
				query = query.Where(x => x.ManagerId == _userContext.GetUserId());

			var users = query
				.OrderBy(x => x.FullName)
				.ProjectToType<UserModel>()
				.ToList();

			return users;
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpGet("api/User/Managers")]
		public ActionResult<List<UserModel>> GetManagers()
		{
			var users = _dbContext.Users
				.Include(x => x.Role)
				.Where(x => x.Role.Name == RoleEnum.Manager)
				.ProjectToType<UserModel>()
				.ToList();

			return users;
		}

		[HttpGet("api/User/Employees")]
		public ActionResult<List<UserModel>> GetEmployees(int? managerId)
		{
			var query = _dbContext.Users
				.Where(x => x.Role.Name == RoleEnum.Employee)
				.AsQueryable();

			if (managerId.HasValue)
				query = query.Where(x => x.ManagerId == managerId);

			var users = _dbContext.Users
				.ProjectToType<UserModel>()
				.ToList();

			return users;
		}

		[HttpGet("api/User/{userId}")]
		public ActionResult<UserModel> GetUser(int userId)
		{
			if (_userContext.GetUserRole() == RoleEnum.Employee && _userContext.GetUserId() != userId)
				return BadRequest(new { message = "User has no permission to see this content!" });

			var user = _dbContext.Users
				.Where(x => x.UserId == userId)
				.ProjectToType<UserModel>()
				.FirstOrDefault();

			user.ProjectTimesheetEntries = _dbContext.ProjectTimesheetEntries
				.Include(x => x.Project)
				.Where(x => x.UserId == userId)
				.ProjectToType<ProjectTimesheetEntryModel>()
				.ToList();

			user.ActivityTimesheetEntries = _dbContext.ActivityTimesheetEntries
				.Include(x => x.Activity)
				.Where(x => x.UserId == userId)
				.ProjectToType<ActivityTimesheetEntryModel>()
				.ToList();

			if (user == null)
				return NotFound();

			return user;
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpPost("api/User")]
		public ActionResult CreateUser([FromBody] UserModel model)
		{
			var newUser = model.Adapt<User>();

			if (ValidateUserName(model.UserName) == false)
				return BadRequest(new { message = "UserName is not valid!" });

			if (ValidatePassword(model.Password) == false)
				return BadRequest(new { message = "Password is not valid!" });

			if (model.ManagerId.HasValue)
			{
				newUser.Manager = _dbContext.Users
				.Where(x => x.UserId == model.ManagerId && x.Role.Name == RoleEnum.Manager)
				.FirstOrDefault();
				if (newUser.Manager == null)
					return BadRequest(new { message = "Unknown manager!" });
			}

			newUser.Role = _dbContext.Roles
					.Where(x => x.RoleId == model.RoleId)
					.FirstOrDefault();
			if (newUser.Role == null)
				return BadRequest(new { message = "Unknown role!" });

			_dbContext.Users.Add(newUser);
			_dbContext.SaveChanges();

			return Created($"/api/User/{newUser.UserId}", newUser.UserId);
		}

		[HttpPut("api/User/{userId}")]
		public IActionResult UpdateUser(int userId, [FromBody] UserModel model)
		{
			model.UserId = userId;

			var dbUser = _dbContext.Users
				.Where(x => x.UserId == model.UserId)
				.FirstOrDefault();
			if (dbUser == null)
				return BadRequest(new { message = "Unknown user!" });

			if (ValidateUserName(model.UserName) == false)
				return BadRequest(new { message = "UserName is not valid!" });

			if (ValidatePassword(model.Password) == false)
				return BadRequest(new { message = "Password is not valid!" });

			if (_userContext.GetUserRole() == RoleEnum.Admin || _userContext.GetUserRole() == RoleEnum.Manager)
			{
				if (model.ManagerId.HasValue && model.ManagerId != dbUser.ManagerId)
				{
					dbUser.Manager = _dbContext.Users
						.Where(x => x.UserId == model.ManagerId && x.Role.Name == RoleEnum.Manager)
						.FirstOrDefault();
					if (dbUser.Manager == null)
						return BadRequest(new { message = "Unknown manager!" });

					dbUser.ManagerId = model.ManagerId;
				}

				if (model.RoleId != dbUser.RoleId)
				{
					dbUser.Role = _dbContext.Roles
						.Where(x => x.RoleId == model.RoleId)
						.FirstOrDefault();
					if (dbUser.Role == null)
						return BadRequest(new { message = "Unknown role!" });

					dbUser.RoleId = model.RoleId;
				}
			}

			dbUser.FullName = model.FullName;
			dbUser.UserName = model.UserName;
			dbUser.Password = model.Password;

			_dbContext.SaveChanges();

			return NoContent();
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpDelete("api/User/{userId}")]
		public IActionResult DeleteUser(int userId)
		{
			var user = _dbContext.Users
				.Include(x => x.Role)
				.Where(x => x.UserId == userId)
				.FirstOrDefault();

			if (user == null)
				return NotFound();

			if (_userContext.GetUserRole() == RoleEnum.Manager && user.Role.Name == RoleEnum.Admin)
				return BadRequest(new { message = "Manager can not perform this action!" });

			if (user.Role.Name == RoleEnum.Manager)
			{
				var isExistEmployee = _dbContext.Users
					.Where(x => x.ManagerId == user.UserId)
					.Any();

				var isExistProject = _dbContext.Projects
					.Where(x => x.ResponsiblePersonId == user.UserId)
					.Any();

				var isExistActivity = _dbContext.Projects
					.Where(x => x.ResponsiblePersonId == user.UserId)
					.Any();

				if (isExistEmployee || isExistProject || isExistActivity)
					return BadRequest(new { message = "Can not delete manager!" });
			}

			_dbContext.Users.Remove(user);
			_dbContext.SaveChanges();

			return NoContent();
		}

		[HttpPut("api/User/{userId}/ChangePassword")]
		public IActionResult ChangePassword(int userId, [FromBody] ChangePasswordModel model)
		{
			model.UserId = userId;

			var dbUser = _dbContext.Users
				.Where(x => x.UserId == model.UserId)
				.FirstOrDefault();
			if (dbUser == null)
				return BadRequest(new { message = "Unknown user!" });

			if (ValidatePassword(model.Password) == false)
				return BadRequest(new { message = "Password is not valid!" });

			dbUser.Password = model.Password;

			_dbContext.SaveChanges();

			return NoContent();
		}

		#region Private methods

		private bool ValidateUserName(string userName)
		{
			if (Regex.IsMatch(userName, @"^\d+") || userName.Any(char.IsUpper))
				return false;

			return true;
		}

		private bool ValidatePassword(string password)
		{
			Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_+])[A-Za-z\d@$!%*?_+&]{8,25}$");

			if (passwordRegex.IsMatch(password))
				return true;

			return false;
		}

		#endregion
	}
}