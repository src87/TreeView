using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeView.TreeView;

namespace TreeViewTests.Filter
{
	[TestClass]
	public class TreeViewFilterTests
	{
		private IEnumerable<ITreeViewFilterable> _filterItems;

		[TestInitialize]
		public void Setup()
		{
			_filterItems = CreateFilterableItems();
		}

		private static IEnumerable<ITreeViewFilterable> CreateFilterableItems()
		{
			yield return new FakeFilterableItem { Id = 1, FilterableFields = new List<string> { "SERIAL111", "John's iPad" }, Rank = 1 };
			yield return new FakeFilterableItem { Id = 2, FilterableFields = new List<string> { "JOEASDAS", "Joe's iPad" }, Rank = 2 };
			yield return new FakeFilterableItem { Id = 3, FilterableFields = new List<string> { "SERIALXXJX", "Fred's Device" }, Rank = 3 };
		}

		[TestMethod]
		public void TreeViewFilter_EmptyTerm_ReturnsAllItems()
		{
			var filter = new TreeViewFilter();
			var expected = _filterItems;
			var term = string.Empty;
			
			var result = filter.Filter(_filterItems, term);
			
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void TreeViewFilter_NullTerm_ReturnsAllItems()
		{
			var filter = new TreeViewFilter();
			var expected = _filterItems;
			string term = null;

			var result = filter.Filter(_filterItems, term);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void TreeViewFilter_TermMatchesSingleItemSingleField_ReturnsSingleItem()
		{
			var filter = new TreeViewFilter();
			var expected = new List<ITreeViewFilterable>
			{
				new FakeFilterableItem { Id = 1, FilterableFields = new List<string> { "SERIAL111", "John's iPad" }, Rank = 1 }
			};
			string term = "John";

			var result = filter.Filter(_filterItems, term).ToList();

			CollectionAssert.AreEqual(expected, result, new FakeFilterableItemComparer());
		}

		[TestMethod]
		public void TreeViewFilter_TermMatchesSingleItemMultipleFields_ReturnsSingleItem()
		{
			var filter = new TreeViewFilter();
			var expected = new List<ITreeViewFilterable>
			{
				new FakeFilterableItem { Id = 2, FilterableFields = new List<string> { "JOEASDAS", "Joe's iPad" }, Rank = 2 }
			};
			string term = "joe";

			var result = filter.Filter(_filterItems, term).ToList();

			CollectionAssert.AreEqual(expected, result, new FakeFilterableItemComparer());
		}

		[TestMethod]
		public void TreeViewFilter_TermMatchesMultipleItemsSingleField_ReturnsMultipleItems()
		{
			var filter = new TreeViewFilter();
			var expected = new List<ITreeViewFilterable>
			{
				new FakeFilterableItem { Id = 1, FilterableFields = new List<string> { "SERIAL111", "John's iPad" }, Rank = 1 },
				new FakeFilterableItem { Id = 2, FilterableFields = new List<string> { "JOEASDAS", "Joe's iPad" }, Rank = 2 }
			};
			string term = "ipad";

			var result = filter.Filter(_filterItems, term).ToList();

			CollectionAssert.AreEqual(expected, result, new FakeFilterableItemComparer());
		}
		
		[TestMethod]
		public void TreeViewFilter_TermMatchesMultipleItemsMultipleFields_ReturnsMultipleItems()
		{
			var filter = new TreeViewFilter();
			var expected = new List<ITreeViewFilterable>
			{
				new FakeFilterableItem { Id = 1, FilterableFields = new List<string> { "SERIAL111", "John's iPad" }, Rank = 1 },
				new FakeFilterableItem { Id = 2, FilterableFields = new List<string> { "JOEASDAS", "Joe's iPad" }, Rank = 2 },
				new FakeFilterableItem { Id = 3, FilterableFields = new List<string> { "SERIALXXJX", "Fred's Device" }, Rank = 3 }
			};
			string term = "j";

			var result = filter.Filter(_filterItems, term).ToList();

			CollectionAssert.AreEqual(expected, result, new FakeFilterableItemComparer());
		}
	}
}