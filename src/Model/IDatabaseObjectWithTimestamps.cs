// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Model;

public interface IDatabaseObjectWithTimestamps
{
	DateTimeOffset CreatedAt { get; set; }
	DateTimeOffset UpdatedAt { get; set; }
}

