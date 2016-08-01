using System;

namespace TreeView.Time
{
	internal class Clock : IClock
	{
		public DateTime Now
		{
			get { return DateTime.UtcNow; }
		}
	}
}