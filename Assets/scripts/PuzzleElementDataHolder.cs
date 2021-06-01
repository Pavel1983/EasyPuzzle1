using UnityEngine;

namespace PuzzleGame
{
    public class PuzzleElementDataHolder : MonoBehaviour
    {
        [SerializeField] private PuzzleElementData _data;

        public string Id => _data.Id;
    }
}
