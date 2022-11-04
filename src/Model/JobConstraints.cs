// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Model;

[Flags]
public enum JobConstraints
{
	None = 0,
	Interactive = 0x1,
	Sovereign = 0x2,
	HighDataVolume = 0x4,
}

