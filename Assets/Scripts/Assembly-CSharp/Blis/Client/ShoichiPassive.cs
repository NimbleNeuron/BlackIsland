using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.ShoichiPassive)]
	public class ShoichiPassive : LocalSkillScript
	{
		public override void Start()
		{
			StartCoroutine(PassiveSkillCheck());
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition) { }


		public override void Finish(bool cancel) { }


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
				{
					float statValue = GameDB.characterState
						.GetData(Singleton<ShoichiSkillPassiveData>.inst.BuffStateLevel[skillData.level]).statValue1;
					return string.Format("{0}%", statValue);
				}
				case 1:
					return Singleton<ShoichiSkillPassiveData>.inst.BuffMaxStackCount.ToString();
				case 2:
				{
					float num = Singleton<ShoichiSkillPassiveData>.inst.BuffMaxDamageByLevel[skillData.level];
					return string.Format("{0}%", num * 100f);
				}
				case 3:
					return Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerDamageByLevel[skillData.level]
						.ToString();
				case 4:
					return ((int) (Singleton<ShoichiSkillPassiveData>.inst.SkillApCoef * SelfStat.AttackPower))
						.ToString();
				case 5:
					return GameDB.characterState
						.GetData(Singleton<ShoichiSkillPassiveData>.inst.BuffStateLevel[skillData.level]).duration
						.ToString();
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipParam(int index)
		{
			switch (index)
			{
				case 0:
					return "StatType/CriticalStrikeChance";
				case 1:
					return "ToolTipType/NomalAttackDamageRatio";
				case 2:
					return "ToolTipType/DaggerDamage";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
				{
					float statValue = GameDB.characterState
						.GetData(Singleton<ShoichiSkillPassiveData>.inst.BuffStateLevel[skillData.level]).statValue1;
					return string.Format("{0}%", statValue);
				}
				case 1:
				{
					float num = Singleton<ShoichiSkillPassiveData>.inst.BuffMaxDamageByLevel[skillData.level];
					return string.Format("{0}%", num * 100f);
				}
				case 2:
					return Singleton<ShoichiSkillPassiveData>.inst.PassiveDaggerDamageByLevel[skillData.level]
						.ToString();
				default:
					return "";
			}
		}


		private IEnumerator PassiveSkillCheck()
		{
			ShoichiPassive shoichiPassive = this;
			List<LocalSummonBase> summonBases = new List<LocalSummonBase>();
			int prevCount = 0;
			while (shoichiPassive.LocalPlayerCharacter.IsAlive)
			{
				LocalSummonBase ownSummon = shoichiPassive.LocalPlayerCharacter.GetOwnSummon(summon =>
				{
					if (!summon.IsAlive)
					{
						return false;
					}

					int num = summon.SummonData.code;
					if (!num.Equals(Singleton<ShoichiSkillPassiveData>.inst.PassiveSummonObjectId) ||
					    summonBases.Contains(summon))
					{
						return false;
					}

					num = summon.OwnerId;
					return num.Equals(MonoBehaviourInstance<ClientService>.inst.MyObjectId);
				});
				if (ownSummon != null)
				{
					shoichiPassive.PlaySoundPoint(shoichiPassive.Self, "Shoichi_Passive_Dager_Set", 15);
					if (ownSummon.OwnerId.Equals(MonoBehaviourInstance<ClientService>.inst.MyObjectId))
					{
						GameObject resource = shoichiPassive.Self.LoadEffect("FX_BI_Shoichi_Passive_Range");
						ownSummon.PlayLocalEffectChildManual("FX_BI_Shoichi_Passive_Range", resource, null);
					}

					ownSummon.Pickable.ForcePickableDisable(false);
					summonBases.Add(ownSummon);
				}

				if (summonBases.Count > 0)
				{
					summonBases.RemoveAll(d => !d.IsAlive);
				}

				int stateStackByGroup = shoichiPassive.GetStateStackByGroup(shoichiPassive.Self,
					Singleton<ShoichiSkillPassiveData>.inst.BuffStateGroup, shoichiPassive.Self.ObjectId);
				if (prevCount != stateStackByGroup &&
				    stateStackByGroup >= Singleton<ShoichiSkillPassiveData>.inst.BuffMaxStackCount)
				{
					shoichiPassive.PlaySoundPoint(shoichiPassive.Self, "Shoichi_Passive_Full", 15);
				}

				prevCount = prevCount != stateStackByGroup ? stateStackByGroup : prevCount;
				yield return new WaitForFixedUpdate();
			}

			// co: dotPeek
			// List<LocalSummonBase> summonBases = new List<LocalSummonBase>();
			// int prevCount = 0;
			// Func<LocalSummonBase, bool> <>9__0;
			// while (base.LocalPlayerCharacter.IsAlive)
			// {
			// 	LocalPlayerCharacter localPlayerCharacter = base.LocalPlayerCharacter;
			// 	Func<LocalSummonBase, bool> condition;
			// 	if ((condition = <>9__0) == null)
			// 	{
			// 		condition = (<>9__0 = ((LocalSummonBase summon) => summon.IsAlive && summon.SummonData.code.Equals(Singleton<ShoichiSkillPassiveData>.inst.PassiveSummonObjectId) && !summonBases.Contains(summon) && summon.OwnerId.Equals(MonoBehaviourInstance<ClientService>.inst.MyObjectId)));
			// 	}
			// 	LocalSummonBase ownSummon = localPlayerCharacter.GetOwnSummon(condition);
			// 	if (ownSummon != null)
			// 	{
			// 		base.PlaySoundPoint(base.Self, "Shoichi_Passive_Dager_Set", 15);
			// 		if (ownSummon.OwnerId.Equals(MonoBehaviourInstance<ClientService>.inst.MyObjectId))
			// 		{
			// 			GameObject resource = base.Self.LoadEffect("FX_BI_Shoichi_Passive_Range");
			// 			ownSummon.PlayLocalEffectChildManual("FX_BI_Shoichi_Passive_Range", resource, null, true);
			// 		}
			// 		ownSummon.Pickable.ForcePickableDisable(false);
			// 		summonBases.Add(ownSummon);
			// 	}
			// 	if (summonBases.Count > 0)
			// 	{
			// 		summonBases.RemoveAll((LocalSummonBase d) => !d.IsAlive);
			// 	}
			// 	int stateStackByGroup = base.GetStateStackByGroup(base.Self, Singleton<ShoichiSkillPassiveData>.inst.BuffStateGroup, base.Self.ObjectId);
			// 	if (prevCount != stateStackByGroup && stateStackByGroup >= Singleton<ShoichiSkillPassiveData>.inst.BuffMaxStackCount)
			// 	{
			// 		base.PlaySoundPoint(base.Self, "Shoichi_Passive_Full", 15);
			// 	}
			// 	prevCount = ((prevCount != stateStackByGroup) ? stateStackByGroup : prevCount);
			// 	yield return new WaitForFixedUpdate();
			// }
			// yield break;
		}
	}
}