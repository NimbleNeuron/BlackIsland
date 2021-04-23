using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LenoxPassiveCaptain)]
	public class LenoxPassiveCaptain : LocalSkillScript
	{
		private const string Lenox_Passive = "Lenox_Passive";


		public override void Start()
		{
			PlayEffectChildManual(Self, "FX_BI_Lenox_Passive_Shield_key", "FX_BI_Lenox_Passive_Shield", "Bip001");
			PlaySoundPoint(Self, "Lenox_Passive", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "FX_BI_Lenox_Passive_Shield_key", true);
		}
	}
}