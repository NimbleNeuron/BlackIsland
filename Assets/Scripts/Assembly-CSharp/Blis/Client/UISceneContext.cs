namespace Blis.Client
{
	public static class UISceneContext
	{
		public enum SceneState
		{
			Loading,

			UIAwaked,

			UIStarted
		}

		public static SceneState currentSceneState;
	}
}