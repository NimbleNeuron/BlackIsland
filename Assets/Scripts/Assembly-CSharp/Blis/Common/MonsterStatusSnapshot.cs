using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class MonsterStatusSnapshot : CharacterStatusSnapshot
	{
		
		[SerializationConstructor]
		public MonsterStatusSnapshot(int hp, int sp, int level, int shield, float moveSpeed) : base(hp, sp, level,
			shield, moveSpeed) { }

		
		public MonsterStatusSnapshot(CharacterStatus status) : base(status) { }
	}
}