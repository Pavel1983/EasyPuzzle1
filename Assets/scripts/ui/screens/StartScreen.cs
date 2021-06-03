using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame.UI
{
    public class StartScreen : BaseScreen
    {
        [SerializeField] private Button _playBtn;

        private void OnEnable()
        {
            _playBtn.onClick.AddListener(OnPlayButton);
        }

        private void OnDisable()
        {
            _playBtn.onClick.RemoveListener(OnPlayButton);
        }

        private void OnPlayButton()
        {
            UI.Instance.Open(GameConstants.GalleryScreen);
        }
    }
}
