using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziActive3_2EnableState)]
	public class RozziActive3_2EnableState : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "Debuff1", "FX_BI_Rozzi_Skill03_Fire2Ready", "Bip001 R Hand");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Debuff1", true);
		}
	}
}