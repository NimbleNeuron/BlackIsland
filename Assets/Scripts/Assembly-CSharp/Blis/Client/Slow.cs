using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.Slow)]
	public class Slow : LocalSkillScript
	{
		private bool createdEffect;

		public override void Start()
		{
			if (!createdEffect)
			{
				PlayEffectChildManual(Self, "common_slow_sate", "FX_BI_Common_Slow_Debuff");
				createdEffect = true;
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "common_slow_sate", true);
			createdEffect = false;
		}
	}
}