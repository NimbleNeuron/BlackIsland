using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class PlayerKillAnnounceInfo
	{
		
		[Key(7)] public int aliveCount;

		
		[Key(8)] public List<int> assistants;

		
		[Key(2)] public int deadCharacterCode;

		
		[Key(0)] public int deadPlayerObjectId;

		
		[Key(3)] public int killCharacterCode;

		
		[Key(4)] public int killCount;

		
		[Key(1)] public int killPlayerObjectId;

		
		[Key(5)] public WeaponType killWeaponType;

		
		[Key(6)] public bool trapKill;
	}
}