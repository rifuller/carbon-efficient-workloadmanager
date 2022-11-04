// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Model;

public static class DbInitialiser
{
	public static void Initialise(DispatcherContext context)
	{
		context.Database.EnsureCreated();

		// Check if database seeded already
		if (context.Workers.Any())
		{
			return;
		}

		var workers = new Worker[]
		{
			new Worker("australiasoutheast-01", Region.AustraliaSouthEast),
			new Worker("uswest-01", Region.WestUS),
		};

		foreach (var worker in workers)
		{
			context.Workers.Add(worker);
		}

		context.SaveChanges();
	}
}
