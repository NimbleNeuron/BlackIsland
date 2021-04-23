using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziActive2DeBuffState)]
	public class RozziActive2DeBuffState : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "RozziActive2Debuff", "FX_BI_Rozzi_Skill02_Debuff", "Bip001");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "RozziActive2Debuff", true);
		}
	}
}