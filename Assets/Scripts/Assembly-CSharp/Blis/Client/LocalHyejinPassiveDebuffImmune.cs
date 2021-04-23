using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.HyejinPassiveDebuffImmune)]
	public class LocalHyejinPassiveDebuffImmune : LocalSkillScript
	{
		private readonly string effectName = "FX_BI_Hyejin_Passive_Debuff3";


		private readonly string fxName = "FX_BI_Hyejin_Passive_Debuff4";

		public override void Start()
		{
			if (CasterId != SingletonMonoBehaviour<PlayerController>.inst.myObjectId)
			{
				return;
			}

			if (Self as LocalCharacter == null)
			{
				return;
			}

			PlayEffectChildManual(Self, "Hyejin_Passive_Debuff_3", effectName, "Fx_Bottom");
			StopEffectChildManual(Self, "Hyejin_Passive_Debuff_Immune", true);
			PlayEffectChildManual(Self, "Hyejin_Passive_Debuff_Immune", fxName, "Fx_Bottom");
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			if (CasterId != SingletonMonoBehaviour<PlayerController>.inst.myObjectId)
			{
				return;
			}

			StopEffectChildManual(Self, "Hyejin_Passive_Debuff_3", true);
			StopEffectChildManual(Self, "Hyejin_Passive_Debuff_Immune", true);
		}
	}
}