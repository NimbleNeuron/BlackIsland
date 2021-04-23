using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ChiaraActive4TransformState)]
	public class LocalChiaraActive4TransformState : LocalSkillScript
	{
		private const string SetAnimatorLayerWeight_Formchange = "RapierFormchange";


		private const string SetAnimatorLayerWeight_FormchangeRun = "RapierFormchangeRun";


		private const string SetAnimatorLayerWeight_RapierFormchangeUnderRun = "RapierFormchangeUnderRun";


		private const string SetAnimatorLayerWeight_Formchange_Skill01 = "FormchangeSkill01Upper";


		private const string SetAnimatorLayerWeight_Rapier = "Rapier";


		private const string SetAnimatorLayerWeight_RapierRun = "RapierRun";


		private const string SetAnimatorLayerWeight_RapierUpper = "Skill01Upper";


		private const string SetAnimatorLayerWeight_RapierUnder = "RapierUnderRun";


		private const string Skill04_RangeEffect_Key = "Skill04_Range";


		private const string Skill04_Effect_Key = "Skill04";


		private const string Skill04_Head_Effect = "FX_BI_Chiara_Passive_04d";


		private const string Skill04_Head_Effect_Key = "Skill04_Head_Effect_Key";


		private const string Skill04_Head_Effect_point = "Bip001 Head";


		private const string Skill04_Range_Effect = "FX_BI_Chiara_Skill04";


		private const string Skill04_Range_Effect_Point = "Root";


		private const string Skill04_Effect_01 = "FX_BI_Chiara_Skill04_Ing";


		private const string Skill04_Ing_sfx = "Chiara_Skill04_Ing";


		private const string Skill04_End = "Chiara_Skill04_End";


		private const string innerFxName = "FX_BI_Chiara_Skill04_Start";


		private static readonly ObjectType[] chiraFxTargetType =
		{
			ObjectType.Dummy,
			ObjectType.Monster,
			ObjectType.PlayerCharacter,
			ObjectType.BotPlayerCharacter
		};


		private readonly Dictionary<LocalCharacter, Transform> attachEffectTargetList =
			new Dictionary<LocalCharacter, Transform>();


		private readonly List<LocalCharacter> attachEffectTargetListKeys = new List<LocalCharacter>();

		public override void Start()
		{
			SetAnimatorLayer(Self, "RapierFormchange", 1f);
			SetAnimatorLayer(Self, "RapierFormchangeRun", 1f);
			SetAnimatorLayer(Self, "RapierFormchangeUnderRun", 1f);
			SetAnimatorLayer(Self, "FormchangeSkill01Upper", 1f);
			SetAnimatorLayer(Self, "Rapier", 0f);
			SetAnimatorLayer(Self, "RapierRun", 0f);
			SetAnimatorLayer(Self, "Skill01Upper", 0f);
			SetAnimatorLayer(Self, "RapierUnderRun", 0f);
			SwitchMaterialChildManualFromDefault(Self, "Chiara_01_LOD1", 0, "Chiara_01_LOD1_Skill");
			SwitchMaterialChildManualFromDefault(Self, "Chiara_01_LOD1_Eye", 0, "Chiara_01_LOD1_Skill");
			ActiveWeaponObject(Self, WeaponMountType.Special_3, true);
			ActiveWeaponObject(Self, WeaponMountType.Special_1, false);
			PlayEffectChildManual(Self, "Skill04_Head_Effect_Key", "FX_BI_Chiara_Passive_04d", "Bip001 Head");
			PlayEffectChildManual(Self, "Skill04_Range", "FX_BI_Chiara_Skill04");
			PlayEffectChildManual(Self, "Skill04", "FX_BI_Chiara_Skill04_Ing", "Root");
			PlaySoundChildManual(Self, "Chiara_Skill04_Ing", 15, true);
			StartCoroutine(CheckRangeIn());
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopEffectChildManual(Self, "Skill04_Range", true);
			StopEffectChildManual(Self, "Skill04", true);
			StopSoundChildManual(Self, "Chiara_Skill04_Ing");
			StopEffectChildManual(Self, "Skill04_Head_Effect_Key", true);
			if (!IsPlaying(SkillId.ChiaraActive4judgment))
			{
				PlaySoundPoint(Self, "Chiara_Skill04_End", 15);
				SetAnimatorLayer(Self, "RapierFormchange", 0f);
				SetAnimatorLayer(Self, "RapierFormchangeRun", 0f);
				SetAnimatorLayer(Self, "RapierFormchangeUnderRun", 0f);
				SetAnimatorLayer(Self, "FormchangeSkill01Upper", 0f);
				StopEffectChildManual(Self, "Skill04_Range", true);
				StopEffectChildManual(Self, "Skill04_Head_Effect_Key", true);
				SetAnimatorLayer(Self, "Rapier", 1f);
				SetAnimatorLayer(Self, "RapierRun", 1f);
				SetAnimatorLayer(Self, "Skill01Upper", 1f);
				SetAnimatorLayer(Self, "RapierUnderRun", 1f);
				ActiveWeaponObject(Self, WeaponMountType.Special_3, false);
				ActiveWeaponObject(Self, WeaponMountType.Special_2, false);
				ActiveWeaponObject(Self, WeaponMountType.Special_1, true);
				SwitchMaterialChildManualFromDefault(Self, "Chiara_01_LOD1", 0, "Chiara_01_LOD1");
				SwitchMaterialChildManualFromDefault(Self, "Chiara_01_LOD1_Eye", 0, "Chiara_01_LOD1");
				PlayAnimation(Self, TriggerSkill04_4);
				SetAnimation(Self, BooleanSkill04, false);
				PlayEffectPoint(Self, "FX_BI_Chiara_Skill04_End");
			}

			StopCoroutines();
			foreach (KeyValuePair<LocalCharacter, Transform> keyValuePair in attachEffectTargetList)
			{
				if (keyValuePair.Value.gameObject != null)
				{
					Object.Destroy(keyValuePair.Value.gameObject);
				}
			}

			attachEffectTargetListKeys.Clear();
			attachEffectTargetList.Clear();
		}


		private IEnumerator CheckRangeIn()
		{
			CollisionCircle3D collision = new CollisionCircle3D(Self.GetPosition(), SkillRange);
			int index = 0;
			for (;;)
			{
				if (index == 0)
				{
					collision.UpdatePosition(Self.GetPosition());
					List<LocalCharacter> characterWithinRange =
						GetCharacterWithinRange(collision, false, true, chiraFxTargetType);
					foreach (LocalCharacter localCharacter in characterWithinRange)
					{
						bool flag = false;
						foreach (LocalCharacter localCharacter2 in attachEffectTargetListKeys)
						{
							if (localCharacter.ObjectId == localCharacter2.ObjectId)
							{
								flag = true;
								break;
							}
						}

						if (!flag)
						{
							GameObject gameObject = PlayEffectChild(localCharacter, "FX_BI_Chiara_Skill04_Start");
							if (gameObject != null)
							{
								attachEffectTargetList.Add(localCharacter, gameObject.transform);
								attachEffectTargetListKeys.Add(localCharacter);
							}
						}
					}

					for (int i = attachEffectTargetListKeys.Count - 1; i >= 0; i--)
					{
						bool flag2 = false;
						using (List<LocalCharacter>.Enumerator enumerator = characterWithinRange.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.ObjectId == attachEffectTargetListKeys[i].ObjectId)
								{
									flag2 = true;
									break;
								}
							}
						}

						if (!flag2)
						{
							GameObject gameObject2 = attachEffectTargetList[attachEffectTargetListKeys[i]].gameObject;
							if (gameObject2 != null)
							{
								Object.Destroy(gameObject2);
							}

							attachEffectTargetList.Remove(attachEffectTargetListKeys[i]);
							attachEffectTargetListKeys.RemoveAt(i);
						}
					}
				}

				int num = index + 1;
				index = num;
				index = num % 3;
				Vector3 position = Self.GetPosition();
				foreach (KeyValuePair<LocalCharacter, Transform> keyValuePair in attachEffectTargetList)
				{
					position.y = keyValuePair.Value.position.y;
					keyValuePair.Value.LookAt(position);
				}

				yield return null;
			}
		}
	}
}