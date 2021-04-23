namespace Core
{
	
	public interface ICollisionObject
	{
		
		bool Collision(CollisionCircle circle);

		
		bool Collision(CollisionCircularSector circularSector);

		
		bool Collision(CollisionBox box);
	}
}