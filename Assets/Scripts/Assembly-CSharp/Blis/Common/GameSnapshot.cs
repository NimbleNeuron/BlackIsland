using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class GameSnapshot
	{
		[Key(3)] public BlisFixedPoint areaRestrictionRemainTime;


		[Key(4)] public Dictionary<int, AreaRestrictionState> areaStateMap;


		[Key(6)] public int day;


		[Key(5)] public DayNight dayNight;


		[Key(7)] public bool isStopAreaRestriction;


		[Key(2)] public Dictionary<int, MoveAgentSnapshot> moveAgentSnapshots;


		[Key(9)] public List<SightSnapshot> sights;


		[Key(0)] public List<UserSnapshot> userList;


		[Key(8)] public BlisFixedPoint wicklineRespawnRemainTime;


		[Key(1)] public List<SnapshotWrapper> worldSnapshot;
	}
}