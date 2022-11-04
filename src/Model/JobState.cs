// Copyright (c) 2022 Richard Fuller
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Model;

public enum JobState
{
	None,
	Queued,
	PendingRoute,
	Dispatched,
	WorkStarted,
	WorkComplete,
	//JobComplete
}

