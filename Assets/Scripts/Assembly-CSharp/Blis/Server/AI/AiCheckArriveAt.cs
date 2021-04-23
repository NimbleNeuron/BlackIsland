using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("이동 목표 지점에 도착했는지 확인. 도착했으면 true, 아직 도착 못했으면 false")]
	public class AiCheckArriveAt : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			Vector3 value = this.target.value;
			Vector3 position = base.agent.GetPosition();
			value.y = 0f;
			position.y = 0f;
			return Vector3.Distance(value, position) <= base.agent.StoppingDistance && !base.agent.IsMoving();
		}

		
		public BBParameter<Vector3> target;
	}
}
