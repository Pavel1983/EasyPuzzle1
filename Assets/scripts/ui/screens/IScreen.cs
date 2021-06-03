namespace PuzzleGame.UI
{
	public interface IScreen
	{
		string Id { get; }
		void Show();
		void Hide();
	}
}