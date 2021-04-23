namespace Blis.Common
{
	
	public class SummonHostileAgent : HostileAgent
	{
		
		public readonly int ownerId;

		
		public SummonHostileAgent(ObjectBase obj, int ownerId) : base(obj)
		{
			this.ownerId = ownerId;
		}

		
		public override HostileType GetHostileType(HostileAgent target)
		{
			if (CheckSameObject(target))
			{
				return HostileType.Ally;
			}

			if (ownerId == target.objectId)
			{
				return HostileType.Ally;
			}

			if (0 < teamNumber && teamNumber == target.teamNumber)
			{
				return HostileType.Ally;
			}

			if (target.objectType.IsSummonObject())
			{
				return HostileType.Ally;
			}

			return HostileType.Enemy;
		}
	}
}