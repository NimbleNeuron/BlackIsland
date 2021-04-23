using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.WicklineActive2)]
	public class WicklineActive2 : LocalSkillScript
	{
		private bool rageCheck;


		public override void Start()
		{
			PlayAnimation(Self, TriggerSkill02);
			SetAnimation(Self, BooleanSkill02, true);
			if (!rageCheck)
			{
				PlayEffectChild(Self, "FX_BI_Wickline_Rage");
				rageCheck = true;
			}
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanSkill02, false);
			StopEffectByTag(Self, "WicklineSkill02Cancel");
			StopSoundByTag(Self, "WicklineSkill02Cancel");
		}
	}
}