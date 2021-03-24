using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeTimesheet.Database.Entities
{
	public class User
	{
		[Key]
		public int UserId { get; set; }

		[Required]
		[MaxLength(100)]
		public string FullName { get; set; }

		[Required]
		[MinLength(6)]
		[MaxLength(15)]
		public string UserName { get; set; }

		[Required]
		[MinLength(8)]
		[MaxLength(20)]
		public string Password { get; set; }

		public int? ManagerId { get; set; }
		[ForeignKey("ManagerId")]
		public User Manager { get; set; }

		[Required]
		public int RoleId { get; set; }
		[ForeignKey("RoleId")]
		public Role Role { get; set; }
	}
}