// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace JobRouter;

public interface IJobRouter
{
	Task<RoutingResult> GetTargetAsync(Job job, CancellationToken ct);

	/// <summary>
	/// Gets the target region for a given job.
	/// </summary>
	/// <param name="job"></param>
	/// <param name="acceptableRegions"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	Task<RoutingResult> GetTargetAsync(Job job, IReadOnlySet<Region> acceptableRegions, CancellationToken ct);
}
