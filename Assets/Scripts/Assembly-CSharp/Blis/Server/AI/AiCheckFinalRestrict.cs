using Blis.Common.Utils;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("더 이상 안전지역이 없는지 확인한다.")]
	public class AiCheckFinalRestrict : ConditionTaskBase
	{
		
		protected override bool OnCheck()
		{
			return MonoBehaviourInstance<GameService>.inst.Level.NextSafeAreaData(base.agent) == null;
		}
	}
}
