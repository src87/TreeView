using System.Collections;
using TreeView.TreeView;

namespace TreeViewTests.Filter
{
	internal class FakeFilterableItemComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			var xItem = (ITreeViewFilterable)x;
			var yItem = (ITreeViewFilterable)y;

			if (xItem.Id < yItem.Id)
				return 1;

			if (xItem.Id > yItem.Id)
				return -1;

			return 0;
		}
	}
}