using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class SkillEvolutionSnapshot
	{
		
		[Key(0)] public Dictionary<int, int> earnedPointMap;

		
		[Key(1)] public Dictionary<SkillEvolutionPointType, int> pointMap;
	}
}