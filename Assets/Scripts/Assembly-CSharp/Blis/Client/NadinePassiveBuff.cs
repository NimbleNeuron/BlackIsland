using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.NadinePassiveBuff)]
	public class NadinePassiveBuff : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectPoint(Self, "FX_BI_Nadine_Passive_Buff");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}