namespace EmployeeTimesheet.Models
{
	public class ProjectModel
	{
		public int ProjectId { get; set; }
		public string Name { get; set; }

		public int ResponsiblePersonId { get; set; }
		public string ResponsiblePersonFullName { get; set; }

		public bool Finished { get; set; }
	}
}