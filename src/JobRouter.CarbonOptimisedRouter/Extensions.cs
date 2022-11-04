// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace JobRouter.CarbonOptimisedRouter;

public static class Extensions
{
	public static void UpdateFromResult(this Job job, RoutingResult routingResult)
	{
		job.BaselineRating = routingResult.HomeRegionRating;

		job.RoutingDecision = routingResult.RoutingDecision;

		job.ExecutionRating = routingResult.TargetRegionRating;
		job.ExecutionRegion = routingResult.TargetRegion;
		job.ExecutionRatingRetrievedAt = routingResult.TargetRegionRatingFetchedAt;
	}
}
