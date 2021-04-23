using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LenoxPassive)]
	public class LenoxPassive : SkillScript
	{
		
		private List<AnglerRewardInfo> anglerRewardItemList;

		
		private WorldPlayerCharacter worldPlayerCharacter;

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Combine(
				inst.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(onFinishNormalAttackEvent));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterOepnItemBoxEvent = (Action<WorldPlayerCharacter, WorldItemBox>) Delegate.Combine(
				inst2.OnAfterOepnItemBoxEvent, new Action<WorldPlayerCharacter, WorldItemBox>(OnAfterOepnItemBoxEvent));
			if (anglerRewardItemList == null)
			{
				anglerRewardItemList = new List<AnglerRewardInfo>();
				Dictionary<ItemGrade, List<AnglerRewardInfo>> dictionary =
					new Dictionary<ItemGrade, List<AnglerRewardInfo>>();
				using (Dictionary<int, ItemData>.ValueCollection.Enumerator enumerator =
					GameDB.item.GetAllItems().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ItemData item = enumerator.Current;
						if (Singleton<LenoxSkillPassiveData>.inst.AnglerRewardItemTypes.Contains(item.itemType) &&
						    !Singleton<LenoxSkillPassiveData>.inst.AnglerRewardExclusionItems.Contains(item.code) &&
						    Singleton<LenoxSkillPassiveData>.inst.AnglerRewardItemProbability.ContainsKey(
							    item.itemGrade))
						{
							if (!dictionary.ContainsKey(item.itemGrade))
							{
								dictionary.Add(item.itemGrade, new List<AnglerRewardInfo>());
							}

							if (dictionary[item.itemGrade].Find(info => info.itemCode.Equals(item.code)) == null)
							{
								dictionary[item.itemGrade].Add(new AnglerRewardInfo(item.code,
									Singleton<LenoxSkillPassiveData>.inst.AnglerRewardItemProbability[item.itemGrade]));
							}
						}
					}
				}

				foreach (KeyValuePair<ItemGrade, List<AnglerRewardInfo>> keyValuePair in dictionary)
				{
					float itemProbability =
						Singleton<LenoxSkillPassiveData>.inst.AnglerRewardItemProbability[keyValuePair.Key] /
						keyValuePair.Value.Count;
					for (int i = 0; i < keyValuePair.Value.Count; i++)
					{
						keyValuePair.Value[i].itemProbability = itemProbability;
					}

					anglerRewardItemList.AddRange(keyValuePair.Value);
				}

				for (int j = 0; j < anglerRewardItemList.Count * 2; j++)
				{
					int index = Random.Range(0, anglerRewardItemList.Count - 1);
					int num = Random.Range(0, anglerRewardItemList.Count - 1);
					if (!index.Equals(num))
					{
						AnglerRewardInfo value = anglerRewardItemList[index];
						anglerRewardItemList[index] = anglerRewardItemList[num];
						anglerRewardItemList[num] = value;
					}
				}

				worldPlayerCharacter = Caster.Character as WorldPlayerCharacter;
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Remove(
				inst.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(onFinishNormalAttackEvent));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterOepnItemBoxEvent = (Action<WorldPlayerCharacter, WorldItemBox>) Delegate.Remove(
				inst2.OnAfterOepnItemBoxEvent, new Action<WorldPlayerCharacter, WorldItemBox>(OnAfterOepnItemBoxEvent));
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			while (Caster.IsAlive)
			{
				yield return WaitForFrame();
			}

			Finish();
		}

		
		private void onFinishNormalAttackEvent(WorldCharacter victim, WorldCharacter attacker)
		{
			if (!attacker.ObjectId.Equals(Caster.ObjectId))
			{
				return;
			}

			if (victim.ObjectType != ObjectType.PlayerCharacter && victim.ObjectType != ObjectType.BotPlayerCharacter)
			{
				return;
			}

			if (Caster.IsHaveStateByGroup(Singleton<LenoxSkillPassiveData>.inst.PassiveCaptainBuffGroup,
				Caster.ObjectId))
			{
				return;
			}

			if (!IsReadySkill(Caster, SkillSlotSet.Passive_1))
			{
				return;
			}

			int shieldAmount = Mathf.FloorToInt(Caster.Character.Stat.MaxHp *
			                                    Singleton<LenoxSkillPassiveData>.inst.PassiveBuffMaxHpRatio);
			ShieldState shieldState = CreateState<ShieldState>(Caster,
				Singleton<LenoxSkillPassiveData>.inst.PassiveCaptainBuffCode[SkillLevel]);
			shieldState.Init(0f, shieldAmount);
			AddState(Caster, shieldState);
		}

		
		public void OnAfterOepnItemBoxEvent(WorldPlayerCharacter playerCharacter, WorldItemBox itemBox)
		{
			int objectId = Caster.Character.ObjectId;
			if (!objectId.Equals(playerCharacter.ObjectId))
			{
				return;
			}

			WorldResourceItemBox worldResourceItemBox;
			if ((worldResourceItemBox = itemBox as WorldResourceItemBox) == null)
			{
				return;
			}

			CollectibleData collectibleData =
				MonoBehaviourInstance<GameService>.inst.CurrentLevel.GetCollectibleData(worldResourceItemBox
					.ResourceDataCode);
			if (collectibleData == null)
			{
				return;
			}

			if (!collectibleData.castingActionType.Equals(CastingActionType.CollectibleOpenSeaFish) &&
			    !collectibleData.castingActionType.Equals(CastingActionType.CollectibleOpenFreshWaterFish))
			{
				return;
			}

			AnglerRewardInfo rewardItemInfo = GetRewardItemInfo(Random.Range(0f, 100f));
			ItemData itemData = GameDB.item.FindItemByCode(rewardItemInfo.itemCode);
			Item item = MonoBehaviourInstance<GameService>.inst.Spawn.CreateItem(itemData, 1);
			if (!worldPlayerCharacter.InventoryHasSpace(item))
			{
				MonoBehaviourInstance<GameService>.inst.Spawn.SpawnItem(Caster.Position, item, Caster.Position);
				return;
			}

			worldPlayerCharacter.AddInventoryItem(item, out objectId);
			worldPlayerCharacter.SendInventoryUpdate(UpdateInventoryType.InsertItem);
		}

		
		public AnglerRewardInfo GetRewardItemInfo(float probability)
		{
			float num = 0f;
			for (int i = 0; i < anglerRewardItemList.Count; i++)
			{
				num += anglerRewardItemList[i].itemProbability;
				if (num >= probability)
				{
					return anglerRewardItemList[i];
				}
			}

			return null;
		}

		
		public class AnglerRewardInfo
		{
			
			public int itemCode;

			
			public float itemProbability;

			
			public AnglerRewardInfo(int code, float probability)
			{
				itemCode = code;
				itemProbability = probability;
			}
		}
	}
}