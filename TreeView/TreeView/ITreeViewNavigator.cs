using System.Collections.Generic;
using System.Windows.Input;

namespace TreeView.TreeView
{
	internal interface ITreeViewNavigator
	{
		ITreeViewSearchable Find(Key key, IEnumerable<ITreeViewSearchable> treeViewItems);
	}
}