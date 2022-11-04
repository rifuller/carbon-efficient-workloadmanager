// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace CarbonAwareWebApiClient;

// Saving time by not having an intermediate data model here.
public class EmissionsData
{
	public string Location { get; set; } = string.Empty;

	public DateTimeOffset Time { get; set; }

	public double Rating { get; set; }

	public TimeSpan Duration { get; set; }
}
