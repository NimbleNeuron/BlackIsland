using UnityEngine;

namespace Blis.Common
{
	public abstract class CollisionObject3D
	{
		protected CollisionObjectProperty property;


		public Vector3 Normalized => property.normalized;


		public float Width => property.width;


		public float Depth => property.depth;


		public float Radius => property.radius;


		public Vector3 Position => property.position;


		public void UpdatePosition(Vector3 position)
		{
			property.UpdatePosition(position);
		}


		public void UpdateRadius(float radius)
		{
			property.UpdateRadius(radius);
		}


		public void UpdateNormalized(Vector3 normalized)
		{
			property.UpdateNormalized(normalized);
		}


		public void UpdateDepth(float depth)
		{
			property.UpdateDepth(depth);
		}


		public void UpdateWidth(float width)
		{
			property.UpdateWidth(width);
		}


		public void UpdateAngle(float angle)
		{
			property.UpdateAngle(angle * 0.5f);
		}


		public bool Collision(CollisionObject3D collisionObject)
		{
			if (collisionObject is CollisionCircle3D)
			{
				return Collision(collisionObject as CollisionCircle3D);
			}

			if (collisionObject is CollisionSector3D)
			{
				return Collision(collisionObject as CollisionSector3D);
			}

			return collisionObject is CollisionBox3D && Collision(collisionObject as CollisionBox3D);
		}


		public abstract bool Collision(CollisionCircle3D circle);


		public abstract bool Collision(CollisionSector3D sector);


		public abstract bool Collision(CollisionBox3D box);
	}
}