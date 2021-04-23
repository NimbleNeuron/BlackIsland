using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.NadineActive4Buff)]
	public class NadineActive4Buff : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "Nadine_Skill04_Active", "FX_BI_Nadine_Skill04_Active");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Nadine_Skill04_Active", true);
		}
	}
}