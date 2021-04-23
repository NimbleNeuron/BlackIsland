using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziNormalAttackDoubleShotState)]
	public class RozziNormalAttackDoubleShotState : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			int skillLevel = Caster.GetSkillLevel(SkillSlotIndex.Passive);
			Caster.MountNormalAttack(Singleton<RozziSkillPassiveData>.inst.DoubleShotSkillCodeByLevel[skillLevel]);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.UnmountNormalAttack();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForSeconds(StateDuration);
			Finish();
		}
	}
}