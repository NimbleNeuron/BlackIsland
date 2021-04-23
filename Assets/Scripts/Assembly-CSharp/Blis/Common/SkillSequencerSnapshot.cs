using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class SkillSequencerSnapshot
	{
		
		[Key(0)] public Dictionary<SkillSlotSet, SkillSequenceSnapshot> sequenceMap =
			new Dictionary<SkillSlotSet, SkillSequenceSnapshot>(new SkillSlotSetComparer());
	}
}