using EmployeeTimesheet.Database;
using EmployeeTimesheet.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeTimesheet.Controllers
{
	[Authorize]
	[ApiController]
	public class RoleController : ControllerBase
	{
		private readonly EmployeeTimesheetDbContext _dbContext;

		public RoleController(EmployeeTimesheetDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet("api/Role")]
		public ActionResult<List<RoleModel>> GetRoles()
		{
			var roles = _dbContext.Roles
				.ProjectToType<RoleModel>()
				.ToList();

			return roles;
		}

		[HttpGet("api/Role/{roleId}")]
		public ActionResult<RoleModel> GetRole(int roleId)
		{
			var role = _dbContext.Roles
				.Where(x => x.RoleId == roleId)
				.ProjectToType<RoleModel>()
				.SingleOrDefault();

			return role;
		}
	}
}