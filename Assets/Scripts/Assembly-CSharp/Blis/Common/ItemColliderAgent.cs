using UnityEngine;

namespace Blis.Common
{
	public class ItemColliderAgent : ColliderAgent
	{
		protected BoxCollider coll;

		protected override Collider GetCollider()
		{
			return coll;
		}


		public Collider Init()
		{
			GameUtil.BindOrAdd<BoxCollider>(gameObject, ref coll);
			coll.size = new Vector3(0.8f, GameConstants.DEFAULT_WORLDOBJECT_COLLIDER_HEIGHT, 0.8f);
			return coll;
		}
	}
}