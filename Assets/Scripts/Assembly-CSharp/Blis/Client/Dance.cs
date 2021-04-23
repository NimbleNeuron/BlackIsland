using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.Dance)]
	public class Dance : LocalSkillScript
	{
		public override void Start()
		{
			SetAnimation(Self, BooleanDance, true);
			PlayAnimation(Self, TriggerDance);
			PlayEffectChildManual(Self, "common_dance_state", "FX_BI_Common_Dance_Debuff");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			SetAnimation(Self, BooleanDance, false);
			StopEffectChildManual(Self, "common_dance_state", false);
		}
	}
}