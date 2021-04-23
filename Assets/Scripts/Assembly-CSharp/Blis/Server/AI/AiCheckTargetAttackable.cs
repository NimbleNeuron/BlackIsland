namespace Blis.Server
{
	
	public class AiCheckTargetAttackable : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			WorldCharacter worldCharacter = base.agent.Controller.GetTarget() as WorldCharacter;
			return worldCharacter != null && base.agent.IsAttackable(worldCharacter);
		}
	}
}
