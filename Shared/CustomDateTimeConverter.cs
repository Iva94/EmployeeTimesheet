using Newtonsoft.Json.Converters;

namespace EmployeeTimesheet.Shared
{
	public class CustomDateConverter : IsoDateTimeConverter
	{
		public CustomDateConverter()
		{
			base.DateTimeFormat = "yyyy-mm-dd";
		}
	}

	public class CustomTimeConverter : IsoDateTimeConverter
	{
		public CustomTimeConverter()
		{
			base.DateTimeFormat = "HH:mm";
		}
	}
}