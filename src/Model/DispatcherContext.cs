// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Microsoft.EntityFrameworkCore;

namespace Model;

public class DispatcherContext : DbContext
{
	public DbSet<Job> Jobs => this.Set<Job>();

	public DbSet<JobStateChange> JobStateChangeHistory => this.Set<JobStateChange>();

	public DbSet<Worker> Workers => this.Set<Worker>();

	public string DbPath { get; }

	public DispatcherContext()
	{
		DbPath = "jobrouting.db";
	}

	// The following configures EF to create a Sqlite database file in the
	// special "local" folder for your platform.
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite($"Data Source={DbPath}");
	}

	public override int SaveChanges()
	{
		foreach (var entry in ChangeTracker.Entries().Where(x => x.Entity is IDatabaseObjectWithTimestamps))
		{
			if (entry.Entity is IDatabaseObjectWithTimestamps entity)
			{
				switch (entry.State)
				{
					case EntityState.Added:
						entity.CreatedAt = DateTimeOffset.Now;
						entity.UpdatedAt = DateTimeOffset.Now;
						break;
					case EntityState.Modified:
						entity.UpdatedAt = DateTimeOffset.Now;
						break;
				}
			}
		}
		return base.SaveChanges();
	}
}
