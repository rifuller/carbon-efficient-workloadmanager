// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.ComponentModel.DataAnnotations;

namespace Model;

public class Worker
{
	public Worker(string name, Region region)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
		}

		Name = name;
		Region = region;
	}

	[Key]
	public string Name { get; set; }

	public Region Region { get; set; }
	// instructions to queue
}

