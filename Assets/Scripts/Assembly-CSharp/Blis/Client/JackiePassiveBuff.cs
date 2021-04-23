using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.JackiePassiveBuff)]
	public class JackiePassiveBuff : LocalSkillScript
	{
		private bool createdEffect;


		public override void Start()
		{
			if (!createdEffect)
			{
				PlayEffectChildManual(Self, "JackiePassiveBuff", "FX_BI_Jackie_Passive_Buff");
				createdEffect = true;
			}

			PlaySoundPoint(Self, "jackie_Passive_Activation");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "JackiePassiveBuff", true);
			createdEffect = false;
		}
	}
}