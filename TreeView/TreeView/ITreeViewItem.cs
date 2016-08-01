namespace TreeView.TreeView
{
	internal interface ITreeViewItem
	{
		int Id { get; }
		bool IsExpanded { get; set; }
		bool IsSelected { get; set; }
		ITreeViewItem Parent { get; set; }
	}
}