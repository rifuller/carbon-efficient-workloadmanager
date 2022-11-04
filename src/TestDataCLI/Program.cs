// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using AutoFixture;
using CarbonAwareWebApiClient;
using CsvHelper;
using CsvHelper.Configuration;
using JobRouter;
using JobRouter.CarbonOptimisedRouter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Model;
using System.CommandLine;
using System.Globalization;

namespace TestDataCLI;

internal class Program
{
	private static readonly CsvConfiguration CsvConfiguration = new(CultureInfo.InvariantCulture)
	{
		NewLine = "\n",
		PrepareHeaderForMatch = args => args.Header.ToLower(),

	};

	private static async Task<int> Main(string[] args)
	{
		var ct = new CancellationToken();

		// Setup service provider for DI
		var services = new ServiceCollection();
		IConfigurationRoot config = new ConfigurationBuilder()
			.AddJsonFile($"appsettings.Development.json")
			.Build();

		services.AddLogging(logBuilder =>
		{
			logBuilder.AddConsole();
		});
		services.AddSingleton<IJobRouter, CarbonOptimisedRouterCore>();
		services.AddSingleton<ICarbonAwareWebApiClient, CarbonAwareWebApiClient.CarbonAwareWebApiClient>();
		services.Configure<CarbonAwareWebApiClientOptions>(config.GetSection(nameof(CarbonAwareWebApiClient)));


		services.AddSingleton<IConfiguration>(config);

		var serviceProvider = services.BuildServiceProvider();

		// Setup command line parser
		var rootCommand = new RootCommand("Sample app for System.CommandLine");

		var countOption = new Option<long>(name: "-count", description: "The number of jobs to generate.");
		var outOption = new Option<string>(name: "-out", description: "The output file to write to.");
		var createCommand = new Command("create", "Create test data.")
		{
			countOption,
			outOption
		};
		createCommand.SetHandler((count, outFile) => Create(count, outFile, ct), countOption, outOption);

		var inOption = new Option<string>(name: "-in", description: "The input file to read from.");
		var evaluateCommand = new Command("evaluate", "Evaluate the efficacy of the carbon-optimised router on a set of synthetic data.")
		{
			inOption,
			outOption
		};
		evaluateCommand.SetHandler((inFile, outFile) => Evaluate(inFile, outFile, serviceProvider, ct), inOption, outOption);

		rootCommand.AddCommand(createCommand);
		rootCommand.AddCommand(evaluateCommand);

		return await rootCommand.InvokeAsync(args);
	}

	private static async Task Evaluate(string inFile, string outFile, ServiceProvider serviceProvider, CancellationToken ct)
	{
		using var reader = new StreamReader(inFile);
		using var csvReader = new CsvReader(reader, CsvConfiguration);

		using var writer = new StreamWriter(outFile);
		using var csvWriter = new CsvWriter(writer, CsvConfiguration);

		var local = serviceProvider.GetRequiredService<IJobRouter>();
		var router = serviceProvider.GetRequiredService<IJobRouter>();

		var jobs = csvReader.GetRecords<Job>().ToArray();
		var tasks = new List<Task>();

		// Take 10 jobs at a time
		for (int i = 0; i < jobs.Length; i += 10)
		{
			var jobsToRoute = jobs.Skip(i).Take(10).ToArray();
			foreach (var job in jobsToRoute)
			{
				tasks.Add(router.GetTargetAsync(job, ct).ContinueWith(task =>
				{
					Console.WriteLine($"{job.JobId} updated.");
					job.UpdateFromResult(task.Result);
				}, ct));
			}

			Task.WaitAll(tasks.ToArray(), ct);
		}

		await csvWriter.WriteRecordsAsync(jobs, ct);
	}

	private static async IAsyncEnumerable<Job> GetEvaluatedJobs(IAsyncEnumerable<Job> jobs, ServiceProvider serviceProvider, CancellationToken ct)
	{
		var local = serviceProvider.GetRequiredService<IJobRouter>();
		var router = serviceProvider.GetRequiredService<IJobRouter>();
		await foreach (var job in jobs)
		{
			var result = await router.GetTargetAsync(job, ct);

			job.UpdateFromResult(result);

			yield return job;
		}
	}

	private static async Task Create(long count, string outFile, CancellationToken ct)
	{
		// check if path exists
		if (System.IO.File.Exists(outFile))
		{
			Console.WriteLine($"Overwriting existing file at {outFile}");
		}

		using var writer = new StreamWriter(outFile);
		using var csv = new CsvWriter(writer, CsvConfiguration);

		await csv.WriteRecordsAsync(CreateTestJobData(count), ct);
	}

	private static IEnumerable<Job> CreateTestJobData(long count)
	{
		Fixture fixture = new Fixture();
		var rand = new Random();

		for (int i = 0; i < count; i++)
		{
			var start = DateTimeOffset.Now.AddDays(-14);
			var duration = TimeSpan.FromDays(7);

			// Get random time
			var time = start.AddMilliseconds(rand.NextDouble() * duration.TotalMilliseconds);

			var job = new Job(
				fixture.Create<string>(),
				JobConstraints.None,
				fixture.Create<Region>(),
				time);

			job.JobId = i;

			yield return job;
		}
	}
}
