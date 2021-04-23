using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("현재 캐릭터의 체력을 입력한 값과 선택한 부등호로 비교한다")]
	public class AiCheckHp : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return OperationTools.Compare(Mathf.FloorToInt((float)base.agent.Status.Hp / (float)base.agent.Stat.MaxHp * 100f), this.hpValue, this.checkType);
		}

		
		public CompareMethod checkType;

		
		public int hpValue;
	}
}
