using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LiDailinPassiveDoubleAttackBuff)]
	public class LiDailinPassiveDoubleAttackBuff : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			Caster.MountNormalAttack(Singleton<LiDailinSkillData>.inst.PassiveDoubleAttackSkillCode);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.UnmountNormalAttack();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}