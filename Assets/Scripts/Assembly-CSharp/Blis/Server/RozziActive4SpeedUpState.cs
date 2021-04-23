using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziActive4SpeedUpState)]
	public class RozziActive4SpeedUpState : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			float num = Caster.GetSkillCooldown(SkillSlotSet.Active4_1) *
			            Singleton<RozziSkillActive4Data>.inst.FullStackCoolDownValue;
			ModifySkillCooldown(Caster, SkillSlotSet.Active4_1, -num);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return null;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}