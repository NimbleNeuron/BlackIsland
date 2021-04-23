using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.BoarCharge)]
	public class BoarCharge : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
			SetAnimation(Self, BooleanSkill01, true);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayEffectChildManual(Self, "Boar_Skill01", "FX_BI_Boar_Skill01_Attack");
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill01, false);
			StopEffectChildManual(Self, "Boar_Skill01", true);
			StopEffectByTag(Self, "BoarSkill01Cancel");
			StopSoundByTag(Self, "BoarSkill01Cancel");
		}
	}
}