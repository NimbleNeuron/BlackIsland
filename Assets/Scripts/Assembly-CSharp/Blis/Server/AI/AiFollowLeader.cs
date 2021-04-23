using Blis.Common;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("리더의 현재 위치로 이동한다.")]
	public class AiFollowLeader : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (base.agent.ObjectType != ObjectType.BotPlayerCharacter)
			{
				base.EndAction(false);
				return;
			}
			WorldBotPlayerCharacter worldBotPlayerCharacter = (WorldBotPlayerCharacter)base.agent;
			if (worldBotPlayerCharacter.Leader == null)
			{
				base.EndAction(false);
				return;
			}
			Vector3 position = worldBotPlayerCharacter.Leader.GetPosition();
			worldBotPlayerCharacter.Controller.MoveTo(position, false);
			this.moveDestination.value = position;
			base.EndAction(true);
		}

		
		public BBParameter<Vector3> moveDestination;
	}
}
