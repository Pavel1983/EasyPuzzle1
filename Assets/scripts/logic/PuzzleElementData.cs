using UnityEngine;

namespace PuzzleGame
{
    [CreateAssetMenu]
    public class PuzzleElementData : ScriptableObject
    {
        public string Id;
        public GameObject UiPrefab;
        public GameObject Prefab;
    }
}
