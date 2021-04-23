namespace Blis.Server
{
	
	public class GroundingState : CharacterState
	{
		
		public GroundingState(int stateCode, WorldCharacter self, WorldCharacter caster) : base(stateCode, self, caster)
		{
		}

		
		public override bool IsDone()
		{
			return false;
		}
	}
}
