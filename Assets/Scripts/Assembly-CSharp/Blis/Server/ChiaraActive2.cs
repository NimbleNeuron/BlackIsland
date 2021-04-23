using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.ChiaraActive2)]
	public class ChiaraActive2 : SkillScript
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

			ShieldState shieldState = CreateState<ShieldState>(Caster, Singleton<ChiaraSkillData>.inst.A2ShieldState);
			shieldState.Init(Singleton<ChiaraSkillData>.inst.A2ApShield,
				Singleton<ChiaraSkillData>.inst.A2BaseShield[SkillLevel]);
			AddState(Caster, shieldState);
			PlaySkillAction(Caster, 1);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override bool IsChangedSkillSequence()
		{
			return Caster.IsHaveStateByGroup(
				GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.A2ShieldState).group, Caster.ObjectId);
		}
	}
}