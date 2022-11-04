// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace JobRouter;

public class RoutingResult
{
	public RoutingResult(double homeRegionRating, RoutingDecision routingDecision, Region targetRegion, double targetRegionRating, DateTimeOffset targetRegionRatingFetchedAt)
	{
		HomeRegionRating = homeRegionRating;
		RoutingDecision = routingDecision;
		TargetRegion = targetRegion;
		TargetRegionRating = targetRegionRating;
		TargetRegionRatingFetchedAt = targetRegionRatingFetchedAt;
	}

	public double HomeRegionRating { get; set; }

	public RoutingDecision RoutingDecision { get; set; }

	public Region TargetRegion { get; set; }

	public double TargetRegionRating { get; set; }

	public DateTimeOffset TargetRegionRatingFetchedAt { get; set; }
}
