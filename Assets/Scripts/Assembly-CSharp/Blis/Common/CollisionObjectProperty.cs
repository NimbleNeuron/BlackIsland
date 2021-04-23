using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class CollisionObjectProperty
	{
		
		[Key(2)] public float angle;

		
		[Key(4)] public float depth;

		
		[Key(5)] public Vector3 normalized;

		
		[Key(6)] public Vector3 position;

		
		[Key(1)] public float radius;

		
		[Key(0)] public CollisionObjectType type;

		
		[Key(3)] public float width;

		
		public static CollisionObjectProperty Circle(Vector3 position, float radius)
		{
			return new CollisionObjectProperty
			{
				type = CollisionObjectType.Circle,
				position = position,
				radius = radius
			};
		}

		
		public static CollisionObjectProperty Sector(Vector3 position, float radius, float angle, Vector3 normalized)
		{
			return new CollisionObjectProperty
			{
				type = CollisionObjectType.Sector,
				position = position,
				radius = radius,
				angle = angle,
				normalized = normalized
			};
		}

		
		public static CollisionObjectProperty Box(Vector3 position, float width, float depth, Vector3 normalized)
		{
			return new CollisionObjectProperty
			{
				type = CollisionObjectType.Box,
				position = position,
				radius = Mathf.Sqrt(width * width + depth * depth),
				width = width,
				depth = depth,
				normalized = normalized
			};
		}

		
		public void UpdatePosition(Vector3 position)
		{
			this.position = position;
		}

		
		public void UpdateRadius(float radius)
		{
			this.radius = radius;
		}

		
		public void UpdateNormalized(Vector3 normalized)
		{
			this.normalized = normalized;
		}

		
		public void UpdateDepth(float depth)
		{
			this.depth = depth;
		}

		
		public void UpdateWidth(float width)
		{
			this.width = width;
		}

		
		public void UpdateAngle(float angle)
		{
			this.angle = angle;
		}

		
		public CollisionObject3D CreateCollisionObject()
		{
			switch (type)
			{
				case CollisionObjectType.Circle:
					return new CollisionCircle3D(this);
				case CollisionObjectType.Sector:
					return new CollisionSector3D(this);
				case CollisionObjectType.Box:
					return new CollisionBox3D(this);
				default:
					return new CollisionCircle3D(this);
			}
		}
	}
}