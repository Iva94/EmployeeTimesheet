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
	public class ProjectController : ControllerBase
	{
		private readonly EmployeeTimesheetDbContext _dbContext;
		private readonly UserContext _userContext;

		public ProjectController(EmployeeTimesheetDbContext dbContext,
			UserContext userContext)
		{
			_dbContext = dbContext;
			_userContext = userContext;
		}

		[HttpGet("api/Project")]
		public ActionResult<List<ProjectModel>> GetProjects(int? userId)
		{
			var query = _dbContext.Projects
				.AsQueryable();

			if (userId.HasValue && _userContext.GetUserRole() == RoleEnum.Employee)
			{
				var responsiblePersonId = _dbContext.Users
					.Where(x => x.UserId == userId)
					.Select(x => x.ManagerId)
					.FirstOrDefault();

				query = query.Where(x => x.ResponsiblePersonId == responsiblePersonId);
			}

			var projects = query
				.OrderBy(x => x.Name)
				.ProjectToType<ProjectModel>()
				.ToList();

			return projects;
		}

		[HttpGet("api/Project/{projectId}")]
		public ActionResult<ProjectModel> GetProject(int projectId)
		{
			var project = _dbContext.Projects
				.Where(x => x.ProjectId == projectId)
				.ProjectToType<ProjectModel>()
				.FirstOrDefault();

			if (project == null)
				return NotFound();

			return project;
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpPost("api/Project")]
		public ActionResult<Project> CreateProject([FromBody] ProjectModel model)
		{
			var newProject = model.Adapt<Project>();

			newProject.ResponsiblePerson = _dbContext.Users
				.Where(x => x.UserId == model.ResponsiblePersonId)
				.FirstOrDefault();
			if (newProject.ResponsiblePerson == null)
				return BadRequest(new { message = "Unknown responsible person!" });

			newProject.Finished = false;

			_dbContext.Projects.Add(newProject);
			_dbContext.SaveChanges();

			return Created($"/api/Project/{newProject.ProjectId}", newProject.ProjectId);
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpPut("api/Project/{projectId}")]
		public IActionResult UpdateProject(int projectId, [FromBody] ProjectModel model)
		{
			model.ProjectId = projectId;

			var dbProject = _dbContext.Projects
				.Where(x => x.ProjectId == model.ProjectId)
				.FirstOrDefault();
			if (dbProject == null)
				return BadRequest(new { message = "Unknown project!" });

			if (model.ResponsiblePersonId != dbProject.ResponsiblePersonId)
			{
				dbProject.ResponsiblePerson = _dbContext.Users
					.Where(x => x.UserId == model.ResponsiblePersonId)
					.FirstOrDefault();
				if (dbProject.ResponsiblePerson == null)
					return BadRequest(new { message = "Unknown responsible person!" });

				dbProject.ResponsiblePersonId = model.ResponsiblePersonId;
			}

			dbProject.Name = model.Name;

			_dbContext.SaveChanges();

			return NoContent();
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpDelete("api/Project/{projectId}")]
		public IActionResult DeleteProject(int projectId)
		{
			var project = _dbContext.Projects
				.Where(x => x.ProjectId == projectId)
				.FirstOrDefault();

			if (project == null)
				return NotFound();

			var canNotDeleteProject = _dbContext.ProjectTimesheetEntries
				.Where(x => x.ProjectId == projectId)
				.Any();

			if (canNotDeleteProject)
				return BadRequest(new { message = "It is not possible to delete the project!" });

			_dbContext.Projects.Remove(project);
			_dbContext.SaveChanges();

			return NoContent();
		}

		[Authorize(Roles = "Admin, Manager")]
		[HttpPut("api/Project/{projectId}/Finish")]
		public IActionResult FinishProject(int projectId)
		{
			var dbProject = _dbContext.Projects
				.Where(x => x.ProjectId == projectId)
				.FirstOrDefault();
			if (dbProject == null)
				return BadRequest(new { message = "Unknown project!" });

			dbProject.Finished = true;

			_dbContext.SaveChanges();

			return NoContent();
		}
	}
}