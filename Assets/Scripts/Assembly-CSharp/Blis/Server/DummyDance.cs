using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.DummyDance)]
	public class DummyDance : SkillScript
	{
		
		private readonly List<SkillAgent> debuffedCharacters = new List<SkillAgent>();

		
		private readonly CollisionCircle3D collision =
			new CollisionCircle3D(Vector3.zero, Singleton<DummyDanceData>.inst.DanceDubuffRange);

		
		protected override void Start()
		{
			base.Start();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		private bool IsInRange(SkillAgent target)
		{
			return GameUtil.DistanceOnPlane(Caster.Position, target.Position) <=
			       Singleton<DummyDanceData>.inst.DanceDubuffRange;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			CharacterStateData monsterDanceStateData =
				GameDB.characterState.GetData(Singleton<DummyDanceData>.inst.MonsterDebuffState);
			CharacterStateData playerDanceStateData =
				GameDB.characterState.GetData(Singleton<DummyDanceData>.inst.PlayerDebuffState);
			while (Caster.IsAlive)
			{
				int num = debuffedCharacters.Count - 1;
				while (0 <= num)
				{
					SkillAgent skillAgent = debuffedCharacters[num];
					if (!IsInRange(skillAgent))
					{
						skillAgent.RemoveStateByGroup(
							skillAgent.ObjectType == ObjectType.Monster
								? monsterDanceStateData.group
								: playerDanceStateData.group, Caster.ObjectId);
						debuffedCharacters.RemoveAt(num);
					}

					num--;
				}

				collision.UpdatePosition(Caster.Position);
				foreach (SkillAgent skillAgent2 in GetCharacters(collision, true, true))
				{
					if (IsInRange(skillAgent2))
					{
						bool flag = false;
						using (List<SkillAgent>.Enumerator enumerator2 = debuffedCharacters.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.ObjectId.Equals(skillAgent2.ObjectId) &&
								    skillAgent2.IsHaveStateByGroup(Singleton<DummyDanceData>.inst.DanceDubuffGroup,
									    Caster.ObjectId))
								{
									flag = true;
									break;
								}
							}
						}

						if (!flag)
						{
							AddState(skillAgent2,
								skillAgent2.ObjectType == ObjectType.Monster
									? monsterDanceStateData.code
									: playerDanceStateData.code);
							if (skillAgent2.IsHaveStateByGroup(
								skillAgent2.ObjectType == ObjectType.Monster
									? monsterDanceStateData.group
									: playerDanceStateData.group, Caster.ObjectId))
							{
								debuffedCharacters.Add(skillAgent2);
							}
						}
					}
				}

				yield return WaitForFrame();
			}
		}
	}
}