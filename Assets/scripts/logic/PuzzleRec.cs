using System;

namespace PuzzleGame
{
	[Serializable]
	public struct PuzzleRec
	{
		public bool Filled;
		[NonSerialized]
		public PuzzleElementDataHolder SceneObject;
	}
}