namespace Blis.Server.CharacterAction
{
	
	public abstract class ChainActionBase : ActionBase
	{
		
		protected ActionBase nextAction;

		
		protected ChainActionBase(WorldMovableCharacter self, ActionBase nextAction, bool findAttackTarget) : base(self,
			findAttackTarget)
		{
			this.nextAction = nextAction;
		}

		
		
		public ActionBase NextAction => nextAction;

		
		public void ReplaceNextAction(ActionBase action)
		{
			nextAction = action;
		}
	}
}