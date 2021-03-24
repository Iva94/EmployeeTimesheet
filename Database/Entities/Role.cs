using System.ComponentModel.DataAnnotations;

namespace EmployeeTimesheet.Database.Entities
{
	public class Role
	{
		[Key]
		public int RoleId { get; set; }
		[Required]
		public string Name { get; set; }
	}

	public static class RoleEnum
	{
		public const string Admin = "Admin";
		public const string Manager = "Manager";
		public const string Employee = "Employee";
	}
}