// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.ComponentModel.DataAnnotations;

namespace Model;

public class JobStateChange
{
	[Key]
	public int JobStateChangeId { get; set; }

	public int JobId { get; set; }

	public Job? Job { get; set; }

	public JobState State { get; set; }

	public Worker? Worker { get; set; }

	public DateTimeOffset StateChangedAt { get; set; }
}

