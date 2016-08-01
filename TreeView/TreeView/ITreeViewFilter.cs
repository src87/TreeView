using System.Collections.Generic;

namespace TreeView.TreeView
{
	public interface ITreeViewFilter
	{
		IEnumerable<ITreeViewFilterable> Filter(IEnumerable<ITreeViewFilterable> treeViewItems, string filterTerm);
	}
}