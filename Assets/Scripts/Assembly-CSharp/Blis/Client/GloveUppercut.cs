using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.GloveUppercut)]
	public class GloveUppercut : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectChildManual(Self, "WSkill_Glove_Buff", "FX_BI_WSkill_Glove_01", "Weapon_R");
			PlayEffectPoint(Self, "FX_BI_WSkill_Glove_03", "Fx_Hand_R");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "WSkill_Glove_Buff", true);
		}
	}
}