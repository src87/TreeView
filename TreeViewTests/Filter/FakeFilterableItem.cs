using System.Collections.Generic;
using TreeView.TreeView;

namespace TreeViewTests.Filter
{
	public class FakeFilterableItem : ITreeViewFilterable
	{
		public IEnumerable<string> FilterableFields { get; set; }
		public int Rank { get; set; }
		public int Id { get; set; }
	}
}