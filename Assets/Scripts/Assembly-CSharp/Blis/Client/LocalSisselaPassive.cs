using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Blis.Client
{
	[SkillScript(SkillId.SisselaPassive)]
	public class LocalSisselaPassive : LocalSisselaSkill
	{
		private const string fxNameDistanceGreen = "FX_BI_SisselaArrow_01";


		private const string fxNameDistanceYellow = "FX_BI_SisselaArrow_02";


		private const string fxNameDistanceRed = "FX_BI_SisselaArrow_03";


		private const string AnimFloatMoveVelocity = "W_moveVelocity";


		private const string AnimFloatMoveSpeed = "W_moveSpeed";


		private new static readonly int FloatMoveVelocity = Animator.StringToHash("W_moveVelocity");


		private new static readonly int FloatMoveSpeed = Animator.StringToHash("W_moveSpeed");


		private static readonly Color enemyWilsonGlowColor = Color.red;


		private static readonly Color allyWilsonGlowColor = Color.cyan;


		private ArrowShowType arrowShowType;


		private bool isWilsonUnion;


		private Transform wilsonDirArrow;


		private Renderer wilsonDirArrowRenderer;


		private RangeIndicator wilsonUnionRange;


		public override void Start()
		{
			StopCoroutines();
			if (Self.ObjectId == MonoBehaviourInstance<ClientService>.inst.MyObjectId)
			{
				StartCoroutine(UpdateWilsonDirArrow());
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDyingConditionAction -= OnDyingCondition;
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDeadAction -= OnDead;
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnRevivalAction -= OnRevival;
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDyingConditionAction += OnDyingCondition;
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnDeadAction += OnDead;
				SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnRevivalAction += OnRevival;
			}

			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnAddEffectStateEvent -= OnAddEffectState;
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnAddEffectStateEvent += OnAddEffectState;
			StartCoroutine(InitWilson());
			StartCoroutine(UpdateWilsonRunAnimation());
		}


		private IEnumerator InitWilson()
		{
			while (wilson == null)
			{
				yield return null;
			}

			if (wilson.ServantData != null)
			{
				yield break;
			}

			wilson.SetServantData(new LocalWilsonData(trsfWilson.parent, trsfWilson.GetChild(0),
				trsfWilson.FindRecursively("Tongue_main")));
			SetWilsonTongueLength(0f);
			if (Self.ObjectId == MonoBehaviourInstance<ClientService>.inst.MyObjectId)
			{
				wilsonDirArrow = PlayEffectChild(Self, "Fx_BI_Sissela_Arrow").transform;
				wilsonDirArrowRenderer = wilsonDirArrow.GetComponentInChildren<Renderer>();
				arrowShowType = ArrowShowType.NOT_SHOW;
				wilsonDirArrow.gameObject.SetActive(false);
				wilsonUnionRange = Object
					.Instantiate<GameObject>(
						SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorPrefab("RangeIndicator"))
					.GetComponent<RangeIndicator>();
				Transform transform = wilsonUnionRange.transform;
				transform.SetParent(wilson.transform);
				transform.localPosition = Vector3.zero;
				wilsonUnionRange.Range = Singleton<SisselaSkillData>.inst.WilsonMinDistance;
				wilsonUnionRange.gameObject.SetActive(false);
			}

			Color color = enemyWilsonGlowColor;
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer &&
			    MonoBehaviourInstance<ClientService>.inst.IsAlly(LocalCharacter))
			{
				color = allyWilsonGlowColor;
			}

			ParticleSystem[] componentsInChildren = wilson.transform.FindRecursively("FX_BI_Sissela_Skill01_Move")
				.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ParticleSystem.MainModule main = componentsInChildren[i].main;
				main.startColor = color;
			}
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition)
		{
			base.Play(actionNo, target, targetPosition);
		}


		public override void Finish(bool cancel) { }


		private IEnumerator UpdateWilsonRunAnimation()
		{
			while (wilson == null)
			{
				yield return null;
			}

			for (;;)
			{
				yield return null;
				if (!isWilsonUnion)
				{
					wilson.SetCharacterAnimatorFloat(FloatMoveVelocity, 0f);
					wilson.SetCharacterAnimatorFloat(FloatMoveSpeed, 0f);
				}
				else if (LocalPlayerCharacter.MoveAgent.IsMoving)
				{
					float moveAnimationVelocity = LocalPlayerCharacter.GetMoveAnimationVelocity();
					wilson.SetCharacterAnimatorFloat(FloatMoveVelocity, moveAnimationVelocity);
					wilson.SetCharacterAnimatorFloat(FloatMoveSpeed, moveAnimationVelocity / 3.5f);
				}
				else
				{
					wilson.SetCharacterAnimatorFloat(FloatMoveVelocity, 0f);
					wilson.SetCharacterAnimatorFloat(FloatMoveSpeed, 0f);
				}
			}
		}


		private IEnumerator UpdateWilsonDirArrow()
		{
			while (wilson == null)
			{
				yield return null;
			}

			while (wilsonDirArrow == null)
			{
				yield return null;
			}

			for (;;)
			{
				yield return null;
				if (isWilsonUnion)
				{
					if (arrowShowType != ArrowShowType.NOT_SHOW)
					{
						arrowShowType = ArrowShowType.NOT_SHOW;
						wilsonDirArrow.gameObject.SetActive(false);
						wilsonUnionRange.gameObject.SetActive(false);
					}
				}
				else
				{
					if (arrowShowType == ArrowShowType.NOT_SHOW)
					{
						wilsonDirArrow.gameObject.SetActive(true);
						wilsonUnionRange.gameObject.SetActive(true);
					}

					Vector3 position = wilson.GetPosition();
					Vector3 position2 = Self.GetPosition();
					position.y = position2.y;
					wilsonDirArrow.LookAt(position);
					float num = GameUtil.DistanceOnPlane(position, position2);
					if (num <= Singleton<SisselaSkillData>.inst.WilsonDistanceColorGreenYellow)
					{
						if (arrowShowType != ArrowShowType.DISTANCE_GREEN)
						{
							arrowShowType = ArrowShowType.DISTANCE_GREEN;
							wilsonDirArrowRenderer.sharedMaterial =
								SingletonMonoBehaviour<ResourceManager>.inst.GetEffectMaterial("FX_BI_SisselaArrow_01");
						}
					}
					else if (num <= Singleton<SisselaSkillData>.inst.WilsonDistanceColorYellowRed)
					{
						if (arrowShowType != ArrowShowType.DISTANCE_YELLOW)
						{
							arrowShowType = ArrowShowType.DISTANCE_YELLOW;
							wilsonDirArrowRenderer.sharedMaterial =
								SingletonMonoBehaviour<ResourceManager>.inst.GetEffectMaterial("FX_BI_SisselaArrow_02");
						}
					}
					else if (arrowShowType != ArrowShowType.DISTANCE_RED)
					{
						arrowShowType = ArrowShowType.DISTANCE_RED;
						wilsonDirArrowRenderer.sharedMaterial =
							SingletonMonoBehaviour<ResourceManager>.inst.GetEffectMaterial("FX_BI_SisselaArrow_03");
					}
				}
			}
		}


		private void OnAddEffectState(CharacterStateValue state, LocalCharacter target)
		{
			if (target.ObjectId != Self.ObjectId)
			{
				return;
			}

			if (state.code == Singleton<SisselaSkillData>.inst.WilsonUnionState)
			{
				isWilsonUnion = true;
				return;
			}

			if (state.code == Singleton<SisselaSkillData>.inst.WilsonSeparateState)
			{
				isWilsonUnion = false;
			}
		}


		private void OnDyingCondition(LocalCharacter target)
		{
			if (target.ObjectId != Self.ObjectId)
			{
				return;
			}

			if (wilsonUnionRange == null)
			{
				return;
			}

			wilsonUnionRange.gameObject.SetActive(false);
		}


		private void OnDead(LocalCharacter target)
		{
			if (target.ObjectId != Self.ObjectId)
			{
				return;
			}

			if (wilsonUnionRange == null)
			{
				return;
			}

			wilsonUnionRange.gameObject.SetActive(false);
		}


		private void OnRevival(LocalCharacter target)
		{
			if (target.ObjectId != Self.ObjectId)
			{
				return;
			}

			if (wilsonUnionRange == null)
			{
				return;
			}

			if (!isWilsonUnion)
			{
				wilsonUnionRange.gameObject.SetActive(true);
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<SisselaSkillData>.inst.PassiveHpRegenBase[skillData.level].ToString();
				case 1:
					return (Singleton<SisselaSkillData>.inst.PassiveHpRegenPerLostHp[skillData.level] * 8 +
					        Singleton<SisselaSkillData>.inst.PassiveHpRegenBase[skillData.level]).ToString();
				case 2:
					return Singleton<SisselaSkillData>.inst.PassiveSkillIncreaseBase[skillData.level].ToString();
				case 3:
					return Mathf
						.RoundToInt(
							Singleton<SisselaSkillData>.inst.PassiveSkillIncreasePerLostHp[skillData.level] * 8f +
							Singleton<SisselaSkillData>.inst.PassiveSkillIncreaseBase[skillData.level]).ToString();
				case 4:
					return (Singleton<SisselaSkillData>.inst.PassiveNormalAttackBaseDamage +
					        LocalPlayerCharacter.Status.Level * Singleton<SisselaSkillData>.inst
						        .PassiveNormalAttackCharacterLvPerDamage).ToString();
				case 5:
					return Mathf
						.RoundToInt((Singleton<SisselaSkillData>.inst.PassiveNormalAttackApDamageNormalType +
						             Singleton<SisselaSkillData>.inst.PassiveNormalAttackApDamageSkillType) *
						            SelfStat.AttackPower).ToString();
				case 6:
					return GameDB.characterState
						.GetData(Singleton<SisselaSkillData>.inst.PassiveNormalAttackDebuff[skillData.level]).duration
						.ToString();
				case 7:
				{
					float num = Mathf.Abs(GameDB.characterState
						.GetData(Singleton<SisselaSkillData>.inst.PassiveNormalAttackDebuff[skillData.level])
						.statValue1);
					return string.Format("{0}%", num);
				}
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/MaxHpRegen";
				case 1:
					return "ToolTipType/MaxSkill";
				case 2:
					return "ToolTipType/DecreaseMoveRatio";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return (Singleton<SisselaSkillData>.inst.PassiveHpRegenPerLostHp[skillData.level] * 8 +
					        Singleton<SisselaSkillData>.inst.PassiveHpRegenBase[skillData.level]).ToString();
				case 1:
					return ((int) Math.Abs(
						Singleton<SisselaSkillData>.inst.PassiveSkillIncreasePerLostHp[skillData.level] * 8f +
						Singleton<SisselaSkillData>.inst.PassiveSkillIncreaseBase[skillData.level])).ToString();
				case 2:
					return Mathf.Abs(GameDB.characterState
						.GetData(Singleton<SisselaSkillData>.inst.PassiveNormalAttackDebuff[skillData.level])
						.statValue1) + "%";
				default:
					return "";
			}
		}


		private enum ArrowShowType
		{
			NOT_SHOW,


			DISTANCE_GREEN,


			DISTANCE_YELLOW,


			DISTANCE_RED
		}
	}
}