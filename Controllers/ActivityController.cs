using EmployeeTimesheet.Database;
using EmployeeTimesheet.Database.Entities;
using EmployeeTimesheet.Models;
using EmployeeTimesheet.Shared;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeTimesheet.Controllers
{
	[Authorize]
	[ApiController]
	public class ActivityController : ControllerBase
	{
		private readonly EmployeeTimesheetDbContext _dbContext;
		private readonly UserContext _userContext;

		public ActivityController(EmployeeTimesheetDbContext dbContext,
			UserContext userContext)
		{
			_dbContext = dbContext;
			_userContext = userContext;
		}

		[HttpGet("api/Activity")]
		public ActionResult<List<ActivityModel>> GetActivities(int? userId)
		{
			var query = _dbContext.Activities
				.AsQueryable();

			if (userId.HasValue && _userContext.GetUserRole() == RoleEnum.Employee)
			{
				var responsiblePersonId = _dbContext.Users
					.Where(x => x.UserId == userId)
					.Select(x => x.ManagerId)
					.FirstOrDefault();

				query = query.Where(x => x.ResponsiblePersonId == responsiblePersonId);
			}

			var activities = query
				.OrderBy(x => x.Name)
				.ProjectToType<ActivityModel>()
				.ToList();

			return activities;
		}

		[HttpGet("api/Activity/{activityId}")]
		public ActionResult<ActivityModel> GetActivity(int activityId)
		{
			var activity = _dbContext.Activities
				.Where(x => x.ActivityId == activityId)
				.ProjectToType<ActivityModel>()
				.FirstOrDefault();

			if (activity == null)
				return NotFound();

			return activity;
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpPost("api/Activity")]
		public ActionResult CreateActivity([FromBody] ActivityModel model)
		{
			var newActivity = model.Adapt<Activity>();

			newActivity.ResponsiblePerson = _dbContext.Users
				.Where(x => x.UserId == model.ResponsiblePersonId)
				.FirstOrDefault();
			if (newActivity.ResponsiblePerson == null)
				return BadRequest(new { message = "Unknown responsible person!" });

			newActivity.Finished = false;

			_dbContext.Activities.Add(newActivity);
			_dbContext.SaveChanges();

			return Created($"/api/Activity/{newActivity.ActivityId}", newActivity.ActivityId);
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpPut("api/Activity/{activityId}")]
		public IActionResult UpdateActivity(int activityId, [FromBody] ActivityModel model)
		{
			model.ActivityId = activityId;

			var dbActivity = _dbContext.Activities
				.Where(x => x.ActivityId == model.ActivityId)
				.FirstOrDefault();
			if (dbActivity == null)
				return BadRequest(new { message = "Unknown activity!" });

			if (model.ResponsiblePersonId != dbActivity.ResponsiblePersonId)
			{
				dbActivity.ResponsiblePerson = _dbContext.Users
					.Where(x => x.UserId == model.ResponsiblePersonId)
					.FirstOrDefault();
				if (dbActivity.ResponsiblePerson == null)
					return BadRequest(new { message = "Unknown responsible person!" });

				dbActivity.ResponsiblePersonId = model.ResponsiblePersonId;
			}

			dbActivity.Name = model.Name;

			_dbContext.SaveChanges();

			return NoContent();
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpDelete("api/Activity/{activityId}")]
		public IActionResult DeleteActivity(int activityId)
		{
			var activity = _dbContext.Activities
				.Where(x => x.ActivityId == activityId)
				.FirstOrDefault();

			if (activity == null)
				return NotFound();

			var canNotDeleteActivity = _dbContext.ActivityTimesheetEntries
				.Where(x => x.ActivityId == activityId)
				.Any();

			if (canNotDeleteActivity)
				return BadRequest(new { message = "It is not possible to delete the activity" });

			_dbContext.Activities.Remove(activity);
			_dbContext.SaveChanges();

			return NoContent();
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpPut("api/Activity/{activityId}/Finish")]
		public IActionResult FinishActivity(int activityId)
		{
			var dbActivity = _dbContext.Activities
				.Where(x => x.ActivityId == activityId)
				.FirstOrDefault();
			if (dbActivity == null)
				return BadRequest(new { message = "Unknown activity!" });

			dbActivity.Finished = true;

			_dbContext.SaveChanges();

			return NoContent();
		}
	}
}