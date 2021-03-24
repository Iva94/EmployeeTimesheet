using System.ComponentModel.DataAnnotations;

namespace EmployeeTimesheet.Models
{
	public class AuthenticationModel
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }
	}
}