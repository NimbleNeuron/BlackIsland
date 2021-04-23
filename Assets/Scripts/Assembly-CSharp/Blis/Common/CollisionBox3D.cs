using Core;
using UnityEngine;

namespace Blis.Common
{
	public class CollisionBox3D : CollisionObject3D
	{
		public CollisionBox3D(CollisionObjectProperty property)
		{
			this.property = property;
		}


		public CollisionBox3D(Vector3 position, float width, float depth, Vector3 normalized)
		{
			property = CollisionObjectProperty.Box(position, width, depth, normalized);
		}


		public CollisionBox Convert2D()
		{
			return new CollisionBox
			{
				width = property.width,
				depth = property.depth,
				normal = new Vector2(property.normalized.x, property.normalized.z),
				position = new Vector2(property.position.x, property.position.z)
			};
		}


		public override bool Collision(CollisionCircle3D circle)
		{
			return Convert2D().Collision(circle.Convert2D());
		}


		public override bool Collision(CollisionSector3D sector)
		{
			return Convert2D().Collision(sector.Convert2D());
		}


		public override bool Collision(CollisionBox3D box)
		{
			return Convert2D().Collision(box.Convert2D());
		}
	}
}