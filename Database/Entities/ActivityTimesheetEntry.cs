using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeTimesheet.Database.Entities
{
	public class ActivityTimesheetEntry
	{
		[Key]
		public int ActivityTimesheetEntryId { get; set; }

		[Required]
		public int ActivityId { get; set; }
		[ForeignKey("ActivityId")]
		public Activity Activity { get; set; }

		[Required]
		public int UserId { get; set; }
		[ForeignKey("UserId")]
		public User User { get; set; }

		[DataType(DataType.Date)]
		public DateTime Date { get; set; }

		public TimeSpan WorkingHours { get; set; }
	}
}