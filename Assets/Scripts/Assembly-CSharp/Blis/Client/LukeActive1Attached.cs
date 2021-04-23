using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LukeActive1Attached)]
	public class LukeActive1Attached : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "LukeActive1_under", "FX_BI_Luke_Skill01_Debuff_Under");
			PlayEffectChildManual(Self, "LukeActive1_Top", "FX_BI_Luke_Skill01_Debuff_Top");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "LukeActive1_under", true);
			StopEffectChildManual(Self, "LukeActive1_Top", true);
		}
	}
}