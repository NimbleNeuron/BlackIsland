using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class SummonStatusSnapshot : CharacterStatusSnapshot
	{
		
		[SerializationConstructor]
		public SummonStatusSnapshot(int hp, int sp, int level, int shield, float moveSpeed) : base(hp, sp, level,
			shield, moveSpeed) { }

		
		public SummonStatusSnapshot(CharacterStatus status) : base(status) { }
	}
}