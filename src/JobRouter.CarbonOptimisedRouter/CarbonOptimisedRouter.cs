// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace JobRouter;

public class CarbonOptimisedRouterCore : IJobRouter
{
	private readonly ICarbonAwareWebApiClient _apiClient;
	private ILogger<CarbonOptimisedRouterCore> _log;

	public CarbonOptimisedRouterCore(ICarbonAwareWebApiClient apiClient, ILogger<CarbonOptimisedRouterCore> log)
	{
		_apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
		_log = log ?? throw new ArgumentNullException(nameof(log));
	}

	/// <inheritdoc/>
	public Task<RoutingResult> GetTargetAsync(Job job, CancellationToken ct)
	{
		// Get all enum values for Region
		var regions = Enum.GetValues(typeof(Region)).Cast<Region>().ToHashSet();
		return this.GetTargetAsync(job, regions, ct);
	}

	/// <inheritdoc/>
	public async Task<RoutingResult> GetTargetAsync(Job job, IReadOnlySet<Region> acceptableRegions, CancellationToken ct)
	{
		// Check acceptableRegions has at least one value
		if (!acceptableRegions.Any())
		{
			throw new ArgumentException("acceptableRegions must have at least one value", nameof(acceptableRegions));
		}

		if (job.StartNotAfter != null)
		{
			throw new NotImplementedException("Can't handle forecasts yet");
		}

		_log.LogInformation("Requesting emissions data");
		var orig = await _apiClient.GetEmissionsDataAsync(job.StartNotBefore, job.HomeRegion, ct);
		var data = await _apiClient.GetEmissionsDataAsync(job.StartNotBefore, acceptableRegions, ct);

		_log.LogInformation("Emissions data received");

		// Select the region with the lowest non-zero rating
		var best = data.Where(x => x.Value.Rating > 0).OrderBy(x => x.Value.Rating).FirstOrDefault();

		return best.Value is not null
			? new RoutingResult(orig.Rating, RoutingDecision.GeoShifted, best.Key, best.Value.Rating, DateTimeOffset.Now)
			: new RoutingResult(orig.Rating, RoutingDecision.GeoShifted, job.HomeRegion, orig.Rating, DateTimeOffset.Now);
	}
}
