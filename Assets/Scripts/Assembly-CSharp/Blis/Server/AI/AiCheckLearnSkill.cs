using Blis.Common;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("SkillSlotIndex에 해당하는 스킬을 배웠는지 확인한다. 배웠으면 true, 아니면 false")]
	public class AiCheckLearnSkill : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return base.agent.IsLearnSkill(this.skillSlotIndex);
		}

		
		public SkillSlotIndex skillSlotIndex;
	}
}
