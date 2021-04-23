using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Blis.Client;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace Blis.Common
{
	public class ResourceManager : SingletonMonoBehaviour<ResourceManager>
	{
		private static readonly string effectCommonPath = "Effects/Common";
		private static readonly string projectileCommonPath = "Projectiles/Common";
		private static readonly string weaponMountCommonPath = "Weapons/Common";
		private static readonly string soundCommonPath = "Sound/FX/Common";
		private static readonly string voiceCommonPath = "Sound/VOICE/Common";
		private static readonly string voiceTutorialPath = "Sound/VOICE/Tutorial";
		private static readonly string objectCommonPath = "Object/Common";
		private static readonly string monsterCommonPath = "Monsters";
		private static readonly string prefabCommonPath = "Prefabs/Common";
		private static readonly string prefabIndicatorPath = "Prefabs/Indicator";
		private static readonly string prefabEmotionPath = "Prefabs/Emotion";
		private static readonly string prefabTutorialPath = "Prefabs/Tutorial";

		private readonly Dictionary<string, SpriteAtlas> atlasMap = new Dictionary<string, SpriteAtlas>();
		private readonly Dictionary<string, Sprite> cachedBackgroundSprites = new Dictionary<string, Sprite>();
		private readonly Dictionary<string, Sprite> cachedCommonSprites = new Dictionary<string, Sprite>();
		private readonly Dictionary<string, Sprite> cachedEmotionSprites = new Dictionary<string, Sprite>();
		private readonly Dictionary<string, Sprite> cachedItemSprites = new Dictionary<string, Sprite>();
		private readonly Dictionary<string, Object> cachedResources = new Dictionary<string, Object>();
		private readonly Dictionary<string, Sprite> cachedSkillSprites = new Dictionary<string, Sprite>();
		private readonly Dictionary<string, Texture2D> dicGetCursorImage = new Dictionary<string, Texture2D>();
		private readonly Dictionary<int, string> GetPathAirSupplyAnnounceSprite = new Dictionary<int, string>();
		private readonly Dictionary<int, string> GetPathAirSupplySprite = new Dictionary<int, string>();
		private readonly Dictionary<ObjectType, string> GetPathClientPrefab =
			new Dictionary<ObjectType, string>(SingletonComparerEnum<ObjectTypeComparer, ObjectType>.Instance);
		private readonly Dictionary<string, string> GetPathDefaultMaterial = new Dictionary<string, string>();
		private readonly Dictionary<string, string> GetPathEffectMaterial = new Dictionary<string, string>();
		private readonly Dictionary<string, string> GetPathFont = new Dictionary<string, string>();
		private readonly Dictionary<int, string> GetPathMarkSprite = new Dictionary<int, string>();
		private readonly Dictionary<int, string> GetPathPingSprite = new Dictionary<int, string>();
		private readonly Dictionary<ObjectType, string> GetPathServerPrefab =
			new Dictionary<ObjectType, string>(SingletonComparerEnum<ObjectTypeComparer, ObjectType>.Instance);
		private readonly Dictionary<int, string> GetPathThrowAmmoWeaponType = new Dictionary<int, string>();
		private readonly Dictionary<int, string> GetPathWeaponMasterySprite = new Dictionary<int, string>();
		private readonly Dictionary<string, Dictionary<string, Object>> loadedCommonResources =
			new Dictionary<string, Dictionary<string, Object>>();
		private readonly Dictionary<string, Dictionary<string, Object>> loadedObjectResources =
			new Dictionary<string, Dictionary<string, Object>>();
		private readonly Dictionary<string, Dictionary<string, Object>> loadedPrefabResources =
			new Dictionary<string, Dictionary<string, Object>>();
		private readonly Dictionary<string, Dictionary<string, Object>> loadedSkinResources =
			new Dictionary<string, Dictionary<string, Object>>();
		private readonly Dictionary<int, string> loadedSkinSpriteSuffixList = new Dictionary<int, string>();

		private CharacterDB characterDB => GameDB.character;
		protected override void OnAwakeSingleton()
		{
			base.OnAwakeSingleton();
			DontDestroyOnLoad(this);
		}

		protected override void OnDestroySingleton()
		{
			base.OnDestroySingleton();
		}


		public void UnloadAll()
		{
			cachedResources.Clear();
			GetPathEffectMaterial.Clear();
			GetPathDefaultMaterial.Clear();
			GetPathFont.Clear();
			GetPathWeaponMasterySprite.Clear();
			GetPathMarkSprite.Clear();
			GetPathThrowAmmoWeaponType.Clear();
			loadedCommonResources.Clear();
			loadedObjectResources.Clear();
			loadedPrefabResources.Clear();
			loadedSkinResources.Clear();
			Resources.UnloadUnusedAssets();
		}


		public void LoadInWorld()
		{
			LoadAllAssets<Object>(loadedCommonResources, effectCommonPath);
			LoadAllAssets<Object>(loadedCommonResources, projectileCommonPath);
			LoadAllAssets<Object>(loadedCommonResources, weaponMountCommonPath);
			LoadAllAssets<Object>(loadedCommonResources, soundCommonPath);
			string voiceCountryCode = Singleton<LocalSetting>.inst.setting.voiceCountryCode;
			LoadAllAssets<Object>(loadedCommonResources, voiceCommonPath);
			LoadAllAssets<Object>(loadedCommonResources, Path.Combine(voiceCommonPath, voiceCountryCode));
			LoadAllAssets<Object>(loadedCommonResources, Path.Combine(voiceTutorialPath, voiceCountryCode));
			LoadAllAssets<Object>(loadedObjectResources, objectCommonPath);
			LoadAllAssets<Object>(loadedObjectResources, monsterCommonPath);
			LoadAllAssets<Object>(loadedPrefabResources, prefabCommonPath);
			LoadAllAssets<Object>(loadedPrefabResources, prefabIndicatorPath);
			LoadAllAssets<Object>(loadedPrefabResources, prefabEmotionPath);
			LoadAllAssets<Object>(loadedPrefabResources, prefabTutorialPath);
		}


		public void LoadInLobby()
		{
			LoadAllAssets<Object>(loadedCommonResources, effectCommonPath);
			LoadAllAssets<Object>(loadedCommonResources, soundCommonPath);
			LoadAllAssets<Object>(loadedPrefabResources, prefabCommonPath);
		}


		public void LoadInServer()
		{
			LoadAllAssets<Object>(loadedPrefabResources, prefabCommonPath);
		}


		public void Load(List<CharacterResourceKey> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				Load(list[i]);
			}
		}


		public void Load(CharacterResourceKey target)
		{
			CharacterSkinData skinData = characterDB.GetSkinData(target.characterCode, target.skinIndex);
			if (skinData == null)
			{
				Log.E(string.Format("Not found SkinData : characterCode({0}), skinIndex({1})", target.characterCode,
					target.skinIndex));
				return;
			}

			LoadAllAssets<Object>(loadedSkinResources, skinData.effectsPath);
			LoadAllAssets<Object>(loadedSkinResources, skinData.projectilesPath);
			LoadAllAssets<Object>(loadedSkinResources, skinData.objectPath);
			LoadAllAssets<Object>(loadedSkinResources, skinData.weaponMountPath);
			LoadAllAssets<Object>(loadedSkinResources, skinData.fxSoundPath);
		}


		private static void LoadAllAssets<T>(Dictionary<string, Dictionary<string, T>> target, string path)
			where T : Object
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			Dictionary<string, T> dictionary;
			if (!target.TryGetValue(path, out dictionary))
			{
				dictionary = new Dictionary<string, T>();
				T[] array = Resources.LoadAll<T>(path);
				for (int i = 0; i < array.Length; i++)
				{
					dictionary[array[i].name] = array[i];
				}

				target[path] = dictionary;
			}
		}


		private static T LoadAsset<T>(Dictionary<string, Dictionary<string, T>> target, string path, string name)
			where T : Object
		{
			if (string.IsNullOrEmpty(path))
			{
				return default;
			}

			Dictionary<string, T> dictionary;
			if (!target.TryGetValue(path, out dictionary))
			{
				dictionary = new Dictionary<string, T>();
				target[path] = dictionary;
			}

			T t;
			if (!dictionary.TryGetValue(name, out t))
			{
				string path2 = Path.Combine(path, name);
				try
				{
					t = Resources.Load<T>(path2);
					dictionary[t.name] = t;
				}
				catch { }
			}

			return t;
		}


		public void LoadAtlas(string path)
		{
			Object[] array = Resources.LoadAll(path, typeof(SpriteAtlas));
			for (int i = 0; i < array.Length; i++)
			{
				if (!atlasMap.ContainsKey(array[i].name))
				{
					atlasMap[array[i].name] = (SpriteAtlas) array[i];
				}
			}
		}


		private T LoadResource<T>(string path) where T : Object
		{
			if (!cachedResources.ContainsKey(path))
			{
				cachedResources[path] = Resources.Load<T>(path);
			}

			return (T) cachedResources[path];
		}


		public GameObject LoadPlayerCharacter(int characterCode, int skinIndex)
		{
			string modelName = GetModelName(characterCode, skinIndex);
			CharacterResourceKey target = new CharacterResourceKey
			{
				characterCode = characterCode,
				skinIndex = skinIndex
			};
			Load(target);
			return LoadResource<GameObject>(modelName);
		}


		public GameObject LoadLobbyCharacter(int characterCode, int skinIndex)
		{
			string lobbyModelName = GetLobbyModelName(characterCode, skinIndex);
			CharacterResourceKey target = new CharacterResourceKey
			{
				characterCode = characterCode,
				skinIndex = skinIndex
			};
			Load(target);
			return LoadResource<GameObject>(lobbyModelName);
		}


		private string GetModelName(int characterCode, int skinIndex)
		{
			CharacterData characterData = characterDB.GetCharacterData(characterCode);
			CharacterSkinData skinData = characterDB.GetSkinData(characterCode, skinIndex);
			if (skinData.index == 0)
			{
				return "Characters/" + characterData.resource + "/" + characterData.resource;
			}

			return string.Concat("Characters/", characterData.resource, "/", characterData.resource, "_",
				skinData.SkinSuffix);
		}


		private string GetLobbyModelName(int characterCode, int skinIndex)
		{
			CharacterData characterData = characterDB.GetCharacterData(characterCode);
			CharacterSkinData skinData = characterDB.GetSkinData(characterCode, skinIndex);
			if (skinData.index == 0)
			{
				return "Characters/" + characterData.resource + "/L_" + characterData.resource;
			}

			return string.Concat("Characters/", characterData.resource, "/L_", characterData.resource, "_",
				skinData.SkinSuffix);
		}


		public GameObject LoadProjectile(int characterCode, int skinIndex, string projectileName)
		{
			if (string.IsNullOrEmpty(projectileName) || characterCode == 0)
			{
				return null;
			}

			CharacterSkinData skinData = characterDB.GetSkinData(characterCode, skinIndex);
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedSkinResources.TryGetValue(skinData.projectilesPath, out dictionary) &&
			    dictionary.TryGetValue(projectileName, out @object))
			{
				return (GameObject) @object;
			}

			@object = LoadProjectile(projectileName, false);
			if (@object == null)
			{
				@object = LoadAsset<Object>(loadedSkinResources, skinData.projectilesPath,
					projectileName) as GameObject;
			}

			return (GameObject) @object;
		}


		public GameObject LoadObject(int characterCode, int skinIndex, string objectName)
		{
			if (string.IsNullOrEmpty(objectName) || characterCode == 0)
			{
				return null;
			}

			CharacterSkinData skinData = characterDB.GetSkinData(characterCode, skinIndex);
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedSkinResources.TryGetValue(skinData.objectPath, out dictionary) &&
			    dictionary.TryGetValue(objectName, out @object))
			{
				return (GameObject) @object;
			}

			@object = LoadObject(objectName, false);
			if (@object == null)
			{
				@object = LoadAsset<Object>(loadedSkinResources, skinData.objectPath, objectName) as GameObject;
			}

			return (GameObject) @object;
		}


		public GameObject LoadEffect(int characterCode, int skinIndex, string effectName)
		{
			if (string.IsNullOrEmpty(effectName) || characterCode == 0)
			{
				return null;
			}

			CharacterSkinData skinData = characterDB.GetSkinData(characterCode, skinIndex);
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedSkinResources.TryGetValue(skinData.effectsPath, out dictionary) &&
			    dictionary.TryGetValue(effectName, out @object))
			{
				return (GameObject) @object;
			}

			@object = LoadEffect(effectName, false);
			if (@object == null)
			{
				@object = LoadAsset<Object>(loadedSkinResources, skinData.effectsPath, effectName) as GameObject;
			}

			return (GameObject) @object;
		}


		public GameObject LoadWeaponMount(int characterCode, int skinIndex, string weaponMountName)
		{
			if (string.IsNullOrEmpty(weaponMountName) || characterCode == 0)
			{
				return null;
			}

			CharacterSkinData skinData = characterDB.GetSkinData(characterCode, skinIndex);
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedSkinResources.TryGetValue(skinData.weaponMountPath, out dictionary) &&
			    dictionary.TryGetValue(weaponMountName, out @object))
			{
				return (GameObject) @object;
			}

			if (loadedSkinResources.TryGetValue(skinData.weaponMountCommonPath, out dictionary) &&
			    dictionary.TryGetValue(weaponMountName, out @object))
			{
				return (GameObject) @object;
			}

			@object = LoadWeaponMount(weaponMountName, false);
			if (@object == null)
			{
				@object =
					LoadAsset<Object>(loadedSkinResources, skinData.weaponMountPath, weaponMountName) as GameObject;
				if (@object == null)
				{
					@object =
						LoadAsset<Object>(loadedSkinResources, skinData.weaponMountCommonPath, weaponMountName) as
							RuntimeAnimatorController;
				}
			}

			return (GameObject) @object;
		}


		public RuntimeAnimatorController LoadWeaponMountAnimator(int characterCode, int skinIndex, string animatorName)
		{
			if (string.IsNullOrEmpty(animatorName) || characterCode == 0)
			{
				return null;
			}

			CharacterSkinData skinData = characterDB.GetSkinData(characterCode, skinIndex);
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedSkinResources.TryGetValue(skinData.weaponMountPath, out dictionary) &&
			    dictionary.TryGetValue(animatorName, out @object))
			{
				return (RuntimeAnimatorController) @object;
			}

			if (loadedSkinResources.TryGetValue(skinData.weaponMountCommonPath, out dictionary) &&
			    dictionary.TryGetValue(animatorName, out @object))
			{
				return (RuntimeAnimatorController) @object;
			}

			@object = LoadWeaponMount(animatorName, false);
			if (@object == null)
			{
				@object =
					LoadAsset<Object>(loadedSkinResources, skinData.weaponMountPath, animatorName) as
						RuntimeAnimatorController;
				if (@object == null)
				{
					@object =
						LoadAsset<Object>(loadedSkinResources, skinData.weaponMountCommonPath, animatorName) as
							RuntimeAnimatorController;
				}
			}

			return (RuntimeAnimatorController) @object;
		}


		public AudioClip LoadFXSound(int characterCode, int skinIndex, string fxSoundName)
		{
			if (string.IsNullOrEmpty(fxSoundName) || characterCode == 0)
			{
				return null;
			}

			CharacterSkinData skinData = characterDB.GetSkinData(characterCode, skinIndex);
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedSkinResources.TryGetValue(skinData.fxSoundPath, out dictionary) &&
			    dictionary.TryGetValue(fxSoundName, out @object))
			{
				return (AudioClip) @object;
			}

			@object = LoadFXSound(fxSoundName, false);
			if (@object == null)
			{
				@object = LoadAsset<Object>(loadedSkinResources, skinData.fxSoundPath, fxSoundName) as AudioClip;
			}

			return (AudioClip) @object;
		}


		public AudioClip LoadVoice(int characterCode, int skinIndex, string characterResource, string voiceName,
			int randomCount = 0)
		{
			if (string.IsNullOrEmpty(voiceName) || characterCode == 0)
			{
				return null;
			}

			string voicePath = GetVoicePath(characterCode, skinIndex);
			string characterVoiceName = GetCharacterVoiceName(characterResource, voiceName, randomCount);
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedSkinResources.TryGetValue(voicePath, out dictionary) &&
			    dictionary.TryGetValue(characterVoiceName, out @object))
			{
				return (AudioClip) @object;
			}

			@object = LoadVoice(characterVoiceName, false);
			if (@object == null)
			{
				@object = LoadAsset<Object>(loadedSkinResources, voicePath, characterVoiceName) as AudioClip;
			}

			return (AudioClip) @object;
		}


		private string GetVoicePath(int characterCode, int skinIndex)
		{
			CharacterSkinData skinData = characterDB.GetSkinData(characterCode, skinIndex);
			string voiceCountryCode = Singleton<LocalSetting>.inst.setting.voiceCountryCode;
			return Path.Combine(skinData.voicePath, voiceCountryCode);
		}


		private static string GetCharacterVoiceName(string characterResource, string soundName, int randomCount = 0)
		{
			string voiceCountryCode = Singleton<LocalSetting>.inst.setting.voiceCountryCode;
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			stringBuilder.Append(characterResource);
			stringBuilder.Append("_");
			stringBuilder.Append(soundName);
			stringBuilder.Append("_");
			if (randomCount > 0)
			{
				stringBuilder.Append(randomCount);
				stringBuilder.Append("_");
			}

			stringBuilder.Append(voiceCountryCode);
			return stringBuilder.ToString();
		}


		public GameObject LoadProjectile(string projectileName, bool debug = true)
		{
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedCommonResources.TryGetValue(projectileCommonPath, out dictionary) &&
			    dictionary.TryGetValue(projectileName, out @object))
			{
				return (GameObject) @object;
			}

			return null;
		}


		public GameObject LoadObject(string objectName, bool debug = true)
		{
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedObjectResources.TryGetValue(objectCommonPath, out dictionary) &&
			    dictionary.TryGetValue(objectName, out @object))
			{
				return (GameObject) @object;
			}

			return null;
		}


		public GameObject LoadMonster(string monsterName, bool debug = true)
		{
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedObjectResources.TryGetValue(monsterCommonPath, out dictionary) &&
			    dictionary.TryGetValue(monsterName, out @object))
			{
				return (GameObject) @object;
			}

			return null;
		}


		public GameObject LoadEffect(string effectName, bool debug = true)
		{
			if (string.IsNullOrEmpty(effectName))
			{
				return null;
			}

			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedCommonResources.TryGetValue(effectCommonPath, out dictionary) &&
			    dictionary.TryGetValue(effectName, out @object))
			{
				return (GameObject) @object;
			}

			return null;
		}


		private Object LoadWeaponMount(string resourceName, bool debug = true)
		{
			Dictionary<string, Object> dictionary;
			Object result;
			if (loadedCommonResources.TryGetValue(weaponMountCommonPath, out dictionary) &&
			    dictionary.TryGetValue(resourceName, out result))
			{
				return result;
			}

			return null;
		}


		public AudioClip LoadFXSound(string fxSoundName, bool debug = true)
		{
			if (string.IsNullOrEmpty(fxSoundName))
			{
				return null;
			}

			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedCommonResources.TryGetValue(soundCommonPath, out dictionary) &&
			    dictionary.TryGetValue(fxSoundName, out @object))
			{
				return (AudioClip) @object;
			}

			return null;
		}


		public AudioClip LoadCommonVoice(string voiceName, bool debug = true)
		{
			if (string.IsNullOrEmpty(voiceName))
			{
				return null;
			}

			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedCommonResources.TryGetValue(voiceCommonPath, out dictionary) &&
			    dictionary.TryGetValue(voiceName, out @object))
			{
				return (AudioClip) @object;
			}

			return null;
		}


		public AudioClip LoadVoice(string voiceName, bool debug = true)
		{
			if (string.IsNullOrEmpty(voiceName))
			{
				return null;
			}

			string voiceCountryCode = Singleton<LocalSetting>.inst.setting.voiceCountryCode;
			string key = Path.Combine(voiceCommonPath, voiceCountryCode);
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedCommonResources.TryGetValue(key, out dictionary) &&
			    dictionary.TryGetValue(voiceName, out @object))
			{
				return (AudioClip) @object;
			}

			return null;
		}


		public AudioClip LoadTutorialVoice(string voiceName, bool debug = true)
		{
			if (string.IsNullOrEmpty(voiceName))
			{
				return null;
			}

			string voiceCountryCode = Singleton<LocalSetting>.inst.setting.voiceCountryCode;
			string key = Path.Combine(voiceTutorialPath, voiceCountryCode);
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedCommonResources.TryGetValue(key, out dictionary) &&
			    dictionary.TryGetValue(voiceName, out @object))
			{
				return (AudioClip) @object;
			}

			return null;
		}


		public Material GetEffectMaterial(string materialName)
		{
			string text;
			if (!GetPathEffectMaterial.TryGetValue(materialName, out text))
			{
				text = "Materials/Effect/" + materialName;
				GetPathEffectMaterial.Add(materialName, text);
			}

			return LoadResource<Material>(text);
		}


		public Material GetDefaultMaterial(string materialName)
		{
			string text;
			if (!GetPathDefaultMaterial.TryGetValue(materialName, out text))
			{
				text = "Materials/Default/" + materialName;
				GetPathDefaultMaterial.Add(materialName, text);
			}

			return LoadResource<Material>(text);
		}


		public Font GetFont(string fontName)
		{
			string text;
			if (!GetPathFont.TryGetValue(fontName, out text))
			{
				text = "UI/Fonts/" + fontName;
				GetPathFont.Add(fontName, text);
			}

			return LoadResource<Font>(text);
		}


		public Sprite GetWeaponMasterySprite(WeaponType weaponType)
		{
			int key = (int) weaponType;
			string text;
			if (!GetPathWeaponMasterySprite.TryGetValue(key, out text))
			{
				text = "Ico_Ability_" + weaponType;
				GetPathWeaponMasterySprite.Add(key, text);
			}

			return GetCommonSprite(text);
		}


		public Sprite GetAirSupplyAnnounceSprite(ItemGrade grade)
		{
			int key = (int) grade;
			string text;
			if (!GetPathAirSupplyAnnounceSprite.TryGetValue(key, out text))
			{
				text = "Ico_Map_AirDrop_" + grade + "_01";
				GetPathAirSupplyAnnounceSprite.Add(key, text);
			}

			return GetCommonSprite(text);
		}


		public Sprite GetAirSupplySprite(ItemGrade grade)
		{
			int key = (int) grade;
			string text;
			if (!GetPathAirSupplySprite.TryGetValue(key, out text))
			{
				text = "Ico_Map_AirDrop_" + grade + "_02";
				GetPathAirSupplySprite.Add(key, text);
			}

			return GetCommonSprite(text);
		}


		public Sprite GetCharacterFullSprite(int characterCode, int skinIndex = 0)
		{
			return GetCommonSprite("CharFull_" + GetCharacterSpriteSuffix(characterCode, skinIndex));
		}


		public Sprite GetCharacterProfileSprite(int characterCode, int skinIndex = 0)
		{
			return GetCommonSprite("CharProfile_" + GetCharacterSpriteSuffix(characterCode, skinIndex));
		}


		public Sprite GetCharacterLobbyPortraitSprite(int characterCode, int skinIndex = 0)
		{
			return GetCommonSprite("CharLobby_" + GetCharacterSpriteSuffix(characterCode, skinIndex));
		}


		public Sprite GetCharacterCommunitySprite(int characterCode, int skinIndex = 0)
		{
			return GetCommonSprite("CharCommunity_" + GetCharacterSpriteSuffix(characterCode, skinIndex));
		}


		public Sprite GetCharacterSkinGradeSprite(SkinGrade grade)
		{
			return GetCommonSprite(string.Format("Img_SkinTier_{0:D2}_{1}", (int) grade, grade));
		}


		public Sprite GetCharacterMapSprite(int characterCode, int skinIndex = 0)
		{
			return GetCommonSprite("CharMap_" + GetCharacterSpriteSuffix(characterCode, skinIndex));
		}


		public Sprite GetCharacterScoreSprite(int characterCode, int skinIndex = 0)
		{
			return GetCommonSprite("CharScore_" + GetCharacterSpriteSuffix(characterCode, skinIndex));
		}


		public Sprite GetCharacterResultSprite(int characterCode, int skinIndex = 0)
		{
			return GetCommonSprite("CharResult_" + GetCharacterSpriteSuffix(characterCode, skinIndex));
		}


		public Sprite GetMonsterProfileSprite(int monsterCode)
		{
			return GetCommonSprite("MonsterProfile_" + GetMonsterSpriteSuffix(monsterCode));
		}


		public Sprite GetMonsterIconSprite(int monsterCode)
		{
			MonsterData monsterData = GameDB.monster.GetMonsterData(monsterCode);
			return GetCommonSprite("MonsterIcon_" + monsterData.resource);
		}


		public Sprite GetSummonProfileSprite(int summonCode)
		{
			string summonSpriteSuffix = GetSummonSpriteSuffix(summonCode);
			return GetCommonSprite("SummonProfile_" + summonSpriteSuffix);
		}


		private string GetCharacterSpriteSuffix(int characterCode, int skinIndex)
		{
			CharacterSkinData skinData = characterDB.GetSkinData(characterCode, skinIndex);
			string text = null;
			if (skinData != null && !loadedSkinSpriteSuffixList.TryGetValue(skinData.code, out text))
			{
				CharacterData characterData = characterDB.GetCharacterData(characterCode);
				if (characterData != null)
				{
					text = characterData.resource + "_" + skinData.SkinSuffix;
					loadedSkinSpriteSuffixList[skinData.code] = text;
				}
			}

			return text;
		}


		private static string GetMonsterSpriteSuffix(int monsterCode)
		{
			return GameDB.monster.GetMonsterData(monsterCode).resource;
		}


		private string GetSummonSpriteSuffix(int summonCode)
		{
			return characterDB.GetSummonData(summonCode).prefabPath;
		}


		public Sprite GetMapIconSprite(bool isAlive)
		{
			return GetCommonSprite(isAlive ? "Ico_Map_Monster_Point" : "Ico_Map_DeathPin_05");
		}


		public Sprite GetKillAnnounceWeaponSprite(WeaponType weaponType)
		{
			return GetCommonSprite("Ico_KillNoti_" + weaponType);
		}


		public Sprite GetTrapKillAnnounceWeaponSprite()
		{
			return GetCommonSprite("Ico_KillNoti_Trap");
		}


		public Sprite GetRankTierIconSprite(RankingTierType rankingTier)
		{
			return GetCommonSprite(string.Format("Ico_Tier_{0:D2}_{1}", (int) rankingTier, rankingTier.ToString()));
		}


		public Sprite GetRankTierGradeSprite(RankingTierGrade rankingTierGrade)
		{
			return GetCommonSprite(string.Format("Img_Tier_Number_{0:D2}", (int) rankingTierGrade));
		}


		public Sprite GetPingSprite(PingType type)
		{
			int key = (int) type;
			string text;
			if (!GetPathPingSprite.TryGetValue(key, out text))
			{
				text = "Ico_Map_Ping_" + type;
				GetPathPingSprite.Add(key, text);
			}

			return GetCommonSprite(text);
		}


		public Sprite GetMarkSprite(int teamSlot)
		{
			if (teamSlot == 0)
			{
				teamSlot = 1;
			}

			string text;
			if (!GetPathMarkSprite.TryGetValue(teamSlot, out text))
			{
				text = "Ico_Map_Mark_0" + teamSlot;
				GetPathMarkSprite.Add(teamSlot, text);
			}

			return GetCommonSprite(text);
		}


		public Sprite GetThrowAmmoWeaponType(WeaponType weaponType)
		{
			int key = (int) weaponType;
			string text;
			if (!GetPathThrowAmmoWeaponType.TryGetValue(key, out text))
			{
				text = "Ico_Gage_" + weaponType;
				GetPathThrowAmmoWeaponType.Add(key, text);
			}

			return GetCommonSprite(text);
		}


		public Texture2D GetCursorImage(string name)
		{
			if (!dicGetCursorImage.ContainsKey(name))
			{
				dicGetCursorImage.Add(name, Resources.Load<Texture2D>("UI/Images/Cursor/" + name));
			}

			return dicGetCursorImage[name];
		}


		public Sprite GetBackgroundSprite(string spriteName)
		{
			if (cachedBackgroundSprites.ContainsKey(spriteName) && cachedBackgroundSprites[spriteName] != null)
			{
				return cachedBackgroundSprites[spriteName];
			}

			Sprite sprite = null;
			SpriteAtlas spriteAtlas;
			if (atlasMap.TryGetValue("Background", out spriteAtlas))
			{
				sprite = spriteAtlas.GetSprite(spriteName);
			}

			cachedBackgroundSprites[spriteName] = sprite;
			return sprite;
		}


		public Sprite GetCommonSprite(string spriteName)
		{
			if (cachedCommonSprites.ContainsKey(spriteName) && cachedCommonSprites[spriteName] != null)
			{
				return cachedCommonSprites[spriteName];
			}

			Sprite sprite = null;
			SpriteAtlas spriteAtlas;
			if (atlasMap.TryGetValue("Common_New", out spriteAtlas))
			{
				sprite = spriteAtlas.GetSprite(spriteName);
			}

			cachedCommonSprites[spriteName] = sprite;
			return sprite;
		}


		public Sprite GetItemSprite(string spriteName)
		{
			if (cachedItemSprites.ContainsKey(spriteName) && cachedItemSprites[spriteName] != null)
			{
				return cachedItemSprites[spriteName];
			}

			Sprite sprite = null;
			SpriteAtlas spriteAtlas;
			if (atlasMap.TryGetValue("Items", out spriteAtlas))
			{
				sprite = spriteAtlas.GetSprite(spriteName);
			}

			cachedItemSprites[spriteName] = sprite;
			return sprite;
		}


		public Sprite GetSkillsSprite(string spriteName)
		{
			if (cachedSkillSprites.ContainsKey(spriteName) && cachedSkillSprites[spriteName] != null)
			{
				return cachedSkillSprites[spriteName];
			}

			Sprite sprite = null;
			SpriteAtlas spriteAtlas;
			if (atlasMap.TryGetValue("Skills", out spriteAtlas))
			{
				sprite = spriteAtlas.GetSprite(spriteName);
			}

			cachedSkillSprites[spriteName] = sprite;
			return sprite;
		}


		public Sprite GetEmotionSprite(string spriteName)
		{
			if (cachedEmotionSprites.ContainsKey(spriteName) && cachedEmotionSprites[spriteName] != null)
			{
				return cachedEmotionSprites[spriteName];
			}

			Sprite sprite = null;
			SpriteAtlas spriteAtlas;
			if (atlasMap.TryGetValue("Emotion", out spriteAtlas))
			{
				sprite = spriteAtlas.GetSprite(spriteName);
			}

			cachedEmotionSprites[spriteName] = sprite;
			return sprite;
		}


		public GameObject GetClientPrefab(ObjectType objectType)
		{
			string text;
			if (!GetPathClientPrefab.TryGetValue(objectType, out text))
			{
				text = "WorldObjects/Client/" + objectType;
				GetPathClientPrefab.Add(objectType, text);
			}

			return LoadResource<GameObject>(text);
		}


		public GameObject GetServerPrefab(ObjectType objectType)
		{
			string text;
			if (!GetPathServerPrefab.TryGetValue(objectType, out text))
			{
				text = "WorldObjects/Server/Server" + objectType;
				GetPathServerPrefab.Add(objectType, text);
			}

			return LoadResource<GameObject>(text);
		}


		public GameObject GetServerBotPlayerPrefab()
		{
			return LoadResource<GameObject>("WorldObjects/Server/ServerBotPlayerCharacter");
		}


		public List<T> GetSpawnPoints<T>(string path)
		{
			GameObject gameObject = LoadResource<GameObject>("WorldObjects/SpawnPoint/" + path);
			gameObject.transform.position = Vector3.zero;
			List<T> list = new List<T>();
			gameObject.GetComponentsInChildren<T>(list);
			return list;
		}


		public GameObject LoadPrefab(string prefabName)
		{
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedPrefabResources.TryGetValue(prefabCommonPath, out dictionary) &&
			    dictionary.TryGetValue(prefabName, out @object))
			{
				return (GameObject) @object;
			}

			Log.E("Not found Resource " + prefabName + " in " + prefabCommonPath);
			return null;
		}


		public GameObject LoadIndicatorPrefab(string prefabName)
		{
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedPrefabResources.TryGetValue(prefabIndicatorPath, out dictionary) &&
			    dictionary.TryGetValue(prefabName, out @object))
			{
				return (GameObject) @object;
			}

			Log.E("Not found Resource " + prefabName + " in " + prefabIndicatorPath);
			return null;
		}


		public GameObject LoadEmotionPrefab(string prefabName)
		{
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedPrefabResources.TryGetValue(prefabEmotionPath, out dictionary) &&
			    dictionary.TryGetValue(prefabName, out @object))
			{
				return (GameObject) @object;
			}

			Log.E("Not found Resource " + prefabName + " in " + prefabEmotionPath);
			return null;
		}


		public GameObject LoadTutorialPrefab(string prefabName)
		{
			Dictionary<string, Object> dictionary;
			Object @object;
			if (loadedPrefabResources.TryGetValue(prefabTutorialPath, out dictionary) &&
			    dictionary.TryGetValue(prefabName, out @object))
			{
				return (GameObject) @object;
			}

			Log.E("Not found Resource " + prefabName + " in " + prefabTutorialPath);
			return null;
		}


		public GameObject GetBulletLinePrefab()
		{
			return LoadPrefab("BulletLine");
		}


		public GameObject LoadQuadPrefab()
		{
			return LoadPrefab("Quad");
		}


		public GameObject LoadPointPinPrefab()
		{
			return LoadPrefab("PointPin");
		}


		public GameObject GetAirSupplyItemBox(ItemGrade itemGrade)
		{
			switch (itemGrade)
			{
				case ItemGrade.Uncommon:
					return LoadResource<GameObject>("ItemBox/AirdropBox_01");
				case ItemGrade.Rare:
					return LoadResource<GameObject>("ItemBox/AirdropBox_02");
				case ItemGrade.Epic:
					return LoadResource<GameObject>("ItemBox/AirdropBox_03");
				case ItemGrade.Legend:
					return LoadResource<GameObject>("ItemBox/AirdropBox_04");
				default:
					throw new ArgumentOutOfRangeException("itemGrade", itemGrade, null);
			}
		}


		public Material LoadIndicatorMaterial(string materialName)
		{
			return LoadResource<Material>("Prefabs/Indicator/Materials/" + materialName);
		}
	}
}