using UnityEngine;

namespace Core
{
	
	public struct CollisionCircle : ICollisionObject
	{
		
		public bool Collision(CollisionCircle circle)
		{
			return Vector2.Distance(position, circle.position) <= radius + circle.radius;
		}

		
		public bool Collision(CollisionCircularSector circularSector)
		{
			if (Vector2.Distance(position, circularSector.position) >= radius + circularSector.radius)
			{
				return false;
			}

			if (position.Equals(circularSector.position))
			{
				return true;
			}

			if (CollisionCircularSector.CollisionEdge(circularSector, -circularSector.angle, this) ||
			    CollisionCircularSector.CollisionEdge(circularSector, circularSector.angle, this))
			{
				return true;
			}

			Vector2 normalized = (position - circularSector.position).normalized;
			float num = Vector2.Dot(circularSector.normal, normalized);
			return (num >= 1f ? 0f : num <= -1f ? 180f : Mathf.Acos(num) * 180f / 3.1415927f) <= circularSector.angle;
		}

		
		public bool Collision(CollisionBox box)
		{
			Vector2 vector;
			return CollisionBox.Collision(box, position, radius, out vector);
		}

		
		public float radius;

		
		public Vector2 position;
	}
}