using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ZahirPassiveDebuff)]
	public class ZahirPassiveDebuff : LocalSkillScript
	{
		private bool createdEffect;


		public override void Start()
		{
			if (!createdEffect)
			{
				PlayEffectChildManual(Self, "Zahir_Passive_Debuff", "FX_BI_Zahir_Passive");
				PlayEffectChildManual(Self, "Zahir_Passive_Debuff_01", "FX_BI_Zahir_Passive_Debuff");
				StopEffectChildManual(Self, "Zahir_Passive_Debuff_Hit", false);
				createdEffect = true;
			}
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Zahir_Passive_Debuff", false);
			StopEffectChildManual(Self, "Zahir_Passive_Debuff_01", false);
			PlayEffectChildManual(Self, "Zahir_Passive_Debuff_Hit", "FX_BI_Zahir_Passive_Hit");
			createdEffect = false;
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			return "";
		}
	}
}