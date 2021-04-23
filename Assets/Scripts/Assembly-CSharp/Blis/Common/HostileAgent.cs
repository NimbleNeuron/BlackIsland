namespace Blis.Common
{
	public class HostileAgent
	{
		public readonly int objectId;


		public readonly ObjectType objectType;


		public readonly int teamNumber;

		public HostileAgent(ObjectBase obj)
		{
			objectType = obj.ObjectType;
			objectId = obj.ObjectId;
			teamNumber = obj.TeamNumber;
		}


		protected bool CheckSameObject(HostileAgent target)
		{
			return target.objectId == objectId;
		}


		public virtual HostileType GetHostileType(HostileAgent target)
		{
			if (CheckSameObject(target))
			{
				return HostileType.Ally;
			}

			return HostileType.Enemy;
		}
	}
}