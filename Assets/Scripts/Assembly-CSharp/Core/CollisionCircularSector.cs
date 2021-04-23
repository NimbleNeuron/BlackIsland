using UnityEngine;

namespace Core
{
	
	public struct CollisionCircularSector : ICollisionObject
	{
		
		public static bool CollisionEdge(CollisionCircularSector circularSector, float edgeAngle,
			CollisionCircle circle)
		{
			float num = Mathf.Cos(edgeAngle * 0.017453292f);
			float num2 = Mathf.Sin(edgeAngle * 0.017453292f);
			Vector2 rhs = new Vector2(circularSector.normal.x * num - circularSector.normal.y * num2,
				circularSector.normal.x * num2 + circularSector.normal.y * num);
			Vector2 vector = circularSector.position - circle.position;
			float num3 = Vector2.Dot(vector, rhs);
			float num4 = Vector2.Dot(vector, vector) - circle.radius * circle.radius;
			if (num3 <= 0f || num4 <= 0f)
			{
				float num5 = num3 * num3 - num4;
				if (num5 >= 0f)
				{
					float num6 = -num3 - Mathf.Sqrt(num5);
					float num7 = -num3 + Mathf.Sqrt(num5);
					if (num6 > 0f && num6 < circularSector.radius || num7 > 0f && num7 < circularSector.radius)
					{
						return true;
					}
				}
			}

			return false;
		}

		
		public static bool CollisionEdge(CollisionCircularSector circularSector, float edgeAngle, Vector2 boxPosition,
			float boxCos, float boxSin, Vector2 FR, Vector2 BL)
		{
			float num = Mathf.Cos(edgeAngle * 0.017453292f);
			float num2 = Mathf.Sin(edgeAngle * 0.017453292f);
			Vector2 a = new Vector2(circularSector.normal.x * num - circularSector.normal.y * num2,
				circularSector.normal.x * num2 + circularSector.normal.y * num);
			Vector2 vector = circularSector.position + a * circularSector.radius - boxPosition;
			Vector2 vector2 = circularSector.position - boxPosition;
			Vector2 vector3 = new Vector2(vector.x * boxCos - vector.y * boxSin + boxPosition.x,
				vector.x * boxSin + vector.y * boxCos + boxPosition.y);
			Vector2 vector4 = new Vector2(vector2.x * boxCos - vector2.y * boxSin + boxPosition.x,
				vector2.x * boxSin + vector2.y * boxCos + boxPosition.y);
			float x = vector3.x;
			float x2 = vector4.x;
			if (vector3.x > vector4.x)
			{
				x = vector4.x;
				x2 = vector3.x;
			}

			if (x2 > FR.x)
			{
				x2 = FR.x;
			}

			if (x < BL.x)
			{
				x = BL.x;
			}

			if (x > x2)
			{
				return false;
			}

			float num3 = vector3.y;
			float num4 = vector4.y;
			float num5 = vector4.x - vector3.x;
			if (Mathf.Abs(num5) > 1E-07f)
			{
				float num6 = (vector4.y - vector3.y) / num5;
				float num7 = vector3.y - num6 * vector3.x;
				num3 = num6 * x + num7;
				num4 = num6 * x2 + num7;
			}

			if (num3 > num4)
			{
				float num8 = num4;
				num4 = num3;
				num3 = num8;
			}

			if (num4 > FR.y)
			{
				num4 = FR.y;
			}

			if (num3 < BL.y)
			{
				num3 = BL.y;
			}

			return num3 <= num4;
		}

		
		public bool Collision(CollisionCircle circle)
		{
			if (Vector2.Distance(circle.position, position) >= circle.radius + radius)
			{
				return false;
			}

			if (position.Equals(circle.position))
			{
				return true;
			}

			if (CollisionEdge(this, -angle, circle) || CollisionEdge(this, angle, circle))
			{
				return true;
			}

			Vector2 normalized = (circle.position - position).normalized;
			float num = Vector2.Dot(normal, normalized);
			return (num >= 1f ? 0f : num <= -1f ? 180f : Mathf.Acos(num) * 180f / 3.1415927f) <= angle;
		}

		
		public bool Collision(CollisionCircularSector circularSector)
		{
			return false;
		}

		
		public bool Collision(CollisionBox box)
		{
			Vector2 a;
			if (!CollisionBox.Collision(box, position, radius, out a))
			{
				return false;
			}

			float num = box.depth / 2f;
			float num2 = box.width / 2f;
			Vector2 fr = new Vector2(box.position.x + num2, box.position.y + num);
			Vector2 bl = new Vector2(box.position.x - num2, box.position.y - num);
			float num3 = Vector2.SignedAngle(box.normal, Vector2.up);
			float num4 = Mathf.Cos(num3 * 0.017453292f);
			float num5 = Mathf.Sin(num3 * 0.017453292f);
			Vector2 vector = position - box.position;
			Vector2 vector2 = new Vector2(vector.x * num4 - vector.y * num5 + box.position.x,
				vector.x * num5 + vector.y * num4 + box.position.y);
			if (CollisionBox.Collision(bl, fr, vector2))
			{
				return true;
			}

			vector = normal + vector;
			Vector2 normalized = new Vector2(vector.x * num4 - vector.y * num5 + box.position.x - vector2.x,
				vector.x * num5 + vector.y * num4 + box.position.y - vector2.y).normalized;
			if (CollisionEdge(this, -angle, box.position, num4, num5, fr, bl) ||
			    CollisionEdge(this, angle, box.position, num4, num5, fr, bl))
			{
				return true;
			}

			Vector2 normalized2 = (a - vector2).normalized;
			float num6 = Vector2.Dot(normalized, normalized2);
			return (num6 >= 1f ? 0f : num6 <= -1f ? 180f : Mathf.Acos(num6) * 180f / 3.1415927f) <= angle;
		}

		
		public float angle;

		
		public float radius;

		
		public Vector2 normal;

		
		public Vector2 position;
	}
}