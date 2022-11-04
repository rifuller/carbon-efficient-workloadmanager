// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.ComponentModel.DataAnnotations;

namespace Model;

public class Job
{
	public Job()
	{
	}

	public Job(string name, JobConstraints constraints, Region homeRegion, DateTimeOffset startNotBefore)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
		}

		Name = name;
		Constraints = constraints;
		HomeRegion = homeRegion;
		StartNotBefore = startNotBefore;
	}

	[Key]
	public int JobId { get; set; }

	public string Name { get; set; } = string.Empty;

	public JobConstraints Constraints { get; set; }

	public Region HomeRegion { get; set; }

	public DateTimeOffset StartNotBefore { get; set; }

	public DateTimeOffset? StartNotAfter { get; set; }

	// The base line
	public double? BaselineRating { get; set; }

	public DateTimeOffset? BaselineRatingFetchedAt { get; set; }

	// Prediction of the rating (for timeshifting)
	public Region? PredictedRegion { get; set; }

	public double? PredictedRating { get; set; }

	public DateTimeOffset? PredictedRatingRetrievedAt { get; set; }

	// Actual rating
	public Region? ExecutionRegion { get; set; }

	public double? ExecutionRating { get; set; }

	public DateTimeOffset? ExecutionRatingRetrievedAt { get; set; }

	public RoutingDecision? RoutingDecision { get; set; }

	public double? Savings => ExecutionRating != null ? BaselineRating - ExecutionRating : null;

	//public Uri? ResultsUri { get; set; }

	//public DateTimeOffset? StartedAt { get; set; }

	//public DateTimeOffset? CompletedAt { get; set; }

	public DateTimeOffset CreatedAt { get; set; }

	public DateTimeOffset UpdatedAt { get; set; }

	public override string ToString()
	{
		return $"{JobId} - Home:{HomeRegion} Baseline: {BaselineRating} - Realised: {ExecutionRating}({RoutingDecision}) Savings: {ExecutionRating - BaselineRating}";
	}
}
