using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AyaActive4Debuff)]
	public class AyaActive4Debuff : LocalSkillScript
	{
		public override void Start()
		{
			PlayAnimation(Self, TriggerCanNotControl);
			PlayEffectChildManual(Self, "AyaActive4Debuff", "FX_BI_Aya_Skill04_Debuff", "Fx_Top");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "AyaActive4Debuff", true);
		}
	}
}