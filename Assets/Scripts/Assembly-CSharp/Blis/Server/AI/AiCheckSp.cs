using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("현재 캐릭터의 스태미너를 입력한 값과 선택한 부등호로 비교한다")]
	public class AiCheckSp : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return OperationTools.Compare(Mathf.FloorToInt((float)base.agent.Status.Sp / (float)base.agent.Stat.MaxSp * 100f), this.spValue, this.checkType);
		}

		
		public CompareMethod checkType;

		
		public int spValue;
	}
}
