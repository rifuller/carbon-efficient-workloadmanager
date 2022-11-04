// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
	public partial class SomeJobPropertiesCanBeNull : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<int>(
				name: "RoutingDecision",
				table: "Jobs",
				type: "INTEGER",
				nullable: true,
				oldClrType: typeof(int),
				oldType: "INTEGER");

			migrationBuilder.AlterColumn<string>(
				name: "ResultsUri",
				table: "Jobs",
				type: "TEXT",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "TEXT");

			migrationBuilder.AlterColumn<DateTime>(
				name: "CompletedAt",
				table: "Jobs",
				type: "TEXT",
				nullable: true,
				oldClrType: typeof(DateTime),
				oldType: "TEXT");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<int>(
				name: "RoutingDecision",
				table: "Jobs",
				type: "INTEGER",
				nullable: false,
				defaultValue: 0,
				oldClrType: typeof(int),
				oldType: "INTEGER",
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "ResultsUri",
				table: "Jobs",
				type: "TEXT",
				nullable: false,
				defaultValue: "",
				oldClrType: typeof(string),
				oldType: "TEXT",
				oldNullable: true);

			migrationBuilder.AlterColumn<DateTime>(
				name: "CompletedAt",
				table: "Jobs",
				type: "TEXT",
				nullable: false,
				defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
				oldClrType: typeof(DateTime),
				oldType: "TEXT",
				oldNullable: true);
		}
	}
}
