using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class SkillScriptSnapshot
	{
		
		[Key(6)] public int casterId;

		
		[Key(3)] public BlisVector cursorPosition;

		
		[Key(4)] public BlisVector releasePosition;

		
		[Key(1)] public int skillCode;

		
		[Key(2)] public int skillEvolutionLevel;

		
		[Key(0)] public SkillId skillId;

		
		[Key(5)] public int targetObjectId;
	}
}