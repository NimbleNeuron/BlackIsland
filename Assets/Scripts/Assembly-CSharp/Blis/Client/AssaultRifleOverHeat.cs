using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.AssaultRifleOverHeat)]
	public class AssaultRifleOverHeat : LocalSkillScript
	{
		public override void Start()
		{
			PlayEffectPoint(Self, "FX_BI_WSkill_AssuaultRifle_01");
			PlayEffectChildManual(Self, "WSkill_AssaultRifle_Active", "FX_BI_WSkill_AssuaultRifle_02", "Equip_R");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "WSkill_AssaultRifle_Active", true);
		}
	}
}