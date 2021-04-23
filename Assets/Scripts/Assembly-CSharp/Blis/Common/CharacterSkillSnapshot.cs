using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class CharacterSkillSnapshot
	{
		
		[Key(7)] public bool isConcentrating;

		
		[Key(1)] public SkillCooldownSnapshot skillCooldownSnapshot;

		
		[Key(3)] public SkillEvolutionSnapshot skillEvolutionSnapshot;

		
		[Key(0)] public SkillLevelSnapshot skillLevelSnapshot;

		
		[Key(2)] public SkillSequencerSnapshot skillSequencerSnapshot;

		
		[Key(5)] public Dictionary<SkillSlotIndex, SkillSlotSet> skillSlotMapSnapshot;

		
		[Key(4)] public SkillStackSnapshot skillStackSnapshot;

		
		[Key(6)] public SpecialSkillId specialSkillId;
	}
}