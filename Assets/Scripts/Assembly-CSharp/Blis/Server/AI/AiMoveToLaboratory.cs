using System;
using Blis.Common;
using Blis.Common.Utils;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("연구소의 무작위 위치로 이동.")]
	public class AiMoveToLaboratory : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (!base.agent.StateEffector.CanCharacterControl())
			{
				base.EndAction(false);
				return;
			}
			try
			{
				CharacterSpawnPoint initialPlayerSpawnPoint = MonoBehaviourInstance<GameService>.inst.Level.GetInitialPlayerSpawnPoint();
				base.agent.Controller.MoveTo(initialPlayerSpawnPoint.GetPosition(), false);
			}
			catch (Exception)
			{
			}
			finally
			{
				base.EndAction(true);
			}
		}
	}
}
