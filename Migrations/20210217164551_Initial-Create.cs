using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace EmployeeTimesheet.Migrations
{
	public partial class InitialCreate : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
					name: "Roles",
					columns: table => new
					{
						RoleId = table.Column<int>(nullable: false)
									.Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
						Name = table.Column<string>(nullable: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_Roles", x => x.RoleId);
					});

			migrationBuilder.CreateTable(
					name: "Users",
					columns: table => new
					{
						UserId = table.Column<int>(nullable: false)
									.Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
						FullName = table.Column<string>(maxLength: 100, nullable: false),
						UserName = table.Column<string>(maxLength: 15, nullable: false),
						Password = table.Column<string>(maxLength: 20, nullable: false),
						ManagerId = table.Column<int>(nullable: true),
						RoleId = table.Column<int>(nullable: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_Users", x => x.UserId);
						table.ForeignKey(
											name: "FK_Users_Users_ManagerId",
											column: x => x.ManagerId,
											principalTable: "Users",
											principalColumn: "UserId",
											onDelete: ReferentialAction.Restrict);
						table.ForeignKey(
											name: "FK_Users_Roles_RoleId",
											column: x => x.RoleId,
											principalTable: "Roles",
											principalColumn: "RoleId",
											onDelete: ReferentialAction.Cascade);
					});

			migrationBuilder.CreateTable(
					name: "Activities",
					columns: table => new
					{
						ActivityId = table.Column<int>(nullable: false)
									.Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
						Name = table.Column<string>(nullable: false),
						ResponsiblePersonId = table.Column<int>(nullable: false),
						Finished = table.Column<bool>(nullable: false, defaultValue: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_Activities", x => x.ActivityId);
						table.ForeignKey(
											name: "FK_Activities_Users_ResponsiblePersonId",
											column: x => x.ResponsiblePersonId,
											principalTable: "Users",
											principalColumn: "UserId",
											onDelete: ReferentialAction.Cascade);
					});

			migrationBuilder.CreateTable(
					name: "Projects",
					columns: table => new
					{
						ProjectId = table.Column<int>(nullable: false)
									.Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
						Name = table.Column<string>(nullable: false),
						ResponsiblePersonId = table.Column<int>(nullable: false),
						Finished = table.Column<bool>(nullable: false, defaultValue: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_Projects", x => x.ProjectId);
						table.ForeignKey(
											name: "FK_Projects_Users_ResponsiblePersonId",
											column: x => x.ResponsiblePersonId,
											principalTable: "Users",
											principalColumn: "UserId",
											onDelete: ReferentialAction.Cascade);
					});

			migrationBuilder.CreateTable(
					name: "ActivityTimesheetEntries",
					columns: table => new
					{
						ActivityTimesheetEntryId = table.Column<int>(nullable: false)
									.Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
						ActivityId = table.Column<int>(nullable: false),
						UserId = table.Column<int>(nullable: false),
						Date = table.Column<DateTime>(nullable: false),
						WorkingHours = table.Column<TimeSpan>(nullable: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_ActivityTimesheetEntries", x => x.ActivityTimesheetEntryId);
						table.ForeignKey(
											name: "FK_ActivityTimesheetEntries_Activities_ActivityId",
											column: x => x.ActivityId,
											principalTable: "Activities",
											principalColumn: "ActivityId",
											onDelete: ReferentialAction.Cascade);
						table.ForeignKey(
											name: "FK_ActivityTimesheetEntries_Users_UserId",
											column: x => x.UserId,
											principalTable: "Users",
											principalColumn: "UserId",
											onDelete: ReferentialAction.Cascade);
					});

			migrationBuilder.CreateTable(
					name: "ProjectTimesheetEntries",
					columns: table => new
					{
						ProjectTimesheetEntryId = table.Column<int>(nullable: false)
									.Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
						ProjectId = table.Column<int>(nullable: false),
						UserId = table.Column<int>(nullable: false),
						Date = table.Column<DateTime>(nullable: false),
						WorkingHours = table.Column<TimeSpan>(nullable: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_ProjectTimesheetEntries", x => x.ProjectTimesheetEntryId);
						table.ForeignKey(
											name: "FK_ProjectTimesheetEntries_Projects_ProjectId",
											column: x => x.ProjectId,
											principalTable: "Projects",
											principalColumn: "ProjectId",
											onDelete: ReferentialAction.Cascade);
						table.ForeignKey(
											name: "FK_ProjectTimesheetEntries_Users_UserId",
											column: x => x.UserId,
											principalTable: "Users",
											principalColumn: "UserId",
											onDelete: ReferentialAction.Cascade);
					});

			migrationBuilder.InsertData(
					table: "Roles",
					columns: new[] { "RoleId", "Name" },
					values: new object[] { 1, "Admin" });

			migrationBuilder.InsertData(
					table: "Roles",
					columns: new[] { "RoleId", "Name" },
					values: new object[] { 2, "Manager" });

			migrationBuilder.InsertData(
					table: "Roles",
					columns: new[] { "RoleId", "Name" },
					values: new object[] { 3, "Employee" });

			migrationBuilder.CreateIndex(
					name: "IX_Activities_ResponsiblePersonId",
					table: "Activities",
					column: "ResponsiblePersonId");

			migrationBuilder.CreateIndex(
					name: "IX_ActivityTimesheetEntries_ActivityId",
					table: "ActivityTimesheetEntries",
					column: "ActivityId");

			migrationBuilder.CreateIndex(
					name: "IX_ActivityTimesheetEntries_UserId",
					table: "ActivityTimesheetEntries",
					column: "UserId");

			migrationBuilder.CreateIndex(
					name: "IX_Projects_ResponsiblePersonId",
					table: "Projects",
					column: "ResponsiblePersonId");

			migrationBuilder.CreateIndex(
					name: "IX_ProjectTimesheetEntries_ProjectId",
					table: "ProjectTimesheetEntries",
					column: "ProjectId");

			migrationBuilder.CreateIndex(
					name: "IX_ProjectTimesheetEntries_UserId",
					table: "ProjectTimesheetEntries",
					column: "UserId");

			migrationBuilder.CreateIndex(
					name: "IX_Users_ManagerId",
					table: "Users",
					column: "ManagerId");

			migrationBuilder.CreateIndex(
					name: "IX_Users_RoleId",
					table: "Users",
					column: "RoleId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
					name: "ActivityTimesheetEntries");

			migrationBuilder.DropTable(
					name: "ProjectTimesheetEntries");

			migrationBuilder.DropTable(
					name: "Activities");

			migrationBuilder.DropTable(
					name: "Projects");

			migrationBuilder.DropTable(
					name: "Users");

			migrationBuilder.DropTable(
					name: "Roles");
		}
	}
}
