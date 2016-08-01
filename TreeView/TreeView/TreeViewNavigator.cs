using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using TreeView.Time;

namespace TreeView.TreeView
{
	internal class TreeViewNavigator : ITreeViewNavigator
	{
		private readonly IClock _clock;

		//TODO change to unique identification of some kind
		private readonly List<string> _matchedItems;
		private DateTime? _lastKeyReceived;
		private string _currentSequence;
		private bool _repeatCharacter;
		public double KeyPressTimeSpan = 1000;

		public TreeViewNavigator(IClock clock)
		{
			_clock = clock;
			_matchedItems = new List<string>();
		}

		public ITreeViewSearchable Find(Key key, IEnumerable<ITreeViewSearchable> treeViewItems)
		{
			_repeatCharacter = false;
			var keyString = new KeyConverter().ConvertToString(key);
			if (keyString == null || !keyString.All(char.IsLetterOrDigit))
				return null;

			keyString = TranslateCharacter(keyString);

			if (IsNewSequence(key))
			{
				if (key == Key.Escape)
					return null;
			}

			_lastKeyReceived = _clock.Now;

			CheckIfRepeatedCharacter(keyString);

			var results = GetAndProcessResults(treeViewItems);

			var foundItem = results.FirstOrDefault();
			if (foundItem != null)
			{
				_matchedItems.Add(foundItem.SearchableField);
			}

			return foundItem;
		}

		private static string TranslateCharacter(string keyString)
		{
			if (keyString == "Space")
				keyString = " ";

			keyString = ExtractNumberCharacterIfDigit(keyString);

			return keyString;
		}

		private static string ExtractNumberCharacterIfDigit(string keyString)
		{
			if (keyString.Any(char.IsDigit))
				keyString = keyString.Reverse().First().ToString(CultureInfo.InvariantCulture);

			return keyString;
		}

		private IEnumerable<ITreeViewSearchable> GetAndProcessResults(IEnumerable<ITreeViewSearchable> treeViewItems)
		{
			var deviceViewModels = treeViewItems.ToList();
			var results = GetResults(deviceViewModels, _currentSequence);

			if (!results.Any())
			{
				_matchedItems.Clear();
				results = GetResults(deviceViewModels, _currentSequence);
			}

			return results;
		}

		//private void Test(string keyString)
		//{
		//	char[] keyArray = keyString.ToCharArray();

		//	keyArray = Array.FindAll(
		//		keyArray,
		//		(c =>
		//			(char.IsLetterOrDigit(c)
		//			|| char.IsWhiteSpace(c)
		//			|| c == '-')));

		//	keyString = new string(keyArray);
		//}

		private void CheckIfRepeatedCharacter(string keyString)
		{
			if (_currentSequence != keyString)
				_currentSequence += keyString;
			else
				_repeatCharacter = true;
		}

		private List<ITreeViewSearchable> GetResults(IEnumerable<ITreeViewSearchable> treeViewItems, string keyString)
		{
			var itemList = treeViewItems.ToList();
			var results = itemList.Where(t => t.SearchableField.ToUpper().StartsWith(keyString)).ToList();
			
			if (_repeatCharacter)
				results.RemoveAll(r => _matchedItems.Contains(r.SearchableField));
			
			var selectedRank = FindSelectedRank(itemList);
			var orderedResults = OrderResults(results, selectedRank);

			return orderedResults;
		}

		private List<ITreeViewSearchable> OrderResults(List<ITreeViewSearchable> results, int selectedRank)
		{
			var orderedResults = _currentSequence.Length > 1
				? 
				OrderResultsForContinuingSequence(results, selectedRank)
				:
				OrderResultsForNewSequence(results, selectedRank);

			return orderedResults;
		}

		private static List<ITreeViewSearchable> OrderResultsForContinuingSequence(List<ITreeViewSearchable> results, int selectedRank)
		{
			var orderedResults = results.Where(r => r.Rank >= selectedRank).ToList();
			orderedResults.AddRange(results.Where(r => r.Rank < selectedRank));
			return orderedResults;
		}

		private static List<ITreeViewSearchable> OrderResultsForNewSequence(List<ITreeViewSearchable> results, int selectedRank)
		{
			var orderedResults = results.Where(r => r.Rank > selectedRank).ToList();
			orderedResults.AddRange(results.Where(r => r.Rank <= selectedRank));
			return orderedResults;
		}

		private static int FindSelectedRank(IEnumerable<ITreeViewSearchable> itemList)
		{
			var selectedRank = 0;
			var selectedItem = itemList.FirstOrDefault(i => i.IsSelected);

			if (selectedItem != null)
			{
				selectedRank = selectedItem.Rank;
			}

			return selectedRank;
		}

		private bool IsNewSequence(Key key)
		{
			if (key == Key.Escape || HasTimeExpired())
			{
				BeginNewSequence();
				return true;
			}

			return false;
		}

		private bool HasTimeExpired()
		{
			if (_lastKeyReceived == null)
				return true;

			var result = false;
			TimeSpan timePassed = _clock.Now - _lastKeyReceived.Value;

			if (timePassed >= TimeSpan.FromMilliseconds(KeyPressTimeSpan))
				result = true;

			return result;
		}

		private void BeginNewSequence()
		{
			_lastKeyReceived = _clock.Now;
			_currentSequence = string.Empty;
			_repeatCharacter = false;
			_matchedItems.Clear();
		}
	}
}