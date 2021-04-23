using Blis.Common;

namespace Blis.Client
{
	public abstract class GameAnnounce
	{
		public static GameAnnounce Create(GameAnnounceType type, byte[] announceInfo)
		{
			GameAnnounce gameAnnounce = null;
			switch (type)
			{
				case GameAnnounceType.AirSupplyNotice:
					gameAnnounce = new AirSupplyAnnounce();
					break;
				case GameAnnounceType.RestrictAreaNotice:
					gameAnnounce = new AreaRestrictionAnnounce();
					break;
				case GameAnnounceType.PlayerKill:
					gameAnnounce = new DeadPlayerAnnounce();
					break;
				case GameAnnounceType.DeadInRestrictArea:
					gameAnnounce = new DeadRestrictionAnnounce();
					break;
				case GameAnnounceType.DeadByMonster:
					gameAnnounce = new MonsterKillAnnounce();
					break;
				case GameAnnounceType.LastSafeConsole:
					gameAnnounce = new LastSafeConsoleAnnounce();
					break;
			}

			if (gameAnnounce == null)
			{
				return null;
			}

			gameAnnounce.Init(announceInfo);
			return gameAnnounce;
		}


		public abstract void Init(byte[] announceInfo);


		public abstract void ShowAnnounce();
	}
}