using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeTimesheet.Database.Entities
{
	public class ProjectTimesheetEntry
	{
		[Key]
		public int ProjectTimesheetEntryId { get; set; }

		[Required]
		public int ProjectId { get; set; }
		[ForeignKey("ProjectId")]
		public Project Project { get; set; }

		[Required]
		public int UserId { get; set; }
		[ForeignKey("UserId")]
		public User User { get; set; }

		[DataType(DataType.Date)]
		public DateTime Date { get; set; }

		public TimeSpan WorkingHours { get; set; }
	}
}