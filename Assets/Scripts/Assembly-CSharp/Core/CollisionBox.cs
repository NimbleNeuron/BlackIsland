using UnityEngine;

namespace Core
{
	
	public struct CollisionBox : ICollisionObject
	{
		
		public static bool Collision(CollisionBox box, Vector2 circlePosition, float circleRadius,
			out Vector2 boxClosestVector)
		{
			float x = circlePosition.x - box.position.x;
			float y = circlePosition.y - box.position.y;
			Vector2 vector = new Vector2(x, y);
			float num = Vector2.SignedAngle(box.normal, Vector2.up);
			float num2 = Mathf.Cos(num * 0.017453292f);
			float num3 = Mathf.Sin(num * 0.017453292f);
			x = vector.x * num2 - vector.y * num3 + box.position.x;
			y = vector.x * num3 + vector.y * num2 + box.position.y;
			Vector2 vector2 = new Vector2(x, y);
			x = Mathf.Max(box.position.x - box.width / 2f, Mathf.Min(box.position.x + box.width / 2f, vector2.x));
			y = Mathf.Max(box.position.y - box.depth / 2f, Mathf.Min(box.position.y + box.depth / 2f, vector2.y));
			boxClosestVector = new Vector2(x, y);
			return Vector2.Distance(boxClosestVector, vector2) < circleRadius;
		}

		
		public bool Collision(CollisionCircle circle)
		{
			Vector2 vector;
			return Collision(this, circle.position, circle.radius, out vector);
		}

		
		public bool Collision(CollisionCircularSector circularSector)
		{
			Vector2 a;
			if (!Collision(this, circularSector.position, circularSector.radius, out a))
			{
				return false;
			}

			float num = depth / 2f;
			float num2 = width / 2f;
			Vector2 fr = new Vector2(position.x + num2, position.y + num);
			Vector2 bl = new Vector2(position.x - num2, position.y - num);
			float num3 = Vector2.SignedAngle(normal, Vector2.up);
			float num4 = Mathf.Cos(num3 * 0.017453292f);
			float num5 = Mathf.Sin(num3 * 0.017453292f);
			Vector2 vector = circularSector.position - position;
			Vector2 vector2 = new Vector2(vector.x * num4 - vector.y * num5 + position.x,
				vector.x * num5 + vector.y * num4 + position.y);
			if (Collision(bl, fr, vector2))
			{
				return true;
			}

			vector = circularSector.normal + vector;
			Vector2 normalized = new Vector2(vector.x * num4 - vector.y * num5 + position.x - vector2.x,
				vector.x * num5 + vector.y * num4 + position.y - vector2.y).normalized;
			if (CollisionCircularSector.CollisionEdge(circularSector, -circularSector.angle, position, num4, num5, fr,
				bl) || CollisionCircularSector.CollisionEdge(circularSector, circularSector.angle, position, num4, num5,
				fr, bl))
			{
				return true;
			}

			Vector2 normalized2 = (a - vector2).normalized;
			float num6 = Vector2.Dot(normalized, normalized2);
			return (num6 >= 1f ? 0f : num6 <= -1f ? 180f : Mathf.Acos(num6) * 180f / 3.1415927f) <=
			       circularSector.angle;
		}

		
		public bool Collision(CollisionBox box)
		{
			Vector2 r1_FL;
			Vector2 r1_FR;
			Vector2 r1_BL;
			Vector2 r1_BR;
			GetCornerPositions(this, out r1_FL, out r1_FR, out r1_BL, out r1_BR);
			Vector2 r2_FL;
			Vector2 r2_FR;
			Vector2 r2_BL;
			Vector2 r2_BR;
			GetCornerPositions(box, out r2_FL, out r2_FR, out r2_BL, out r2_BR);
			return IsIntersectingAABB_OBB(r1_FL, r1_FR, r1_BL, r1_BR, r2_FL, r2_FR, r2_BL, r2_BR) &&
			       SATRectangleRectangle(r1_FL, r1_FR, r1_BL, r1_BR, r2_FL, r2_FR, r2_BL, r2_BR);
		}

		
		public static bool Collision(Vector2 BL, Vector2 FR, Vector2 unRotatedTargetPoint)
		{
			return unRotatedTargetPoint.x > BL.x && unRotatedTargetPoint.x < FR.x && unRotatedTargetPoint.y > BL.y &&
			       unRotatedTargetPoint.y < FR.y;
		}

		
		public static void GetCornerPositions(CollisionBox box, out Vector2 FL, out Vector2 FR, out Vector2 BL,
			out Vector2 BR)
		{
			Vector2 a = Vector2.Perpendicular(box.normal);
			float d = box.depth / 2f;
			float d2 = box.width / 2f;
			FL = box.position + box.normal * d + a * d2;
			FR = box.position + box.normal * d - a * d2;
			BL = box.position - box.normal * d + a * d2;
			BR = box.position - box.normal * d - a * d2;
		}

		
		private static bool IsIntersectingAABB_OBB(Vector2 r1_FL, Vector2 r1_FR, Vector2 r1_BL, Vector2 r1_BR,
			Vector2 r2_FL, Vector2 r2_FR, Vector2 r2_BL, Vector2 r2_BR)
		{
			bool result = false;
			float r1_minX = Mathf.Min(r1_FL.x, Mathf.Min(r1_FR.x, Mathf.Min(r1_BL.x, r1_BR.x)));
			float r1_maxX = Mathf.Max(r1_FL.x, Mathf.Max(r1_FR.x, Mathf.Max(r1_BL.x, r1_BR.x)));
			float r2_minX = Mathf.Min(r2_FL.x, Mathf.Min(r2_FR.x, Mathf.Min(r2_BL.x, r2_BR.x)));
			float r2_maxX = Mathf.Max(r2_FL.x, Mathf.Max(r2_FR.x, Mathf.Max(r2_BL.x, r2_BR.x)));
			float r1_minZ = Mathf.Min(r1_FL.y, Mathf.Min(r1_FR.y, Mathf.Min(r1_BL.y, r1_BR.y)));
			float r1_maxZ = Mathf.Max(r1_FL.y, Mathf.Max(r1_FR.y, Mathf.Max(r1_BL.y, r1_BR.y)));
			float r2_minZ = Mathf.Min(r2_FL.y, Mathf.Min(r2_FR.y, Mathf.Min(r2_BL.y, r2_BR.y)));
			float r2_maxZ = Mathf.Max(r2_FL.y, Mathf.Max(r2_FR.y, Mathf.Max(r2_BL.y, r2_BR.y)));
			if (IsIntersectingAABB(r1_minX, r1_maxX, r1_minZ, r1_maxZ, r2_minX, r2_maxX, r2_minZ, r2_maxZ))
			{
				result = true;
			}

			return result;
		}

		
		public static bool IsIntersectingAABB(float r1_minX, float r1_maxX, float r1_minZ, float r1_maxZ, float r2_minX,
			float r2_maxX, float r2_minZ, float r2_maxZ)
		{
			bool result = true;
			if (r1_minX > r2_maxX)
			{
				result = false;
			}
			else if (r2_minX > r1_maxX)
			{
				result = false;
			}
			else if (r1_minZ > r2_maxZ)
			{
				result = false;
			}
			else if (r2_minZ > r1_maxZ)
			{
				result = false;
			}

			return result;
		}

		
		private static bool SATRectangleRectangle(Vector2 r1_FL, Vector2 r1_FR, Vector2 r1_BL, Vector2 r1_BR,
			Vector2 r2_FL, Vector2 r2_FR, Vector2 r2_BL, Vector2 r2_BR)
		{
			bool flag = false;
			if (!IsOverlapping(GetNormal(r1_BL, r1_FL), r1_FL, r1_FR, r1_BL, r1_BR, r2_FL, r2_FR, r2_BL, r2_BR))
			{
				return flag;
			}

			if (!IsOverlapping(GetNormal(r1_FL, r1_FR), r1_FL, r1_FR, r1_BL, r1_BR, r2_FL, r2_FR, r2_BL, r2_BR))
			{
				return flag;
			}

			if (!IsOverlapping(GetNormal(r2_BL, r2_FL), r1_FL, r1_FR, r1_BL, r1_BR, r2_FL, r2_FR, r2_BL, r2_BR))
			{
				return flag;
			}

			return IsOverlapping(GetNormal(r2_FL, r2_FR), r1_FL, r1_FR, r1_BL, r1_BR, r2_FL, r2_FR, r2_BL, r2_BR) ||
			       flag;
		}

		
		private static Vector3 GetNormal(Vector2 startPos, Vector2 endPos)
		{
			Vector2 vector = endPos - startPos;
			return new Vector2(-vector.y, vector.x);
		}

		
		private static float DotProduct(Vector2 v1, Vector2 v2)
		{
			return v1.x * v2.x + v1.y * v2.y;
		}

		
		private static bool IsOverlapping(Vector3 normal, Vector2 r1_FL, Vector2 r1_FR, Vector2 r1_BL, Vector2 r1_BR,
			Vector2 r2_FL, Vector2 r2_FR, Vector2 r2_BL, Vector2 r2_BR)
		{
			bool result = false;
			float a = DotProduct(normal, r1_FL);
			float a2 = DotProduct(normal, r1_FR);
			float a3 = DotProduct(normal, r1_BL);
			float b = DotProduct(normal, r1_BR);
			float num = Mathf.Min(a, Mathf.Min(a2, Mathf.Min(a3, b)));
			float num2 = Mathf.Max(a, Mathf.Max(a2, Mathf.Max(a3, b)));
			float a4 = DotProduct(normal, r2_FL);
			float a5 = DotProduct(normal, r2_FR);
			float a6 = DotProduct(normal, r2_BL);
			float b2 = DotProduct(normal, r2_BR);
			float num3 = Mathf.Min(a4, Mathf.Min(a5, Mathf.Min(a6, b2)));
			float num4 = Mathf.Max(a4, Mathf.Max(a5, Mathf.Max(a6, b2)));
			if (num <= num4 && num3 <= num2)
			{
				result = true;
			}

			return result;
		}

		
		public float width;

		
		public float depth;

		
		public Vector2 normal;

		
		public Vector2 position;
	}
}