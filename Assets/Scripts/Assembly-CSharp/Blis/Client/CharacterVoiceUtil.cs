using System;
using System.Collections.Generic;
using System.Text;
using Blis.Common;

namespace Blis.Client
{
	public static class CharacterVoiceUtil
	{
		private static readonly Dictionary<int, CharacterVoiceType> areaConvertToCharacterVoiceType =
			new Dictionary<int, CharacterVoiceType>();


		private static readonly Dictionary<int, CharacterVoiceType> killConvertToCharacterVoiceType =
			new Dictionary<int, CharacterVoiceType>();


		public static CharacterVoiceType CastingActionTypeConvertToCharVoiceType(CastingActionType castingActionType)
		{
			switch (castingActionType)
			{
				case CastingActionType.ToRest:
					return CharacterVoiceType.Rest;
				case CastingActionType.AirSupplyOpen:
					return CharacterVoiceType.OpenAirSupply;
				case CastingActionType.BoxOpen:
					return CharacterVoiceType.OpenBox;
				case CastingActionType.CollectibleOpenWater:
					return CharacterVoiceType.CollectWater;
				case CastingActionType.CollectibleOpenWood:
					return CharacterVoiceType.CollectWood;
				case CastingActionType.CollectibleOpenStone:
					return CharacterVoiceType.CollectStone;
				case CastingActionType.CollectibleOpenSeaFish:
					return CharacterVoiceType.CollectSeaFish;
				case CastingActionType.CollectibleOpenFreshWaterFish:
					return CharacterVoiceType.CollectFreshWaterFish;
				case CastingActionType.CollectibleOpenPotato:
					return CharacterVoiceType.CollectPotato;
				case CastingActionType.CollectibleOpenTreeOfLife:
					return CharacterVoiceType.CollectLifeoftree;
				case CastingActionType.CraftCommon:
					return CharacterVoiceType.CraftCommon;
				case CastingActionType.CraftUnCommon:
					return CharacterVoiceType.CraftUncommon;
				case CastingActionType.CraftRare:
					return CharacterVoiceType.CraftRare;
				case CastingActionType.CraftEpic:
					return CharacterVoiceType.CraftEpic;
				case CastingActionType.CraftLegend:
					return CharacterVoiceType.CraftLegend;
				case CastingActionType.AreaSecurityCameraSight:
					return CharacterVoiceType.UseSecurityConsol;
				case CastingActionType.Hyperloop:
					return CharacterVoiceType.UseHyperLoop;
			}

			return CharacterVoiceType.None;
		}


		public static CharacterVoiceType MasteryTypeConvertToCharacterVoiceType(MasteryType masteryType)
		{
			switch (masteryType)
			{
				case MasteryType.Glove:
					return CharacterVoiceType.LearnGloveSkill;
				case MasteryType.Tonfa:
					return CharacterVoiceType.LearnTonfaSkill;
				case MasteryType.Bat:
					return CharacterVoiceType.LearnBatSkill;
				case MasteryType.Whip:
					return CharacterVoiceType.LearnWhipSkill;
				case MasteryType.HighAngleFire:
					return CharacterVoiceType.LearnHighAngleFireSkill;
				case MasteryType.DirectFire:
					return CharacterVoiceType.LearnDirectFireSkill;
				case MasteryType.Bow:
					return CharacterVoiceType.LearnBowSkill;
				case MasteryType.CrossBow:
					return CharacterVoiceType.LearnCrossBowSkill;
				case MasteryType.Pistol:
					return CharacterVoiceType.LearnPistolSkill;
				case MasteryType.AssaultRifle:
					return CharacterVoiceType.LearnAssaultRifleSkill;
				case MasteryType.SniperRifle:
					return CharacterVoiceType.LearnSniperRifleSkill;
				case MasteryType.Hammer:
					return CharacterVoiceType.LearnHammerSkill;
				case MasteryType.Axe:
					return CharacterVoiceType.LearnAxeSkill;
				case MasteryType.OneHandSword:
					return CharacterVoiceType.LearnOneHandSwordSkill;
				case MasteryType.TwoHandSword:
					return CharacterVoiceType.LearnTwoHandSwordSkill;
				case MasteryType.DualSword:
					return CharacterVoiceType.LearnDualSwordSkill;
				case MasteryType.Spear:
					return CharacterVoiceType.LearnSpearSkill;
				case MasteryType.Nunchaku:
					return CharacterVoiceType.LearnNunchakuSkill;
				case MasteryType.Rapier:
					return CharacterVoiceType.LearnRapierSkill;
				case MasteryType.Guitar:
					return CharacterVoiceType.LearnGuitarSkill;
			}

			return CharacterVoiceType.None;
		}


		public static CharacterVoiceType EmotionGameInputTypeConvertToCharacterVoiceType(GameInputEvent emotionInput)
		{
			switch (emotionInput)
			{
				case GameInputEvent.Emotion1:
					return CharacterVoiceType.Emotion1;
				case GameInputEvent.Emotion2:
					return CharacterVoiceType.Emotion2;
				case GameInputEvent.Emotion3:
					return CharacterVoiceType.Emotion3;
				case GameInputEvent.Emotion4:
					return CharacterVoiceType.Emotion4;
				case GameInputEvent.Emotion5:
					return CharacterVoiceType.Emotion5;
				case GameInputEvent.Emotion6:
					return CharacterVoiceType.Emotion6;
				default:
					return CharacterVoiceType.None;
			}
		}


		public static string GetCharacterPickVoice(string voicePath, string characterResource)
		{
			string voiceCountryCode = Singleton<LocalSetting>.inst.setting.voiceCountryCode;
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			stringBuilder.Append(voicePath);
			stringBuilder.Append(voiceCountryCode);
			stringBuilder.Append("/");
			stringBuilder.Append(characterResource);
			stringBuilder.Append("_selected_1_");
			stringBuilder.Append(voiceCountryCode);
			return stringBuilder.ToString();
		}


		public static CharacterVoiceType AreaConvertToCharacterVoiceType(int areaCode)
		{
			if (!areaConvertToCharacterVoiceType.ContainsKey(areaCode))
			{
				string value = string.Format("MoveInArea{0}", areaCode);
				areaConvertToCharacterVoiceType.Add(areaCode,
					(CharacterVoiceType) Enum.Parse(typeof(CharacterVoiceType), value));
			}

			return areaConvertToCharacterVoiceType[areaCode];
		}


		public static CharacterVoiceType RankConvertToCharacterVoiceType(int rank)
		{
			if (rank == 1)
			{
				return CharacterVoiceType.BattleWin;
			}

			if (rank >= 2 && rank <= 7)
			{
				return CharacterVoiceType.BattleLoseHighRank;
			}

			return CharacterVoiceType.BattleLoseLowRank;
		}


		public static CharacterVoiceType KillConvertToCharacterVoiceType(int killCount)
		{
			if (!killConvertToCharacterVoiceType.ContainsKey(killCount))
			{
				string value = string.Format("PlayerKillCount{0}", killCount);
				killConvertToCharacterVoiceType.Add(killCount,
					(CharacterVoiceType) Enum.Parse(typeof(CharacterVoiceType), value));
			}

			return killConvertToCharacterVoiceType[killCount];
		}
	}
}