using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PuzzleGame.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UI : MonoBehaviour
    {
        [SerializeField] private GameObject[] _screenPrefabs;
        
        private static UI _instance = null;
        private Stack<IScreen> _screens = new Stack<IScreen>();
        private Canvas _canvas;
        private Dictionary<string, GameObject> _screenId2PrefabDict = new Dictionary<string, GameObject>();
        private List<IScreen> _cachedScreens = new List<IScreen>();

        public static UI Instance => _instance;

        #region life cycle
        private void Awake()
        {
            DontDestroyOnLoad(this);
            _instance = this;
            Init();
            
            Open(GameConstants.StartScreen);
        }

        private void Init()
        {
            _canvas = GetComponent<Canvas>();
            for (int i = 0; i < _screenPrefabs.Length; ++i)
            {
                var prefab = _screenPrefabs[i];
                if (prefab != null)
                {
                    IScreen screen = prefab.GetComponent<IScreen>();
                    _screenId2PrefabDict.Add(screen.Id, prefab);
                }
            }
        }

        #endregion

        public void Open(string screenId, bool hidePrevious = true)
        {
            // 1. Проверяем есть ли в стеке такой экран, если да возвращаемся туда
            if (_screens.Count > 0 && _screens.Any(scr => scr.Id == screenId))
            {
                while (_screens.Count > 0)
                {
                    var curScreen = _screens.Peek();

                    if (curScreen.Id == screenId)
                    {
                        break;
                    }
                    
                    if (hidePrevious)
                    {
                        curScreen.Hide();
                        _cachedScreens.Add(curScreen);
                    }

                    _screens.Pop();
                }
            }
            else
            // 2. Если нет в стеке ищем в кеше
            if (_cachedScreens.Any(scr => scr.Id == screenId))
            {
                if (hidePrevious)
                    _screens.Peek().Hide();
                
                var screen = _cachedScreens.First(scr => scr.Id == screenId);
                _cachedScreens.Remove(screen);
                _screens.Push(screen);

                screen.Show();
            }
            else
            // 3. Если нет в стеке, то инстанциируем префаб и добавляем в стек
            if (_screenId2PrefabDict.TryGetValue(screenId, out var screenPrefab))
            {
                if (hidePrevious && _screens.Count > 0)
                    _screens.Peek().Hide();
                
                var newScreenObject = Instantiate(screenPrefab, _canvas.transform, false);
                IScreen newScreen = newScreenObject.GetComponent<IScreen>();
                if (newScreen != null)
                {
                    newScreen.Show();
                    _screens.Push(newScreen);
                }
            }
            else
            {
                Debug.LogError($"Can't instantiate screen prefab with id:{screenId} because it cannot be found!");
            }
        }

        public void Back()
        {
            if (_screens.Count > 1)
            {
                var curScreen = _screens.Peek();
                curScreen.Hide();
                
                _screens.Pop();
                _screens.Peek().Show();
                _cachedScreens.Add(curScreen);
            }
        }
    }
}