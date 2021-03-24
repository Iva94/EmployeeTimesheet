using System.Collections.Generic;

namespace EmployeeTimesheet.Models
{
	public class UserModel
	{
		public int UserId { get; set; }
		public string FullName { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }

		public int? ManagerId { get; set; }
		public string ManagerFullName { get; set; }

		public int RoleId { get; set; }
		public string RoleName { get; set; }

		public List<ProjectTimesheetEntryModel> ProjectTimesheetEntries { get; set; }
		public List<ActivityTimesheetEntryModel> ActivityTimesheetEntries { get; set; }
	}
}