using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ShoichiPassiveMax)]
	public class ShoichiPassiveMax : LocalSkillScript
	{
		private const string Fx_Bottom = "Fx_Bottom";


		private const string Shoichi_Skill_P = "Shoichi_Skill_P";


		private const string FX_BI_Shoichi_PassiveBuff_full = "FX_BI_Shoichi_PassiveBuff_full";

		public override void Start()
		{
			PlayEffectChildManual(Self, "Shoichi_Skill_P", "FX_BI_Shoichi_PassiveBuff_full", "Fx_Bottom");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Shoichi_Skill_P", true);
		}
	}
}