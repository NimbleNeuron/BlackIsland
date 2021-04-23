using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.NadinePassive)]
	public class NadinePassive : LocalSkillScript
	{
		private readonly List<LocalMonster> cache = new List<LocalMonster>();


		private readonly Collider[] colliders = new Collider[300];


		private readonly List<LocalMonster> inSkillSightMonsters = new List<LocalMonster>();


		public override void Start()
		{
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				StartCoroutine(ScanMonsters());
			}
		}


		private IEnumerator ScanMonsters()
		{
			float passiveSightRange = Singleton<NadineSkillPassiveData>.inst.PassiveSightRange;
			for (;;)
			{
				int num = Physics.OverlapSphereNonAlloc(Self.GetPosition(), passiveSightRange, colliders,
					GameConstants.LayerMask.WORLD_OBJECT_LAYER);
				cache.Clear();
				for (int i = 0; i < num; i++)
				{
					LocalMonster component = colliders[i].GetComponent<LocalMonster>();
					if (!(component == null))
					{
						cache.Add(component);
						if (!inSkillSightMonsters.Remove(component))
						{
							component.ShowMiniMapIcon(MiniMapIconType.Skill);
						}
					}
				}

				inSkillSightMonsters.ForEach(delegate(LocalMonster x) { x.HideMiniMapIcon(MiniMapIconType.Skill); });
				inSkillSightMonsters.Clear();
				for (int j = 0; j < cache.Count; j++)
				{
					cache[j].ShowMiniMapIcon(MiniMapIconType.Skill);
					inSkillSightMonsters.Add(cache[j]);
				}

				yield return new WaitForSeconds(0.5f);
			}
		}


		public override void Play(int actionNo, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel)
		{
			StopCoroutines();
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<NadineSkillPassiveData>.inst.StackCountOnKillCommon[skillData.level].ToString();
				case 1:
					return Singleton<NadineSkillPassiveData>.inst.StackCountOnKillUncommon[skillData.level].ToString();
				case 2:
					return Singleton<NadineSkillPassiveData>.inst.StackCountOnKillRare[skillData.level].ToString();
				case 3:
					return Singleton<NadineSkillPassiveData>.inst.StackCountOnKillEpic[skillData.level].ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "ToolTipType/AnimalGrade01";
				case 1:
					return "ToolTipType/AnimalGrade02";
				case 2:
					return "ToolTipType/AnimalGrade03";
				case 3:
					return "ToolTipType/AnimalGrade04";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<NadineSkillPassiveData>.inst.StackCountOnKillCommon[skillData.level].ToString();
				case 1:
					return Singleton<NadineSkillPassiveData>.inst.StackCountOnKillUncommon[skillData.level].ToString();
				case 2:
					return Singleton<NadineSkillPassiveData>.inst.StackCountOnKillRare[skillData.level].ToString();
				case 3:
					return Singleton<NadineSkillPassiveData>.inst.StackCountOnKillEpic[skillData.level].ToString();
				default:
					return "";
			}
		}
	}
}