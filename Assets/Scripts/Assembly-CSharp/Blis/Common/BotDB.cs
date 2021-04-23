using System;
using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class BotDB
	{
		private List<BotAiModelData> botAiModelList;


		private List<BotCraft> botCraftList;


		private List<BotMastery> botMasteryList;


		private List<BotNickNameData> botNickNameDataList;


		private List<BotSkillBuild> botSkillSequenceList;

		public void SetData<T>(List<T> data)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(BotMastery))
			{
				botMasteryList = data.Cast<BotMastery>().ToList<BotMastery>();
				return;
			}

			if (typeFromHandle == typeof(BotCraft))
			{
				botCraftList = data.Cast<BotCraft>().ToList<BotCraft>();
				return;
			}

			if (typeFromHandle == typeof(BotSkillBuild))
			{
				botSkillSequenceList = data.Cast<BotSkillBuild>().ToList<BotSkillBuild>();
				return;
			}

			if (typeFromHandle == typeof(BotNickNameData))
			{
				botNickNameDataList = data.Cast<BotNickNameData>().ToList<BotNickNameData>();
				return;
			}

			if (typeFromHandle == typeof(BotAiModelData))
			{
				botAiModelList = data.Cast<BotAiModelData>().ToList<BotAiModelData>();
			}
		}


		public List<BotMastery> GetBotMasteryBySetPoint(int setPoint)
		{
			return botMasteryList.FindAll(b => b.masterySetPoint == setPoint);
		}


		public List<BotCraft> GetBotCraftBySetPoint(int setPoint)
		{
			return botCraftList.FindAll(b => b.craftSetPoint == setPoint);
		}


		public List<BotSkillBuild> GetBotSkillSequence(int characterCode)
		{
			return botSkillSequenceList.FindAll(b => b.characterCode == characterCode);
		}


		public List<string> GetNickNameList(string serverRegion)
		{
			return (from n in botNickNameDataList
				where n.serverRegion == serverRegion
				select n.botNickName).ToList<string>();
		}


		public bool IsBotName(string nickname)
		{
			return botNickNameDataList.Exists(n => n.botNickName == nickname);
		}


		public List<int> GetCreatableBot()
		{
			return new List<int>
			{
				1,
				2,
				3,
				4,
				6,
				7,
				9,
				10,
				11
			};
		}


		public string GetBotAiModel(string id)
		{
			if (!HasModel(id))
			{
				return "";
			}

			return botAiModelList.First(n => n.id == id).model;
		}


		public bool HasModel(string id)
		{
			return botAiModelList.Exists(n => n.id == id);
		}
	}
}