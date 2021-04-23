using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class SkillLevelSnapshot
	{
		[Key(1)] public Dictionary<SkillSlotIndex, int> skillEvolutionLevelMap;


		[Key(0)] public Dictionary<SkillSlotIndex, int> skillLevelMap;


		[Key(3)] public Dictionary<MasteryType, int> weaponSkillEvolutionLevelMap;


		[Key(2)] public Dictionary<MasteryType, int> weaponSkillLevelMap;
	}
}