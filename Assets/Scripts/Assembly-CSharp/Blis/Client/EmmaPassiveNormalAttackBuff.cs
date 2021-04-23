using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.EmmaPassiveNormalAttackBuff)]
	public class EmmaPassiveNormalAttackBuff : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "Emma_Passive_Ready", "FX_BI_Emma_Passive_Ready", "Bip001 L Hand");
			PlaySoundPoint(Self, "Emma_Passive_Ready", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Emma_Passive_Ready", true);
		}
	}
}