using UnityEngine;

namespace Blis.Common
{
	public class WorldObjectPool : ObjectPool
	{
		public override void InitPool()
		{
			AllocPool(100, new GameObject("Projectile"));
		}
	}
}