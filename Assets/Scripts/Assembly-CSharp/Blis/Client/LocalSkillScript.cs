using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Blis.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blis.Client
{
	public abstract class LocalSkillScript
	{
		protected const float normalAttackComboTime = 3f;


		private const int ParamCount = 20;


		private static readonly LocalCharacterStat EmptyStat = new LocalCharacterStat();


		protected static readonly int FloatMoveVelocity = Animator.StringToHash("moveVelocity");


		protected static readonly int FloatMoveSpeed = Animator.StringToHash("moveSpeed");


		protected static readonly int FloatAttackSpeed = Animator.StringToHash("attackSpeed");


		protected static readonly int TriggerCanNotControl = LocalMovableCharacter.TriggerCanNotControl;


		protected static readonly int TriggerReload = Animator.StringToHash("reload");


		protected static readonly int TriggerReloadCancel = Animator.StringToHash("reloadCancel");


		protected static readonly int TriggerAttack01 = Animator.StringToHash("tAttack01");


		protected static readonly int TriggerAttack02 = Animator.StringToHash("tAttack02");


		protected static readonly int TriggerSkill01 = Animator.StringToHash("tSkill01");


		protected static readonly int TriggerSkill01_2 = Animator.StringToHash("tSkill01_2");


		protected static readonly int TriggerSkill01_3 = Animator.StringToHash("tSkill01_3");


		protected static readonly int TriggerSkill01_4 = Animator.StringToHash("tSkill01_4");


		protected static readonly int TriggerSkill02 = Animator.StringToHash("tSkill02");


		protected static readonly int TriggerSkill02_2 = Animator.StringToHash("tSkill02_2");


		protected static readonly int TriggerSkill02_3 = Animator.StringToHash("tSkill02_3");


		protected static readonly int TriggerSkill03 = Animator.StringToHash("tSkill03");


		protected static readonly int TriggerSkill03_2 = Animator.StringToHash("tSkill03_2");


		protected static readonly int TriggerSkill03_3 = Animator.StringToHash("tSkill03_3");


		protected static readonly int TriggerSkill04 = Animator.StringToHash("tSkill04");


		protected static readonly int TriggerSkill04_2 = Animator.StringToHash("tSkill04_2");


		protected static readonly int TriggerSkill04_3 = Animator.StringToHash("tSkill04_3");


		protected static readonly int TriggerSkill04_4 = Animator.StringToHash("tSkill04_4");


		protected static readonly int TriggerDance = Animator.StringToHash("tDance");


		protected static readonly int BooleanSkill01 = Animator.StringToHash("bSkill01");


		protected static readonly int BooleanSkill02 = Animator.StringToHash("bSkill02");


		protected static readonly int BooleanSkill03 = Animator.StringToHash("bSkill03");


		protected static readonly int BooleanSkill04 = Animator.StringToHash("bSkill04");


		protected static readonly int BooleanSkill04_02 = Animator.StringToHash("bSkill04_02");


		protected static readonly int BooleanMotionWait = Animator.StringToHash("bMotionWait");


		protected static readonly int BooleanDance = Animator.StringToHash("bDance");


		private static readonly string[] getTooltipParam = new string[20];


		private static readonly string[] getNextlevelTooltipParam = new string[20];


		private static readonly string[] getNextlevelTooltipValue = new string[20];


		private readonly List<Coroutine> coroutines = new List<Coroutine>();


		private LocalObject caster;


		private int casterId;


		private Collider[] colliders;


		private List<LocalCharacter> collisionObjs;


		private float concentrationStartTime;


		protected SkillData data;


		protected int evolutionLevel;


		protected float lastNormalAttackTime;


		private LocalCharacter localCharacter;


		private LocalPlayerCharacter localPlayerCharacter;


		private HashSet<SkillSlotSet> lockSkillSet;


		private HashSet<SpecialSkillId> lockSpecialSkill;


		private LocalObject self;


		private SkillId skillId;


		protected LocalObject targetObject;


		public int CasterId => casterId;


		protected LocalObject Self => self;


		protected LocalCharacter LocalCharacter => localCharacter;


		protected LocalPlayerCharacter LocalPlayerCharacter => localPlayerCharacter;


		protected Vector3 SelfForward => Self.transform.forward;


		protected Vector3 SelfRight => Self.transform.right;


		protected float ConcentrationStartTime => concentrationStartTime;


		protected CharacterStatBase SelfStat {
			get
			{
				if (!(localCharacter != null))
				{
					return EmptyStat;
				}

				return localCharacter.Stat;
			}
		}


		public SkillData SkillData => data;


		protected float SkillRange {
			get
			{
				SkillData skillData = data;
				if (skillData == null)
				{
					return 0f;
				}

				return skillData.range;
			}
		}


		protected float SkillLength {
			get
			{
				SkillData skillData = data;
				if (skillData == null)
				{
					return 0f;
				}

				return skillData.length;
			}
		}


		protected float SkillAngle {
			get
			{
				SkillData skillData = data;
				if (skillData == null)
				{
					return 0f;
				}

				return skillData.angle;
			}
		}


		public SkillId SkillId => skillId;


		protected Coroutine StartCoroutine(IEnumerator func)
		{
			Coroutine coroutine = self.StartThrowingCoroutine(func,
				delegate(Exception exception)
				{
					Log.E(string.Format("[EXCEPTION][SKILL][{0}] Message:{1}, StackTrace:{2}", skillId,
						exception.Message, exception.StackTrace));
				});
			if (coroutine == null)
			{
				return null;
			}

			coroutines.Add(coroutine);
			return coroutine;
		}


		protected void StopCoroutine(Coroutine cor)
		{
			if (cor == null)
			{
				return;
			}

			for (int i = 0; i < coroutines.Count; i++)
			{
				if (coroutines[i] == cor)
				{
					self.StopCoroutine(coroutines[i]);
					return;
				}
			}
		}


		protected void StopCoroutines()
		{
			for (int i = 0; i < coroutines.Count; i++)
			{
				if (coroutines[i] != null)
				{
					self.StopCoroutine(coroutines[i]);
				}
			}

			coroutines.Clear();
		}


		public void Init(LocalObject self, SkillData skillData)
		{
			skillId = LocalSkillScriptManager.inst.GetSkillId(this);
			data = skillData;
			if (this.self == null || this.self.ObjectId != self.ObjectId)
			{
				this.self = self;
				localCharacter = self as LocalCharacter;
				localPlayerCharacter = self as LocalPlayerCharacter;
			}
		}


		public void Setup(LocalObject caster, int evolutionLevel, LocalObject target = null)
		{
			this.caster = caster;
			casterId = caster != null ? caster.ObjectId : 0;
			targetObject = target;
			this.evolutionLevel = evolutionLevel;
		}


		public abstract void Start();


		public abstract void Play(int actionNo, LocalObject target, Vector3? targetPosition);


		public abstract void Finish(bool cancel);


		public void StartInternal()
		{
			foreach (SpecialSkillId specialSkillId in GameDB.skill.GetSkillGroupData(data.group).canNotSpecialSkillIds)
			{
				LockSkillSlot(specialSkillId);
			}
		}


		public void FinishInternal()
		{
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnSkillFinish(localPlayerCharacter, skillId);
			UnlockSkillSlotAll();
		}


		public string[] GetTooltipParams(SkillData skillData)
		{
			for (int i = 0; i < 20; i++)
			{
				getTooltipParam[i] = GetTooltipValue(skillData, i);
			}

			return getTooltipParam;
		}


		public string[] GetNextlevelTooltipParams(SkillData skillData)
		{
			for (int i = 0; i < 20; i++)
			{
				getNextlevelTooltipParam[i] = string.Empty;
			}

			for (int j = 0; j < 20; j++)
			{
				string nextLevelTooltipParam = GetNextLevelTooltipParam(j);
				if (string.IsNullOrEmpty(nextLevelTooltipParam))
				{
					break;
				}

				getNextlevelTooltipParam[j] = Ln.Get(nextLevelTooltipParam);
			}

			return getNextlevelTooltipParam;
		}


		public string[] GetNextlevelTooltipValues(SkillData skillData, SkillData nextSkillData)
		{
			if (skillData == null || nextSkillData == null)
			{
				return null;
			}

			for (int i = 0; i < 20; i++)
			{
				getNextlevelTooltipValue[i] = string.Empty;
			}

			for (int j = 0; j < 20; j++)
			{
				string nextLevelTooltipValue = GetNextLevelTooltipValue(skillData, skillData.level, j);
				string nextLevelTooltipValue2 = GetNextLevelTooltipValue(nextSkillData, nextSkillData.level, j);
				if (string.IsNullOrEmpty(nextLevelTooltipValue) || string.IsNullOrEmpty(nextLevelTooltipValue2))
				{
					break;
				}

				StringBuilder stringBuilder = GameUtil.StringBuilder;
				stringBuilder.Clear();
				stringBuilder.Append(nextLevelTooltipValue);
				stringBuilder.Append(" -> ");
				stringBuilder.Append(nextLevelTooltipValue2);
				getNextlevelTooltipValue[j] = stringBuilder.ToString();
			}

			return getNextlevelTooltipValue;
		}


		public virtual void StartConcentration()
		{
			concentrationStartTime = Time.time;
		}


		public virtual string GetTooltipValue(SkillData skillData, int index)
		{
			return "0";
		}


		public virtual string GetNextLevelTooltipParam(int index)
		{
			return "";
		}


		public virtual string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			return "";
		}


		public virtual bool GetSkillRange(ref float minRange, ref float maxRange)
		{
			return false;
		}


		public virtual float GetSkillWidth()
		{
			if (SkillData == null)
			{
				return 0f;
			}

			return SkillData.width;
		}


		public virtual float GetSkillInnerRange()
		{
			if (SkillData == null)
			{
				return 0f;
			}

			return SkillData.innerRange;
		}


		public virtual float GetSkillLength()
		{
			if (SkillData == null)
			{
				return 0f;
			}

			return SkillData.length;
		}


		public virtual float GetSkillAngle()
		{
			if (SkillData == null)
			{
				return 0f;
			}

			return SkillData.angle;
		}


		public virtual void OnUpdateWeapon() { }


		protected void PlayAnimation(LocalObject target, int id)
		{
			LocalMovableCharacter localMovableCharacter = GetLocalMovableCharacter(target);
			if (localMovableCharacter == null)
			{
				return;
			}

			localMovableCharacter.SetAnimation(id);
		}


		protected void SetAnimation(LocalObject target, int id, bool value)
		{
			LocalMovableCharacter localMovableCharacter = GetLocalMovableCharacter(target);
			if (localMovableCharacter == null)
			{
				return;
			}

			localMovableCharacter.SetAnimation(id, value);
		}


		protected void SetAnimation(LocalObject target, int id, int value)
		{
			LocalMovableCharacter localMovableCharacter = GetLocalMovableCharacter(target);
			if (localMovableCharacter == null)
			{
				return;
			}

			localMovableCharacter.SetAnimation(id, value);
		}


		protected void SetAnimation(LocalObject target, int id, float value)
		{
			LocalMovableCharacter localMovableCharacter = GetLocalMovableCharacter(target);
			if (localMovableCharacter == null)
			{
				return;
			}

			localMovableCharacter.SetAnimation(id, value);
		}


		protected void PlayWeaponAnimation(LocalObject target, WeaponMountType mountType, int id)
		{
			target.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter player)
			{
				player.SetWeaponMountAnimation(mountType, id);
			});
		}


		protected void PlayWeaponAnimation(LocalObject target, WeaponMountType mountType, int id, bool value)
		{
			target.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter player)
			{
				player.SetWeaponMountAnimation(mountType, id, value);
			});
		}


		protected void PlayWeaponAnimation(LocalObject target, WeaponMountType mountType, int id, int value)
		{
			target.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter player)
			{
				player.SetWeaponMountAnimation(mountType, id, value);
			});
		}


		protected void PlayWeaponAnimation(LocalObject target, WeaponMountType mountType, int id, float value)
		{
			target.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter player)
			{
				player.SetWeaponMountAnimation(mountType, id, value);
			});
		}


		protected void ActiveObject(LocalObject target, string childName, bool isActive)
		{
			target.ActiveObject(childName, isActive);
		}


		protected void ActiveWeaponObject(LocalObject target, WeaponMountType mountType, bool isActive)
		{
			target.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter player)
			{
				player.ActiveWeaponObject(mountType, isActive);
			});
		}


		protected float GetAnimatorFloatParameter(LocalObject target, int id)
		{
			float result = 0f;
			LocalMovableCharacter localMovableCharacter = GetLocalMovableCharacter(target);
			if (localMovableCharacter != null)
			{
				float? characterAnimatorFloat = localMovableCharacter.GetCharacterAnimatorFloat(id);
				result = characterAnimatorFloat != null ? characterAnimatorFloat.Value : 0f;
			}

			return result;
		}


		protected int GetAnimatorIntegerParameter(LocalObject target, int id)
		{
			int result = 0;
			LocalMovableCharacter localMovableCharacter = GetLocalMovableCharacter(target);
			if (localMovableCharacter != null)
			{
				int? characterAnimatorInteger = localMovableCharacter.GetCharacterAnimatorInteger(id);
				result = characterAnimatorInteger != null ? characterAnimatorInteger.Value : 0;
			}

			return result;
		}


		protected bool GetAnimatorBoolParameter(LocalObject target, int id)
		{
			bool result = false;
			LocalMovableCharacter localMovableCharacter = GetLocalMovableCharacter(target);
			if (localMovableCharacter != null)
			{
				bool? characterAnimatorBool = localMovableCharacter.GetCharacterAnimatorBool(id);
				result = characterAnimatorBool != null && characterAnimatorBool.Value;
			}

			return result;
		}


		protected void ResetAnimatorTrigger(LocalObject target, int id)
		{
			LocalMovableCharacter localMovableCharacter = GetLocalMovableCharacter(target);
			localMovableCharacter.SetCharacterAnimatorResetTrigger(id);
			LocalPlayerCharacter localPlayerCharacter = localMovableCharacter as LocalPlayerCharacter;
			if (localPlayerCharacter == null)
			{
				return;
			}

			localPlayerCharacter.WeaponMount.ResetTrigger(id);
		}


		protected void ResetWeaponLayers(LocalObject target, WeaponType exceptType = WeaponType.None)
		{
			LocalPlayerCharacter localPlayerCharacter = GetLocalPlayerCharacter(target);
			if (localPlayerCharacter != null)
			{
				localPlayerCharacter.WeaponMount.ResetWeaponLayers(localPlayerCharacter.CharacterCode, exceptType);
			}
		}


		protected void SetAnimatorLayer(LocalObject target, string layerName, float weight)
		{
			LocalPlayerCharacter localPlayerCharacter = GetLocalPlayerCharacter(target);
			if (localPlayerCharacter != null)
			{
				int characterAnimatorLayerIndex = localPlayerCharacter.GetCharacterAnimatorLayerIndex(layerName);
				if (0 <= characterAnimatorLayerIndex && characterAnimatorLayerIndex <
					localPlayerCharacter.GetCharacterAnimatorLayerCount())
				{
					localPlayerCharacter.SetCharacterAnimatorLayerWeight(characterAnimatorLayerIndex, weight);
				}
			}
		}


		protected void SetAnimatorLayer(LocalObject target, string layerName, float targetWeight, float deltaTime)
		{
			LocalPlayerCharacter localPlayerCharacter = GetLocalPlayerCharacter(target);
			if (localPlayerCharacter == null)
			{
				return;
			}

			localPlayerCharacter.SetCharacterAnimatorLayerByName(layerName, targetWeight, deltaTime);
		}


		protected void SetAnimatorLayerWeight(MasteryType equipWeaponMasteryType, CharacterMasteryData masteryData,
			string endOfWord, float weight)
		{
			SetAnimatorLayerWeight(equipWeaponMasteryType, masteryData.weapon1, endOfWord, weight);
			SetAnimatorLayerWeight(equipWeaponMasteryType, masteryData.weapon2, endOfWord, weight);
			SetAnimatorLayerWeight(equipWeaponMasteryType, masteryData.weapon3, endOfWord, weight);
			SetAnimatorLayerWeight(equipWeaponMasteryType, masteryData.weapon4, endOfWord, weight);
		}


		private void SetAnimatorLayerWeight(MasteryType equipWeaponMasteryType, MasteryType masteryType,
			string endOfWord, float weight)
		{
			if (equipWeaponMasteryType != MasteryType.None && masteryType != MasteryType.None)
			{
				SetAnimatorLayer(Self, string.Format("{0}{1}", masteryType.GetWeaponType(), endOfWord),
					equipWeaponMasteryType == masteryType ? weight : 0f);
			}
		}


		protected void SetCurrentWeaponAnimatorLayer(LocalObject target)
		{
			LocalCharacter localCharacter = GetLocalCharacter(target);
			if (localCharacter != null)
			{
				MasteryType equipWeaponMasteryType = GetEquipWeaponMasteryType(localCharacter);
				if (equipWeaponMasteryType != MasteryType.None)
				{
					SetAnimatorLayer(Self, equipWeaponMasteryType.GetWeaponType().ToString(), 1f);
				}
			}
		}


		protected void SetAnimatorCullingMode(LocalObject target, AnimatorCullingMode cullingMode)
		{
			LocalCharacter localCharacter = GetLocalCharacter(target);
			if (localCharacter != null)
			{
				localCharacter.SetAnimatorCullingMode(cullingMode);
			}
		}


		protected LocalObject GetEffectCaster()
		{
			if (caster != null)
			{
				return caster;
			}

			return Self;
		}


		protected void PlayEffectChild(List<LocalObject> targets, string effectName, string parentName = null)
		{
			foreach (LocalObject target in targets)
			{
				PlayEffectChild(target, effectName, parentName);
			}
		}


		protected GameObject PlayEffectChild(LocalObject target, string effectName, string parentName = null)
		{
			if (target == null || target.IsOutSight)
			{
				return null;
			}

			GameObject resource = GetEffectCaster().LoadEffect(effectName);
			return target.PlayLocalEffectChild(resource, parentName);
		}


		protected void PlayEffectChildManual(LocalObject target, string effectKey, string effectName,
			string parentName = null, bool includeInactive = true)
		{
			if (target == null)
			{
				return;
			}

			StopEffectChildManual(target, effectKey, true);
			if (target.IsOutSight)
			{
				return;
			}

			GameObject resource = GetEffectCaster().LoadEffect(effectName);
			target.PlayLocalEffectChildManual(effectKey, resource, parentName, includeInactive);
		}


		protected void StopEffectChildManual(LocalObject target, string effectKey, bool destroyImmediate)
		{
			if (target == null)
			{
				return;
			}

			target.StopLocalEffectChildManual(effectKey, destroyImmediate);
		}


		protected void SetNoParentEffectManual(LocalObject target, string effectKey)
		{
			if (target == null)
			{
				return;
			}

			target.SetNoParentEffectManual(effectKey);
		}


		protected void SetNoParentEffectByTag(LocalObject target, string tag)
		{
			if (target == null)
			{
				return;
			}

			target.SetNoParentEffectByTag(tag);
		}


		protected void HasEffectKey(LocalObject target, string effectKey)
		{
			if (target == null)
			{
				return;
			}

			target.HasEffectKey(effectKey);
		}


		protected void PlayEffectPoint(List<LocalObject> targets, string effectName)
		{
			foreach (LocalObject target in targets)
			{
				PlayEffectPoint(target, effectName);
			}
		}


		protected void PlayEffectPoint(List<LocalObject> targets, string effectName, Vector3 offset)
		{
			foreach (LocalObject target in targets)
			{
				PlayEffectPoint(target, effectName, offset);
			}
		}


		protected void PlayEffectPoint(List<LocalObject> targets, string effectName, string parentName)
		{
			foreach (LocalObject target in targets)
			{
				PlayEffectPoint(target, effectName, parentName);
			}
		}


		protected GameObject PlayEffectPoint(LocalObject target, string effectName)
		{
			if (target == null || target.IsOutSight)
			{
				return null;
			}

			GameObject resource = GetEffectCaster().LoadEffect(effectName);
			return target.PlayLocalEffectPoint(resource, Vector3.zero);
		}


		protected GameObject PlayEffectPoint(LocalObject target, string effectName, Vector3 offset)
		{
			if (target == null || target.IsOutSight)
			{
				return null;
			}

			GameObject resource = GetEffectCaster().LoadEffect(effectName);
			return target.PlayLocalEffectPoint(resource, offset);
		}


		protected GameObject PlayEffectPoint(LocalObject target, string effectName, string parentName)
		{
			if (target == null || target.IsOutSight)
			{
				return null;
			}

			GameObject resource = GetEffectCaster().LoadEffect(effectName);
			return target.PlayLocalEffectPoint(resource, parentName);
		}


		protected void StopEffectByTag(LocalObject target, string tag)
		{
			if (target == null)
			{
				return;
			}

			target.StopLocalEffectByTag(tag);
		}


		protected void StopSoundByTag(LocalObject target, string tag)
		{
			if (target == null)
			{
				return;
			}

			target.StopLocalSoundByTag(tag);
		}


		protected void PlaySoundPoint(List<LocalObject> targets, string soundName, int maxDistance)
		{
			foreach (LocalObject target in targets)
			{
				PlaySoundPoint(target, soundName, maxDistance);
			}
		}


		protected void PlaySoundPoint(LocalObject target, string soundName, int maxDistance = 10)
		{
			if (target == null || target.IsOutSight)
			{
				return;
			}

			AudioClip audioClip = GetEffectCaster().LoadFXSound(soundName);
			Singleton<SoundControl>.inst.PlayFXSound(audioClip, skillId.ToString(), maxDistance, target.GetPosition(),
				false);
		}


		protected void PlaySoundChildManual(List<LocalObject> targets, string soundName, int maxDistance,
			bool loop = false)
		{
			foreach (LocalObject target in targets)
			{
				PlaySoundChildManual(target, soundName, maxDistance, loop);
			}
		}


		protected void PlaySoundChildManual(LocalObject target, string soundName, int maxDistance, bool loop = false,
			bool isStopParentIsNull = false)
		{
			if (target == null || target.IsOutSight)
			{
				return;
			}

			AudioClip audioClip = GetEffectCaster().LoadFXSound(soundName);
			Singleton<SoundControl>.inst.PlayFXSoundChild(audioClip, skillId.ToString(), maxDistance, loop,
				target.transform, isStopParentIsNull);
		}


		protected void StopSoundChildManual(List<LocalObject> targets, string soundKey)
		{
			foreach (LocalObject target in targets)
			{
				StopSoundChildManual(target, soundKey);
			}
		}


		protected void StopSoundChildManual(LocalObject target, string soundName)
		{
			if (target == null)
			{
				return;
			}

			Singleton<SoundControl>.inst.StopFxSoundChild(target.transform, soundName);
		}


		protected MasteryType GetEquipWeaponMasteryType(LocalObject target)
		{
			MasteryType weaponMasteryType = MasteryType.None;
			target.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter player)
			{
				weaponMasteryType = player.GetEquipWeaponMasteryType();
			});
			return weaponMasteryType;
		}


		protected void StartPlayerConcentrating(SkillSlotSet skillSlotSet, float chargingTime, float minRange,
			float maxRange, float minAngle, float maxAngle, bool useProgress, bool showIndicator,
			Vector3? direction = null)
		{
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.StartPlayerConcentratingBySkill(skillSlotSet,
					chargingTime, minRange, maxRange, minAngle, maxAngle, useProgress, showIndicator, direction);
			}
		}


		protected void LockSkillSlots(SkillSlotIndex skillSlotIndex)
		{
			if (!SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				return;
			}

			foreach (SkillSlotSet skillSlotSet in skillSlotIndex.Index2SlotList())
			{
				LockSkillSlot(skillSlotSet);
			}
		}


		protected void UnlockSkillSlots(SkillSlotIndex skillSlotIndex)
		{
			if (!SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				return;
			}

			foreach (SkillSlotSet skillSlotSet in skillSlotIndex.Index2SlotList())
			{
				UnlockSkillSlot(skillSlotSet);
			}
		}


		protected void LockSkillSlot(SpecialSkillId specialSkillId)
		{
			if (!SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				return;
			}

			if (lockSpecialSkill == null)
			{
				lockSpecialSkill =
					new HashSet<SpecialSkillId>(SingletonComparerEnum<SpecialSkillIdComparer, SpecialSkillId>.Instance);
			}

			if (lockSpecialSkill.Contains(specialSkillId))
			{
				return;
			}

			lockSpecialSkill.Add(specialSkillId);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.LockSkillSlot(specialSkillId, true);
		}


		protected void UnlockSkillSlot(SpecialSkillId specialSkillId)
		{
			if (!SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				return;
			}

			if (lockSpecialSkill == null || !lockSpecialSkill.Contains(specialSkillId))
			{
				return;
			}

			lockSpecialSkill.Remove(specialSkillId);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.LockSkillSlot(specialSkillId, false);
		}


		protected void LockSkillSlot(SkillSlotSet skillSlotSet)
		{
			if (!SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				return;
			}

			if (lockSkillSet == null)
			{
				lockSkillSet =
					new HashSet<SkillSlotSet>(SingletonComparerEnum<SkillSlotSetComparer, SkillSlotSet>.Instance);
			}

			if (lockSkillSet.Contains(skillSlotSet))
			{
				return;
			}

			lockSkillSet.Add(skillSlotSet);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.LockSkillSlot(skillSlotSet, true);
		}


		protected void UnlockSkillSlot(SkillSlotSet skillSlotSet)
		{
			if (!SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				return;
			}

			if (lockSkillSet == null || !lockSkillSet.Contains(skillSlotSet))
			{
				return;
			}

			lockSkillSet.Remove(skillSlotSet);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.LockSkillSlot(skillSlotSet, false);
		}


		private void UnlockSkillSlotAll()
		{
			if (!SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				return;
			}

			if (lockSkillSet != null)
			{
				foreach (SkillSlotSet skillSlotSet in lockSkillSet)
				{
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.LockSkillSlot(skillSlotSet, false);
				}

				lockSkillSet.Clear();
			}

			if (lockSpecialSkill != null)
			{
				foreach (SpecialSkillId specialSkillId in lockSpecialSkill)
				{
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.LockSkillSlot(specialSkillId, false);
				}

				lockSpecialSkill.Clear();
			}
		}


		protected int GetStateStackByGroup(LocalObject target, int stateGroup, int casterId)
		{
			LocalCharacter localCharacter = GetLocalCharacter(target);
			if (localCharacter == null)
			{
				return 0;
			}

			return localCharacter.GetStateStackByGroup(stateGroup, casterId);
		}


		protected void AddBulletLine(LocalObject self, string bulletLineKey, LocalObject target, string materialName,
			float startWidth, float endWidth)
		{
			LocalMovableCharacter localMovableCharacter = GetLocalMovableCharacter(self);
			if (localMovableCharacter == null)
			{
				return;
			}

			localMovableCharacter.AddBulletLine(bulletLineKey, target, materialName, startWidth, endWidth);
		}


		protected void RemoveBulletLine(LocalObject self, string bulletLineKey)
		{
			LocalMovableCharacter localMovableCharacter = GetLocalMovableCharacter(self);
			if (localMovableCharacter == null)
			{
				return;
			}

			localMovableCharacter.RemoveBulletLine(bulletLineKey);
		}


		protected void SwitchMaterialChildManualFromDefault(LocalObject target, string childName, int index,
			string materialName)
		{
			LocalCharacter localCharacter = GetLocalCharacter(target);
			if (localCharacter == null)
			{
				return;
			}

			localCharacter.SwitchMaterialChildManualFromDefault(childName, index, materialName);
		}


		protected void SwitchMaterialChildManualFromEffect(LocalObject target, string childName, int index,
			string materialName)
		{
			LocalCharacter localCharacter = GetLocalCharacter(target);
			if (localCharacter == null)
			{
				return;
			}

			localCharacter.SwitchMaterialChildManualFromEffect(childName, index, materialName);
		}


		protected void ResetMaterialChildManual(LocalObject target, string childName, int index)
		{
			LocalCharacter localCharacter = GetLocalCharacter(target);
			if (localCharacter == null)
			{
				return;
			}

			localCharacter.ResetMaterialChildManual(childName, index);
		}


		protected void RotationLocalObject(LocalObject target, Vector3 direction)
		{
			LocalCharacter localCharacter = GetLocalCharacter(target);
			if (localCharacter == null)
			{
				return;
			}

			localCharacter.RotationLocalObject(direction);
		}


		protected void BlockAllySight(LocalObject target, BlockedSightType blockedSightType, bool block)
		{
			LocalCharacter localCharacter = GetLocalCharacter(target);
			if (localCharacter == null)
			{
				return;
			}

			localCharacter.SightAgent.BlockAllySight(blockedSightType, block);
		}


		private LocalCharacter GetLocalCharacter(LocalObject target)
		{
			LocalCharacter component;
			if (target.ObjectId == self.ObjectId)
			{
				component = localCharacter;
			}
			else
			{
				component = target.GetComponent<LocalCharacter>();
			}

			return component;
		}


		private LocalMovableCharacter GetLocalMovableCharacter(LocalObject target)
		{
			LocalMovableCharacter result;
			if (target.ObjectId == self.ObjectId)
			{
				result = localCharacter as LocalMovableCharacter;
			}
			else
			{
				result = target.GetComponent<LocalMovableCharacter>();
			}

			return result;
		}


		private LocalPlayerCharacter GetLocalPlayerCharacter(LocalObject target)
		{
			LocalPlayerCharacter result;
			if (target.ObjectId == self.ObjectId)
			{
				result = localCharacter as LocalPlayerCharacter;
			}
			else
			{
				result = target.GetComponent<LocalPlayerCharacter>();
			}

			return result;
		}


		protected List<LocalCharacter> GetCharacterWithinRange(CollisionObject3D collisionObject, bool includeAlly,
			bool includeEnemy, ObjectType[] objectTypes = null)
		{
			if (collisionObjs == null)
			{
				collisionObjs = new List<LocalCharacter>();
			}
			else
			{
				collisionObjs.Clear();
			}

			if (colliders == null)
			{
				colliders = new Collider[50];
			}

			int num = Physics.OverlapSphereNonAlloc(collisionObject.Position, collisionObject.Radius, colliders,
				GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			for (int i = 0; i < num; i++)
			{
				LocalCharacter component = colliders[i].GetComponent<LocalCharacter>();
				if (!(component == null) && component.IsAlive && self.ObjectId != component.ObjectId &&
				    !component.IsUntargetable())
				{
					bool flag = false;
					HostileType hostileType = this.localCharacter == null
						? HostileType.Enemy
						: this.localCharacter.HostileAgent.GetHostileType(component.HostileAgent);
					if (!flag && includeAlly)
					{
						flag |= hostileType.Equals(HostileType.Ally);
					}

					if (!flag && includeEnemy)
					{
						flag |= hostileType.Equals(HostileType.Enemy);
					}

					if (flag)
					{
						bool flag2 = objectTypes == null || objectTypes.Length == 0;
						if (!flag2)
						{
							foreach (ObjectType objectType in objectTypes)
							{
								if (component.ObjectType == objectType)
								{
									flag2 = true;
									break;
								}
							}
						}

						if (flag2 && collisionObject.Collision(component.GetCollisionObject()))
						{
							foreach (LocalCharacter localCharacter in collisionObjs)
							{
								int objectId = localCharacter.ObjectId;
								int objectId2 = component.ObjectId;
							}

							collisionObjs.Add(component);
						}
					}
				}
			}

			return collisionObjs;
		}


		public virtual void OnDisplaySkillIndicator(Splat indicator) { }


		public virtual void OnHideSkillIndicator(Splat indicator) { }


		public bool IsPlaying(SkillId skillId)
		{
			return !(localCharacter == null) && localCharacter.IsPlayingSkill(skillId);
		}


		public virtual Vector3 GetSkillMyCharacterPos()
		{
			return SingletonMonoBehaviour<PlayerController>.inst.GetMyCharacterPos();
		}


		public virtual UseSkillErrorCode IsEnableSkillSlot()
		{
			return UseSkillErrorCode.None;
		}


		public virtual UseSkillErrorCode IsCanUseSkill(LocalObject hitTarget, Vector3? cursorPosition)
		{
			return UseSkillErrorCode.None;
		}


		public virtual bool PickingOrderCompare(LocalObject prevTarget, LocalObject nextTarget)
		{
			return prevTarget.GetObjectOrder() > nextTarget.GetObjectOrder();
		}


		public int GetRandom(int min, int max)
		{
			return Random.Range(min, max);
		}
	}
}