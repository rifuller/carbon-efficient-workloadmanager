// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
	public partial class AddPredictedActual : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "ResultsUri",
				table: "Jobs",
				newName: "StartNotAfter");

			migrationBuilder.RenameColumn(
				name: "CompletedAt",
				table: "Jobs",
				newName: "PredictedRatingRetrievedAt");

			migrationBuilder.AddColumn<double>(
				name: "BaselineRating",
				table: "Jobs",
				type: "REAL",
				nullable: true);

			migrationBuilder.AddColumn<DateTimeOffset>(
				name: "BaselineRatingFetchedAt",
				table: "Jobs",
				type: "TEXT",
				nullable: true);

			migrationBuilder.AddColumn<double>(
				name: "ExecutionRating",
				table: "Jobs",
				type: "REAL",
				nullable: true);

			migrationBuilder.AddColumn<DateTimeOffset>(
				name: "ExecutionRatingRetrievedAt",
				table: "Jobs",
				type: "TEXT",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "ExecutionRegion",
				table: "Jobs",
				type: "INTEGER",
				nullable: true);

			migrationBuilder.AddColumn<double>(
				name: "PredictedRating",
				table: "Jobs",
				type: "REAL",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "PredictedRegion",
				table: "Jobs",
				type: "INTEGER",
				nullable: true);

			migrationBuilder.AddColumn<DateTimeOffset>(
				name: "StartNotBefore",
				table: "Jobs",
				type: "TEXT",
				nullable: false,
				defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "BaselineRating",
				table: "Jobs");

			migrationBuilder.DropColumn(
				name: "BaselineRatingFetchedAt",
				table: "Jobs");

			migrationBuilder.DropColumn(
				name: "ExecutionRating",
				table: "Jobs");

			migrationBuilder.DropColumn(
				name: "ExecutionRatingRetrievedAt",
				table: "Jobs");

			migrationBuilder.DropColumn(
				name: "ExecutionRegion",
				table: "Jobs");

			migrationBuilder.DropColumn(
				name: "PredictedRating",
				table: "Jobs");

			migrationBuilder.DropColumn(
				name: "PredictedRegion",
				table: "Jobs");

			migrationBuilder.DropColumn(
				name: "StartNotBefore",
				table: "Jobs");

			migrationBuilder.RenameColumn(
				name: "StartNotAfter",
				table: "Jobs",
				newName: "ResultsUri");

			migrationBuilder.RenameColumn(
				name: "PredictedRatingRetrievedAt",
				table: "Jobs",
				newName: "CompletedAt");
		}
	}
}
