using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.YukiActive2)]
	public class YukiActive2 : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			AddState(Caster, Singleton<YukiSkillActive2Data>.inst.DefenceBuffState,
				Singleton<YukiSkillActive2Data>.inst.Duration);
			ModifySkillCooldown(Caster, SkillSlotSet.Active3_1,
				Singleton<YukiSkillActive2Data>.inst.CooldownReduceActive3[SkillLevel]);
			StartConcentration();
			yield return WaitForSeconds(SkillConcentrationTime);
			FinishConcentration(false);
			Caster.ExtraPointModifyTo(Caster, Caster.Character.Stat.MaxExtraPoint);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}