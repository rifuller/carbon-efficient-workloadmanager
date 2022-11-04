// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace CarbonAwareWebApiClient;

public class CarbonAwareWebApiClient : ICarbonAwareWebApiClient
{
	private static readonly HttpClient _client = new();

	private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	private readonly IOptionsMonitor<CarbonAwareWebApiClientOptions> _options;
	private readonly ILogger<CarbonAwareWebApiClient> _log;

	private readonly IReadOnlyDictionary<string, Region> WattTimeLocationToRegionMap = new Dictionary<string, Region>()
	{
		{ "NEM_VIC", Region.AustraliaSouthEast },
		{ "NEM_NSW", Region.AustraliaEast },
		{ "PJM_ROANOKE", Region.EastUS },
		{ "MISO_MASON_CITY", Region.CentralUS },
		{ "CAISO_NORTH", Region.WestUS },
		{ "GCPD", Region.WestUS2 },
		{ "UK", Region.UKSouth },
		{ "IE", Region.NorthEurope },
		{ "IESO_NORTH", Region.CanadaCentral },
		{ "DE", Region.GermanyWestCentral },
		{ "NO", Region.NorwayEast },
	};

	public CarbonAwareWebApiClient(IOptionsMonitor<CarbonAwareWebApiClientOptions> options, ILogger<CarbonAwareWebApiClient> log)
	{
		_options = options ?? throw new ArgumentNullException(nameof(options));
		_log = log ?? throw new ArgumentNullException(nameof(log));
	}

	public async Task<Dictionary<Region, EmissionsData>> GetEmissionsDataAsync(DateTimeOffset datetime, IEnumerable<Region> regions, CancellationToken ct)
	{
		// Check there is at least one region
		if (!regions.Any())
		{
			throw new ArgumentException("At least one region must be specified", nameof(regions));
		}

		string locations = string.Join("&", regions.Select(region => $"location={region.ToString().ToLowerInvariant()}"));
		string time = HttpUtility.UrlEncode(datetime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
		var uri = new Uri($"{_options.CurrentValue.BaseUri}emissions/bylocations?time={time}&{locations}");

		_log.LogInformation("Calling Carbon Aware API: {Uri}", uri);

		// Call REST API
		var response = await _client.GetAsync(uri, ct);

		// Check response
		if (!response.IsSuccessStatusCode)
		{
			throw new HttpRequestException($"Error calling Carbon Aware API: {response.StatusCode} - {response.ReasonPhrase}", null, statusCode: System.Net.HttpStatusCode.InternalServerError);
		}

		// Deserialize response
		var body = response.Content.ReadAsStream(ct);
		List<EmissionsData>? emissionsData = await JsonSerializer.DeserializeAsync<List<EmissionsData>>(body, _jsonSerializerOptions, ct);

		// Check response
		if (emissionsData == null)
		{
			throw new HttpRequestException("Error deserializing Carbon Aware API response", null, statusCode: System.Net.HttpStatusCode.InternalServerError);
		}

		foreach (var data in emissionsData)
		{
			_log.LogInformation("Response: {Location} - {Timestamp} - {Rating} - {Duration}", data.Location, data.Time.ToString("o"), data.Rating, data.Duration);
		}

		// Check for duplicate locations
		var set = emissionsData.Select(x => x.Location).ToHashSet();
		if (set.Count != emissionsData.Count)
		{
			_log.LogWarning("Detected duplicate locations in response!!! Taking the first.");
			_log.LogWarning(uri.ToString());
			emissionsData = new List<EmissionsData> { emissionsData.First() };
		}

		return emissionsData.ToDictionary(x => this.ParseLocation(x.Location), x => x);
	}

	private Region ParseLocation(string location)
	{
		return Enum.TryParse<Region>(location, true, out Region region)
			? region
			: WattTimeLocationToRegionMap.ContainsKey(location)
				? WattTimeLocationToRegionMap[location]
				: throw new ApplicationException($"Unrecognised location returned by API '{location}'");
	}

	public async Task<EmissionsData> GetEmissionsDataAsync(DateTimeOffset datetime, Region region, CancellationToken ct)
	{
		var results = await this.GetEmissionsDataAsync(datetime, new[] { region }, ct);
		return results.First().Value;
	}
}
