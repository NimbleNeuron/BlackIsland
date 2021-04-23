using ParadoxNotion.Design;
using UnityEngine;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("타겟과의 거리 확인. checkRange보다 가까우면 true, 아니면 false")]
	public class AiCheckTargetDistance : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			WorldObject target = base.agent.Controller.GetTarget();
			return !(target == null) && Vector3.Distance(base.agent.GetPosition(), target.GetPosition()) <= this.checkRange;
		}

		
		public float checkRange;
	}
}
