using EmployeeTimesheet.Database;
using EmployeeTimesheet.Database.Entities;
using EmployeeTimesheet.Models;
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
	public class ActivityTimesheetEntryController : ControllerBase
	{
		private readonly EmployeeTimesheetDbContext _dbContext;

		public ActivityTimesheetEntryController(EmployeeTimesheetDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet("api/ActivityTimesheetEntry")]
		public ActionResult<List<ActivityTimesheetEntryModel>> GetActivityTimesheetEntries(int? userId)
		{
			var query = _dbContext.ActivityTimesheetEntries
				.AsQueryable();

			if (userId.HasValue)
				query = query.Where(x => x.UserId == userId);

			var activityTimesheetEntries = query
				.OrderBy(x => x.Date)
				.ProjectToType<ActivityTimesheetEntryModel>()
				.ToList();

			return activityTimesheetEntries;
		}

		[HttpGet("api/ActivityTimesheetEntry/{activityTimesheetEntryId}")]
		public ActionResult<ActivityTimesheetEntryModel> GetActivityTimesheetEntry(int activityTimesheetEntryId)
		{
			var activityTimesheetEntry = _dbContext.ActivityTimesheetEntries
			.Where(x => x.ActivityTimesheetEntryId == activityTimesheetEntryId)
			.ProjectToType<ActivityTimesheetEntryModel>()
			.FirstOrDefault();

			if (activityTimesheetEntry == null)
				return NotFound();

			return activityTimesheetEntry;
		}

		[HttpPost("api/ActivityTimesheetEntry")]
		public ActionResult<ActivityTimesheetEntry> CreateActivityTimesheetEntry([FromBody] ActivityTimesheetEntryModel model)
		{
			var newActivityTimesheetEntry = model.Adapt<ActivityTimesheetEntry>();

			newActivityTimesheetEntry.Activity = _dbContext.Activities
				.Where(x => x.ActivityId == model.ActivityId)
				.FirstOrDefault();
			if (newActivityTimesheetEntry.Activity == null)
				return BadRequest(new { message = "Unknown activity!" });

			newActivityTimesheetEntry.User = _dbContext.Users
				.Where(x => x.UserId == model.UserId)
				.FirstOrDefault();
			if (newActivityTimesheetEntry.User == null)
				return BadRequest(new { message = "Unknown user!" });

			if (model.Date.Date < DateTime.Now.Date)
				return BadRequest(new { message = "Can not enter working hours for this date!" });

			newActivityTimesheetEntry.Date = model.Date;
			newActivityTimesheetEntry.WorkingHours = new TimeSpan(model.Hours ?? 0, model.Minutes ?? 0, 0);

			_dbContext.ActivityTimesheetEntries.Add(newActivityTimesheetEntry);
			_dbContext.SaveChanges();

			return Created($"/api/ActivityTimesheetEntry/{newActivityTimesheetEntry.ActivityTimesheetEntryId}", newActivityTimesheetEntry.ActivityTimesheetEntryId);
		}

		[HttpPut("api/ActivityTimesheetEntry/{activityTimesheetEntryId}")]
		public IActionResult UpdateActivityTimesheetEntry(int activityTimesheetEntryId, [FromBody] ActivityTimesheetEntryModel model)
		{
			model.ActivityTimesheetEntryId = activityTimesheetEntryId;

			var dbActivityTimesheetEntry = _dbContext.ActivityTimesheetEntries
				.Where(x => x.ActivityTimesheetEntryId == model.ActivityTimesheetEntryId)
				.FirstOrDefault();
			if (dbActivityTimesheetEntry == null)
				return BadRequest(new { message = "Unknown activity timesheet entry!" });

			if (model.Date.Date < DateTime.Now.Date)
				return BadRequest(new { message = "Can not enter working hours for this date!" });

			dbActivityTimesheetEntry.Date = model.Date;
			dbActivityTimesheetEntry.WorkingHours = new TimeSpan(model.Hours ?? 0, model.Minutes ?? 0, 0);

			_dbContext.SaveChanges();

			return NoContent();
		}

		[HttpDelete("api/ActivityTimesheetEntry/{activityTimesheetEntryId}")]
		public ActionResult<ActivityTimesheetEntry> DeleteActivityTimesheetEntry(int activityTimesheetEntryId)
		{
			var activityTimesheetEntry = _dbContext.ActivityTimesheetEntries
				.Where(x => x.ActivityTimesheetEntryId == activityTimesheetEntryId)
				.FirstOrDefault();

			if (activityTimesheetEntry == null)
				return NotFound();

			_dbContext.ActivityTimesheetEntries.Remove(activityTimesheetEntry);
			_dbContext.SaveChanges();

			return activityTimesheetEntry;
		}
	}
}