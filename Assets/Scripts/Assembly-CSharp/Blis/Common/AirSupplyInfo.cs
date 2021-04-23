using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	public class AirSupplyInfo
	{
		[Key(2)] public Vector3 dropPosition;


		[Key(1)] public ItemGrade itemGrade;


		[Key(0)] public int objectId;
	}
}