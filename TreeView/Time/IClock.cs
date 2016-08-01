using System;

namespace TreeView.Time
{
	internal interface IClock
	{
		DateTime Now { get; }
	}
}