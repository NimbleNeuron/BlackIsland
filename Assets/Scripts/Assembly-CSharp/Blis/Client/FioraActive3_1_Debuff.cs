using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.FioraActive3_1_Debuff)]
	public class FioraActive3_1_Debuff : LocalSkillScript
	{
		private bool createdEffect;


		public override void Start()
		{
			if (!createdEffect)
			{
				PlayEffectChildManual(Self, "FioraActive3_1_Debuff", "FX_BI_Common_Slow_Debuff");
				createdEffect = true;
			}
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "FioraActive3_1_Debuff", true);
			createdEffect = false;
		}
	}
}