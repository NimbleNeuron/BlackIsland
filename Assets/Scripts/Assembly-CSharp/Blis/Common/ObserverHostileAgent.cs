namespace Blis.Common
{
	
	public class ObserverHostileAgent : HostileAgent
	{
		
		private int selectedTeamNumber;

		
		private int selectedTeamSlot;

		
		public ObserverHostileAgent(ObjectBase obj) : base(obj) { }

		
		
		public int SelectedTeamNumber => selectedTeamNumber;

		
		
		public int SelectedTeamSlot => selectedTeamSlot;

		
		public void SelectTeamNumber(int teamNumber)
		{
			selectedTeamNumber = teamNumber;
		}

		
		public void SelectTeamSlot(int teamSlot)
		{
			selectedTeamSlot = teamSlot;
		}

		
		public override HostileType GetHostileType(HostileAgent target)
		{
			if (CheckSameObject(target))
			{
				return HostileType.Ally;
			}

			if (target.teamNumber <= 0)
			{
				return HostileType.Enemy;
			}

			if (selectedTeamNumber == target.teamNumber)
			{
				return HostileType.Ally;
			}

			return HostileType.Enemy;
		}
	}
}