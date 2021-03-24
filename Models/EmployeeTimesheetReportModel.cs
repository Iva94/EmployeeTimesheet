using System;
using System.Collections.Generic;

namespace EmployeeTimesheet.Models
{
	public class EmployeeTimesheetReportParameters
	{
		public string GroupBy { get; set; }
		public DateTime DateFrom { get; set; }
		public DateTime DateTo { get; set; }
	}

	public class EmployeeTimesheetReportModel
	{
		public int ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string UserFullName { get; set; }

		public List<TimeSpan> Hours { get; set; }
		public double Sum { get; set; }
	}
}