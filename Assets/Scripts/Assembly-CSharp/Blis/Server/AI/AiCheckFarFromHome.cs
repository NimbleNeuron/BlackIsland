using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("HomeLocation으로 돌아가야하는 거리까지 이동했는지 확인. 돌아가야 되면 true, 아니면 false")]
	public class AiCheckFarFromHome : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			float num;
			if (this.useManualAwayDistance)
			{
				num = this.manualDistance;
			}
			else
			{
				num = base.agent.Stat.SightRange * 2f;
			}
			return Vector3.Distance(this.homeLocation.value, base.agent.GetPosition()) > num;
		}

		
		public BBParameter<Vector3> homeLocation;

		
		public bool useManualAwayDistance;

		
		public float manualDistance;
	}
}
