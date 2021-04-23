using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class SkillCooldownSnapshot
	{
		
		[Key(0)] public Dictionary<SkillSlotSet, float> cooldownMap;

		
		[Key(1)] public Dictionary<SkillSlotSet, float> maxCooldownMap;

		
		[Key(2)] public Dictionary<MasteryType, float> weaponSkillCooldownMap;
	}
}