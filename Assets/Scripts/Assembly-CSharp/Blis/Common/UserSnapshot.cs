using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class UserSnapshot
	{
		[Key(9)] public List<BulletItem> bulletItems;


		[Key(1)] public SnapshotWrapper characterSnapshot;


		[Key(3)] public List<EquipItem> equips;


		[Key(5)] public int exp;


		[Key(7)] public List<InvenItem> inventoryItems;


		[Key(8)] public List<int> outSightCharacterIds;


		[Key(2)] public byte[] playerSnapshot;


		[Key(6)] public float survivalTime;


		[Key(0)] public long userId;


		[Key(4)] public int walkableNavMask;
	}
}