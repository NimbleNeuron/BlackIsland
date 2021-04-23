using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.OneHandSwordDagger)]
	public class OneHandSwordDagger : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			Caster.MountNormalAttack(Singleton<OneHandSwordSkillActiveData>.inst.DaggerAttackSkillCode[SkillLevel]);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForSeconds(StateDuration);
			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.UnmountNormalAttack();
		}
	}
}