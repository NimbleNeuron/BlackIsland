using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public interface IBlockDamage
	{
		
		int BlockDamage(Vector3? damagePoint, DamageSubType damageSubType, int damage);
	}
}