using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AssaultRifleOverHeat)]
	public class AssaultRifleOverHeat : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForSeconds(StateDuration);
			Finish();
		}

		
		protected override void Start()
		{
			base.Start();
			Caster.MountNormalAttack(
				Singleton<AssaultRifleSkillActiveData>.inst.AssaultRifleOverHeatAttack[SkillLevel]);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Caster.UnmountNormalAttack();
		}
	}
}