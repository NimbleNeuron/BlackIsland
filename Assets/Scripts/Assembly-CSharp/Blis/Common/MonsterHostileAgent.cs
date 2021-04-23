namespace Blis.Common
{
	
	public class MonsterHostileAgent : HostileAgent
	{
		
		public MonsterHostileAgent(ObjectBase obj) : base(obj) { }

		
		public override HostileType GetHostileType(HostileAgent target)
		{
			if (CheckSameObject(target))
			{
				return HostileType.Ally;
			}

			if (target.objectType == ObjectType.Monster || target.objectType == ObjectType.Dummy ||
			    target.objectType.IsSummonObject())
			{
				return HostileType.Ally;
			}

			if (0 < teamNumber && teamNumber == target.teamNumber)
			{
				return HostileType.Ally;
			}

			return HostileType.Enemy;
		}
	}
}