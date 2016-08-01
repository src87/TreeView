using TreeView.TreeView;

namespace TreeViewTests.Navigation
{
	public class FakeSearchableItem : ITreeViewSearchable
	{
        public int Id { get; set; }
		public string SearchableField { get; set; }
		public int Rank { get; set; }
		public bool IsSelected { get; set; }
	}
}