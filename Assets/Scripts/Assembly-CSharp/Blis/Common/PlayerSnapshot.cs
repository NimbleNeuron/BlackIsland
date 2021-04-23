using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class PlayerSnapshot : ISnapshot
	{
		[Key(2)] public byte[] characterSkillSnapshot;


		[Key(1)] public bool disconnected;


		[Key(7)] public Dictionary<IgnoreType, HashSet<int>> ignoreTargets;


		[Key(4)] public List<MasteryValue> masteryValues;


		[Key(0)] public bool observing;


		[Key(6)] public int rank;


		[Key(3)] public int skillPoint;


		[Key(5)] public int visitedAreaMaskCodeFlag;
	}
}