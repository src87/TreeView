namespace TreeView.TreeView
{
	internal interface ITreeViewSearchable
	{
		string SearchableField { get; }
		int Rank { get; set; }
		bool IsSelected { get; set; }
	}
}