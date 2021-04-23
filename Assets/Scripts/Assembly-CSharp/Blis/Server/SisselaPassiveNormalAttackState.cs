using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaPassiveNormalAttackState)]
	public class SisselaPassiveNormalAttackState : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			Caster.MountNormalAttack(Singleton<SisselaSkillData>.inst.PassiveNormalAttackMountSkillCode);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.UnmountNormalAttack();
			PlayPassiveSkill(info);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}