using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class CharacterStatusSnapshot
	{
		
		[IgnoreMember] protected const int LAST_KEY_IDX = 4;

		
		[Key(0)] public int hp;

		
		[Key(2)] public int level;

		
		[Key(4)] public float moveSpeed;

		
		[Key(3)] public int shield;

		
		[Key(1)] public int sp;

		
		[SerializationConstructor]
		public CharacterStatusSnapshot(int hp, int sp, int level, int shield, float moveSpeed)
		{
			this.hp = hp;
			this.sp = sp;
			this.level = level;
			this.shield = shield;
			this.moveSpeed = moveSpeed;
		}

		
		public CharacterStatusSnapshot(CharacterStatus status)
		{
			hp = status.Hp;
			sp = status.Sp;
			level = status.Level;
			shield = status.Shield;
			moveSpeed = status.MoveSpeed;
		}
	}
}