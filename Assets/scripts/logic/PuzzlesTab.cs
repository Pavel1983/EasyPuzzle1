using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame
{
	public class PuzzlesTab: MonoBehaviour
	{
		[SerializeField] private Transform _content;
		[SerializeField] private PuzzlesData _puzzlesData;

		private Dictionary<string, GameObject> _elements = new Dictionary<string, GameObject>();

		public void FillTab(string[] puzzleIds)
		{
			ClearTab();
			
			for (int i = 0; i < puzzleIds.Length; ++i)
			{
				string id = puzzleIds[i];
				if (!string.IsNullOrEmpty(id))
				{
					// SomeValidator.ValidateId(id);
					PuzzleElementData puzzle = _puzzlesData.GetPuzzleData(puzzleIds[i]);
					var element = Instantiate(puzzle.UiPrefab, _content, false);
					_elements.Add(puzzleIds[i], element);
				}
			}
			
			Refresh();
		}

		public void AddElement(string puzzleId)
		{
			if (!_elements.ContainsKey(puzzleId))
			{
				PuzzleElementData puzzle = _puzzlesData.GetPuzzleData(puzzleId);
				var element = Instantiate(puzzle.UiPrefab, _content, false);
				_elements.Add(puzzleId, element);
				Refresh();
			}
		}

		public void RemoveElement(string puzzleId)
		{
			if (_elements.TryGetValue(puzzleId, out var puzzleElement))
			{
				Destroy(puzzleElement);
				_elements.Remove(puzzleId);
				Refresh();
			}
		}

		public List<GameObject> GetAllPuzzleObjectFromTab()
		{
			return _elements.Select(item => item.Value).ToList();
		}

		// todo: подумать над тем чтобы возвращать интерфейс вместо конкретного объекта
		public GameObject GetSinglePuzzleObjectFromTab(string puzzleId)
		{
			if (_elements.TryGetValue(puzzleId, out var puzzleObject))
				return puzzleObject;

			return null;
		}

		private void ClearTab()
		{
			foreach (var element in _elements)
			{
				Destroy(element.Value.gameObject);
			}
			
			_elements.Clear();
		}

		private void Refresh()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(_content.GetComponent<RectTransform>());
		}
	}
}