// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using NSubstitute;

namespace JobRouter.CarbonOptimisedRouter.Tests
{
	[TestClass]
	public class CarbonOptimisedRouterCoreTests
	{
		IFixture _fixture;
		ILogger<CarbonOptimisedRouterCore> _log;

		public CarbonOptimisedRouterCoreTests()
		{
			_fixture = new Fixture().Customize(new AutoNSubstituteCustomization() { ConfigureMembers = true });
			_log = _fixture.Create<ILogger<CarbonOptimisedRouterCore>>();
		}

		[TestMethod]
		public void GetTargetAsyncThrowsWhenAcceptableRegionsIsEmpty()
		{
			Assert.ThrowsExceptionAsync<ArgumentException>(() =>
			{

				var router = new CarbonOptimisedRouterCore(_fixture.Create<ICarbonAwareWebApiClient>(), _log);
				return router.GetTargetAsync(_fixture.Create<Job>(), new HashSet<Region>(), CancellationToken.None);
			});
		}

		[TestMethod]
		public void GetTargetAsyncReturnsRegionWithLowestNonZeroRating()
		{
			var client = _fixture.Create<ICarbonAwareWebApiClient>();
			Dictionary<Region, EmissionsData> data = new()
			{
				{ Region.UKSouth, new EmissionsData() { Rating = 2 } },
				{ Region.NorwayEast, new EmissionsData() { Rating = 1 } },
				{ Region.CanadaCentral, new EmissionsData() { Rating = 0 } },
			};
			client.GetEmissionsDataAsync(Arg.Any<DateTimeOffset>(), Arg.Any<IEnumerable<Region>>(), Arg.Any<CancellationToken>()).Returns(data);

			var router = new CarbonOptimisedRouterCore(client, _log);
			var result = router.GetTargetAsync(_fixture.Create<Job>(), data.Select(x => x.Key).ToHashSet(), CancellationToken.None).Result;

			Assert.AreEqual(Region.NorwayEast, result.TargetRegion);
		}


		[TestMethod]
		public void GetTargetAsyncReturnsClosestGeographicalRegionWhenRatingsAreEqual()
		{
		}

	}
}
