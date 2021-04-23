using Core;
using UnityEngine;

namespace Blis.Common
{
	public class CollisionCircle3D : CollisionObject3D
	{
		public CollisionCircle3D(CollisionObjectProperty property)
		{
			this.property = property;
		}


		public CollisionCircle3D(Vector3 position, float radius)
		{
			property = CollisionObjectProperty.Circle(position, radius);
		}


		public CollisionCircle Convert2D()
		{
			return new CollisionCircle
			{
				radius = property.radius,
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