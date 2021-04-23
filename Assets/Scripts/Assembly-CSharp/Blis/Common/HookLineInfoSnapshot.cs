using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class HookLineInfoSnapshot : ISnapshot
	{
		
		[Key(0)] public int hookLineCode;

		
		[Key(4)] public BlisFixedPoint linkedTimeStack;

		
		[Key(1)] public int linkFromObjectId;

		
		[Key(2)] public int linkToObjectId;

		
		[Key(3)] public Vector3? linkToPoint;
	}
}