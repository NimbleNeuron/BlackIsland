using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.SisselaPassive)]
	public class SisselaPassive : SisselaSkillScript
	{
		
		private bool passiveBuffStateExist;

		
		private int pastLostHpSection = -1;

		
		private bool pastPassiveBuffStateExist;

		
		private float timeStack;

		
		private WorldSummonServant wilson;

		
		protected override void Start()
		{
			base.Start();
			timeStack = 0f;
			pastLostHpSection = -1;
			pastPassiveBuffStateExist = false;
			passiveBuffStateExist = false;
			AddState(Caster, Singleton<SisselaSkillData>.inst.PassiveSkillIncreaseStateCode);
			WorldCharacter character = Caster.Character;
			character.ChangeHpEvent =
				(Action<int>) Delegate.Combine(character.ChangeHpEvent, new Action<int>(OnHpOrMaxHpChange));
			WorldCharacter character2 = Caster.Character;
			character2.ChangeMaxHpEvent =
				(Action<int>) Delegate.Combine(character2.ChangeMaxHpEvent, new Action<int>(OnHpOrMaxHpChange));
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnCompleteAddStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Combine(
				inst.OnCompleteAddStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteAddState));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnCompleteRemoveStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Combine(
				inst2.OnCompleteRemoveStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteRemoveState));
			if (wilson != null)
			{
				return;
			}

			WorldPlayerCharacter worldPlayerCharacter = (WorldPlayerCharacter) Caster.Character;
			wilson = GetWilson(worldPlayerCharacter);
			if (wilson != null)
			{
				return;
			}

			wilson = MonoBehaviourInstance<GameService>.inst.Spawn.SpawnSummon(worldPlayerCharacter,
				Singleton<SisselaSkillData>.inst.WilsonSummonCode, Caster.Position) as WorldSummonServant;
			wilson.LockRotation(true);
			wilson.SetServantData(new WilsonData(wilson.transform.parent));
			SetWilsonState(true, wilson);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			WorldCharacter character = Caster.Character;
			character.ChangeHpEvent =
				(Action<int>) Delegate.Remove(character.ChangeHpEvent, new Action<int>(OnHpOrMaxHpChange));
			WorldCharacter character2 = Caster.Character;
			character2.ChangeMaxHpEvent =
				(Action<int>) Delegate.Remove(character2.ChangeMaxHpEvent, new Action<int>(OnHpOrMaxHpChange));
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnCompleteAddStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Remove(
				inst.OnCompleteAddStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteAddState));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnCompleteRemoveStateEvent = (Action<WorldCharacter, CharacterState>) Delegate.Remove(
				inst2.OnCompleteRemoveStateEvent, new Action<WorldCharacter, CharacterState>(OnCompleteRemoveState));
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			for (;;)
			{
				if (!IsWilsonUnion())
				{
					UpdateWilsonSeperate();
				}

				timeStack += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (timeStack >= 1f)
				{
					timeStack -= 1f;
					int lostHpSection =
						SisselaSkillData.GetLostHpSection(Caster.Character.Status.Hp, Caster.Character.Stat.MaxHp);
					if (lostHpSection != -1)
					{
						int skillLevel = SkillLevel;
						int num = Singleton<SisselaSkillData>.inst.PassiveHpRegenPerLostHp[skillLevel] * lostHpSection +
						          Singleton<SisselaSkillData>.inst.PassiveHpRegenBase[skillLevel];
						if (passiveBuffStateExist)
						{
							num += (int) (num * Singleton<SisselaSkillData>.inst.A4BuffIncreaseRate);
						}

						HpHealTo(Caster, 0, 0f, num, true, 0);
					}
				}

				yield return WaitForFrame();
			}
		}

		
		private void OnHpOrMaxHpChange(int hp)
		{
			int lostHpSection =
				SisselaSkillData.GetLostHpSection(Caster.Character.Status.Hp, Caster.Character.Stat.MaxHp);
			if (pastLostHpSection == lostHpSection && pastPassiveBuffStateExist == passiveBuffStateExist)
			{
				return;
			}

			pastPassiveBuffStateExist = passiveBuffStateExist;
			pastLostHpSection = lostHpSection;
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<SisselaSkillData>.inst.PassiveSkillIncreaseStateCode);
			if (lostHpSection == -1)
			{
				Caster.SetExternalNonCalculateStatValue(data.group, Caster.ObjectId, 0f);
				return;
			}

			int skillLevel = SkillLevel;
			float num = Singleton<SisselaSkillData>.inst.PassiveSkillIncreaseBase[skillLevel];
			num += Singleton<SisselaSkillData>.inst.PassiveSkillIncreasePerLostHp[skillLevel] * lostHpSection;
			if (passiveBuffStateExist)
			{
				num += num * Singleton<SisselaSkillData>.inst.A4BuffIncreaseRate;
			}

			Caster.SetExternalNonCalculateStatValue(data.group, Caster.ObjectId, num);
		}

		
		private void OnCompleteAddState(WorldCharacter target, CharacterState characterState)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId != target.ObjectId)
			{
				return;
			}

			if (characterState == null || characterState.Caster == null)
			{
				return;
			}

			if (Caster.ObjectId != characterState.Caster.ObjectId)
			{
				return;
			}

			if (GameDB.characterState.GetData(Singleton<SisselaSkillData>.inst.A4BuffStateCode).group !=
			    characterState.StateData.group)
			{
				return;
			}

			passiveBuffStateExist = true;
			OnHpOrMaxHpChange(Caster.Character.Status.Hp);
		}

		
		private void OnCompleteRemoveState(WorldCharacter target, CharacterState characterState)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId != target.ObjectId)
			{
				return;
			}

			if (characterState == null || characterState.Caster == null)
			{
				return;
			}

			if (Caster.ObjectId != characterState.Caster.ObjectId)
			{
				return;
			}

			if (GameDB.characterState.GetData(Singleton<SisselaSkillData>.inst.A4BuffStateCode).group !=
			    characterState.StateData.group)
			{
				return;
			}

			passiveBuffStateExist = false;
			OnHpOrMaxHpChange(Caster.Character.Status.Hp);
		}

		
		public override void OnUpgradePassiveSkill()
		{
			base.OnUpgradePassiveSkill();
			pastLostHpSection = -1;
			OnHpOrMaxHpChange(Caster.Character.Status.Hp);
		}

		
		private void UpdateWilsonSeperate()
		{
			if (!wilson.IsStopped())
			{
				return;
			}

			if (!wilson.CanControl)
			{
				return;
			}

			float num = GameUtil.DistanceOnPlane(Caster.Position, wilson.GetPosition());
			if (num < Singleton<SisselaSkillData>.inst.WilsonMinDistance ||
			    num > Singleton<SisselaSkillData>.inst.WilsonMaxDistance)
			{
				PlaySkillAction(Caster, SkillId.SisselaPassive, 1004);
				SetWilsonState(true, wilson);
			}
		}
	}
}