using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyunwooPassiveBuff)]
	public class HyunwooPassiveBuff : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			int stackByGroup = Target.GetStackByGroup(info.stateData.group, Caster.ObjectId);
			if (info.stateData.maxStack <= stackByGroup)
			{
				PlaySkillAction(Caster, info.stateData.GroupData.skillId, 1);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}