using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HartActive2Buff)]
	public class HartActive2Buff : LocalSkillScript
	{
		private const string Hart_Skill02_Buff = "Hart_Skill02_Buff";


		private readonly Dictionary<int, string> Effect_Skill = new Dictionary<int, string>();


		private readonly Dictionary<int, string> SkillBuff_SFX = new Dictionary<int, string>();

		public HartActive2Buff()
		{
			Effect_Skill.Add(0, "Fx_BI_Hart_Skill02_Buff");
			Effect_Skill.Add(1, "Fx_BI_Hart_Skill02_Buff_Evolve");
			Effect_Skill.Add(2, "Fx_BI_Hart_Skill02_Buff_Evolve");
			SkillBuff_SFX.Add(0, "hart_Skill02_Buff");
			SkillBuff_SFX.Add(1, "hart_Skill02_Evo_Buff");
			SkillBuff_SFX.Add(2, "hart_Skill02_Evo_Buff");
		}


		public override void Start()
		{
			PlayEffectChildManual(Self, "Hart_Skill02_Buff", Effect_Skill[evolutionLevel]);
			PlaySoundChildManual(Self, SkillBuff_SFX[evolutionLevel], 15);
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Hart_Skill02_Buff", true);
			StopSoundChildManual(Self, SkillBuff_SFX[evolutionLevel]);
		}
	}
}