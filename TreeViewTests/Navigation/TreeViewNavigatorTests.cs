using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TreeView.Time;
using TreeView.TreeView;

namespace TreeViewTests.Navigation
{
	[TestClass]
	public class TreeViewNavigatorTests
	{
		private IEnumerable<ITreeViewSearchable> _searchItems;
		private static readonly DateTime Date = new DateTime(2000, 1, 1, 1, 1, 1);
		private Mock<IClock> _mockClock;
		private TreeViewNavigator _navigator;

		[TestInitialize]
		public void Setup()
		{
			_searchItems = new List<ITreeViewSearchable>();
			_searchItems = CreateSearchableItems();
			_mockClock = new Mock<IClock>();
			_mockClock.Setup(c => c.Now).Returns(Date);
			_navigator = new TreeViewNavigator(_mockClock.Object);
		}

		private static IEnumerable<ITreeViewSearchable> CreateSearchableItems()
		{
			yield return new FakeSearchableItem { Id = 1, SearchableField = "iPad 0001", Rank = 1, IsSelected = true };
			yield return new FakeSearchableItem { Id = 2, SearchableField = "iPad 0002", Rank = 2 };
			yield return new FakeSearchableItem { Id = 3, SearchableField = "iPad 0003", Rank = 3 };
			yield return new FakeSearchableItem { Id = 4, SearchableField = "Jack's iPad", Rank = 4 };
			yield return new FakeSearchableItem { Id = 5, SearchableField = "Jason's iPad", Rank = 5 };
			yield return new FakeSearchableItem { Id = 6, SearchableField = "Jeff's iPad", Rank = 6 };
			yield return new FakeSearchableItem { Id = 7, SearchableField = "Jim's iPad", Rank = 7 };
			yield return new FakeSearchableItem { Id = 8, SearchableField = "JJ's iPad", Rank = 8 };
			yield return new FakeSearchableItem { Id = 9, SearchableField = "John's iPad", Rank = 9 };
			yield return new FakeSearchableItem { Id = 10, SearchableField = "Judith's iPad", Rank = 10 };
		}

		private static IEnumerable<ITreeViewSearchable> CreateMidwaySelectedSearchableItems()
		{
			yield return new FakeSearchableItem { Id = 1, SearchableField = "iPad 0001", Rank = 1 };
			yield return new FakeSearchableItem { Id = 2, SearchableField = "iPad 0002", Rank = 2 };
			yield return new FakeSearchableItem { Id = 3, SearchableField = "iPad 0003", Rank = 3 };
			yield return new FakeSearchableItem { Id = 4, SearchableField = "Jack's iPad", Rank = 4 };
			yield return new FakeSearchableItem { Id = 5, SearchableField = "Jason's iPad", Rank = 5 };
			yield return new FakeSearchableItem { Id = 6, SearchableField = "Jeff's iPad", Rank = 6, IsSelected = true };
			yield return new FakeSearchableItem { Id = 7, SearchableField = "Jim's iPad", Rank = 7 };
			yield return new FakeSearchableItem { Id = 8, SearchableField = "JJ's iPad", Rank = 8 };
			yield return new FakeSearchableItem { Id = 9, SearchableField = "John's iPad", Rank = 9 };
			yield return new FakeSearchableItem { Id = 10, SearchableField = "Judith's iPad", Rank = 10 };
		}

		[TestMethod]
		public void TreeViewNavigator_FindSingleLetter_FindsFirstResult()
		{
			var item = Find(Key.J);
			var result = item.SearchableField;

			Assert.AreEqual("Jack's iPad", result);
		}

		[TestMethod]
		public void TreeViewNavigator_FindRepeatedLetter_FindsNextResult()
		{
			Find(Key.J);
			var item = Find(Key.J);
			var result = item.SearchableField;

			Assert.AreEqual("Jason's iPad", result);
		}

		[TestMethod]
		public void TreeViewNavigator_FindUnmatchedSequence_FindReturnsNull()
		{
			Find(Key.H);
			var result = Find(Key.A);

			Assert.IsNull(result);
		}

		[TestMethod]
		public void TreeViewNavigator_PartialMatch_ReturnsInitialMatchThenNull()
		{
			var item = Find(Key.J);
			var firstResult = item.SearchableField;
			var secondResult = Find(Key.V);

			Assert.AreEqual("Jack's iPad", firstResult);
			Assert.IsNull(secondResult);
		}

		[TestMethod]
		public void TreeViewNavigator_FindFirstTwoLetters_FindsFirstResultTwice()
		{
			var item = Find(Key.J);
			var firstResult = item.SearchableField;
			item = Find(Key.A);
			var secondResult = item.SearchableField;

			Assert.AreEqual("Jack's iPad", firstResult);
			Assert.AreEqual("Jack's iPad", secondResult);
		}

		[TestMethod]
		public void TreeViewNavigator_NewSequenceMatch_ReturnsInitialMatchThenSecondMatch()
		{
			var item = Find(Key.I);
			var firstResult = item.SearchableField;
			_mockClock.Setup(c => c.Now).Returns(Date + TimeSpan.FromMilliseconds(_navigator.KeyPressTimeSpan));
			item = Find(Key.J);
			var secondResult = item.SearchableField;

			Assert.AreEqual("iPad 0002", firstResult);
			Assert.AreEqual("Jack's iPad", secondResult);
		}

		[TestMethod]
		public void TreeViewNavigator_TwoLettersWithEscapeToStartNewSequence_ReturnsInitialMatchThenSecondMatch()
		{
			var item = Find(Key.I);
			var firstResult = item.SearchableField;
			Find(Key.Escape);
			item = Find(Key.J);
			var secondResult = item.SearchableField;

			Assert.AreEqual("iPad 0002", firstResult);
			Assert.AreEqual("Jack's iPad", secondResult);
		}

		[TestMethod]
		public void TreeViewNavigator_FindFullNameWithSpaceAndNumbers_ReturnsCorrectMatch()
		{
			Find(Key.I);
			Find(Key.P);
			Find(Key.A);
			Find(Key.D);
			Find(Key.Space);
			Find(Key.D0);
			Find(Key.NumPad0);
			Find(Key.D0);
			var item = Find(Key.D2);
			var result = item.SearchableField;

			Assert.AreEqual("iPad 0002", result);
		}

		[TestMethod]
		public void TreeViewNavigator_FindMatchingName_ReturnsFirstMatchEachLetter()
		{
			var item = Find(Key.I);
			var firstResult = item.SearchableField; 
			item = Find(Key.P);
			var secondResult = item.SearchableField;
			item = Find(Key.A);
			var thirdResult = item.SearchableField;
			item = Find(Key.D);
			var fourthResult = item.SearchableField;
			var expected = "iPad 0002";

			Assert.AreEqual(expected, firstResult);
			Assert.AreEqual(expected, secondResult);
			Assert.AreEqual(expected, thirdResult);
			Assert.AreEqual(expected, fourthResult);
		}

		[TestMethod]
		public void TreeViewNavigator_FirstTwoLetters_FindsMatchingItem()
		{
			Find(Key.J);
			var item = Find(Key.I);
			var result = item.SearchableField;

			Assert.AreEqual("Jim's iPad", result);
		}

		[TestMethod]
		public void TreeViewNavigator_SameLetterRepeatedly_LoopsBackToStart()
		{
			Find(Key.I);
			Find(Key.I);
			Find(Key.I);
			var item = Find(Key.I);
			var result = item.SearchableField;

			Assert.AreEqual("iPad 0002", result);
		}

		[TestMethod]
		public void TreeViewNavigator_MidwaySelectedFindSingleLetter_ReturnsFirstResult()
		{
			_searchItems = CreateMidwaySelectedSearchableItems();

			var item = Find(Key.I);
			var result = item.SearchableField;

			Assert.AreEqual("iPad 0001", result);
		}

		[TestMethod]
		public void TreeViewNavigator_MidwaySelectedFindSingleLetter_ReturnsFirstResultAfterSelected()
		{
			_searchItems = CreateMidwaySelectedSearchableItems();

			var item = Find(Key.J);
			var result = item.SearchableField;

			Assert.AreEqual("Jim's iPad", result);
		}

		private ITreeViewSearchable Find(Key key)
		{
			var item = _navigator.Find(key, _searchItems);
			if (item != null)
			{
				UpdateSelectedValues(item);
			}

			return item;
		}
		
		private void UpdateSelectedValues(ITreeViewSearchable selectedItem)
		{
			var itemList = new List<ITreeViewSearchable>();
			foreach (var item in _searchItems)
			{
				item.IsSelected = item.SearchableField == selectedItem.SearchableField;
				itemList.Add(item);
			}

			_searchItems = itemList;
		}
	}
}
