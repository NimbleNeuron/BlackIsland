using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.Stun)]
	public class Stun : LocalSkillScript
	{
		private bool createdEffect;

		public override void Start()
		{
			if (!createdEffect)
			{
				PlayEffectChildManual(Self, "common_stun_state", "FX_BI_Common_Stun_Debuff", "Fx_Top");
				createdEffect = true;
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "common_stun_state", true);
			createdEffect = false;
		}
	}
}