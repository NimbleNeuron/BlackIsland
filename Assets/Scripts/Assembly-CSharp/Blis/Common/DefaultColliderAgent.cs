using UnityEngine;

namespace Blis.Common
{
	
	public class DefaultColliderAgent : ColliderAgent
	{
		
		protected override Collider GetCollider()
		{
			return null;
		}
	}
}
