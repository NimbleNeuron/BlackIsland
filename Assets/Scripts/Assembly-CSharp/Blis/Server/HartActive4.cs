using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HartActive4)]
	public class HartActive4 : HartSkillScript
	{
		
		private readonly List<SkillAgent> debuffedCharacters = new List<SkillAgent>();

		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private float skillUpdateTime;

		
		private int tick;

		
		private float tickTime;

		
		protected override void Start()
		{
			base.Start();
			tick = 0;
			tickTime = 0f;
			skillUpdateTime = 0f;
			debuffedCharacters.Clear();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			CharacterStateData data = GameDB.characterState.GetData(Singleton<HartSkillActive4Data>.inst.BuffState);
			AddState(Caster, data.code);
			CharacterStateData monsterDanceStateData =
				GameDB.characterState.GetData(Singleton<HartSkillActive4Data>.inst.MonsterDebuffState);
			CharacterStateData playerDanceStateData =
				GameDB.characterState.GetData(Singleton<HartSkillActive4Data>.inst.PlayerDebuffState);
			float SkillUpdateTime = Singleton<HartSkillActive4Data>.inst.SkillUpdateTime;
			float IntervalTime = Singleton<HartSkillActive4Data>.inst.IntervalTime;
			int IntervalCount = Singleton<HartSkillActive4Data>.inst.IntervalCount;
			while (tick < IntervalCount)
			{
				skillUpdateTime += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (SkillUpdateTime <= skillUpdateTime)
				{
					skillUpdateTime -= SkillUpdateTime;
					int num = debuffedCharacters.Count - 1;
					while (0 <= num)
					{
						SkillAgent skillAgent = debuffedCharacters[num];
						if (!IsInRange(skillAgent) || !skillAgent.IsHaveStateByGroup(
							skillAgent.ObjectType == ObjectType.Monster
								? monsterDanceStateData.group
								: playerDanceStateData.group, Caster.ObjectId))
						{
							skillAgent.RemoveStateByGroup(
								skillAgent.ObjectType == ObjectType.Monster
									? monsterDanceStateData.group
									: playerDanceStateData.group, Caster.ObjectId);
							debuffedCharacters.RemoveAt(num);
						}

						num--;
					}

					CollisionCircle3D collisionObject = new CollisionCircle3D(Caster.Position, SkillRange);
					using (List<SkillAgent>.Enumerator enumerator =
						GetCharacters(collisionObject, true, true).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SkillAgent character = enumerator.Current;
							if (IsInRange(character) &&
							    !debuffedCharacters.Any(x => x.ObjectId.Equals(character.ObjectId)))
							{
								AddState(character,
									character.ObjectType == ObjectType.Monster
										? monsterDanceStateData.code
										: playerDanceStateData.code);
								if (character.IsHaveStateByGroup(
									character.ObjectType == ObjectType.Monster
										? monsterDanceStateData.group
										: playerDanceStateData.group, Caster.ObjectId))
								{
									debuffedCharacters.Add(character);
								}
							}
						}
					}
				}

				tickTime += MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (IntervalTime <= tickTime)
				{
					tickTime -= IntervalTime;
					tick++;
					HpHealTo(Caster, Caster.Stat.MaxHp, Singleton<HartSkillActive4Data>.inst.HpCoef[SkillLevel],
						Singleton<HartSkillActive4Data>.inst.HpAmount[SkillLevel], true,
						Singleton<HartSkillActive4Data>.inst.HealEffectAndSound);
					if (IsEvolution())
					{
						parameterCollection.Clear();
						parameterCollection.Add(SkillScriptParameterType.Damage,
							Singleton<HartSkillActive4Data>.inst.DecSpAmount[SkillEvolutionLevel]);
						DirectDamageTo(debuffedCharacters, DamageType.Sp, DamageSubType.Normal, 0, parameterCollection,
							Singleton<HartSkillActive4Data>.inst.DebuffEffectAndSound);
					}
				}

				yield return WaitForFrame();
			}

			FinishConcentration(false);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (cancel)
			{
				CharacterStateData data = GameDB.characterState.GetData(Singleton<HartSkillActive4Data>.inst.BuffState);
				if (Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
				{
					Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
				}
			}

			CharacterStateData data2 =
				GameDB.characterState.GetData(Singleton<HartSkillActive4Data>.inst.MonsterDebuffState);
			CharacterStateData data3 =
				GameDB.characterState.GetData(Singleton<HartSkillActive4Data>.inst.PlayerDebuffState);
			foreach (SkillAgent skillAgent in debuffedCharacters)
			{
				skillAgent.RemoveStateByGroup(skillAgent.ObjectType == ObjectType.Monster ? data2.group : data3.group,
					Caster.ObjectId);
			}

			debuffedCharacters.Clear();
		}

		
		private bool IsInRange(SkillAgent target)
		{
			return GameUtil.DistanceOnPlane(Caster.Position, target.Position) <= SkillRange;
		}
	}
}