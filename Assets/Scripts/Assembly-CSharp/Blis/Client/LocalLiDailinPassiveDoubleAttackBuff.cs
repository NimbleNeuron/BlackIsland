using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LiDailinPassiveDoubleAttackBuff)]
	public class LocalLiDailinPassiveDoubleAttackBuff : LocalSkillScript
	{
		private const string Passive_Bone_Foot_L = "Bip001 L Foot";


		private const string Passive_Bone_Foot_R = "Bip001 R Foot";


		private const string Passive_Bone_Arm_L = "Bip001 L Hand";


		private const string Passive_Bone_Arm_R = "Bip001 R Hand";


		private const string Passive_effect_name = "FX_BI_Dailin_Passive";


		private const string Passive_on_key = "passive_01_on";


		private const string Passive_01_key = "passive";


		private const string Passive_02_key = "passive_01";


		private const string Passive_Effect_On = "FX_BI_Dailin_Skill02B";


		public override void Start()
		{
			PlayEffectChildManual(Self, "passive_01_on", "FX_BI_Dailin_Skill02B");
			LocalPlayerCharacter target = Self as LocalPlayerCharacter;
			MasteryType equipWeaponMasteryType = GetEquipWeaponMasteryType(target);
			if (equipWeaponMasteryType == MasteryType.Glove)
			{
				PlayEffectChildManual(Self, "passive", "FX_BI_Dailin_Passive", "Bip001 L Foot");
				PlayEffectChildManual(Self, "passive_01", "FX_BI_Dailin_Passive", "Bip001 R Foot");
				return;
			}

			if (equipWeaponMasteryType != MasteryType.Nunchaku)
			{
				return;
			}

			PlayEffectChildManual(Self, "passive", "FX_BI_Dailin_Passive", "Bip001 L Hand");
			PlayEffectChildManual(Self, "passive_01", "FX_BI_Dailin_Passive", "Bip001 R Hand");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "passive", true);
			StopEffectChildManual(Self, "passive_01", true);
			StopEffectChildManual(Self, "passive_01_on", true);
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			return "";
		}
	}
}