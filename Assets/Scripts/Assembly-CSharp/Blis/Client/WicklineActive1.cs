using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.WicklineActive1)]
	public class WicklineActive1 : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill01);
			SetAnimation(Self, BooleanSkill01, true);
			PlayEffectChildManual(Self, "WicklineActive1", "FX_BI_Wickline_Skill01_Move02", "Bip001 Spine");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			if (actionNo == 1)
			{
				PlayEffectChildManual(Self, "WicklineActive1Attack", "FX_BI_Wickline_Skill01_Move03", "Bip001 Spine");
				StopEffectChildManual(Self, "WicklineActive1", true);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill01, false);
			StopEffectChildManual(Self, "WicklineActive1Attack", true);
		}
	}
}