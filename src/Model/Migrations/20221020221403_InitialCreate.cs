// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
	public partial class InitialCreate : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Jobs",
				columns: table => new
				{
					JobId = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false),
					Flags = table.Column<int>(type: "INTEGER", nullable: false),
					HomeRegion = table.Column<int>(type: "INTEGER", nullable: false),
					RoutingDecision = table.Column<int>(type: "INTEGER", nullable: false),
					ResultsUri = table.Column<string>(type: "TEXT", nullable: false),
					CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Jobs", x => x.JobId);
				});

			migrationBuilder.CreateTable(
				name: "Workers",
				columns: table => new
				{
					Name = table.Column<string>(type: "TEXT", nullable: false),
					Region = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Workers", x => x.Name);
				});

			migrationBuilder.CreateTable(
				name: "JobStateChangeHistory",
				columns: table => new
				{
					JobStateChangeId = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					JobId = table.Column<int>(type: "INTEGER", nullable: false),
					State = table.Column<int>(type: "INTEGER", nullable: false),
					WorkerName = table.Column<string>(type: "TEXT", nullable: false),
					StateChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_JobStateChangeHistory", x => x.JobStateChangeId);
					table.ForeignKey(
						name: "FK_JobStateChangeHistory_Jobs_JobId",
						column: x => x.JobId,
						principalTable: "Jobs",
						principalColumn: "JobId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_JobStateChangeHistory_Workers_WorkerName",
						column: x => x.WorkerName,
						principalTable: "Workers",
						principalColumn: "Name",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_JobStateChangeHistory_JobId",
				table: "JobStateChangeHistory",
				column: "JobId");

			migrationBuilder.CreateIndex(
				name: "IX_JobStateChangeHistory_WorkerName",
				table: "JobStateChangeHistory",
				column: "WorkerName");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "JobStateChangeHistory");

			migrationBuilder.DropTable(
				name: "Jobs");

			migrationBuilder.DropTable(
				name: "Workers");
		}
	}
}
