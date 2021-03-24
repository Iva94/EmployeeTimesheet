using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeTimesheet.Models
{
	public class ActivityTimesheetEntryModel
	{
		public int ActivityTimesheetEntryId { get; set; }

		public int ActivityId { get; set; }
		public string ActivityName { get; set; }

		public int UserId { get; set; }
		public string UserFullName { get; set; }

		[DataType(DataType.Date)]
		public DateTime Date { get; set; }

		public TimeSpan WorkingHours { get; set; }

		public int? Hours { get; set; }
		public int? Minutes { get; set; }
	}
}