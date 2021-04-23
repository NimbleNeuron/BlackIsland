using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class SkillStackSnapshot
	{
		[Key(0)] public Dictionary<SkillSlotSet, int> stackCharacterSkill;


		[Key(1)] public Dictionary<SkillSlotSet, BlisFixedPoint> stackCharacterSkillIntervalTime;


		[Key(2)] public List<SkillSlotSet> stackCharacterSkillTimers;


		[Key(3)] public Dictionary<MasteryType, int> stackWeaponSkill;


		[Key(4)] public Dictionary<MasteryType, BlisFixedPoint> stackWeaponSkillIntervalTime;


		[Key(5)] public List<MasteryType> stackWeaponSkillTimers;
	}
}