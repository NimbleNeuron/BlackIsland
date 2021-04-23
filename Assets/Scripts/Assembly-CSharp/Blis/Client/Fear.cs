using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.Fear)]
	public class Fear : LocalSkillScript
	{
		private bool createdEffect;

		private void Ref()
		{
			Reference.Use(createdEffect);
		}


		public override void Start()
		{
			PlayEffectChildManual(Self, "common_fear_sate", "FX_BI_Common_Fear_Debuff", "Fx_Top");
			PlaySoundPoint(Self, "Common_Fear", 15);
			createdEffect = true;
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "common_fear_sate", true);
			createdEffect = false;
		}
	}
}