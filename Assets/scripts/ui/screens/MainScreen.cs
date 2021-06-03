using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace PuzzleGame.UI
{
	public class MainScreen : BaseScreen
	{
		[SerializeField] private Button _backButton;
		[SerializeField] private float _puzzleHoldTimeBeforeDrag = 0.3f;
		[SerializeField] private PuzzlesData _puzzlesData;
		[SerializeField] private PuzzlesTab _puzzlesTab;
		[SerializeField] private GameData _gameData;

		// locals
		private float _puzzleHoldTime = 0;
		private bool _puzzlePressed;
		private string _currentPuzzleId;
		private Level _level;
		private Camera _mainCamera;
		private int _curLevel = 0;

		#region life cycle
		private void OnEnable()
		{
			_backButton.onClick.AddListener(OnBackButton);

			if (_level == null)
			{
				TryLoadLevelFromPrefs();
				LoadLevel();
			}
		}

		private void OnDisable()
		{
			_backButton.onClick.RemoveListener(OnBackButton);
			if (_level != null)
			{
				Destroy(_level.gameObject);
			}
		}

		private void Start()
		{
			//TryLoadLevelFromPrefs();
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

		#region event handlers
		
		private void OnBackButton()
		{
			UI.Instance.Back();
		}
		
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

		private void LoadLevel()
		{
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

		private void TryLoadLevelFromPrefs()
		{
			_curLevel = PlayerPrefs.GetInt(GameConstants.PrefsLevel, 0);

			Assert.IsTrue(_gameData.LevelData.Count > 0);

			Level levelPrefab = null;
			if (_gameData.LevelData.Count < _curLevel)
			{
				levelPrefab = _gameData.LevelData[_curLevel].LevelPrefab;
			}
			else
			{
				PlayerPrefs.SetInt(GameConstants.PrefsLevel, 0);
				_curLevel = 0;
				
				levelPrefab = _gameData.LevelData[_curLevel].LevelPrefab;
			}

			_level = Instantiate(levelPrefab);
		}

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
