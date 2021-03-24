using EmployeeTimesheet.Database;
using EmployeeTimesheet.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeTimesheet.Controllers
{
	[Authorize(Roles = "Admin, Manager")]
	[ApiController]
	public class EmloyeeTimesheetReportController : Controller
	{
		private readonly EmployeeTimesheetDbContext _dbContext;

		public EmloyeeTimesheetReportController(EmployeeTimesheetDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet("api/EmloyeeTimesheetReport")]
		public ActionResult<List<EmployeeTimesheetReportModel>> GetProjectTimesheetEntryReport([FromBody] EmployeeTimesheetReportParameters parameters)
		{
			var query = _dbContext.ProjectTimesheetEntries
				.Include(x => x.Project)
				.Include(x => x.User)
				.AsQueryable();

			var reportModels = new List<EmployeeTimesheetReportModel>();

			if (parameters.GroupBy.ToLower() == "project")
			{
				//TODO
			}

			var projectTimesheetEntries = query
				.OrderBy(x => x.Date)
				.ProjectToType<EmployeeTimesheetReportModel>()
				.ToList();

			return reportModels;
		}
	}
}
