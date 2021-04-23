namespace Blis.Common
{
	public class PlayerHostileAgent : HostileAgent
	{
		public PlayerHostileAgent(ObjectBase obj) : base(obj) { }


		public override HostileType GetHostileType(HostileAgent target)
		{
			if (CheckSameObject(target))
			{
				return HostileType.Ally;
			}

			if (target.objectType.IsSummonObject() && ((SummonHostileAgent) target).ownerId == objectId)
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