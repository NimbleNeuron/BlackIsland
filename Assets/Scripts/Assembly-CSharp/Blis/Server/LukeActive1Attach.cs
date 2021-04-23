using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeActive1Attach)]
	public class LukeActive1Attach : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			SwitchSkillSet(SkillSlotIndex.Active1, SkillSlotSet.Active1_2);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		protected override void Finish(bool cancel = false)
		{
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			Caster.SwitchSkillSet(SkillSlotIndex.Active1, SkillSlotSet.Active1_1);
			base.Finish(cancel);
		}
	}
}