using Blis.Common.Utils;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("현재 전투에 참여중인 플레이어의 평균 숙련도 값과 입력한 값을 선택한 부등호로 비교한다.")]
	public class AiCheckMasteryAve : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			int num2;
			int num = MonoBehaviourInstance<GameService>.inst.Player.AlivePlayersSumMasteryLevel(out num2);
			return num2 != 0 && OperationTools.Compare(num / num2, this.masteryLevel, this.checkType);
		}

		
		public CompareMethod checkType;

		
		public int masteryLevel;
	}
}
