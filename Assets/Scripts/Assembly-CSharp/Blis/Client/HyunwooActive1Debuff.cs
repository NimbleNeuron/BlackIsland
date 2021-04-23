using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyunwooActive1Debuff)]
	public class HyunwooActive1Debuff : LocalSkillScript
	{
		private bool createdEffect;


		public override void Start()
		{
			if (!createdEffect)
			{
				PlayEffectChildManual(Self, "Hyunwoo_Skill01_Debuff", "FX_BI_Hyunwoo_Skill01_Debuff");
				createdEffect = true;
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Hyunwoo_Skill01_Debuff", true);
			createdEffect = false;
		}
	}
}