using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class PlayerStatusSnapshot : CharacterStatusSnapshot
	{
		
		[Key(6)] public int bullet;

		
		[Key(5)] public int exp;

		
		[Key(7)] public int extraPoint;

		
		[Key(8)] public int monsterKill;

		
		[Key(9)] public int playerKill;

		
		[Key(10)] public int playerKillAssist;

		
		[SerializationConstructor]
		public PlayerStatusSnapshot(int hp, int sp, int level, int shield, float moveSpeed, int exp, int bullet,
			int extraPoint, int monsterKill, int playerKill, int playerKillAssist) : base(hp, sp, level, shield,
			moveSpeed)
		{
			this.exp = exp;
			this.bullet = bullet;
			this.extraPoint = extraPoint;
			this.monsterKill = monsterKill;
			this.playerKill = playerKill;
			this.playerKillAssist = playerKillAssist;
		}

		
		public PlayerStatusSnapshot(CharacterStatus status) : base(status)
		{
			exp = status.Exp;
			bullet = status.Bullet;
			extraPoint = status.ExtraPoint;
			monsterKill = status.MonsterKill;
			playerKill = status.PlayerKill;
			playerKillAssist = status.PlayerKillAssist;
		}
	}
}