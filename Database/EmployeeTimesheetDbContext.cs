using EmployeeTimesheet.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTimesheet.Database
{
	public class EmployeeTimesheetDbContext : DbContext
	{
		public EmployeeTimesheetDbContext(DbContextOptions options)
				: base(options)
		{ }

		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<Activity> Activities { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<ActivityTimesheetEntry> ActivityTimesheetEntries { get; set; }
		public DbSet<ProjectTimesheetEntry> ProjectTimesheetEntries { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Project>()
					.Property(x => x.Finished)
					.HasDefaultValue(false);

			modelBuilder.Entity<Activity>()
					.Property(x => x.Finished)
					.HasDefaultValue(false);

			//Seed(modelBuilder);
		}

		public void Seed(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Role>().HasData(
					new Role() { RoleId = 1, Name = "Admin" },
					new Role() { RoleId = 2, Name = "Manager" },
					new Role() { RoleId = 3, Name = "Employee" }
			);

			modelBuilder.Entity<User>().HasData(
					new User { UserId = 1, FullName = "Marko Markovic", UserName = "marko123", Password = "marko123", ManagerId = null, RoleId = 1 },
					new User { UserId = 2, FullName = "Janko Jankovic", UserName = "janko123", Password = "janko123", ManagerId = null, RoleId = 2 },
					new User { UserId = 3, FullName = "Mirna Mirkovic", UserName = "mirna123", Password = "mirna123", ManagerId = 2, RoleId = 2 },
					new User { UserId = 4, FullName = "Ana	Janjic", UserName = "anaana123", Password = "anaana123", ManagerId = 2, RoleId = 3 },
					new User { UserId = 4, FullName = "Milovan	Kralj", UserName = "milovan123", Password = "milovan123", ManagerId = 3, RoleId = 3 },
					new User { UserId = 5, FullName = "Esma Acan", UserName = "esma123", Password = "esma123", ManagerId = 3, RoleId = 3 }
			);

			modelBuilder.Entity<Project>().HasData(
					new Project { ProjectId = 1, ResponsiblePersonId = 1, Name = "Project 1", Finished = false },
					new Project { ProjectId = 2, ResponsiblePersonId = 1, Name = "Project 2", Finished = false },
					new Project { ProjectId = 3, ResponsiblePersonId = 1, Name = "Project 3", Finished = false },

					new Project { ProjectId = 4, ResponsiblePersonId = 2, Name = "Project 4", Finished = false },
					new Project { ProjectId = 5, ResponsiblePersonId = 2, Name = "Project 5", Finished = false },
					new Project { ProjectId = 6, ResponsiblePersonId = 2, Name = "Project 6", Finished = false },

					new Project { ProjectId = 7, ResponsiblePersonId = 3, Name = "Project 7", Finished = false },
					new Project { ProjectId = 8, ResponsiblePersonId = 3, Name = "Project 8", Finished = false },
					new Project { ProjectId = 9, ResponsiblePersonId = 3, Name = "Project 9", Finished = false }
			);

			modelBuilder.Entity<Activity>().HasData(
					new Activity { ActivityId = 1, ResponsiblePersonId = 1, Name = "Activity 1", Finished = false },
					new Activity { ActivityId = 2, ResponsiblePersonId = 1, Name = "Activity 2", Finished = false },
					new Activity { ActivityId = 3, ResponsiblePersonId = 1, Name = "Activity 3", Finished = false },

					new Activity { ActivityId = 4, ResponsiblePersonId = 2, Name = "Activity 4", Finished = false },
					new Activity { ActivityId = 5, ResponsiblePersonId = 2, Name = "Activity 5", Finished = false },
					new Activity { ActivityId = 6, ResponsiblePersonId = 2, Name = "Activity 6", Finished = false },

					new Activity { ActivityId = 7, ResponsiblePersonId = 3, Name = "Activity 7", Finished = false },
					new Activity { ActivityId = 8, ResponsiblePersonId = 3, Name = "Activity 8", Finished = false },
					new Activity { ActivityId = 9, ResponsiblePersonId = 3, Name = "Activity 9", Finished = false }
			);
		}
	}
}