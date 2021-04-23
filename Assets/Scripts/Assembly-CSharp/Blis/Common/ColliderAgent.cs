using UnityEngine;

namespace Blis.Common
{
	public abstract class ColliderAgent : MonoBehaviour
	{
		protected abstract Collider GetCollider();


		public virtual Vector3 ClosestPointOnBounds(Vector3 position)
		{
			if (!(GetCollider() == null))
			{
				return GetCollider().ClosestPointOnBounds(position);
			}

			return position;
		}
	}
}