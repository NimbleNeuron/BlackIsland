using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukeActive3SlowDebuffState)]
	public class LukeActive3SlowDebuffState : LocalSkillScript
	{
		private const string Skill02_Mob = "FX_BI_Common_Slow_Debuff";


		private const string Effect_SKill3_key = "Effect_SKill3_key";


		public override void Start()
		{
			PlayEffectChildManual(Self, "Effect_SKill3_key", "FX_BI_Common_Slow_Debuff");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Effect_SKill3_key", true);
		}
	}
}