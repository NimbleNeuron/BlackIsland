using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.BowActiveDebuff)]
	public class BowActiveDebuff : LocalSkillScript
	{
		private bool createdEffect;


		public override void Start()
		{
			if (!createdEffect)
			{
				PlayEffectChildManual(Self, "Bow_Skill_Debuff", "FX_BI_Common_Slow_Debuff");
				createdEffect = true;
			}
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Bow_Skill_Debuff", true);
			createdEffect = false;
		}
	}
}