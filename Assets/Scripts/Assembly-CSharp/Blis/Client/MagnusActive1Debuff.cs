using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.MagnusActive1Debuff)]
	public class MagnusActive1Debuff : LocalSkillScript
	{
		private bool createdEffect;


		public override void Start()
		{
			if (!createdEffect)
			{
				PlayEffectChildManual(Self, "Magnus_Skill01_Debuff", "FX_BI_Common_Slow_Debuff");
				createdEffect = true;
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Magnus_Skill01_Debuff", true);
			createdEffect = false;
		}
	}
}