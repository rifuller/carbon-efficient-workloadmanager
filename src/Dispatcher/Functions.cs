// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace DispatcherFunctions;

public class Functions
{
	private readonly ILogger<Functions> _log;
	private readonly IJobRouter _router;

	public Functions(IJobRouter router, ILogger<Functions> log)
	{
		_log = log ?? throw new ArgumentNullException(nameof(log));
		_router = router ?? throw new ArgumentNullException(nameof(router));
	}

	/// <summary>
	/// Evaluates the metadata of the job and routers the request to the appropriate worker queue.
	/// </summary>
	[Function("RouteJob")]
	public async Task RouteJobImplAsync(
		[ServiceBusTrigger("dispatch-queue", Connection = "ServiceBusConnectionString")] int jobId)
	{
		using var db = new DispatcherContext();
		var job = await db.Jobs.FindAsync(jobId);

		if (job != null)
		{
			_log.LogInformation("Found job {Id},\"{Name}\",{Constraints},{HomeRegion}", job.JobId, job.Name, job.Constraints, job.HomeRegion);

			var result = await _router.GetTargetAsync(job, CancellationToken.None);

			_log.LogInformation("Home region={Home}; Destination region={Dest}", job.HomeRegion, result.TargetRegion.ToString());
		}
		else
		{
			_log.LogWarning("Job {Id} not found", jobId);
		}
	}

	/// <summary>
	/// Produces one job and sends it to the router queue.
	/// </summary>
	[Function("ProduceOneJob")]
	[ServiceBusOutput("dispatch-queue", Connection = "ServiceBusConnectionString")]
	public async Task<int> ProduceOneJobImplAsync(
		[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req,
		FunctionContext executionContext)
	{
		using var db = new DispatcherContext();
		string name = "Test job " + Guid.NewGuid();
		Job job = new(name, JobConstraints.None, Region.AustraliaSouthEast, DateTimeOffset.Now);
		db.Jobs.Add(job);
		await db.SaveChangesAsync();

		_log.LogInformation("Creating job \"{Name}\"", name);

		return job.JobId;
	}

	[Function("hello")]
	public static string HelloCore(
		[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req,
		FunctionContext executionContext)
	{
		return "Hi! " + DateTime.UtcNow.ToString("o");
	}
}
