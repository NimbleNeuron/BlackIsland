using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blis.Common
{
	public class GameDBLoader : SingletonMonoBehaviour<GameDBLoader>
	{
		public delegate void JobAction(Action<string> callback);
		private const int MAX_CONCURRENT_JOB = 2;
		private const int LOAD_GAMEDB_TIMEOUT = 30;
		private int concurrentJob;
		private Queue<JobAction> jobQueue;
		private int maxJobCount;
		private GameDBPersist persist;
		private ResultContext result;
		private bool useLocalCache;

		public ResultContext Result => result;


		public Coroutine Load(string dataPath)
		{
			persist = new GameDBPersist(dataPath);
			jobQueue = new Queue<JobAction>();
			maxJobCount = 0;
			concurrentJob = 0;
			useLocalCache = false;
			return StartCoroutine(_Load());
		}


		public Coroutine LoadCache(string dataPath)
		{
			persist = new GameDBPersist(dataPath);
			jobQueue = new Queue<JobAction>();
			maxJobCount = 0;
			concurrentJob = 0;
			useLocalCache = BSERVersion.isDebugBuild;
			return StartCoroutine(_Load());
		}


		private void AddEntry<TypeModel>(string key, Action<List<TypeModel>> callback) where TypeModel : class
		{
			maxJobCount++;
			jobQueue.Enqueue(delegate(Action<string> cb)
			{
				if (useLocalCache)
				{
					persist.LoadFromFile<TypeModel>(key, delegate(string err, List<TypeModel> data)
					{
						concurrentJob--;
						if (err == null)
						{
							callback(data);
						}

						if (cb != null)
						{
							cb(err);
						}
					});
					return;
				}

				persist.Load<TypeModel>(key, delegate(string err, List<TypeModel> data)
				{
					concurrentJob--;
					if (err == null)
					{
						callback(data);
					}

					if (cb != null)
					{
						cb(err);
					}
				});
			});
		}


		private void InitializeEntries()
		{
			AddEntry<ServerRegionData>("ServerRegion",
				delegate(List<ServerRegionData> data) { GameDB.platform.SetData<ServerRegionData>(data); });
			AddEntry<AreaPrimitiveData>("Area",
				delegate(List<AreaPrimitiveData> data) { GameDB.level.SetData<AreaPrimitiveData>(data); });
			AddEntry<RestrictedAreaData>("RestrictedArea",
				delegate(List<RestrictedAreaData> data) { GameDB.level.SetData<RestrictedAreaData>(data); });
			AddEntry<ItemSpawnData>("ItemSpawn",
				delegate(List<ItemSpawnData> data) { GameDB.level.SetData<ItemSpawnData>(data); });
			AddEntry<CollectibleData>("Collectible",
				delegate(List<CollectibleData> data) { GameDB.level.SetData<CollectibleData>(data); });
			AddEntry<NearByAreaData>("NearByArea",
				delegate(List<NearByAreaData> data) { GameDB.level.SetData<NearByAreaData>(data); });
			AddEntry<AreaSoundData>("AreaSound",
				delegate(List<AreaSoundData> data) { GameDB.level.SetData<AreaSoundData>(data); });
			AddEntry<SoundGroupData>("SoundGroup",
				delegate(List<SoundGroupData> data) { GameDB.level.SetData<SoundGroupData>(data); });
			AddEntry<FootstepData>("FootStep",
				delegate(List<FootstepData> data) { GameDB.level.SetData<FootstepData>(data); });
			AddEntry<CharacterData>("Character",
				delegate(List<CharacterData> data) { GameDB.character.SetData<CharacterData>(data); });
			AddEntry<CharacterSkinData>("CharacterSkin",
				delegate(List<CharacterSkinData> data) { GameDB.character.SetData<CharacterSkinData>(data); });
			AddEntry<CharacterExpData>("CharacterExp",
				delegate(List<CharacterExpData> data) { GameDB.character.SetData<CharacterExpData>(data); });
			AddEntry<CharacterLevelUpStatData>("CharacterLevelUpStat",
				delegate(List<CharacterLevelUpStatData> data)
				{
					GameDB.character.SetData<CharacterLevelUpStatData>(data);
				});
			AddEntry<CharacterModeModifierData>("CharacterModeModifier",
				delegate(List<CharacterModeModifierData> data)
				{
					GameDB.character.SetData<CharacterModeModifierData>(data);
				});
			AddEntry<SummonData>("SummonObject",
				delegate(List<SummonData> data) { GameDB.character.SetData<SummonData>(data); });
			AddEntry<ActionCostData>("ActionCost",
				delegate(List<ActionCostData> data) { GameDB.character.SetData<ActionCostData>(data); });
			AddEntry<CriticalChanceData>("CriticalChance",
				delegate(List<CriticalChanceData> data) { GameDB.character.SetData<CriticalChanceData>(data); });
			AddEntry<WeaponMountData>("WeaponMount",
				delegate(List<WeaponMountData> data) { GameDB.character.SetData<WeaponMountData>(data); });
			AddEntry<WeaponAnimatorLayersData>("WeaponAnimatorLayers",
				delegate(List<WeaponAnimatorLayersData> data)
				{
					GameDB.character.SetData<WeaponAnimatorLayersData>(data);
				});
			AddEntry<ItemWeaponData>("ItemWeapon",
				delegate(List<ItemWeaponData> data) { GameDB.item.SetData<ItemWeaponData>(data); });
			AddEntry<ItemArmorData>("ItemArmor",
				delegate(List<ItemArmorData> data) { GameDB.item.SetData<ItemArmorData>(data); });
			AddEntry<ItemConsumableData>("ItemConsumable",
				delegate(List<ItemConsumableData> data) { GameDB.item.SetData<ItemConsumableData>(data); });
			AddEntry<ItemMiscData>("ItemMisc",
				delegate(List<ItemMiscData> data) { GameDB.item.SetData<ItemMiscData>(data); });
			AddEntry<ItemSpecialData>("ItemSpecial",
				delegate(List<ItemSpecialData> data) { GameDB.item.SetData<ItemSpecialData>(data); });
			AddEntry<BulletCapacity>("BulletCapacity",
				delegate(List<BulletCapacity> data) { GameDB.item.SetData<BulletCapacity>(data); });
			AddEntry<ItemOptionCategoryData>("ItemSearchOption",
				delegate(List<ItemOptionCategoryData> data) { GameDB.item.SetData<ItemOptionCategoryData>(data); });
			AddEntry<ItemFindInfo>("HowToFindItem",
				delegate(List<ItemFindInfo> data) { GameDB.item.SetData<ItemFindInfo>(data); });
			AddEntry<CollectAndHuntData>("NaviCollectAndHunt",
				delegate(List<CollectAndHuntData> data) { GameDB.item.SetData<CollectAndHuntData>(data); });
			AddEntry<MasteryLevelData>("MasteryLevel",
				delegate(List<MasteryLevelData> data) { GameDB.mastery.SetData<MasteryLevelData>(data); });
			AddEntry<MasteryExpData>("MasteryExp",
				delegate(List<MasteryExpData> data) { GameDB.mastery.SetData<MasteryExpData>(data); });
			AddEntry<CharacterMasteryData>("CharacterMastery",
				delegate(List<CharacterMasteryData> data) { GameDB.mastery.SetData<CharacterMasteryData>(data); });
			AddEntry<WeaponTypeInfoData>("WeaponTypeInfo",
				delegate(List<WeaponTypeInfoData> data) { GameDB.mastery.SetData<WeaponTypeInfoData>(data); });
			AddEntry<MonsterData>("Monster",
				delegate(List<MonsterData> data) { GameDB.monster.SetData<MonsterData>(data); });
			AddEntry<ItemDropGroupData>("DropGroup",
				delegate(List<ItemDropGroupData> data) { GameDB.monster.SetData<ItemDropGroupData>(data); });
			AddEntry<MonsterLevelUpStatData>("MonsterLevelUpStat",
				delegate(List<MonsterLevelUpStatData> data) { GameDB.monster.SetData<MonsterLevelUpStatData>(data); });
			AddEntry<MonsterSpawnLevelData>("MonsterSpawnLevel",
				delegate(List<MonsterSpawnLevelData> data) { GameDB.monster.SetData<MonsterSpawnLevelData>(data); });
			AddEntry<RecommendStarting>("RecommendedList",
				delegate(List<RecommendStarting> data) { GameDB.recommend.SetData<RecommendStarting>(data); });
			AddEntry<RecommendArea>("RecommendedArea",
				delegate(List<RecommendArea> data) { GameDB.recommend.SetData<RecommendArea>(data); });
			AddEntry<RecommendItem>("RecommendedItemList",
				delegate(List<RecommendItem> data) { GameDB.recommend.SetData<RecommendItem>(data); });
			AddEntry<StartItem>("StartItem",
				delegate(List<StartItem> data) { GameDB.recommend.SetData<StartItem>(data); });
			AddEntry<ProjectileData>("ProjectileSetting",
				delegate(List<ProjectileData> data) { GameDB.projectile.SetData<ProjectileData>(data); });
			AddEntry<HookLineProjectileData>("HookLineProjectile",
				delegate(List<HookLineProjectileData> data)
				{
					GameDB.projectile.SetData<HookLineProjectileData>(data);
				});
			AddEntry<EffectAndSoundData>("EffectAndSound",
				delegate(List<EffectAndSoundData> data) { GameDB.effectAndSound.SetData<EffectAndSoundData>(data); });
			AddEntry<NoiseData>("Noise",
				delegate(List<NoiseData> data) { GameDB.effectAndSound.SetData<NoiseData>(data); });
			AddEntry<SecurityConsoleEventData>("SecurityConsolEvent",
				delegate(List<SecurityConsoleEventData> data)
				{
					GameDB.effectAndSound.SetData<SecurityConsoleEventData>(data);
				});
			AddEntry<BotMastery>("BotMastery",
				delegate(List<BotMastery> data) { GameDB.bot.SetData<BotMastery>(data); });
			AddEntry<BotCraft>("BotCraft", delegate(List<BotCraft> data) { GameDB.bot.SetData<BotCraft>(data); });
			AddEntry<BotSkillBuild>("BotSkillBuild",
				delegate(List<BotSkillBuild> data) { GameDB.bot.SetData<BotSkillBuild>(data); });
			AddEntry<BotNickNameData>("BotNickName",
				delegate(List<BotNickNameData> data) { GameDB.bot.SetData<BotNickNameData>(data); });
			AddEntry<BotAiModelData>("BotAiModel",
				delegate(List<BotAiModelData> data) { GameDB.bot.SetData<BotAiModelData>(data); });
			AddEntry<SkillGroupData>("SkillGroup",
				delegate(List<SkillGroupData> data) { GameDB.skill.SetData<SkillGroupData>(data); });
			AddEntry<SkillData>("Skill", delegate(List<SkillData> data) { GameDB.skill.SetData<SkillData>(data); });
			AddEntry<SkillEvolutionData>("SkillEvolution",
				delegate(List<SkillEvolutionData> data) { GameDB.skill.SetData<SkillEvolutionData>(data); });
			AddEntry<SkillEvolutionPointData>("SkillEvolutionPoint",
				delegate(List<SkillEvolutionPointData> data) { GameDB.skill.SetData<SkillEvolutionPointData>(data); });
			AddEntry<CharacterStateGroupData>("CharacterStateGroup",
				delegate(List<CharacterStateGroupData> data)
				{
					GameDB.characterState.SetData<CharacterStateGroupData>(data);
				});
			AddEntry<CharacterStateData>("CharacterState",
				delegate(List<CharacterStateData> data) { GameDB.characterState.SetData<CharacterStateData>(data); });
			AddEntry<ProductAssetData>("ProductAsset",
				delegate(List<ProductAssetData> data) { GameDB.product.SetData<ProductAssetData>(data); });
			AddEntry<ProductCharacterData>("ProductCharacter",
				delegate(List<ProductCharacterData> data) { GameDB.product.SetData<ProductCharacterData>(data); });
			AddEntry<ProductInstantData>("ProductInstant",
				delegate(List<ProductInstantData> data) { GameDB.product.SetData<ProductInstantData>(data); });
			AddEntry<CharacterVoiceData>("CharacterVoice",
				delegate(List<CharacterVoiceData> data) { GameDB.characterVoice.SetData<CharacterVoiceData>(data); });
			AddEntry<CharacterVoiceRandomCountData>("CharacterVoiceRandomCount",
				delegate(List<CharacterVoiceRandomCountData> data)
				{
					GameDB.characterVoice.SetData<CharacterVoiceRandomCountData>(data);
				});
			AddEntry<TutorialRewardData>("TutorialReward",
				delegate(List<TutorialRewardData> data) { GameDB.tutorial.SetData<TutorialRewardData>(data); });
			AddEntry<UserLevelData>("Level",
				delegate(List<UserLevelData> data) { GameDB.user.SetData<UserLevelData>(data); });
			AddEntry<MissionData>("Mission",
				delegate(List<MissionData> data) { GameDB.mission.SetData<MissionData>(data); });
			AddEntry<EmotionIconData>("Emotion",
				delegate(List<EmotionIconData> data) { GameDB.emotionIcon.SetData<EmotionIconData>(data); });
			AddEntry<CharacterAttributeData>("CharacterAttributes",
				delegate(List<CharacterAttributeData> data)
				{
					GameDB.characterAttibute.SetData<CharacterAttributeData>(data);
				});
			AddEntry<CharacterSkillVideoData>("CharacterSkillVideos",
				delegate(List<CharacterSkillVideoData> data)
				{
					GameDB.characterSkillVideoDB.SetData<CharacterSkillVideoData>(data);
				});
		}


		private IEnumerator _Load()
		{
			if (!persist.isInitialized)
			{
				yield return persist.Init(err =>
				{
					if (err != null)
					{
						result = new ResultContext(false, err);
					}
					else
					{
						result = new ResultContext(true, "");
					}
				});
				if (!result.success)
				{
					yield break;
				}
			}

			InitializeEntries();
			DateTime lastTime = DateTime.Now;
			
			bool lastReseult = true;
			string lastErr = "";
			while (true)
			{
				int maxJobCount = this.maxJobCount;
				int count = jobQueue.Count;
				if (concurrentJob != 0 || jobQueue.Count != 0)
				{
					if ((DateTime.Now - lastTime).TotalSeconds <= 30.0)
					{
						if (concurrentJob < 2 && jobQueue.Count > 0)
						{
							++concurrentJob;
							jobQueue.Dequeue()(err =>
							{
								if (err == null)
								{
									return;
								}

								lastReseult = false;
								lastErr = err;
							});
							lastTime = DateTime.Now;
						}

						if (lastReseult)
						{
							yield return null;
						}
						else
						{
							Log.E("[GameDB] Failed to load entry");
							result = new ResultContext(false, lastErr);
							
							yield break;
						}
					}
					else
					{
						Log.E("[GameDB] Load Timeout");
						result = new ResultContext(false, "Timeout");

						yield break;
					}
				}
				else
				{
					break;
				}
			}

			persist.SaveVersionData();
			GameDB.PostInitialize();
			result = new ResultContext(true, "");
		}


		public class ResultContext
		{
			public readonly string reason;


			public readonly bool success;


			public ResultContext(bool success, string reason)
			{
				this.success = success;
				this.reason = reason;
			}
		}
	}
}