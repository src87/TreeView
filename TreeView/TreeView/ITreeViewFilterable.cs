using System.Collections.Generic;

namespace TreeView.TreeView
{
	public interface ITreeViewFilterable
	{
		IEnumerable<string> FilterableFields { get; }
		int Rank { get; set; }
		int Id { get; }
	}
}