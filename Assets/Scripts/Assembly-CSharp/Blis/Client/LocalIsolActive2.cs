using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.IsolActive2)]
	public class LocalIsolActive2 : LocalSkillScript
	{
		private const string Skill02_Ground_EffectKey = "Isol_Skill02";


		private const string Skill02_Ground_EffectKey_01 = "Isol_Skill02_01";


		private const string Skill02_Ground_EffectKey_02 = "Isol_Skill02_02";


		private const string Skill02_Ground = "FX_BI_Isol_Skill02_Ground";


		private const string Skill02_Ground_End = "FX_BI_Isol_Skill02_Ground_End";


		private const string Skill02_Ground_End_EffectKey = "Root";


		private const string Skill02_Parent = "Root";


		private const string SetAnimatorLayerWeight_Aiming = "Aiming";


		private const string SetAnimatorLayerWeight_AimingShot = "AimingShot";


		private const string SKill02_cartridge_AssaultRifle = "FX_BI_Isol_Skill02";


		private const string SKill02_cartridge_Pistol = "FX_BI_Isol_Skill02_pistol";


		private const string ShotFirePoint = "ShotPoint";


		private const string ShotFire = "FX_BI_Isol_Skill02_ShotFire";


		private const string Skill02_Hit = "FX_BI_Isol_Skill02_Hit";


		private const string Skill02_Hit_Key = "Skill02_Hit";


		private const string Skill02_Hit_Positon = "Bip001 Spine";


		private const string Skill02_playsound = "Isol_Skill02_Active";


		private const string Skill02_playsound_End = "Isol_Skill02_End";


		private const string Skill02_End = "Skill02";


		private const string Skill02_Fx = "Isol_Skill02_Attack_Ing";


		private const string Skill02_Fx_02 = "Isol_Skill02_loop_P";


		private readonly int hMoveHash = Animator.StringToHash("hMove");


		private readonly List<LocalCharacter> onFxCharacters = new List<LocalCharacter>();


		private readonly int vMoveHash = Animator.StringToHash("vMove");


		private readonly WaitForSeconds waitForSeconds200ms = new WaitForSeconds(0.2f);


		private float hMoveOld;


		private Vector3 lastFramePosition;


		private Vector3 lastMoveDirection;


		private ParticleSystem ps;


		private Transform trsfFx;


		private float updateTime;


		private float vMoveOld;


		public override void Start()
		{
			LocalPlayerCharacter localPlayerCharacter = Self as LocalPlayerCharacter;
			MasteryType equipWeaponMasteryType = GetEquipWeaponMasteryType(localPlayerCharacter);
			PlayAnimation(Self, TriggerReloadCancel);
			switch (equipWeaponMasteryType)
			{
				case MasteryType.CrossBow:
					PlaySoundChildManual(Self, "Isol_Skill02_loop_P", 15);
					break;
				case MasteryType.Pistol:
					PlaySoundChildManual(Self, "Isol_Skill02_loop_P", 15);
					break;
				case MasteryType.AssaultRifle:
					PlaySoundChildManual(Self, "Isol_Skill02_Attack_Ing", 15);
					break;
			}

			SetAnimation(Self, BooleanSkill02, true);
			LockSkillSlot(SkillSlotSet.Active1_1);
			LockSkillSlot(SkillSlotSet.Active3_1);
			LockSkillSlot(SkillSlotSet.Active4_1);
			LockSkillSlot(SkillSlotSet.WeaponSkill);
			PlayEffectChildManual(Self, "Isol_Skill02", "FX_BI_Isol_Skill02_Ground", "Root");
			PlayEffectChildManual(Self, "Isol_Skill02_02", "FX_BI_Isol_Skill02_ShotFire", "ShotPoint");
			PlaySoundPoint(Self, "Isol_Skill02_Active", 15);
			GameObject gameObject = PlayEffectChild(Self, "FX_BI_Isol_Skill02_Ground_End", "Root");
			trsfFx = gameObject != null ? gameObject.transform : null;
			CharacterMasteryData characterMasteryData =
				GameDB.mastery.GetCharacterMasteryData(localPlayerCharacter.CharacterCode);
			SetAnimatorLayerWeight(equipWeaponMasteryType, characterMasteryData, "Aiming", 1f);
			SetAnimatorLayerWeight(equipWeaponMasteryType, characterMasteryData, "AimingShot", 0.7f);
			lastFramePosition = Self.GetPosition();
			StartCoroutine(UpdateAnimationParameter());
			StartCoroutine(UpdateFxOnInRangeEnemy());
			if (equipWeaponMasteryType == MasteryType.Pistol)
			{
				PlayEffectChildManual(Self, "Isol_Skill02_01", "FX_BI_Isol_Skill02_pistol");
				return;
			}

			if (equipWeaponMasteryType != MasteryType.AssaultRifle)
			{
				return;
			}

			PlayEffectChildManual(Self, "Isol_Skill02_01", "FX_BI_Isol_Skill02");
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			ResetAnimatorTrigger(Self, TriggerReloadCancel);
			SetAnimation(Self, BooleanSkill02, false);
			LocalPlayerCharacter target = Self as LocalPlayerCharacter;
			switch (GetEquipWeaponMasteryType(target))
			{
				case MasteryType.CrossBow:
					StopSoundChildManual(Self, "Isol_Skill02_loop_P");
					break;
				case MasteryType.Pistol:
					StopSoundChildManual(Self, "Isol_Skill02_loop_P");
					break;
				case MasteryType.AssaultRifle:
					StopSoundChildManual(Self, "Isol_Skill02_Attack_Ing");
					break;
			}

			StopSoundByTag(Self, "Skill02");
			UnlockSkillSlot(SkillSlotSet.Active1_1);
			UnlockSkillSlot(SkillSlotSet.Active3_1);
			UnlockSkillSlot(SkillSlotSet.Active4_1);
			UnlockSkillSlot(SkillSlotSet.WeaponSkill);
			StopCoroutines();
			PlaySoundPoint(Self, "Isol_Skill02_End", 15);
			StopEffectChildManual(Self, "Isol_Skill02", true);
			StopEffectChildManual(Self, "Isol_Skill02_01", true);
			StopEffectChildManual(Self, "Isol_Skill02_02", true);
			if (trsfFx != null)
			{
				ps = trsfFx.GetComponent<ParticleSystem>();
				ps.Stop();
			}

			foreach (LocalCharacter localCharacter in onFxCharacters)
			{
				localCharacter.StopLocalEffectChildManual("Skill02_Hit", true);
			}

			onFxCharacters.Clear();
		}


		private IEnumerator UpdateFxOnInRangeEnemy()
		{
			CollisionSector3D sector = new CollisionSector3D(Self.GetPosition(), SkillRange, SkillAngle, SelfForward);
			for (;;)
			{
				sector.UpdatePosition(Self.GetPosition());
				List<LocalCharacter> characterWithinRange = GetCharacterWithinRange(sector, false, true);
				foreach (LocalCharacter localCharacter in characterWithinRange)
				{
					if (!onFxCharacters.Contains(localCharacter))
					{
						onFxCharacters.Add(localCharacter);
						GameObject resource = Self.LoadEffect("FX_BI_Isol_Skill02_Hit");
						localCharacter.PlayLocalEffectChildManual("Skill02_Hit", resource, "Bip001 Spine");
					}
				}

				for (int i = onFxCharacters.Count - 1; i >= 0; i--)
				{
					LocalCharacter localCharacter2 = onFxCharacters[i];
					if (!characterWithinRange.Contains(localCharacter2))
					{
						localCharacter2.StopLocalEffectChildManual("Skill02_Hit", true);
						onFxCharacters.RemoveAt(i);
					}
				}

				yield return waitForSeconds200ms;
			}
		}


		private IEnumerator UpdateAnimationParameter()
		{
			updateTime = 0f;
			vMoveOld = 0f;
			hMoveOld = 0f;
			lastMoveDirection = Vector3.zero;
			lastFramePosition = Self.GetPosition();
			lastFramePosition.y = 0f;
			for (;;)
			{
				Vector3 position = Self.GetPosition();
				position.y = 0f;
				float animatorFloatParameter = GetAnimatorFloatParameter(Self, vMoveHash);
				float animatorFloatParameter2 = GetAnimatorFloatParameter(Self, hMoveHash);
				Vector3 normalized = (position - lastFramePosition).normalized;
				if (0.01f <= (normalized - lastMoveDirection).sqrMagnitude)
				{
					updateTime = 0f;
					vMoveOld = animatorFloatParameter;
					hMoveOld = animatorFloatParameter2;
					lastMoveDirection = normalized;
				}

				float b = Vector3.Dot(normalized, SelfForward);
				float b2 = Vector3.Dot(normalized, SelfRight);
				updateTime += Time.deltaTime * 5f;
				if (1f < updateTime)
				{
					updateTime = 1f;
				}

				SetAnimation(Self, vMoveHash, Mathf.Lerp(vMoveOld, b, updateTime));
				SetAnimation(Self, hMoveHash, Mathf.Lerp(hMoveOld, b2, updateTime));
				lastFramePosition = position;
				yield return null;
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<IsolSkillActive2Data>.inst.DurationTime.ToString();
				case 1:
					return Singleton<IsolSkillActive2Data>.inst.DamageTermTime.ToString();
				case 2:
					return Singleton<IsolSkillActive2Data>.inst.Damage[skillData.level].ToString();
				case 3:
					return ((int) (Singleton<IsolSkillActive2Data>.inst.SkillApCoef * SelfStat.AttackPower)).ToString();
				case 4:
					return GameDB.characterState
						.GetData(Singleton<IsolSkillActive2Data>.inst.DebuffState[skillData.level]).duration.ToString();
				case 5:
					return (int) Math.Abs(GameDB.characterState
						.GetData(Singleton<IsolSkillActive2Data>.inst.DebuffState[skillData.level]).statValue1) + "%";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/Damage";
				case 1:
					return "ToolTipType/CoolTime";
				case 2:
					return "ToolTipType/Cost";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<IsolSkillActive2Data>.inst.Damage[level].ToString();
				case 1:
					return skillData.cooldown.ToString();
				case 2:
					return skillData.cost.ToString();
				default:
					return "";
			}
		}
	}
}