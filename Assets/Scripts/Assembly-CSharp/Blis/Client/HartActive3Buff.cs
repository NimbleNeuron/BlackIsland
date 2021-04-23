using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HartActive3Buff)]
	public class HartActive3Buff : LocalSkillScript
	{
		private const string FX_BI_Hart_Skill03_Evolve_Buff = "FX_BI_Hart_Skill03_Evolve_Buff";


		private const string Fx_Hand_L = "Fx_Hand_R";


		private const string Fx_Hand_R = "Fx_Hand_R";


		public override void Start()
		{
			PlayEffectChild(Self, "FX_BI_Hart_Skill03_Evolve_Buff", "Fx_Hand_R");
			PlayEffectChild(Self, "FX_BI_Hart_Skill03_Evolve_Buff", "Fx_Hand_R");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }
	}
}