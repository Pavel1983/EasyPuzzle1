using UnityEngine;

namespace PuzzleGame.UI
{
    public class GalleryScreen : BaseScreen
    {
        [SerializeField] private GameData _gameData;
        [SerializeField] private Transform _content;
        [SerializeField] private GameObject _previewPrefab;

        #region life cycle
        private void Awake()
        {
            Init();
        }
        #endregion

        private void Init()
        {
            for (int i = 0; i < _gameData.LevelData.Count; ++i)
            {
                var levelData = _gameData.LevelData[i];
                if (levelData != null)
                {
                    var previewObject = Instantiate(_previewPrefab, _content, false);
                    var levelPreview = previewObject.GetComponent<PreviewElement>();
                    if (levelPreview == null)
                    {
                        Debug.LogError($"Can't find PreviewElement component");
                        return;
                    }

                    levelPreview.Setup(levelData.LevelPreview, i != 0, i);
                    if (i == 0)
                    {
                        var interactionDetector = previewObject.GetComponent<InteractionDetector>();
                        if (interactionDetector != null)
                        {
                            interactionDetector.EventPointerClick += OnLevelClick;
                        }
                    }
                }
            }
        }

        private void OnLevelClick(InteractionDetector levelObject)
        {
            var levelPreview = levelObject.GetComponent<PreviewElement>();
            PlayerPrefs.SetInt(GameConstants.PrefsLevel, levelPreview.LevelNum);
            UI.Instance.Open(GameConstants.MainScreen);
        }
    }
}
