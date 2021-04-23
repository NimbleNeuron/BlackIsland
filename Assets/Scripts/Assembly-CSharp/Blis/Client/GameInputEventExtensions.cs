namespace Blis.Client
{
	public static class GameInputEventExtensions
	{
		public static bool IsChatEvent(this GameInputEvent inputEvent)
		{
			return inputEvent == GameInputEvent.ChatActive || inputEvent == GameInputEvent.ChatActive2;
		}


		public static bool IsFixedKeyEvent(this GameInputEvent inputEvent)
		{
			if (inputEvent != GameInputEvent.Escape)
			{
				switch (inputEvent)
				{
					case GameInputEvent.PingTarget:
					case GameInputEvent.MarkTarget:
					case GameInputEvent.AddGuide:
					case GameInputEvent.ChatItem:
					case GameInputEvent.ThrowItem:
					case GameInputEvent.MoveExpandMap:
					case GameInputEvent.NormalMatchingSolo:
					case GameInputEvent.NormalMatchingDuo:
					case GameInputEvent.NormalMatchingSquad:
					case GameInputEvent.RankMatchingSolo:
					case GameInputEvent.RankMatchingDuo:
					case GameInputEvent.RankMatchingSquad:
						return true;
				}

				return false;
			}

			return true;
		}
	}
}