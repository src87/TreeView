namespace TreeView.TreeView
{
	internal interface ITreeViewSearchable
	{
        int Id { get; set; }
		string SearchableField { get; }
		int Rank { get; set; }
		bool IsSelected { get; set; }
	}
}