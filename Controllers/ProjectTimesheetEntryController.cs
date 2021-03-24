using EmployeeTimesheet.Database;
using EmployeeTimesheet.Database.Entities;
using EmployeeTimesheet.Models;
using EmployeeTimesheet.Shared;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeTimesheet.Controllers
{
	[Authorize]
	[ApiController]
	public class ProjectTimesheetEntryController : ControllerBase
	{
		private readonly EmployeeTimesheetDbContext _dbContext;
		private readonly UserContext _userContext;

		public ProjectTimesheetEntryController(EmployeeTimesheetDbContext dbContext,
			UserContext userContext)
		{
			_dbContext = dbContext;
			_userContext = userContext;
		}

		[HttpGet("api/ProjectTimesheetEntry")]
		public ActionResult<List<ProjectTimesheetEntryModel>> GetProjectTimesheetEntries(int? userId)
		{
			var query = _dbContext.ProjectTimesheetEntries
				.AsQueryable();

			if (userId.HasValue)
				query = query.Where(x => x.UserId == userId);

			var projectTimesheetEntries = query
				.OrderBy(x => x.Date)
				.ProjectToType<ProjectTimesheetEntryModel>()
				.ToList();

			return projectTimesheetEntries;
		}

		[HttpGet("api/ProjectTimesheetEntry/{projectTimesheetEntryId}")]
		public ActionResult<ProjectTimesheetEntryModel> GetProjectTimesheetEntry(int projectTimesheetEntryId)
		{
			var projectTimesheetEntry = _dbContext.ProjectTimesheetEntries
			.Where(x => x.ProjectTimesheetEntryId == projectTimesheetEntryId)
			.ProjectToType<ProjectTimesheetEntryModel>()
			.FirstOrDefault();

			if (projectTimesheetEntry == null)
				return NotFound();

			return projectTimesheetEntry;
		}

		[HttpPost("api/ProjectTimesheetEntry")]
		public ActionResult<ProjectTimesheetEntry> CreateProjectTimesheetEntry([FromBody] ProjectTimesheetEntryModel model)
		{
			var newProjectTimesheetEntry = model.Adapt<ProjectTimesheetEntry>();

			newProjectTimesheetEntry.Project = _dbContext.Projects
				.Where(x => x.ProjectId == model.ProjectId)
				.FirstOrDefault();
			if (newProjectTimesheetEntry.Project == null)
				return BadRequest(new { message = "Unknown project!" });

			newProjectTimesheetEntry.User = _dbContext.Users
				.Where(x => x.UserId == model.UserId)
				.FirstOrDefault();
			if (newProjectTimesheetEntry.User == null)
				return BadRequest(new { message = "Unknown user!" });

			if (model.Date.Date < DateTime.Now.Date)
				return BadRequest(new { message = "Can not enter working hours for this date!" });

			newProjectTimesheetEntry.Date = model.Date;
			newProjectTimesheetEntry.WorkingHours = new TimeSpan(model.Hours ?? 0, model.Minutes ?? 0, 0);

			_dbContext.ProjectTimesheetEntries.Add(newProjectTimesheetEntry);
			_dbContext.SaveChanges();

			return Created($"/api/ProjectTimesheetEntry/{newProjectTimesheetEntry.ProjectTimesheetEntryId}", newProjectTimesheetEntry.ProjectTimesheetEntryId);
		}

		[HttpPut("api/ProjectTimesheetEntry/{projectTimesheetEntryId}")]
		public IActionResult UpdateProjectTimesheetEntry(int projectTimesheetEntryId, [FromBody] ProjectTimesheetEntryModel model)
		{
			model.ProjectTimesheetEntryId = projectTimesheetEntryId;

			var dbProjectTimesheetEntry = _dbContext.ProjectTimesheetEntries
				.Where(x => x.ProjectTimesheetEntryId == model.ProjectTimesheetEntryId)
				.FirstOrDefault();
			if (dbProjectTimesheetEntry == null)
				return BadRequest(new { message = "Unknown project timesheet entry!" });

			if (model.Date.Date < DateTime.Now.Date)
				return BadRequest(new { message = "Can not enter working hours for this date!" });

			dbProjectTimesheetEntry.Date = model.Date;
			dbProjectTimesheetEntry.WorkingHours = new TimeSpan(model.Hours ?? 0, model.Minutes ?? 0, 0);

			_dbContext.SaveChanges();

			return NoContent();
		}

		[HttpDelete("api/ProjectTimesheetEntry/{projectTimesheetEntryId}")]
		public IActionResult DeleteProjectTimesheetEntry(int projectTimesheetEntryId)
		{
			var projectTimesheetEntry = _dbContext.ProjectTimesheetEntries
				.Where(x => x.ProjectTimesheetEntryId == projectTimesheetEntryId)
				.FirstOrDefault();

			if (projectTimesheetEntry == null)
				return NotFound();

			_dbContext.ProjectTimesheetEntries.Remove(projectTimesheetEntry);
			_dbContext.SaveChanges();

			return NoContent();
		}
	}
}