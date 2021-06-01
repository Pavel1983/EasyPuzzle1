using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PuzzleGame
{
	[CreateAssetMenu]
	public class PuzzlesData: ScriptableObject
	{
		public List<PuzzleElementData> Value;

		public PuzzleElementData GetPuzzleData(string puzzleId)
		{
			return Value.FirstOrDefault(item => item.Id == puzzleId);
		}
	}
}