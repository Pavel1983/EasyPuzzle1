using System.Collections.Generic;
using PuzzleGame.UI;
using UnityEngine;

namespace PuzzleGame
{
	[CreateAssetMenu]
	public class GameData : ScriptableObject
	{
		public List<LevelData> LevelData = new List<LevelData>();
	}
}