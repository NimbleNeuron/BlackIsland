using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraActive4judgment)]
	public class LocalChiaraActive4judgment : LocalSkillScript
	{
		private const string SetAnimatorLayerWeight_Formchange = "RapierFormchange";


		private const string SetAnimatorLayerWeight_FormchangeRun = "RapierFormchangeRun";


		private const string SetAnimatorLayerWeight_RapierFormchangeUnderRun = "RapierFormchangeUnderRun";


		private const string SetAnimatorLayerWeight_Formchange_Skill01 = "FormchangeSkill01Upper";


		private const string SetAnimatorLayerWeight_Rapier = "Rapier";


		private const string SetAnimatorLayerWeight_RapierRun = "RapierRun";


		private const string SetAnimatorLayerWeight_RapierUpper = "Skill01Upper";


		private const string SetAnimatorLayerWeight_RapierUnder = "RapierUnderRun";


		private const string Weapon_Special_2 = "Weapon_Special_2";


		private const string Skill04_02_Start_Sfx = "Chiara_Skill04_02_Start";


		private const string Skill04_RangeEffect_Key = "Skill04_Range";


		private const string Skill04_Effect_Key = "Skill04";


		private const string Skill04_Range_Effect = "FX_BI_Chiara_Skill04";


		private const string Skill04_Range_Effect_Point = "Root";


		private const string Skill04_Effect_01 = "FX_BI_Chiara_Skill04_Ing";


		private const string Skill04_Ing_sfx = "Chiara_Skill04_Ing";


		private const string Skill04_End = "Chiara_Skill04_End";


		private int passiveMaxStack;


		private int passiveStateGroup;


		private void Init()
		{
			if (passiveStateGroup == 0)
			{
				CharacterStateData data =
					GameDB.characterState.GetData(Singleton<ChiaraSkillData>.inst.PassiveDebuffStateCode[1]);
				passiveStateGroup = data.group;
				passiveMaxStack = data.maxStack;
			}
		}


		public override void Start()
		{
			SetAnimatorCullingMode(Self, AnimatorCullingMode.AlwaysAnimate);
			PlayAnimation(Self, TriggerSkill04_2);
			SetAnimation(Self, BooleanSkill04_02, true);
			PlaySoundChildManual(Self, "Chiara_Skill04_02_Start", 15);
			ActiveWeaponObject(Self, WeaponMountType.Equip_R, false);
			ActiveWeaponObject(Self, WeaponMountType.Equip_L, false);
			PlayEffectChildManual(Self, "Skill04_Range", "FX_BI_Chiara_Skill04");
			PlayEffectChildManual(Self, "Skill04", "FX_BI_Chiara_Skill04_Ing", "Root");
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active2_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1)
			{
				PlayAnimation(Self, TriggerSkill04_3);
				SetAnimatorLayer(Self, "RapierFormchange", 1f);
				SetAnimatorLayer(Self, "RapierFormchangeRun", 1f);
				SetAnimatorLayer(Self, "RapierFormchangeUnderRun", 1f);
				SetAnimatorLayer(Self, "FormchangeSkill01Upper", 1f);
				StopSoundChildManual(Self, "Chiara_Skill04_02_Start");
				ActiveWeaponObject(Self, WeaponMountType.Special_1, true);
				ActiveWeaponObject(Self, WeaponMountType.Special_2, false);
				ActiveWeaponObject(Self, WeaponMountType.Special_3, false);
				SetAnimatorLayer(Self, "Rapier", 1f);
				SetAnimatorLayer(Self, "RapierRun", 1f);
			}
		}


		public override void Finish(bool cancel)
		{
			SetAnimatorCullingMode(Self, AnimatorCullingMode.CullUpdateTransforms);
			SwitchMaterialChildManualFromDefault(Self, "Chiara_01_LOD1", 0, "Chiara_01_LOD1");
			SwitchMaterialChildManualFromDefault(Self, "Chiara_01_LOD1_Eye", 0, "Chiara_01_LOD1");
			SetAnimatorLayer(Self, "RapierFormchange", 0f);
			SetAnimatorLayer(Self, "RapierFormchangeRun", 0f);
			SetAnimatorLayer(Self, "RapierFormchangeUnderRun", 0f);
			SetAnimatorLayer(Self, "FormchangeSkill01Upper", 0f);
			SetAnimatorLayer(Self, "Rapier", 1f);
			SetAnimatorLayer(Self, "RapierRun", 1f);
			SetAnimatorLayer(Self, "Skill01Upper", 1f);
			SetAnimatorLayer(Self, "RapierUnderRun", 1f);
			PlayAnimation(Self, TriggerSkill04_4);
			SetAnimation(Self, BooleanSkill04, false);
			SetAnimation(Self, BooleanSkill04_02, false);
			ActiveWeaponObject(Self, WeaponMountType.Special_1, true);
			ActiveWeaponObject(Self, WeaponMountType.Special_2, false);
			ActiveWeaponObject(Self, WeaponMountType.Special_3, false);
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active2_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
		}


		public override UseSkillErrorCode IsCanUseSkill(LocalObject hitTarget, Vector3? cursorPosition)
		{
			Init();
			LocalCharacter localCharacter = hitTarget as LocalCharacter;
			if (localCharacter == null || !localCharacter.IsAlive)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			if (localCharacter.GetStateStackByGroup(passiveStateGroup, Self.ObjectId) < passiveMaxStack)
			{
				return UseSkillErrorCode.NotInvalidTarget;
			}

			return UseSkillErrorCode.None;
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<ChiaraSkillData>.inst.A4JudgmentBaseDamage[skillData.level].ToString();
				case 1:
					return ((int) (Singleton<ChiaraSkillData>.inst.A4JudgmentApDamage * SelfStat.AttackPower))
						.ToString();
				case 2:
				{
					int num = Mathf.Abs((int) (Singleton<ChiaraSkillData>.inst.A4KillCooldownModify * 100f));
					return string.Format("{0}%", num);
				}
				default:
					return "";
			}
		}
	}
}