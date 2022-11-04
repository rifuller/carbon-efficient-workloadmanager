// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace CarbonAwareWebApiClient;

public interface ICarbonAwareWebApiClient
{
	Task<Dictionary<Region, EmissionsData>> GetEmissionsDataAsync(DateTimeOffset datetime, IEnumerable<Region> regions, CancellationToken ct);

	Task<EmissionsData> GetEmissionsDataAsync(DateTimeOffset datetime, Region region, CancellationToken ct);
}
