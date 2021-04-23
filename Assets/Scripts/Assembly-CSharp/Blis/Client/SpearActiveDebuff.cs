using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SpearActiveDebuff)]
	public class SpearActiveDebuff : LocalSkillScript
	{
		private bool createdEffect;


		public override void Start()
		{
			if (!createdEffect)
			{
				PlayEffectChildManual(Self, "SpearActiveDebuff", "FX_BI_Common_Slow_Debuff");
				createdEffect = true;
			}
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "SpearActiveDebuff", true);
			createdEffect = false;
		}
	}
}