using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.SisselaActive4Mark)]
	public class LocalSisselaActive4Mark : LocalSisselaSkill
	{
		private readonly string effectLine = "FX_BI_Sissela_Skill04_Char";


		private readonly string effectWilson = "FX_BI_Sissela_Skill04_Loop";


		public override void Start()
		{
			PlayEffectChildManual(Self, "FX_BI_Sissela_Skill04_Char", effectLine);
			PlayEffectChildManual(Self, "FX_BI_Sissela_Skill04_Loop", effectWilson, "Fx_Top");
			PlaySoundPoint(Self, "Sissela_Skill04_Count", 15);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			base.Play(action, target, targetPosition);
			if (action == 1)
			{
				PlayEffectChild(Self, "FX_BI_Sissela_Skill04_Buff", "Fx_Top");
			}
		}


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "FX_BI_Sissela_Skill04_Char", true);
			StopEffectChildManual(Self, "FX_BI_Sissela_Skill04_Loop", true);
		}
	}
}