using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukeActive4SlowDebuffState)]
	public class LukeActive4SlowDebuffState : LocalSkillScript
	{
		private const string Skill02_Mob = "FX_BI_Common_Slow_Debuff";


		private const string Effect_SKill4_key = "Effect_SKill4_key";


		public override void Start()
		{
			PlayEffectChildManual(Self, "Effect_SKill4_key", "FX_BI_Common_Slow_Debuff");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Effect_SKill4_key", true);
		}
	}
}