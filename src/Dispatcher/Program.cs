// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace DispatcherFunctions;

public class Program
{
	public static void Main()
	{
		var host = new HostBuilder()
		.ConfigureFunctionsWorkerDefaults()
		.ConfigureAppConfiguration(configBuilder =>
		{
			string env = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production";
			configBuilder
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{env}.json", optional: true)
				.AddEnvironmentVariables();
		})
		.ConfigureServices((context, services) =>
			{
				var config = context.Configuration;

				services.AddLogging();
				services.AddSingleton<IJobRouter, CarbonOptimisedRouterCore>();
				services.AddSingleton<DispatcherContext>();

				services.AddSingleton<ICarbonAwareWebApiClient, CarbonAwareWebApiClient.CarbonAwareWebApiClient>();
				services.Configure<CarbonAwareWebApiClientOptions>(config.GetSection(nameof(CarbonAwareWebApiClient)));
			})
			.Build();

		CreateDbIfNotExists(host);

		host.Run();
	}

	private static void CreateDbIfNotExists(IHost host)
	{
		using var scope = host.Services.CreateScope();
		var services = scope.ServiceProvider;
		try
		{
			var context = services.GetRequiredService<DispatcherContext>();
			DbInitialiser.Initialise(context);
		}
		catch (Exception ex)
		{
			var logger = services.GetRequiredService<ILogger<Program>>();
			logger.LogError(ex, "An error occurred creating the DB.");
		}
	}

}
