using UnityEngine;
using UnityEngine.Assertions;

namespace PuzzleGame.UI
{
	public class MainScreen : MonoBehaviour, IScreen
	{
		[SerializeField] private float _puzzleHoldTimeBeforeDrag = 0.3f;
		[SerializeField] private PuzzlesData _puzzlesData;
		[SerializeField] private PuzzlesTab _puzzlesTab;

		private float _puzzleHoldTime = 0;
		private bool _puzzlePressed;
		private string _currentPuzzleId;
		private Level _level;
		private Camera _mainCamera;

		#region life cycle
		private void Start()
		{
			_level = FindObjectOfType<Level>();
			Assert.IsTrue(_level != null);

			if (_level.Load())
			{
				_level.EventReturnPuzzleBack += OnPuzzleBack;
				
				var puzzleCollection = _level.GetPuzzleCollection();
				_puzzlesTab.FillTab(puzzleCollection);

				var puzzleObjects = _puzzlesTab.GetAllPuzzleObjectFromTab();
				for (int i = 0; i < puzzleObjects.Count; ++i)
				{
					var interactionDetector = puzzleObjects[i].GetComponent<InteractionDetector>();
					interactionDetector.EventPointerDown += OnPuzzleHold;
					interactionDetector.EventPointerUp += OnPuzzleRelease;
				}
				
				_mainCamera = Camera.main;
			}		
		}

		private void Update()
		{
			if (_puzzlePressed)
			{
				_puzzleHoldTime += Time.deltaTime;

				if (_puzzleHoldTime > _puzzleHoldTimeBeforeDrag)
				{
					SelectPuzzle(_currentPuzzleId);
					
					_puzzlePressed = false;
					_puzzleHoldTime = 0.0f;
				}
			}
		}
		#endregion
		
		#region IScreen impl
		public string Id { get; }

		public void Show()
		{
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
		#endregion

		#region event handlers
		
		private void OnPuzzleRelease(InteractionDetector detector)
		{
			_puzzlePressed = false;
			_puzzleHoldTime = 0.0f;
		}

		private void OnPuzzleHold(InteractionDetector detector)
		{
			_puzzlePressed = true;
			_currentPuzzleId = detector.GetComponent<PuzzleElementDataHolder>().Id;
		}
		
		private void OnPuzzleBack(string puzzleId)
		{
			_puzzlesTab.AddElement(puzzleId);
			var puzzleObject = _puzzlesTab.GetSinglePuzzleObjectFromTab(puzzleId);
			if (puzzleObject != null)
			{
				var interactionDetector = puzzleObject.GetComponent<InteractionDetector>();
				interactionDetector.EventPointerDown += OnPuzzleHold;
				interactionDetector.EventPointerUp += OnPuzzleRelease;
			}
			else 
				Debug.LogError($"Can't find puzzle object in a puzzles tab with id {puzzleId}");
		}
		
		#endregion
		
		private void SelectPuzzle(string puzzleId)
		{
			_puzzlesTab.RemoveElement(puzzleId);

			var puzzleData = _puzzlesData.GetPuzzleData(puzzleId);
			if (puzzleData != null)
			{
				// Создаём объект поля (перенести в фабрику)
				var puzzleElement = Instantiate(puzzleData.Prefab);
				Vector3 newPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
				newPos.z = 0;
				puzzleElement.transform.position = newPos;
				//InteractionDetector puzzleReleaseDetector = puzzleElement.AddComponent<InteractionDetector>();
				_level.TrackPuzzle(puzzleElement);
			}
		}
	}
}
