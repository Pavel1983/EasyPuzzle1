using UnityEngine;

namespace PuzzleGame.UI
{
	[CreateAssetMenu]
	public class LevelData : ScriptableObject
	{
		public Level LevelPrefab;
		public Sprite LevelPreview;
	}
}