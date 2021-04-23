using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyunwooPassiveBuff)]
	public class HyunwooPassiveBuff : LocalSkillScript
	{
		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayEffectChildManual(Self, "HyunwooPassiveBuffL", "FX_BI_Hyunwoo_Passive_Buff", "Fx_Hand_L");
				PlayEffectChildManual(Self, "HyunwooPassiveBuffR", "FX_BI_Hyunwoo_Passive_Buff", "Fx_Hand_R");
				PlaySoundPoint(Self, "hyunwoo_Passive_Activation_v2");
			}
		}


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "HyunwooPassiveBuffL", true);
			StopEffectChildManual(Self, "HyunwooPassiveBuffR", true);
			PlaySoundPoint(Self, "hyunwoo_Passive_Heal");
		}
	}
}