using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame.UI
{
    public class PreviewElement : MonoBehaviour
    {
        [SerializeField] private Image _lockImg;
        [SerializeField] private Image _mainImg;

        private int _levelNum;

        public int LevelNum => _levelNum;

        public void Setup(Sprite mainIcon, bool lockedFlag, int levelNum)
        {
            _mainImg.sprite = mainIcon;
            _lockImg.gameObject.SetActive(lockedFlag);
            _levelNum = levelNum;

        }
    }
}
