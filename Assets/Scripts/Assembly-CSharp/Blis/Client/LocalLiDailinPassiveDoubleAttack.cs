using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.LiDailinPassiveDoubleAttack)]
	public class LocalLiDailinPassiveDoubleAttack : LocalSkillScript
	{
		private const string Passive_Bone_Foot_L = "Bip001 L Foot";


		private const string Passive_Bone_Foot_R = "Bip001 R Foot";


		private const string Passive_Bone_Arm_L = "Bip001 L Hand";


		private const string Passive_Bone_Arm_R = "Bip001 R Hand";


		private const string Passive_effect_name = "FX_BI_Dailin_Passive";


		private const string Passive_effecton = "FX_BI_Dailin_Skill02B";


		private const string Passive_on_key = "passive_01_on";


		private const string Passive_01_key = "passive";


		private const string Passive_02_key = "passive_01";


		private static readonly int TriggerAttack01_p = Animator.StringToHash("tAttack01_p");


		private static readonly int TriggerAttack02_p = Animator.StringToHash("tAttack02_p");


		private int playCount;


		public override void Start()
		{
			playCount++;
			if (3f < Time.time - lastNormalAttackTime)
			{
				playCount = 0;
			}

			lastNormalAttackTime = Time.time;
			PlayAnimation(Self, playCount % 2 == 0 ? TriggerAttack01_p : TriggerAttack02_p);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			PlayEffectChildManual(Self, "passive_01_on", "FX_BI_Dailin_Skill02B");
			LocalPlayerCharacter target2 = Self as LocalPlayerCharacter;
			MasteryType equipWeaponMasteryType = GetEquipWeaponMasteryType(target2);
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


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "passive", true);
			StopEffectChildManual(Self, "passive_01", true);
			StopEffectChildManual(Self, "passive_01_on", true);
			if (cancel)
			{
				StopEffectByTag(Self, "LIDailinNormalAttackCancel");
				StopSoundByTag(Self, "LIDailinNormalAttackCancel");
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			return "";
		}
	}
}