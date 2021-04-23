using Newtonsoft.Json;

namespace Blis.Common
{
	public class User : UserAsset
	{
		private RankingTierChangeType afterTierChangeType;
		private RankingTierGrade afterTierGrade;
		private RankingTierType afterTierType;
		private bool batchMode;
		private RankingTierChangeType beforeTierChangeType;
		private RankingTierGrade beforeTierGrade;
		private RankingTierType beforeTierType;
		private string currency;
		private int freeNicknameChange;
		private int gainAP;
		private int gainMMR;
		private int gainXP;
		private bool lastBatchMode;

		[JsonProperty("lv")] private int level;
		private int mmr;
		[JsonProperty("ne")] private int needExp;
		[JsonProperty("nn")] private string nickname;
		[JsonProperty("tp")] private int tutorialProgress;

		[JsonConstructor]
		public User(long userNum, string nickname, int activeCharacterNum, int tutorialProgress, int level, int needExp)
		{
			this.UserNum = userNum;
			this.nickname = nickname;
			this.ActiveCharacterNum = activeCharacterNum;
			this.tutorialProgress = tutorialProgress;
			this.level = level;
			this.needExp = needExp;
		}

		[JsonIgnore]
		[field: JsonProperty("un")]
		public long UserNum { get; }

		[JsonIgnore] public string Nickname => nickname;

		[JsonIgnore]
		[field: JsonProperty("acn")]
		public int ActiveCharacterNum { get; }

		[JsonIgnore] public int Level => level;

		[JsonIgnore] public int NeedXP => needExp;


		public int MMR => mmr;
		public int GainMMR => gainMMR;
		public int GainAP => gainAP;
		public int GainXP => gainXP;
		public string Currency => currency;
		public bool HaveFreeNicknameChange => freeNicknameChange > 0;
		public bool BatchMode => batchMode;
		public bool LastBatchMode => lastBatchMode;
		public RankingTierChangeType BeforeTierChangeType => beforeTierChangeType;
		public RankingTierChangeType AfterTierChangeType => afterTierChangeType;
		public RankingTierType BeforeTierType => beforeTierType;
		public RankingTierGrade BeforeTierGrade => beforeTierGrade;
		public RankingTierType AfterTierType => afterTierType;
		public RankingTierGrade AfterTierGrade => afterTierGrade;

		public void SetNickname(string nickname)
		{
			this.nickname = nickname;
		}

		public void SetUserLevel(int level)
		{
			this.level = level;
		}

		public void SetUserNeedExp(int needExp)
		{
			this.needExp = needExp;
		}

		public void SetUserMMR(int mmr)
		{
			this.mmr = mmr;
		}

		public void SetUserGainMMR(int gainMMR)
		{
			this.gainMMR = gainMMR;
		}

		public void SetUserGainAP(int gainAP)
		{
			this.gainAP = gainAP;
		}

		public void SetUserGainXP(int gainXP)
		{
			this.gainXP = gainXP;
		}

		public bool GetTutorialClearState(TutorialType tutorialType)
		{
			return ((1 << (tutorialType - TutorialType.BasicGuide)) & tutorialProgress) != 0;
		}

		public void SetCurrency(string currency)
		{
			this.currency = currency;
		}

		public void UseFreeNicknameChange()
		{
			freeNicknameChange--;
		}

		public void SetFreeNicknameChange(int freeNicknameChange)
		{
			this.freeNicknameChange = freeNicknameChange;
		}

		public void SetLastBatchMode(bool lastBatchMode)
		{
			this.lastBatchMode = lastBatchMode;
		}

		public void SetBatchMode(bool batchMode)
		{
			this.batchMode = batchMode;
		}

		public void SetBeforeTierChangeType(RankingTierChangeType beforeTierChangeType)
		{
			this.beforeTierChangeType = beforeTierChangeType;
		}

		public void SetAfterTierChangeType(RankingTierChangeType afterTierChangeType)
		{
			this.afterTierChangeType = afterTierChangeType;
		}

		public void SetBeforeTierType(RankingTierType rankingTierType)
		{
			beforeTierType = rankingTierType;
		}

		public void SetBeforeTierGrade(RankingTierGrade rankingTierGrade)
		{
			beforeTierGrade = rankingTierGrade;
		}

		public void SetAfterTierType(RankingTierType rankingTierType)
		{
			afterTierType = rankingTierType;
		}

		public void SetAfterTierGrade(RankingTierGrade rankingTierGrade)
		{
			afterTierGrade = rankingTierGrade;
		}
	}
}