using System;
using System.Collections.Generic;
using System.Linq;

namespace TreeView.TreeView
{
	internal class TreeViewFilter : ITreeViewFilter
	{
		public IEnumerable<ITreeViewFilterable> Filter(IEnumerable<ITreeViewFilterable> treeViewItems, string filterTerm)
		{
			if (string.IsNullOrEmpty(filterTerm))
				return treeViewItems;

			var filteredItems = treeViewItems.Where(item => TermFoundInFields(item.FilterableFields, filterTerm));

			return filteredItems;
		}

		private static bool TermFoundInFields(IEnumerable<string> fields, string searchTerm)
		{
			var found = fields.Any(field => field.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
			return found;
		}
	}
}