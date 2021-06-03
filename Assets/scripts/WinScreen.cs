using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame.UI
{
    public class WinScreen : BaseScreen
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _tryAgainButton;

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnGalleryButton);
            _tryAgainButton.onClick.AddListener(OnTryAgainButton);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnGalleryButton);
            _tryAgainButton.onClick.RemoveListener(OnTryAgainButton);
        }

        #region event handlers
        private void OnTryAgainButton()
        {
            ClearCurrentPrefsLevelData();
            UI.Instance.Open(GameConstants.MainScreen);
        }

        private void OnGalleryButton()
        {
            ClearCurrentPrefsLevelData();
            UI.Instance.Open(GameConstants.GalleryScreen);
        }

        private void ClearCurrentPrefsLevelData()
        {
            int currentLevel = PlayerPrefs.GetInt(GameConstants.PrefsLevel, 0);
            PlayerPrefs.DeleteKey($"level{currentLevel}");
        }
        
        #endregion
    }
}
