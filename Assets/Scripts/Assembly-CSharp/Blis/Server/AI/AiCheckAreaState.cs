using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("현재 AI가 위치한 지역의 상태.")]
	public class AiCheckAreaState : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return base.agent.IsAlive && MonoBehaviourInstance<GameService>.inst.Area.getAreaDataList((Area area) => (area.AreaRestrictionState == AreaRestrictionState.Normal && this.checkNormal) || (area.AreaRestrictionState == AreaRestrictionState.Reserved && this.checkReserved) || (area.AreaRestrictionState == AreaRestrictionState.Restricted && this.checkRestricted)).FirstOrDefault((AreaData area) => area.maskCode == base.agent.GetCurrentAreaMask()) != null;
		}

		
		public bool checkNormal;

		
		public bool checkReserved;

		
		public bool checkRestricted;
	}
}
