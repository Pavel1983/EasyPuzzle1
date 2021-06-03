using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace PuzzleGame
{
    public class PuzzleId2PuzzleRecDict : SerializableDictionary<string, PuzzleRec>
    {
    }

    public class Level : MonoBehaviour
    {
        public event Action<string> EventReturnPuzzleBack;
        
        [SerializeField] private float _magneticMinDistance;
        
        private string[] _puzzleIds;
        private GameObject _currentControlledPuzzle;
        private string _currentConrolledPuzzleId;
        private Camera _mainCamera;
        private PuzzleId2PuzzleRecDict _completeness = new PuzzleId2PuzzleRecDict();

        #region life cycle

        private void Awake()
        {
            _mainCamera = Camera.main;
            RestoreCompleteness();
        }

        private void Update()
        {
            if (_currentControlledPuzzle != null)
            {
                Vector3 newPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                newPos.z = 0;
                _currentControlledPuzzle.transform.position = newPos;

                if (Input.GetMouseButtonUp(0))
                {
                    OnPuzzleReleased();
                }
            }
        }

        private void OnDestroy()
        {
            SaveCompleteness();
        }

        #endregion
        public bool Load()
        {
            var puzzles = GetComponentsInChildren<PuzzleElementDataHolder>();
            _puzzleIds = puzzles.Select(item => item.Id).ToArray();

            if (_completeness.Count == 0)
            {
                for (int i = 0; i < _puzzleIds.Length; ++i)
                {
                    PuzzleRec rec = new PuzzleRec();
                    rec.Filled = false;
                    rec.SceneObject = puzzles[i];

                    _completeness.Add(_puzzleIds[i], rec);
                }
            }
            else
            {
                foreach (var puzzleId in _completeness.Keys.ToList())
                {
                    //string puzzleId = puzzleObjectRec.Key;
                    var foundPuzzleObject = puzzles.FirstOrDefault(p => p.Id == puzzleId);
                    Assert.IsTrue(foundPuzzleObject != null);
                    PuzzleRec rec = _completeness[puzzleId];
                    rec.SceneObject = foundPuzzleObject;
                    _completeness[puzzleId] = rec;
                }
            }

            // hack 
            for (int i = 0; i < puzzles.Length; ++i)
            {
                var sr = puzzles[i].GetComponent<SpriteRenderer>();
                Color color = sr.color;
                color.a = _completeness[puzzles[i].Id].Filled ? 1.0f : 0.2f;
                sr.color = color;
            }
            // end hack

            return true;
        }

        public void TrackPuzzle(GameObject puzzleObject)
        {
            _currentControlledPuzzle = puzzleObject;
            _currentConrolledPuzzleId = puzzleObject.GetComponent<PuzzleElementDataHolder>().Id;
        }

        public string[] GetPuzzleCollection()
        {
            return _completeness.Where(item => !item.Value.Filled).Select(p => p.Key).ToArray();
        }
        
        #region Events
        private void OnPuzzleReleased()
        {
            var inMagnetArea = 
                InMagnetArea(_currentConrolledPuzzleId, 
                             _currentControlledPuzzle.transform.position, 
                             _magneticMinDistance,
                             out var magnetObject);
            
            if (inMagnetArea)
            {
                Destroy(_currentControlledPuzzle.gameObject);

                FillPuzzle(_currentConrolledPuzzleId);
                if (GameOver())
                {
                    PuzzleGame.UI.UI.Instance.Open(GameConstants.WinScreen);
                }
            }
            else
            {
                // возвращаем элемент в таб с элементами
                string puzzleId = _currentControlledPuzzle.GetComponent<PuzzleElementDataHolder>().Id;
                Destroy(_currentControlledPuzzle.gameObject);
                EventReturnPuzzleBack?.Invoke(puzzleId);
            }
            
            _currentControlledPuzzle = null;
        }
        #endregion

        private void SaveCompleteness()
        {
            int curLevel = PlayerPrefs.GetInt(GameConstants.PrefsLevel, 0);
            PlayerPrefs.SetString($"level{curLevel}", JsonUtility.ToJson(_completeness));
        }

        private void RestoreCompleteness()
        {
            int curLevel = PlayerPrefs.GetInt(GameConstants.PrefsLevel, 0);
            if (PlayerPrefs.HasKey($"level{curLevel}"))
            {
                string jsonString = PlayerPrefs.GetString($"level{curLevel}");
                _completeness = JsonUtility.FromJson<PuzzleId2PuzzleRecDict>(jsonString);
            }
        }

        private bool InMagnetArea(string puzzleId, Vector3 pos, float magneticRange, out GameObject magnetObject)
        {
            if (_completeness.TryGetValue(puzzleId, out var puzzleRec))
            {
                if ((puzzleRec.SceneObject.transform.position - pos).magnitude < magneticRange)
                {
                    magnetObject = puzzleRec.SceneObject.gameObject;
                    return true;
                }

                magnetObject = null;
                return false;
            }
            
            magnetObject = null;
            Debug.LogError($"InMagnetArea:Can't find id {puzzleId}"); 
            return false;

        }

        private void FillPuzzle(string puzzleToFillId)
        {
            var puzzleToFill = _completeness[puzzleToFillId];
            puzzleToFill.Filled = true;
            _completeness[puzzleToFillId] = puzzleToFill;
            
            var puzzleAnimator = puzzleToFill.SceneObject.GetComponent<IPuzzleAnimator>();
            if (puzzleAnimator != null)
                puzzleAnimator.Animate();
            else
            {
                // по дефолту просто ставим альфу в полностью непрозрачный
                var objectRenderer = puzzleToFill.SceneObject.GetComponent<SpriteRenderer>();
                if (objectRenderer == null)
                    Debug.LogError("Can't find object renderer");
                else
                {
                    Color color = objectRenderer.color;
                    color.a = 1.0f;
                    objectRenderer.color = color;
                }
            }
        }

        private bool GameOver()
        {
            foreach (var puzzleRec in _completeness)
            {
                if (puzzleRec.Value.Filled)
                    continue;

                return false;
            }

            return true;
        }
    }
}
