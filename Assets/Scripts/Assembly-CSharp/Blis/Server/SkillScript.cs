using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class SkillScript
	{
		
		
		public SkillAgent Caster
		{
			get
			{
				return this.caster;
			}
		}

		
		
		public SkillAgent Target
		{
			get
			{
				return this.target;
			}
		}

		
		
		public bool IsConcentrating
		{
			get
			{
				return this.isConcentrating;
			}
		}

		
		
		public SkillUseInfo Info
		{
			get
			{
				return this.info;
			}
		}

		
		
		public SkillSlotSet SkillSlotSet
		{
			get
			{
				return this.info.skillSlotSet;
			}
		}

		
		
		public SkillId SkillId
		{
			get
			{
				return this.info.skillData.SkillId;
			}
		}

		
		
		public SkillId PassiveSkillId
		{
			get
			{
				return this.info.skillData.PassiveSkillId;
			}
		}

		
		
		public MasteryType MasteryType
		{
			get
			{
				return this.info.weaponSkillMastery;
			}
		}

		
		
		public int SkillCode
		{
			get
			{
				return this.info.SkillCode;
			}
		}

		
		
		public int SkillGroup
		{
			get
			{
				return this.info.SkillGroup;
			}
		}

		
		
		public int SkillLevel
		{
			get
			{
				int skillLevel;
				if (this.info.skillSlotSet != SkillSlotSet.WeaponSkill)
				{
					skillLevel = this.caster.GetSkillLevel(this.info.skillSlotSet.SlotSet2Index());
				}
				else
				{
					skillLevel = this.caster.GetSkillLevel(this.info.weaponSkillMastery);
				}
				if (skillLevel == 0)
				{
					Log.V("Skill Level 0! Caster : {0} / SlotIndex : {1}", this.caster.GetType().ToString(), this.info.skillSlotSet.ToString());
					skillLevel = this.info.SkillLevel;
				}
				return skillLevel;
			}
		}

		
		
		public int SkillEvolutionLevel
		{
			get
			{
				return this.info.skillEvolutionLevel;
			}
		}

		
		
		public float SkillRange
		{
			get
			{
				return this.info.SkillRange;
			}
		}

		
		
		public float SkillInnerRange
		{
			get
			{
				return this.info.SkillInnerRange;
			}
		}

		
		
		public float SkillWidth
		{
			get
			{
				return this.info.SkillWidth;
			}
		}

		
		
		public float SkillLenth
		{
			get
			{
				return this.info.SkillLength;
			}
		}

		
		
		public float SkillAngle
		{
			get
			{
				return this.info.SkillAngle;
			}
		}

		
		
		public SkillCostType SkillCostType
		{
			get
			{
				return this.info.SkillCostType;
			}
		}

		
		
		public float SkillCost
		{
			get
			{
				return (float)this.info.SkillCost;
			}
		}

		
		
		public float SkillCooldown
		{
			get
			{
				return this.info.SkillCooldown;
			}
		}

		
		
		public float SkillCastingTime1
		{
			get
			{
				return this.info.SkillCastingTime1;
			}
		}

		
		
		public float SkillConcentrationTime
		{
			get
			{
				return this.info.SkillConcentrationTime;
			}
		}

		
		
		public float SkillCastingTime2
		{
			get
			{
				return this.info.SkillCastingTime2;
			}
		}

		
		
		public float SkillFinishDelayTime
		{
			get
			{
				return this.info.FinishDelayTime;
			}
		}

		
		
		public SkillCastWaysType SkillCastWaysType
		{
			get
			{
				return this.info.SkillCastWaysType;
			}
		}

		
		
		public SkillCastType SkillCastType
		{
			get
			{
				return this.info.SkillCastType;
			}
		}

		
		
		public SkillTargetType SkillTargetType
		{
			get
			{
				return this.info.SkillTargetType;
			}
		}

		
		
		public SkillPlayType SkillPlayType
		{
			get
			{
				return this.info.SkillPlayType;
			}
		}

		
		
		public bool canAdditionalAction
		{
			get
			{
				return this.info.CanAdditionalAction;
			}
		}

		
		
		public virtual bool CanMoveDuringSkillPlaying
		{
			get
			{
				return this.info.CanMoveDuringSkillPlaying;
			}
		}

		
		
		public virtual bool CanStopDuringSkillPlaying
		{
			get
			{
				return true;
			}
		}

		
		
		public SequenceIncreaseType SequenceIncreaseType
		{
			get
			{
				return this.info.SequenceIncreaseType;
			}
		}

		
		
		public int StateCode
		{
			get
			{
				return this.info.StateCode;
			}
		}

		
		
		public int StateGroup
		{
			get
			{
				return this.info.StateGroup;
			}
		}

		
		
		public float StateDuration
		{
			get
			{
				return this.info.StateDuration;
			}
		}

		
		
		public StateType StateType
		{
			get
			{
				return this.info.StateType;
			}
		}

		
		public void StartAction(Action<SkillScript> action)
		{
			this.OnStart = action;
		}

		
		public void FinishAction(Action<SkillScript, bool, bool> action)
		{
			this.OnFinish = action;
		}

		
		public Coroutine StartCoroutine(IEnumerator func, bool isCancelWhenFinish = true)
		{
			Coroutine coroutine = this.mono.StartThrowingCoroutine(func, delegate(Exception exception)
			{
				Log.E(string.Format("[EXCEPTION][SKILL][{0}] Message:{1}, StackTrace:{2}", this.SkillId, exception.Message, exception.StackTrace));
			});
			if (coroutine == null)
			{
				return null;
			}
			if (isCancelWhenFinish)
			{
				this.coroutines.Add(coroutine);
			}
			return coroutine;
		}

		
		protected void StopCoroutine(Coroutine cor)
		{
			this.coroutines.Remove(cor);
			this.mono.StopCoroutine(cor);
		}

		
		private void StopCoroutines()
		{
			for (int i = 0; i < this.coroutines.Count; i++)
			{
				if (this.coroutines[i] != null)
				{
					this.mono.StopCoroutine(this.coroutines[i]);
				}
			}
			this.coroutines.Clear();
		}

		
		public void Setup(SkillUseInfo info, MonoBehaviour mono)
		{
			this.info = info;
			this.caster = info.caster;
			this.target = info.target;
			this.mono = mono;
		}

		
		public void OverwriteSkillUseInfo(SkillUseInfo info)
		{
			this.info = info;
		}

		
		public abstract IEnumerator Play(object extraData = null);

		
		public virtual SkillScriptParameterCollection GetParameters(SkillAgent target, DamageType type, DamageSubType subType, int damageId)
		{
			return SkillScript.emptySkillScriptParameterCollection;
		}

		
		protected virtual void Start()
		{
			this.isPlaying = true;
			this.startTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			this.isConcentrating = false;
			this.isDonePlayAgain = false;
			foreach (SpecialSkillId specialSkillId in GameDB.skill.GetSkillGroupData(this.info.skillData.group).canNotSpecialSkillIds)
			{
				this.LockSkillSlot(specialSkillId);
			}
			Action<SkillScript> onStart = this.OnStart;
			if (onStart == null)
			{
				return;
			}
			onStart(this);
		}

		
		protected IEnumerator FirstCastingTime()
		{
			if (0f < this.SkillCastingTime1)
			{
				yield return this.WaitForSeconds(this.SkillCastingTime1);
			}
		}

		
		protected IEnumerator SecondCastingTime()
		{
			if (0f < this.SkillCastingTime2)
			{
				yield return this.WaitForSeconds(this.SkillCastingTime2);
			}
		}

		
		protected IEnumerator FinishDelayTime()
		{
			if (0f < this.SkillFinishDelayTime)
			{
				yield return this.WaitForSeconds(this.SkillFinishDelayTime);
			}
		}

		
		public void Stop(bool cancel)
		{
			this.Finish(cancel);
		}

		
		protected virtual void Finish(bool cancel = false)
		{
			if (!this.isPlaying)
			{
				return;
			}
			this.StopCoroutines();
			this.FinishConcentration(cancel);
			foreach (SpecialSkillId specialSkillId in GameDB.skill.GetSkillGroupData(this.info.skillData.group).canNotSpecialSkillIds)
			{
				this.UnlockSkillSlot(specialSkillId);
			}
			if (this.reserveAutomaticalUnlock)
			{
				this.caster.LockRotation(false);
				this.reserveAutomaticalUnlock = false;
			}
			Action<SkillScript, bool, bool> onFinish = this.OnFinish;
			if (onFinish != null)
			{
				onFinish(this, this.IsChangedSkillSequence(), cancel);
			}
			this.isPlaying = false;
		}

		
		public virtual void OnUpgradePassiveSkill()
		{
		}

		
		protected virtual bool IsChangedSkillSequence()
		{
			return this.SequenceIncreaseType == SequenceIncreaseType.Always;
		}

		
		protected bool IsEvolution()
		{
			return 0 < this.SkillEvolutionLevel;
		}

		
		protected bool Evolution(int evolveLevel)
		{
			return evolveLevel > 0 && evolveLevel <= this.SkillEvolutionLevel;
		}

		
		protected WaitForFrameUpdate WaitForFrame()
		{
			return this.waitFrame.Frame(1);
		}

		
		protected WaitForFrameUpdate WaitForFrames(int frame)
		{
			return this.waitFrame.Frame(frame);
		}

		
		protected WaitForFrameUpdate WaitForSeconds(float seconds)
		{
			return this.waitFrame.Seconds(seconds);
		}

		
		protected WaitForFrameUpdate WaitForSecondsByAttackSpeed(SkillAgent target, float seconds)
		{
			return this.waitFrame.Seconds(seconds * target.Stat.AttackDelay);
		}

		
		protected void StartConcentration()
		{
			if (this.isConcentrating)
			{
				return;
			}
			if (this.SkillCastType != SkillCastType.Concentration)
			{
				return;
			}
			this.isConcentrating = true;
			this.Caster.StartConcentration(this.info.skillData);
		}

		
		protected void FinishConcentration(bool cancel)
		{
			if (!this.isConcentrating)
			{
				return;
			}
			if (this.SkillCastType != SkillCastType.Concentration)
			{
				return;
			}
			this.isConcentrating = false;
			this.Caster.FinishConcentration(this.info.skillSlotSet, this.info.weaponSkillMastery, this.info.skillData, cancel);
		}

		
		public void PlayAgain(WorldCharacter hitTarget)
		{
			if (this.isDonePlayAgain)
			{
				return;
			}
			this.isDonePlayAgain = true;
			this.OnPlayAgain(hitTarget);
		}

		
		public void PlayAgain(Vector3 hitPoint)
		{
			if (this.isDonePlayAgain)
			{
				return;
			}
			this.isDonePlayAgain = true;
			this.OnPlayAgain(hitPoint);
		}

		
		protected virtual void OnPlayAgain(WorldCharacter hitTarget)
		{
		}

		
		protected virtual void OnPlayAgain(Vector3 hitPoint)
		{
		}

		
		public virtual void OnSelect(WorldCharacter hitTarget, Vector3 hitPoint, Vector3 releasePoint)
		{
		}

		
		public virtual bool UseOnMove()
		{
			return false;
		}

		
		public virtual void OnMove(Vector3 hitPoint)
		{
		}

		
		public virtual bool UseOnTargetOn()
		{
			return false;
		}

		
		public virtual void OnTargetOn(WorldObject target)
		{
		}

		
		protected bool IsReadySkill(SkillAgent target, SkillSlotSet skillSlotSet)
		{
			return target.IsReadySkill(skillSlotSet);
		}

		
		protected void PlaySkillAction(SkillAgent actor, int actionNo, SkillAgent target = null, Vector3? targetPosition = null)
		{
			int targetId = (target != null) ? target.ObjectId : 0;
			actor.PlaySkillAction(this.info.skillData.SkillId, actionNo, targetId, (targetPosition != null) ? new BlisVector(targetPosition.Value) : null);
		}

		
		protected void PlaySkillAction(SkillAgent actor, int actionNo, List<SkillActionTarget> targets)
		{
			actor.PlaySkillAction(this.info.skillData.SkillId, actionNo, targets);
		}

		
		protected void PlaySkillAction(SkillAgent actor, SkillId skillId, int actionNo, int targetId = 0, BlisVector targetPosition = null)
		{
			actor.PlaySkillAction(skillId, actionNo, targetId, targetPosition);
		}

		
		protected void PlayPassiveSkill(SkillUseInfo skillUseInfo)
		{
			this.Caster.PlayPassiveSkill(skillUseInfo, 0, 0, null);
		}

		
		protected void PlayPassiveSkill(SkillUseInfo skillUseInfo, int actionNo, int targetId, BlisVector targetPosition = null)
		{
			this.Caster.PlayPassiveSkill(skillUseInfo, actionNo, targetId, targetPosition);
		}

		
		protected void DirectDamageTo(IEnumerable<SkillAgent> targets, DamageType type, DamageSubType subType, int damageId, SkillScriptParameterCollection parameters, int effectAndSoundCode, bool isCheckAlly = true, int minRemain = 0, float damageMasteryModifier = 1f, bool targetInCombat = true)
		{
			this.casterInfo.SetAttackerStat(this.Caster.Owner, this.casterCachedStat);
			foreach (SkillAgent skillAgent in targets)
			{
				if (type == DamageType.Normal || !skillAgent.ObjectType.IsSummonObject())
				{
					this.Caster.DirectDamageTo(skillAgent, this.casterInfo, type, subType, this.info.SkillCode, damageId, parameters, this.SkillSlotSet, minRemain, damageMasteryModifier, effectAndSoundCode, isCheckAlly, targetInCombat);
				}
			}
		}

		
		protected void DirectDamageTo(SkillAgent target, DamageType type, DamageSubType subType, int damageId, SkillScriptParameterCollection parameters, int effectAndSoundCode, bool isCheckAlly = true, int minRemain = 0, float damageMasteryModifier = 1f, bool targetInCombat = true)
		{
			if (type != DamageType.Normal && target.ObjectType.IsSummonObject())
			{
				return;
			}
			this.casterInfo.SetAttackerStat(this.Caster.Owner, this.casterCachedStat);
			this.Caster.DirectDamageTo(target, this.casterInfo, type, subType, this.info.SkillCode, damageId, parameters, this.SkillSlotSet, minRemain, damageMasteryModifier, effectAndSoundCode, isCheckAlly, targetInCombat);
		}

		
		protected void DamageTo(List<SkillAgent> targets, DamageType type, DamageSubType subType, int damageId, SkillScriptParameterCollection parameters, int effectAndSoundCode, bool isCheckAlly = true, int minRemain = 0, float damageMasteryModifier = 1f, bool targetInCombat = true)
		{
			if (targets == null || targets.Count == 0)
			{
				return;
			}
			SkillScriptParameterCollection skillScriptParameterCollection = SkillScriptParameterCollection.Create();
			foreach (SkillAgent skillAgent in targets)
			{
				skillScriptParameterCollection.Clear();
				skillScriptParameterCollection.Merge(parameters);
				this.DamageTo(skillAgent, type, subType, damageId, skillScriptParameterCollection, effectAndSoundCode, isCheckAlly, minRemain, damageMasteryModifier, targetInCombat);
			}
		}

		
		protected DamageInfo DamageTo(SkillAgent target, DamageType type, DamageSubType subType, int damageId, SkillScriptParameterCollection parameters, int effectAndSoundCode, bool isCheckAlly = true, int minRemain = 0, float damageMasteryModifier = 1f, bool targetInCombat = true)
		{
			if (type != DamageType.Normal && target.ObjectType.IsSummonObject())
			{
				return null;
			}
			this.casterInfo.SetAttackerStat(this.Caster.Owner, this.casterCachedStat);
			Vector3 damageDirection = GameUtil.Direction(this.caster.Position, target.Position);
			return this.Caster.DamageTo(target, this.casterInfo, type, subType, this.info.SkillCode, damageId, parameters, this.SkillSlotSet, this.caster.Position, damageDirection, isCheckAlly, minRemain, damageMasteryModifier, effectAndSoundCode, targetInCombat);
		}

		
		protected void DamageTo(List<SkillAgent> targets, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, Vector3 damagePoint, Vector3 damageDirection, int effectAndSoundCode, bool isCheckAlly = true, int minRemain = 0, float damageMasteryModifier = 1f, bool targetInCombat = true)
		{
			if (targets == null || targets.Count == 0)
			{
				return;
			}
			AttackerInfo attackerInfo2 = attackerInfo;
			if (!skillSlotSet.IsNormalAttack())
			{
				this.casterInfo.SetAttackerStat(this.Caster.Owner, this.casterCachedStat);
				attackerInfo2 = this.casterInfo;
			}
			SkillScriptParameterCollection skillScriptParameterCollection = SkillScriptParameterCollection.Create();
			foreach (SkillAgent skillAgent in targets)
			{
				skillScriptParameterCollection.Clear();
				skillScriptParameterCollection.Merge(parameters);
				this.DamageTo(skillAgent, attackerInfo2, type, subType, damageId, skillScriptParameterCollection, skillSlotSet, damagePoint, damageDirection, effectAndSoundCode, isCheckAlly, minRemain, damageMasteryModifier, targetInCombat);
			}
		}

		
		protected DamageInfo DamageTo(SkillAgent target, AttackerInfo attackerInfo, DamageType type, DamageSubType subType, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, Vector3 damagePoint, Vector3 damageDirection, int effectAndSoundCode, bool isCheckAlly = true, int minRemain = 0, float damageMasteryModifier = 1f, bool targetInCombat = true)
		{
			if (type != DamageType.Normal && target.ObjectType.IsSummonObject())
			{
				return null;
			}
			AttackerInfo attackerInfo2 = attackerInfo;
			if (!skillSlotSet.IsNormalAttack())
			{
				this.casterInfo.SetAttackerStat(this.Caster.Owner, this.casterCachedStat);
				attackerInfo2 = this.casterInfo;
			}
			return this.Caster.DamageTo(target, attackerInfo2, type, subType, this.info.SkillCode, damageId, parameters, skillSlotSet, damagePoint, damageDirection, isCheckAlly, minRemain, damageMasteryModifier, effectAndSoundCode, targetInCombat);
		}

		
		protected DamageInfo DamageToSummon(WorldSummonBase target, DamageType type, DamageSubType subType, int damageId, SkillScriptParameterCollection parameters, int effectAndSoundCode, bool isCheckAlly = true, int minRemain = 0, float damageMasteryModifier = 1f)
		{
			if (target.SummonData.isInvincibility)
			{
				return null;
			}
			this.casterInfo.SetAttackerStat(this.Caster.Owner, this.casterCachedStat);
			return this.Caster.DamageTo(target.SkillAgent, this.casterInfo, type, subType, this.info.SkillCode, damageId, parameters, this.SkillSlotSet, target.GetPosition(), GameUtil.Direction(this.caster.Position, target.GetPosition()), isCheckAlly, minRemain, damageMasteryModifier, effectAndSoundCode, true);
		}

		
		protected void DamageToSummon(WorldSummonBase target, DamageType type, DamageSubType subType, int damageId, SkillScriptParameterCollection parameters, SkillSlotSet skillSlotSet, Vector3 damagePoint, Vector3 damageDirection, int effectAndSoundCode, bool isCheckAlly, int minRemain, float damageMasteryModifier)
		{
			if (target.SummonData.isInvincibility)
			{
				return;
			}
			this.casterInfo.SetAttackerStat(this.Caster.Owner, this.casterCachedStat);
			this.Caster.DamageTo(target.SkillAgent, this.casterInfo, type, subType, this.info.SkillCode, damageId, parameters, skillSlotSet, damagePoint, damageDirection, isCheckAlly, minRemain, damageMasteryModifier, effectAndSoundCode, true);
		}

		
		protected void HealTo(SkillAgent target, int hpBaseAmount, float hpCoefficient, int hpFixAmount, int spBaseAmount, float spCoefficient, int spFixAmount, bool showUI, int effectAndSoundCode)
		{
			this.Caster.HealTo(target, hpBaseAmount, hpCoefficient, hpFixAmount, spBaseAmount, spCoefficient, spFixAmount, showUI, effectAndSoundCode);
		}

		
		protected void HpHealTo(SkillAgent target, int baseAmount, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode)
		{
			this.Caster.HpHealTo(target, baseAmount, coefficient, fixAmount, showUI, effectAndSoundCode);
		}

		
		protected void LostHpHealTo(SkillAgent target, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode)
		{
			this.Caster.LostHpHealTo(target, coefficient, fixAmount, showUI, effectAndSoundCode);
		}

		
		protected void SpHealTo(SkillAgent target, int baseAmount, float coefficient, int fixAmount, bool showUI, int effectAndSoundCode)
		{
			this.Caster.SpHealTo(target, baseAmount, coefficient, fixAmount, showUI, effectAndSoundCode);
		}

		
		protected void ExtraPointModifyTo(SkillAgent target, int deltaAmount)
		{
			this.Caster.ExtraPointModifyTo(target, deltaAmount);
		}

		
		protected void ModifySkillCooldown(SkillAgent target, SkillSlotSet skillSlotSetFlag, float time)
		{
			target.ModifySkillCooldown(skillSlotSetFlag, time);
		}

		
		protected MasteryType GetEquipWeaponMasteryType(SkillAgent target)
		{
			return target.GetEquipWeaponMasteryType();
		}

		
		protected List<SkillAgent> GetCharacters(CollisionObject3D collisionObject, bool includeAlly, bool includeEnemy)
		{
			return this.GetAgentWithinRange(collisionObject, includeAlly, includeEnemy, SkillScript.getCharactersObjectTypes);
		}

		
		protected List<SkillAgent> GetEnemyCharacters(CollisionObject3D collisionObject)
		{
			return this.GetAgentWithinRange(collisionObject, false, true, SkillScript.getEnemyCharactersObjectTypes);
		}

		
		protected List<SkillAgent> GetAllies(CollisionObject3D collisionObject)
		{
			return this.GetAgentWithinRange(collisionObject, true, false, null);
		}

		
		protected List<SkillAgent> GetEnemies(CollisionObject3D collisionObject)
		{
			return this.GetAgentWithinRange(collisionObject, false, true, null);
		}

		
		private int GetOverlapNonAllocSize(CollisionObject3D collisionObject, ref Collider[] colliders)
		{
			CollisionBox3D collisionBox3D;
			if ((collisionBox3D = (collisionObject as CollisionBox3D)) != null)
			{
				Vector3 halfExtents = new Vector3(collisionBox3D.Width * 0.5f, 1f, collisionBox3D.Depth * 0.5f);
				return Physics.OverlapBoxNonAlloc(collisionBox3D.Position, halfExtents, colliders, GameUtil.LookRotation(collisionBox3D.Normalized), GameConstants.LayerMask.WORLD_OBJECT_LAYER);
			}
			return Physics.OverlapSphereNonAlloc(collisionObject.Position, collisionObject.Radius, colliders, GameConstants.LayerMask.WORLD_OBJECT_LAYER);
		}

		
		private List<SkillAgent> GetAgentWithinRange(CollisionObject3D collisionObject, bool includeAlly, bool includeEnemy, ObjectType[] objectTypes)
		{
			if (this.collisionAgents == null)
			{
				this.collisionAgents = new List<SkillAgent>();
			}
			else
			{
				this.collisionAgents.Clear();
			}
			if (this.colliders == null)
			{
				this.colliders = new Collider[100];
			}
			int overlapNonAllocSize = this.GetOverlapNonAllocSize(collisionObject, ref this.colliders);
			while (this.colliders.Length <= overlapNonAllocSize)
			{
				this.colliders = new Collider[this.colliders.Length + 100];
				overlapNonAllocSize = this.GetOverlapNonAllocSize(collisionObject, ref this.colliders);
			}
			for (int i = 0; i < overlapNonAllocSize; i++)
			{
				WorldCharacter component = this.colliders[i].GetComponent<WorldCharacter>();
				if (!(component == null) && component.IsAlive)
				{
					if (component.IsDyingCondition)
					{
						SkillGroupData skillGroupData = GameDB.skill.GetSkillGroupData(this.info.SkillGroup);
						if (skillGroupData != null && skillGroupData.impossibleDyingConditionTarget)
						{
							goto IL_1AC;
						}
					}
					if (this.Caster.ObjectId != component.SkillAgent.ObjectId && !component.IsUntargetable())
					{
						bool flag = false;
						HostileType hostileType = this.Caster.GetHostileType(component);
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
							if (flag2 && collisionObject.Collision(component.SkillAgent.CollisionObject))
							{
								this.collisionAgents.Add(component.SkillAgent);
							}
						}
					}
				}
				IL_1AC:;
			}
			return this.collisionAgents;
		}

		
		protected ProjectileProperty PopProjectileProperty(SkillAgent owner, int dataCode)
		{
			SkillUseInfo skillUseInfo = SkillUseInfo.Create(this.info);
			skillUseInfo.injected = true;
			return MonoBehaviourInstance<GameService>.inst.World.PopProjectileProperty(owner, dataCode, skillUseInfo);
		}

		
		protected WorldProjectile LaunchProjectile(ProjectileProperty projectileProperty)
		{
			return this.LaunchProjectile(projectileProperty, this.Caster.Position);
		}

		
		protected WorldProjectile LaunchProjectile(ProjectileProperty projectileProperty, Vector3 spawnPosition)
		{
			if (projectileProperty.ProjectileData.type == ProjectileType.Direction && projectileProperty.ProjectileData.serverInterpolationPosition != 0f)
			{
				spawnPosition += projectileProperty.ProjectileData.serverInterpolationPosition * projectileProperty.Direction;
			}
			WorldProjectile worldProjectile = MonoBehaviourInstance<GameService>.inst.World.Spawn<WorldProjectile>(spawnPosition);
			worldProjectile.Init(projectileProperty);
			MonoBehaviourInstance<GameServer>.inst.EnqueueCommand(new CmdSpawn
			{
				snapshot = worldProjectile.CreateSnapshotWrapper()
			});
			this.Caster.ConsumeBullet(projectileProperty.ProjectileData);
			return worldProjectile;
		}

		
		protected HookLineProperty CreateHookLineProjectile(SkillAgent owner, int projectileCode, int hookLineCode)
		{
			SkillUseInfo skillUseInfo = SkillUseInfo.Create(this.info);
			skillUseInfo.injected = true;
			return new HookLineProperty(owner, projectileCode, hookLineCode, skillUseInfo);
		}

		
		protected WorldHookLineProjectile LaunchHookLineProjectile(HookLineProperty hookLineProperty)
		{
			Vector3 vector = hookLineProperty.LinkFromCharacter.GetPosition();
			if (hookLineProperty.ProjectileData.type == ProjectileType.Direction && hookLineProperty.ProjectileData.serverInterpolationPosition != 0f)
			{
				vector += hookLineProperty.ProjectileData.serverInterpolationPosition * hookLineProperty.Direction;
			}
			WorldHookLineProjectile worldHookLineProjectile = MonoBehaviourInstance<GameService>.inst.World.Spawn<WorldHookLineProjectile>(vector);
			worldHookLineProjectile.Init(hookLineProperty);
			MonoBehaviourInstance<GameServer>.inst.EnqueueCommand(new CmdSpawn
			{
				snapshot = worldHookLineProjectile.CreateSnapshotWrapper()
			});
			this.Caster.ConsumeBullet(hookLineProperty.ProjectileData);
			return worldHookLineProjectile;
		}

		
		protected bool CanActionDuringSkillPlaying()
		{
			return this.Caster.CanActionDuringSkillPlaying();
		}

		
		protected bool IsEnoughBullet()
		{
			return this.Caster.IsEnoughBullet();
		}

		
		protected void CheckReload()
		{
			if (this.IsEnoughBullet())
			{
				return;
			}
			this.Caster.GunReload(true);
		}

		
		protected bool IsLockSkillSlot(SkillSlotSet skillSlotSet)
		{
			return this.Caster.IsLockSkillSlot(skillSlotSet);
		}

		
		protected void LockSkillSlots(SkillSlotIndex skillSlotIndex)
		{
			foreach (SkillSlotSet skillSlotSet in skillSlotIndex.Index2SlotList())
			{
				this.LockSkillSlot(skillSlotSet);
			}
		}

		
		protected void UnlockSkillSlots(SkillSlotIndex skillSlotIndex)
		{
			foreach (SkillSlotSet skillSlotSet in skillSlotIndex.Index2SlotList())
			{
				this.UnlockSkillSlot(skillSlotSet);
			}
		}

		
		protected void LockSkillSlot(SkillSlotSet skillSlotSet)
		{
			this.Caster.LockSkillSlot(skillSlotSet, true);
		}

		
		protected void UnlockSkillSlot(SkillSlotSet skillSlotSet)
		{
			this.Caster.LockSkillSlot(skillSlotSet, false);
		}

		
		protected void LockSkillSlot(SpecialSkillId specialSkillId)
		{
			this.Caster.LockSkillSlot(specialSkillId, true);
		}

		
		protected void UnlockSkillSlot(SpecialSkillId specialSkillId)
		{
			this.Caster.LockSkillSlot(specialSkillId, false);
		}

		
		protected void LockSkillSlotWithPacket(SkillSlotSet skillSlotSetFlag, bool isLock)
		{
			this.Caster.LockSkillSlotsWithPacket(skillSlotSetFlag, isLock);
		}

		
		protected bool SwitchSkillSet(SkillSlotIndex skillSlotIndex, SkillSlotSet skillSlotSet)
		{
			return this.Caster.SwitchSkillSet(skillSlotIndex, skillSlotSet);
		}

		
		protected void LookAtTarget(SkillAgent rotateTarget, SkillAgent lookToTarget, float duration = 0f, bool isServerRotateInstant = false)
		{
			Vector3 direction = GameUtil.DirectionOnPlane(rotateTarget.Position, lookToTarget.Position);
			this.LookAtDirection(rotateTarget, direction, duration, isServerRotateInstant);
		}

		
		protected void LookAtDirection(SkillAgent rotateTarget, Vector3 direction, float duration = 0f, bool isServerRotateInstant = false)
		{
			direction.y = 0f;
			direction = direction.normalized;
			if (this.SkillSlotSet.SlotSet2Index() == SkillSlotIndex.Attack)
			{
				duration /= this.Caster.Stat.AttackSpeed;
				isServerRotateInstant = true;
			}
			rotateTarget.LookAt(direction, duration, isServerRotateInstant);
		}

		
		protected void LookAtDirectionWithSpeed(SkillAgent rotateTarget, Vector3 direction, float angularSpeed = 0f, bool isServerRotateInstant = false)
		{
			float duration = GameUtil.GetDirectionAngle(rotateTarget.Forward, direction) / angularSpeed;
			this.LookAtDirection(rotateTarget, direction, duration, isServerRotateInstant);
		}

		
		protected void LookAtPosition(SkillAgent rotateTarget, Vector3 position, float angularSpeed = 0f, bool isServerRotateInstant = false)
		{
			Vector3 direction = GameUtil.DirectionOnPlane(rotateTarget.Position, position);
			this.LookAtDirection(rotateTarget, direction, angularSpeed, isServerRotateInstant);
		}

		
		protected void CasterLockRotation(bool isAutomaticalUnlock)
		{
			this.caster.LockRotation(true);
			this.reserveAutomaticalUnlock = isAutomaticalUnlock;
		}

		
		protected void CasterUnlockLotation()
		{
			this.caster.LockRotation(false);
			this.reserveAutomaticalUnlock = false;
		}

		
		protected T CreateState<T>(SkillAgent target, int stateCode, int initStack = 0, float? duration = null) where T : CharacterState
		{
			return this.CreateState(target, stateCode, initStack, duration) as T;
		}

		
		protected CharacterState CreateState(SkillAgent target, int stateCode, int initStack = 0, float? duration = null)
		{
			SkillUseInfo skillUseInfo = SkillUseInfo.Create(this.Info);
			skillUseInfo.stateData = GameDB.characterState.GetData(stateCode);
			skillUseInfo.injected = true;
			return CharacterState.Create(skillUseInfo, target.Character, this.Caster.Owner, initStack, duration);
		}

		
		protected void AddState(IEnumerable<SkillAgent> targets, int stateCode, float? duration = null)
		{
			foreach (SkillAgent skillAgent in targets)
			{
				this.AddState(skillAgent, stateCode, duration);
			}
		}

		
		protected void AddState(SkillAgent target, int stateCode, float? duration = null)
		{
			this.AddState(target, stateCode, 0, duration);
		}

		
		protected void AddState(SkillAgent customCaster, SkillAgent target, int stateCode, int stack = 0, float? duration = null)
		{
			CharacterStateData data = GameDB.characterState.GetData(stateCode);
			CharacterState state = CharacterState.Create(SkillUseInfo.Create(customCaster, target, this.info.skillData, this.info.skillSlotSet, this.info.weaponSkillMastery, 0, Vector3.zero, Vector3.zero, data, true), target.Character, customCaster.Owner, stack, duration);
			this.AddState(target, state, customCaster);
		}

		
		protected void AddState(WorldSummonBase summonCaster, SkillAgent target, int stateCode, int stack = 0, float? duration = null)
		{
			CharacterStateData data = GameDB.characterState.GetData(stateCode);
			CharacterState state = CharacterState.Create(SkillUseInfo.Create(summonCaster.SkillAgent, target, this.info.skillData, this.info.skillSlotSet, this.info.weaponSkillMastery, 0, this.info.cursorPosition, this.info.releasePosition, data, true), target.Character, summonCaster, stack, duration);
			this.AddState(target, state, summonCaster.SkillAgent);
		}

		
		protected void AddState(SkillAgent target, int stateCode, int stack, float? duration = null)
		{
			CharacterState state = this.CreateState(target, stateCode, stack, duration);
			this.AddState(target, state);
		}

		
		protected void AddState(SkillAgent target, CharacterState state)
		{
			this.AddState(target, state, null);
		}

		
		private void AddState(SkillAgent target, CharacterState state, SkillAgent customCaster)
		{
			target.AddState(state, (customCaster != null) ? customCaster.ObjectId : this.Caster.ObjectId);
		}

		
		protected int GetSkillSequence(SkillSlotSet skillSlotSet)
		{
			return this.Caster.GetSkillSequence(skillSlotSet);
		}

		
		public virtual UseSkillErrorCode IsCanUseSkill(WorldCharacter hitTarget, Vector3? cursorPosition, WorldMovableCharacter caster)
		{
			return UseSkillErrorCode.None;
		}

		
		public virtual void UpgradeSkill()
		{
		}

		
		protected virtual Vector3 GetSkillSubjectPosition()
		{
			return this.Caster.Position;
		}

		
		protected virtual void GetSkillRange(ref float minRange, ref float maxRange)
		{
		}

		
		protected Vector3 GetSkillPoint()
		{
			return this.GetSkillPoint(this.info.cursorPosition);
		}

		
		protected Vector3 GetSkillPoint(Vector3 hitPoint)
		{
			Vector3 vector = hitPoint;
			switch (this.info.skillData.CastWaysType)
			{
			case SkillCastWaysType.Directional:
			{
				Vector3 skillSubjectPosition = this.GetSkillSubjectPosition();
				Vector3 a = GameUtil.Direction(skillSubjectPosition, vector);
				SkillScript.plane.distance = -Vector3.Dot(SkillScript.plane.normal, skillSubjectPosition);
				Ray ray = new Ray(new Vector3(hitPoint.x, hitPoint.y + 100f, hitPoint.z), Vector3.down);
				float distance;
				if (SkillScript.plane.Raycast(ray, out distance))
				{
					a = GameUtil.DirectionOnPlane(skillSubjectPosition, ray.GetPoint(distance));
					vector = ray.GetPoint(distance);
				}
				float num = 0f;
				float num2 = this.SkillRange;
				this.GetSkillRange(ref num, ref num2);
				num2 = Mathf.Max(num2, 0.1f);
				Vector3 origin = skillSubjectPosition + a * num2;
				origin.y = 100f;
				RaycastHit raycastHit;
				Vector3 vector2;
				if (Physics.Raycast(new Ray(origin, Vector3.down), out raycastHit, 200f, GameConstants.LayerMask.GROUND_LAYER, QueryTriggerInteraction.Collide) && MoveAgent.CanStandToPosition(raycastHit.point, 2147483640, out vector2))
				{
					vector = vector2;
				}
				break;
			}
			case SkillCastWaysType.PickPoint:
			{
				vector = hitPoint;
				Vector3 vector3;
				if (MoveAgent.CanStandToPosition(hitPoint, 2147483640, out vector3))
				{
					vector = vector3;
				}
				else
				{
					Vector3 skillSubjectPosition2 = this.GetSkillSubjectPosition();
					SkillScript.plane.distance = -Vector3.Dot(SkillScript.plane.normal, skillSubjectPosition2);
					Ray ray2 = new Ray(new Vector3(hitPoint.x, hitPoint.y + 100f, hitPoint.z), Vector3.down);
					float distance2;
					if (SkillScript.plane.Raycast(ray2, out distance2))
					{
						vector = ray2.GetPoint(distance2);
					}
				}
				break;
			}
			case SkillCastWaysType.PickPointInArea:
			{
				Vector3 skillSubjectPosition3 = this.GetSkillSubjectPosition();
				SkillScript.plane.distance = -Vector3.Dot(SkillScript.plane.normal, skillSubjectPosition3);
				Ray ray3 = new Ray(new Vector3(hitPoint.x, hitPoint.y + 100f, hitPoint.z), Vector3.down);
				float distance3;
				if (SkillScript.plane.Raycast(ray3, out distance3))
				{
					vector = ray3.GetPoint(distance3);
				}
				float num3 = Mathf.Max(this.SkillRange, 0.1f);
				if (num3 < GameUtil.DistanceOnPlane(skillSubjectPosition3, vector))
				{
					vector = skillSubjectPosition3 + GameUtil.DirectionOnPlane(skillSubjectPosition3, vector) * num3;
				}
				Vector3 origin2 = vector;
				origin2.y = 100f;
				RaycastHit raycastHit2;
				Vector3 vector4;
				if (Physics.Raycast(new Ray(origin2, Vector3.down), out raycastHit2, 200f, GameConstants.LayerMask.GROUND_LAYER, QueryTriggerInteraction.Collide) && MoveAgent.CanStandToPosition(raycastHit2.point, 2147483640, out vector4))
				{
					vector = vector4;
				}
				break;
			}
			}
			return vector;
		}

		
		private MonoBehaviour mono;

		
		private SkillAgent caster;

		
		protected readonly AttackerInfo casterInfo = new AttackerInfo();

		
		protected readonly SimpleCharacterStat casterCachedStat = new SimpleCharacterStat();

		
		private SkillAgent target;

		
		protected bool isPlaying;

		
		protected float startTime;

		
		private bool isConcentrating;

		
		private bool isDonePlayAgain;

		
		protected SkillUseInfo info;

		
		private Action<SkillScript> OnStart;

		
		private Action<SkillScript, bool, bool> OnFinish;

		
		private readonly List<Coroutine> coroutines = new List<Coroutine>();

		
		private const float SecondsPerFrame = 0.0333f;

		
		private static readonly SkillScriptParameterCollection emptySkillScriptParameterCollection = SkillScriptParameterCollection.Create();

		
		private readonly WaitForFrameUpdate waitFrame = new WaitForFrameUpdate();

		
		private static readonly ObjectType[] getCharactersObjectTypes = new ObjectType[]
		{
			ObjectType.PlayerCharacter,
			ObjectType.Monster,
			ObjectType.BotPlayerCharacter,
			ObjectType.Dummy
		};

		
		private static readonly ObjectType[] getEnemyCharactersObjectTypes = new ObjectType[]
		{
			ObjectType.PlayerCharacter,
			ObjectType.Monster,
			ObjectType.BotPlayerCharacter,
			ObjectType.Dummy
		};

		
		private List<SkillAgent> collisionAgents;

		
		private const int cacheSize = 100;

		
		private Collider[] colliders;

		
		private bool reserveAutomaticalUnlock;

		
		private static Plane plane = new Plane(Vector3.up, Vector3.zero);
	}
}
