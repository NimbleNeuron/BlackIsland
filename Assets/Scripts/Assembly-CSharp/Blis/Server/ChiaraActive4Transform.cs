using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraActive4Transform)]
	public class ChiaraActive4Transform : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			int maxHpBonus = Singleton<ChiaraSkillData>.inst.A4TransformMaxHpBonus[SkillLevel];
			CharacterState characterState = CreateState(Caster, Singleton<ChiaraSkillData>.inst.A4TransformStateCode);
			characterState.AddExternalStat(StatType.MaxHpBonus, maxHpBonus, StatType.None, 0f);
			AddState(Caster, characterState);
			yield return WaitForFrame();
			HpHealTo(Caster, 0, 0f, maxHpBonus, false, 0);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}