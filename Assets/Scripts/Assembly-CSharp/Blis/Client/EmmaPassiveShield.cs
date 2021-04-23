using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.EmmaPassiveShield)]
	public class EmmaPassiveShield : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "Emma_Passive_Shield", "FX_BI_Emma_Passive_Shield", "Bip001");
			PlaySoundPoint(Self, "Emma_Passive_Shield", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Emma_Passive_Shield", true);
		}
	}
}