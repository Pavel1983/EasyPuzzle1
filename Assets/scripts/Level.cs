using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PuzzleGame
{
    public struct PuzzleRec
    {
        public bool Filled;
        public PuzzleElementDataHolder SceneObject;
    }

    public class Level : MonoBehaviour
    {
        public event Action<string> EventReturnPuzzleBack;
        
        // временно
        [SerializeField] private float _magneticMinDistance;
        
        private Transform[] _puzzleTransforms;
        private string[] _puzzleIds;
        private GameObject _currentControlledPuzzle;
        private string _currentConrolledPuzzleId;
        private Camera _mainCamera;

        private Dictionary<string, PuzzleRec> _completeness = new Dictionary<string, PuzzleRec>();

        #region life cycle

        private void Awake()
        {
            _mainCamera = Camera.main;
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

        #endregion
        public bool Load()
        {
            var puzzles = GetComponentsInChildren<PuzzleElementDataHolder>();
            _puzzleTransforms = puzzles.Select(p => p.transform).ToArray();
            
            // hack 
            for (int i = 0; i < puzzles.Length; ++i)
            {
                var sr = puzzles[i].GetComponent<SpriteRenderer>();
                Color color = sr.color;
                color.a = 0.2f;
                sr.color = color;
            }
            // end hack

            _puzzleIds = puzzles.Select(item => item.Id).ToArray();

            for (int i = 0; i < _puzzleIds.Length; ++i)
            {
                PuzzleRec rec = new PuzzleRec();
                rec.Filled = false;
                rec.SceneObject = puzzles[i];
                
                _completeness.Add(_puzzleIds[i], rec);
            }

            return true;
        }

        public void TrackPuzzle(GameObject puzzleObject)
        {
            _currentControlledPuzzle = puzzleObject;
            _currentConrolledPuzzleId = puzzleObject.GetComponent<PuzzleElementDataHolder>().Id;
        }

        public string[] GetPuzzleCollection()
        {
            return _puzzleIds;
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
                // анимимируем попадание
                Debug.Log($"{_currentControlledPuzzle.gameObject.name} destroyed. New element filled");
                Destroy(_currentControlledPuzzle.gameObject);

                FillPuzzle(_currentConrolledPuzzleId);
                if (GameOver())
                {
                    Debug.Log("GameOver");
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
