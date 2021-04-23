using System.Collections.Generic;
using BIOutline;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[RequireComponent(typeof(CharacterFloatingUI))]
	public abstract class LocalMovableCharacter : LocalCharacter, ILocalMoveAgentOwner
	{
		public const float MeterPerSecond = 3.5f;
		public const string AnimFloatMoveVelocity = "moveVelocity";
		public const string AnimFloatMoveSpeed = "moveSpeed";
		public const string AnimFloatAttackSpeed = "attackSpeed";
		private const string AnimTriggerCanNotControl = "tCanNotControl";
		public const string AnimTriggerReload = "reload";
		public const string AnimTriggerReloadCancel = "reloadCancel";
		public const string AnimBoolCastingAction = "bCastingAction";
		public static readonly int FloatMoveVelocity = Animator.StringToHash("moveVelocity");
		public static readonly int FloatMoveSpeed = Animator.StringToHash("moveSpeed");
		public static readonly int FloatAttackSpeed = Animator.StringToHash("attackSpeed");
		public static readonly int TriggerCanNotControl = Animator.StringToHash("tCanNotControl");
		public static readonly int TriggerReload = Animator.StringToHash("reload");
		public static readonly int BoolCastingAction = Animator.StringToHash("bCastingAction");
		private readonly Dictionary<string, BulletLine> bulletLines = new Dictionary<string, BulletLine>();
		private readonly List<LocalProjectile> ownProjectiles = new List<LocalProjectile>();
		private SkillSlotSet concentrateSkillSet;
		private LocalPolymorph currentPolymorph;
		private Dictionary<int, LocalPolymorph> dictionaryPolymorph;
		protected LocalMoveAgent moveAgent;
		private float moveVelocity;
		private PolymorphController polymorphController;
		private int preAreaMaskCode;
		private SkillControllerSnapshot skillControllerSnapshot;
		protected override bool IsInBush => moveAgent.IsInBush;
		public SkillSlotSet ConcentrateSkillSet => concentrateSkillSet;
		public bool IsConcentrating => concentrateSkillSet > SkillSlotSet.None;
		public float MoveVelocity => moveVelocity;
		public LocalPolymorph Polymorph => currentPolymorph;
		public LocalMoveAgent MoveAgent => moveAgent;
		public void UpdateMoveSpeed(float moveSpeed)
		{
			Status.SetMoveSpeed(moveSpeed);
			moveAgent.UpdateMoveSpeed(moveSpeed);
		}
		public void MoveInCurve(Vector3 startRotation, float angularSpeed)
		{
			if (!isAlive)
			{
				return;
			}

			moveAgent.MoveInCurve(startRotation, angularSpeed, Status.MoveSpeed);
		}
		public void MoveInDirection(Vector3 direction)
		{
			if (!isAlive)
			{
				return;
			}

			moveAgent.MoveInDirection(direction, Status.MoveSpeed);
		}


		public virtual void MoveToDestination(BlisVector startPosition, BlisVector destination, BlisVector[] corners)
		{
			if (!isAlive)
			{
				return;
			}

			moveAgent.MoveToDestination(startPosition, destination, corners, Status.MoveSpeed);
			OnStartMove();
		}


		public void MoveStraight(BlisVector startPosition, BlisVector destination, float duration,
			EasingFunction.Ease ease, bool canRotate = false)
		{
			if (!isAlive)
			{
				return;
			}

			if (GameUtil.DistanceOnPlane(startPosition.ToVector3(), transform.position) > 1f)
			{
				moveAgent.MoveStraight(startPosition, destination, duration, ease, canRotate);
			}
			else
			{
				moveAgent.MoveStraight(new BlisVector(transform.position), destination, duration, ease, canRotate);
			}

			OnStartMove();
		}


		public void MoveStraightWithoutNav(Vector3 moveStartPos, Vector3 moveEndPos, float duration)
		{
			if (GameUtil.DistanceOnPlane(moveStartPos, transform.position) > 1f)
			{
				moveAgent.MoveStraightWithoutNav(moveStartPos, moveEndPos, duration);
				return;
			}

			moveAgent.MoveStraightWithoutNav(transform.position, moveEndPos, duration);
		}


		public void MoveToTargetWithoutNav(Vector3 moveStartPos, LocalCharacter target, float moveSpeed,
			float arriveRadius)
		{
			moveAgent.MoveToTargetWithoutNav(transform.position, target, moveSpeed, arriveRadius);
		}


		public void WarpTo(BlisVector destination)
		{
			if (!isAlive)
			{
				return;
			}

			moveAgent.Warp(destination);
			OnStartMove();
		}


		public virtual void StopMove()
		{
			moveAgent.Stop();
			UpdateCharacterAnimator();
			PolymorphController polymorphController = this.polymorphController;
			if (polymorphController == null)
			{
				return;
			}

			polymorphController.UpdateAnimator(GetMoveAnimationVelocity());
		}


		public void PauseMove()
		{
			moveAgent.Pause();
		}


		public void ResumeMove()
		{
			moveAgent.Resume();
		}


		public void LockRotation(bool isLock, Quaternion rotation)
		{
			moveAgent.LockRotation(isLock);
			if (isLock)
			{
				moveAgent.InstanceLookAt(rotation);
			}
		}


		public override void LookAt(Quaternion lookFrom, Quaternion lookTo, float angularSpeed)
		{
			moveAgent.LookAt(lookFrom, lookTo, angularSpeed);
		}

		protected override int GetTeamNumber()
		{
			return 0;
		}


		public override void Init(byte[] snapshotData)
		{
			base.Init(snapshotData);
			CharacterSnapshot characterSnapshot = serializer.Deserialize<CharacterSnapshot>(snapshotData);
			LocalSkillPlayer localSkillPlayer = this.localSkillPlayer;
			if (localSkillPlayer != null)
			{
				localSkillPlayer.FinishAll();
			}

			this.localSkillPlayer = new LocalSkillPlayer(this);
			this.localSkillPlayer.SetOnBeforeStartSkillAction(OnBeforeStartSkill);
			skillControllerSnapshot = characterSnapshot.skillController;
			preAreaMaskCode = 0;
		}


		protected virtual void OnBeforeStartSkill(SkillData skillData) { }


		public void InitSkillController()
		{
			if (skillControllerSnapshot != null)
			{
				localSkillPlayer.Init(skillControllerSnapshot);
				skillControllerSnapshot = null;
			}
		}


		protected override LocalCharacterStat CreateCharacterStat()
		{
			return new LocalCharacterStat();
		}


		protected override void InitMoveAgent(MoveAgentSnapshot snapshot)
		{
			base.InitMoveAgent(snapshot);
			GameUtil.BindOrAdd<LocalMoveAgent>(gameObject, ref moveAgent);
			moveAgent.Init(0.19f, 2147483640);
			moveAgent.ApplySnapshot(snapshot, MonoBehaviourInstance<ClientService>.inst.World);
		}


		protected override void UpdateInternal()
		{
			base.UpdateInternal();
			LocalMoveAgent localMoveAgent = moveAgent;
			if (localMoveAgent != null)
			{
				localMoveAgent.FrameUpdate(Time.deltaTime);
			}

			UpdateCharacterAnimator();
			PolymorphController polymorphController = this.polymorphController;
			if (polymorphController == null)
			{
				return;
			}

			polymorphController.UpdateAnimator(GetMoveAnimationVelocity());
		}


		protected virtual void UpdateCharacterAnimator()
		{
			if (Stat != null)
			{
				SetCharacterAnimatorFloat(FloatAttackSpeed, Stat.AttackSpeed);
			}

			moveVelocity = GetMoveAnimationVelocity();
			SetCharacterAnimatorFloat(FloatMoveVelocity, moveVelocity);
			SetCharacterAnimatorFloat(FloatMoveSpeed, moveVelocity / 3.5f);
		}


		public float GetMoveAnimationVelocity()
		{
			if (!CanMoveAnimation())
			{
				return 0f;
			}

			if (IsAlive && IsRun())
			{
				return Status.MoveSpeed;
			}

			return 0f;
		}


		private bool CanMoveAnimation()
		{
			if (States != null)
			{
				using (List<CharacterStateValue>.Enumerator enumerator = States.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.StateType.ApplyMoveAnimation())
						{
							return false;
						}
					}
				}

				return true;
			}

			return true;
		}


		public override void OnDead(LocalCharacter attacker)
		{
			base.OnDead(attacker);
			ReturnPolymorphObject();
			localSkillPlayer.FinishAll();
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDead(this);
		}


		public override int GetCurrentAreaMask()
		{
			int currentAreaMask = AreaUtil.GetCurrentAreaMask(GetPosition());
			if (currentAreaMask == 0)
			{
				return preAreaMaskCode;
			}

			preAreaMaskCode = currentAreaMask;
			return currentAreaMask;
		}


		public override AreaData GetCurrentAreaData(LevelData currentLevel)
		{
			return AreaUtil.GetCurrentAreaDataByMask(currentLevel, moveAgent.WalkableNavMask, GetCurrentAreaMask());
		}


		private bool IsRun()
		{
			return moveAgent.IsMoving;
		}


		protected virtual void OnStartMove() { }


		public virtual void OnStopNormalAttack()
		{
			SetCharacterAnimatorResetTrigger(TriggerCanNotControl);
			SetCharacterAnimatorTrigger(TriggerCanNotControl);
		}


		public virtual void OnCrowdControl(StateType stateType)
		{
			if (stateType.CanControl())
			{
				return;
			}

			SetCharacterAnimatorResetTrigger(TriggerCanNotControl);
			SetCharacterAnimatorTrigger(TriggerCanNotControl);
		}


		public virtual void SetAnimation(int id)
		{
			SetCharacterAnimatorResetTrigger(id);
			SetCharacterAnimatorTrigger(id);
		}


		public virtual void SetAnimation(int id, float value)
		{
			SetCharacterAnimatorFloat(id, value);
		}


		public virtual void SetAnimation(int id, bool value)
		{
			SetCharacterAnimatorBool(id, value);
		}


		public virtual void SetAnimation(int id, int value)
		{
			SetCharacterAnimatorInteger(id, value);
		}


		public void StartConcentration(SkillData skillData)
		{
			SkillSlotSet skillSlotSet = SkillSlotSet.None;
			MasteryType masteryType = MasteryType.None;
			if (!GameDB.skill.FindSkillSlotAndMastery(CharacterCode, skillData.code, ObjectType, ref skillSlotSet,
				ref masteryType))
			{
				return;
			}

			concentrateSkillSet = skillSlotSet;
			if (0f < skillData.ConcentrationTime)
			{
				IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter localPlayer)
				{
					localPlayer.OnStartConcentration(skillData);
				});
			}
		}


		public void EndConcentration(SkillData skillData, bool cancel)
		{
			concentrateSkillSet = SkillSlotSet.None;
			IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter localPlayer)
			{
				localPlayer.OnEndConcentration(skillData, cancel);
			});
		}


		public string GetSkillTooltip(SkillData skillData, int evolutionLevel)
		{
			string[] tooltipParam = LocalSkillPlayer.GetTooltipParam(skillData, evolutionLevel);
			string skillDescKey = LnUtil.GetSkillDescKey(skillData.group);
			object[] param_ = tooltipParam;
			return Ln.Format(skillDescKey, param_);
		}


		public string[] GetNextLevelTooltipParam(SkillData skillData, int evolutionLevel)
		{
			return LocalSkillPlayer.GetNextLevelTooltipParam(skillData, evolutionLevel);
		}


		public string[] GetNextLevelTooltipValue(SkillData skillData, SkillData nextSkillData, int evolutionLevel)
		{
			return LocalSkillPlayer.GetNextLevelTooltipValue(skillData, nextSkillData, evolutionLevel);
		}


		public void AddBulletLine(string bulletLineKey, LocalObject target, string materialName, float startWidth,
			float endWidth)
		{
			Transform transform = this.transform.FindRecursively("Fx_Center");
			if (transform == null)
			{
				transform = this.transform;
			}

			if (!bulletLines.ContainsKey(bulletLineKey))
			{
				GameObject gameObject =
					Instantiate<GameObject>(SingletonMonoBehaviour<ResourceManager>.inst.GetBulletLinePrefab(),
						this.transform);
				gameObject.transform.localPosition = Vector3.zero;
				BulletLine component = gameObject.GetComponent<BulletLine>();
				bulletLines.Add(bulletLineKey, component);
			}

			Transform transform2 = target.transform;
			BulletLineAnchor componentInChildren = target.GetComponentInChildren<BulletLineAnchor>();
			if (componentInChildren != null)
			{
				transform2 = componentInChildren.transform;
			}

			bulletLines[bulletLineKey].Link(transform, transform2);
			bulletLines[bulletLineKey].SetMaterial(materialName);
			bulletLines[bulletLineKey].SetWidth(startWidth, endWidth);
		}


		public void RemoveBulletLine(string bulletLineKey)
		{
			if (bulletLines.ContainsKey(bulletLineKey))
			{
				Destroy(bulletLines[bulletLineKey].gameObject);
				bulletLines.Remove(bulletLineKey);
			}
		}


		public void ChangePolymorphObject(LocalObject caster, int stateGroupCode)
		{
			if (dictionaryPolymorph == null)
			{
				dictionaryPolymorph = new Dictionary<int, LocalPolymorph>();
				GameUtil.BindOrAdd<PolymorphController>(gameObject, ref polymorphController);
			}

			if (!dictionaryPolymorph.ContainsKey(stateGroupCode))
			{
				CreatePolymorphObject(caster, stateGroupCode);
			}

			if (currentPolymorph == null || currentPolymorph != dictionaryPolymorph[stateGroupCode])
			{
				currentPolymorph = dictionaryPolymorph[stateGroupCode];
				polymorphController.SetAnimator(currentPolymorph.gameObject);
			}

			SetActiveCharacterObjectChild(false);
			currentPolymorph.gameObject.SetActive(true);
			Outliner component = GetComponent<Outliner>();
			if (component == null)
			{
				return;
			}

			component.OrganizeDrawCall();
		}


		private void CreatePolymorphObject(LocalObject caster, int stateGroupCode)
		{
			string polymorphPrefabPath = GameDB.characterState.GetGroupData(stateGroupCode).polymorphPrefabPath;
			GameObject gameObject = Instantiate<GameObject>(caster.LoadObject(polymorphPrefabPath), transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			LocalPolymorph localPolymorph = null;
			GameUtil.BindOrAdd<LocalPolymorph>(gameObject, ref localPolymorph);
			localPolymorph.Init(this);
			dictionaryPolymorph.Add(stateGroupCode, localPolymorph);
		}


		public void ReturnPolymorphObject()
		{
			if (currentPolymorph != null)
			{
				currentPolymorph.gameObject.SetActive(false);
			}

			SetActiveCharacterObjectChild(true);
			Outliner component = GetComponent<Outliner>();
			if (component == null)
			{
				return;
			}

			component.OrganizeDrawCall();
		}


		public override bool SetCursor(LocalPlayerCharacter myPlayer)
		{
			if (base.SetCursor(myPlayer))
			{
				return true;
			}

			if (!IsAlive)
			{
				MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.Item);
				return true;
			}

			if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(this))
			{
				if (!myPlayer.IsEquippedWeapon())
				{
					MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.Disarmed);
				}
				else
				{
					MonoBehaviourInstance<GameInput>.inst.GameCursor.SetCursorTarget(CursorTarget.EnemyNotSummon);
				}

				return true;
			}

			return false;
		}


		private void SetActiveCharacterObjectChild(bool active)
		{
			ClientCharacter componentInChildren = characterObject.GetComponentInChildren<ClientCharacter>();
			if (componentInChildren != null && 0 < componentInChildren.transform.childCount)
			{
				for (int i = 0; i < componentInChildren.transform.childCount; i++)
				{
					componentInChildren.transform.GetChild(i).gameObject.SetActive(active);
				}
			}
		}


		public override void OutSight()
		{
			base.OutSight();
			moveAgent.Stop();
		}
	}
}