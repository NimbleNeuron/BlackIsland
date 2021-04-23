using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ZahirPassive)]
	public class ZahirPassive : LocalSkillScript
	{
		private readonly List<LocalPlayerCharacter> cachedPlayers = new List<LocalPlayerCharacter>();


		public override void Start() { }


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (action == 1 && SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				StartCoroutine(ScanDeadPlayers());
				PlayEffectChild(Self, "FX_BI_Zahir_Passive_Find");
			}
		}


		private IEnumerator ScanDeadPlayers()
		{
			yield return new WaitForSeconds(0.1f);
			cachedPlayers.Clear();
			foreach (PlayerContext playerContext in MonoBehaviourInstance<ClientService>.inst.GetPlayers())
			{
				if (!MonoBehaviourInstance<ClientService>.inst.IsAlly(playerContext.Character) &&
				    !playerContext.Character.IsAlive)
				{
					cachedPlayers.Add(playerContext.Character);
					playerContext.Character.ShowMapIcon(MiniMapIconType.Skill);
					playerContext.Character.ShowMiniMapIcon(MiniMapIconType.Skill);
				}
			}

			if (0 < cachedPlayers.Count)
			{
				yield return new WaitForSeconds(Singleton<ZahirSkillPassiveData>.inst.PassiveDuration);
				HideCachedPlayers();
			}
		}


		public override void Finish(bool cancel)
		{
			if (0 < cachedPlayers.Count)
			{
				HideCachedPlayers();
				cachedPlayers.Clear();
			}
		}


		private void HideCachedPlayers()
		{
			SightAgent selfSightAgent = null;
			Self.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter selfPlayer)
			{
				selfSightAgent = selfPlayer.SightAgent;
			});
			for (int i = 0; i < cachedPlayers.Count; i++)
			{
				cachedPlayers[i].HideMapIcon(MiniMapIconType.Skill);
				if (selfSightAgent != null)
				{
					if (!selfSightAgent.IsInAllySight(cachedPlayers[i].SightAgent, cachedPlayers[i].GetPosition(),
						cachedPlayers[i].Stat.Radius, cachedPlayers[i].IsInvisible))
					{
						cachedPlayers[i].HideMiniMapIcon(MiniMapIconType.Skill);
					}
				}
				else
				{
					cachedPlayers[i].HideMiniMapIcon(MiniMapIconType.Skill);
				}
			}
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<ZahirSkillPassiveData>.inst.BuffState[skillData.level]);
					return string.Format("{0}%", data.statValue1);
				}
				case 1:
					return Singleton<ZahirSkillPassiveData>.inst.DamageByLevel[skillData.level].ToString();
				case 2:
					return ((int) (Singleton<ZahirSkillPassiveData>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 3:
					return Singleton<ZahirSkillPassiveData>.inst.PassiveDuration.ToString();
				case 4:
					return skillData.cooldown.ToString();
				case 5:
					return GameDB.characterState.GetData(Singleton<ZahirSkillPassiveData>.inst.PassiveDebuffStateCode)
						.duration.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			if (index == 0)
			{
				return "ToolTipType/Damage";
			}

			return "";
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			if (index == 0)
			{
				int num = (int) (Singleton<ZahirSkillPassiveData>.inst.SkillApCoef * SelfStat.AttackPower);
				return (Singleton<ZahirSkillPassiveData>.inst.DamageByLevel[level] + num).ToString();
			}

			return "";
		}
	}
}