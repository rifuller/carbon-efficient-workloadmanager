// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
	public partial class Changes : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_JobStateChangeHistory_Workers_WorkerName",
				table: "JobStateChangeHistory");

			migrationBuilder.RenameColumn(
				name: "Flags",
				table: "Jobs",
				newName: "Constraints");

			migrationBuilder.AlterColumn<string>(
				name: "WorkerName",
				table: "JobStateChangeHistory",
				type: "TEXT",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "TEXT");

			migrationBuilder.AddForeignKey(
				name: "FK_JobStateChangeHistory_Workers_WorkerName",
				table: "JobStateChangeHistory",
				column: "WorkerName",
				principalTable: "Workers",
				principalColumn: "Name");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_JobStateChangeHistory_Workers_WorkerName",
				table: "JobStateChangeHistory");

			migrationBuilder.RenameColumn(
				name: "Constraints",
				table: "Jobs",
				newName: "Flags");

			migrationBuilder.AlterColumn<string>(
				name: "WorkerName",
				table: "JobStateChangeHistory",
				type: "TEXT",
				nullable: false,
				defaultValue: "",
				oldClrType: typeof(string),
				oldType: "TEXT",
				oldNullable: true);

			migrationBuilder.AddForeignKey(
				name: "FK_JobStateChangeHistory_Workers_WorkerName",
				table: "JobStateChangeHistory",
				column: "WorkerName",
				principalTable: "Workers",
				principalColumn: "Name",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
