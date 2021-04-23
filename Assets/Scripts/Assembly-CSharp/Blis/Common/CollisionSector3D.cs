using Core;
using UnityEngine;

namespace Blis.Common
{
	public class CollisionSector3D : CollisionObject3D
	{
		public CollisionSector3D(CollisionObjectProperty property)
		{
			this.property = property;
		}


		public CollisionSector3D(Vector3 position, float radius, float angle, Vector3 normalized)
		{
			property = CollisionObjectProperty.Sector(position, radius, angle * 0.5f, normalized);
		}


		public new void UpdateNormalized(Vector3 normalized)
		{
			property.UpdateNormalized(normalized);
		}


		public CollisionCircularSector Convert2D()
		{
			return new CollisionCircularSector
			{
				angle = property.angle,
				radius = property.radius,
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