using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.RozziActive4SpeedUpState)]
	public class RozziActive4SpeedUpState : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "RozziActive4Buff", "FX_BI_Rozzi_Skill04_Buff");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "RozziActive4Buff", true);
		}
	}
}