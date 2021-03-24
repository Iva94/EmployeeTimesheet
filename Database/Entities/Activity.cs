using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeTimesheet.Database.Entities
{
	public class Activity
	{
		[Key]
		public int ActivityId { get; set; }
		[Required]
		public string Name { get; set; }

		[Required]
		public int ResponsiblePersonId { get; set; }
		[ForeignKey("ResponsiblePersonId")]
		public User ResponsiblePerson { get; set; }

		public bool Finished { get; set; }
	}
}