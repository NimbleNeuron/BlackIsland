using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class PlayerCharacterSnapshot : CharacterSnapshot
	{
		
		[Key(7)] public int characterCode;

		
		[Key(12)] public bool isDyingCondition;

		
		[Key(14)] public bool isRest;

		
		[Key(15)] public SkillSlotSet lockedSlotSetFlag;

		
		[Key(13)] public BlisVector[] mapMarks;

		
		[Key(9)] public Dictionary<MasteryType, int> masteryLevels;

		
		[Key(8)] public int skinCode;

		
		[Key(11)] public int teamNumber;

		
		[Key(10)] public int whoKilledMe;
	}
}