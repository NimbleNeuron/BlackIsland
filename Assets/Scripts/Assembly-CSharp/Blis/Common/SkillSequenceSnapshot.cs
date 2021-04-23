using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class SkillSequenceSnapshot
	{
		
		[Key(0)] public int currentSequence;

		
		[Key(1)] public int maxSequence;
	}
}