using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class SkillControllerSnapshot
	{
		
		[Key(1)] public List<SkillScriptSnapshot> passiveScripts;

		
		[Key(0)] public List<SkillScriptSnapshot> playingScripts;

		
		[Key(2)] public List<SkillScriptSnapshot> playingStateScripts;
	}
}