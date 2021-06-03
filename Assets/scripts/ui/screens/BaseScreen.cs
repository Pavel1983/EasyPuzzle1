using UnityEngine;

namespace PuzzleGame.UI
{
	public abstract class BaseScreen : MonoBehaviour, IScreen
	{
		[SerializeField] private string _id;
		
		public string Id => _id;
        
		public virtual void Show()
		{
			gameObject.SetActive(true);
		}

		public virtual void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}