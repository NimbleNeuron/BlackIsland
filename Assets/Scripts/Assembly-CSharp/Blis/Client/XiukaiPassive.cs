using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	[SkillScript(SkillId.XiukaiPassive)]
	public class XiukaiPassive : LocalSkillScript
	{
		public override void Start()
		{
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnBeforeMakeItemProcessEvent +=
				OnBeforeMakeItemProcessEvent;
		}


		public Item OnBeforeMakeItemProcessEvent(LocalPlayerCharacter playerCharacter, Item makeItem)
		{
			if (!Self.ObjectId.Equals(playerCharacter.ObjectId))
			{
				return makeItem;
			}

			if (makeItem == null || makeItem.ItemData.itemType != ItemType.Consume)
			{
				return makeItem;
			}

			if (makeItem.ItemData.GetSubTypeData<ItemConsumableData>() == null)
			{
				return makeItem;
			}

			makeItem.AddRecoveryItem(Singleton<XiukaiSkillPassiveData>.inst.AddRecoveryPercent);
			makeItem.SetItemSpecialType(ItemMadeType.XiukaiMade);
			return makeItem;
		}


		public override void Play(int action, LocalObject target, Vector3? targetPosition)
		{
			if (SingletonMonoBehaviour<PlayerController>.inst.IsMe(Self.ObjectId))
			{
				switch (action)
				{
					case 2:
					{
						AudioClip audioClip = Self.LoadFXSound("Xiukai_Passive_Uncommon");
						Singleton<SoundControl>.inst.PlayFXSound(audioClip, "XiukaiPassive", 10, Self.GetPosition(),
							false);
						return;
					}
					case 3:
					{
						AudioClip audioClip2 = Self.LoadFXSound("Xiukai_Passive_rare");
						Singleton<SoundControl>.inst.PlayFXSound(audioClip2, "XiukaiPassive", 10, Self.GetPosition(),
							false);
						return;
					}
					case 4:
					{
						AudioClip audioClip3 = Self.LoadFXSound("Xiukai_Passive_unique");
						Singleton<SoundControl>.inst.PlayFXSound(audioClip3, "XiukaiPassive", 10, Self.GetPosition(),
							false);
						return;
					}
					case 5:
					{
						AudioClip audioClip4 = Self.LoadFXSound("Xiukai_Passive_legend");
						Singleton<SoundControl>.inst.PlayFXSound(audioClip4, "XiukaiPassive", 10, Self.GetPosition(),
							false);
						break;
					}
					default:
						return;
				}
			}
		}


		public override void Finish(bool cancel)
		{
			SingletonMonoBehaviour<LocalBattleEventCollector>.inst.OnBeforeMakeItemProcessEvent -=
				OnBeforeMakeItemProcessEvent;
		}


		public override string GetTooltipValue(SkillData skillData, int index)
		{
			switch (index)
			{
				case 0:
				{
					int addRecoveryPercent = Singleton<XiukaiSkillPassiveData>.inst.AddRecoveryPercent;
					return string.Format("{0}%", addRecoveryPercent);
				}
				case 1:
				{
					CharacterStateData data =
						GameDB.characterState.GetData(Singleton<XiukaiSkillPassiveData>.inst.BuffState);
					return (GetStateStackByGroup(Self, data.group, Self.ObjectId) *
					        Singleton<XiukaiSkillPassiveData>.inst.AddMaxHp).ToString();
				}
				case 2:
					return Singleton<XiukaiSkillPassiveData>.inst.UncommonStack[skillData.level].ToString();
				case 3:
					return Singleton<XiukaiSkillPassiveData>.inst.RareStack[skillData.level].ToString();
				case 4:
					return Singleton<XiukaiSkillPassiveData>.inst.EpicStack[skillData.level].ToString();
				case 5:
					return Singleton<XiukaiSkillPassiveData>.inst.LegendStack[skillData.level].ToString();
				case 6:
				{
					int addMaxHp = Singleton<XiukaiSkillPassiveData>.inst.AddMaxHp;
					return addMaxHp.ToString();
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
					return "ToolTipType/GradeUncommon";
				case 1:
					return "ToolTipType/GradeRare";
				case 2:
					return "ToolTipType/GradeHero";
				default:
					return "";
			}
		}


		public override string GetNextLevelTooltipValue(SkillData skillData, int level, int index)
		{
			switch (index)
			{
				case 0:
					return Singleton<XiukaiSkillPassiveData>.inst.UncommonStack[level].ToString();
				case 1:
					return Singleton<XiukaiSkillPassiveData>.inst.RareStack[level].ToString();
				case 2:
					return Singleton<XiukaiSkillPassiveData>.inst.EpicStack[level].ToString();
				case 3:
					return Singleton<XiukaiSkillPassiveData>.inst.LegendStack[level].ToString();
				default:
					return "";
			}
		}
	}
}