using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class Lobby
	{
		public const int RankBaseLevel = 20;
		public const int RankCharCount = 3;
		public static Lobby inst;
		private List<BattleUser> battleUsers;
		private List<Character> characterList;
		private float clientTime;
		private bool dailyMissionRefreshFlag;
		private List<UserEmotion> emotionList;
		private List<InventoryApi.UserEmoticonSlot> equipEmotionList;
		private List<int> freeRotation;
		private DateTime normalMatchingPenaltyTime;
		private RankingSeason rankingSeason;
		private DateTime rankMatchingPenaltyTime;
		private DateTime serverTime;
		private List<Skin> skinList;
		private User user;

		public Lobby(User user)
		{
			this.user = user;
			LobbyContext = new LobbyContext();
		}

		public LobbyContext LobbyContext { get; }

		public User User => user;

		public List<Character> CharacterList => characterList;


		public List<Skin> SkinList => skinList;


		public List<UserEmotion> EmotionList => emotionList;


		public List<InventoryApi.UserEmoticonSlot> EquipEmotionList => equipEmotionList;


		public List<int> FreeRotation => freeRotation;


		public bool DailyMissionRefreshFlag => dailyMissionRefreshFlag;


		public static void Init(User user)
		{
			inst = new Lobby(user);
		}


		public void Destroy()
		{
			inst = null;
		}


		public void ResetUser()
		{
			inst.user = null;
		}


		public void SetCharacterList(List<Character> characterList)
		{
			this.characterList = characterList;
		}


		public void AddCharacter(int code)
		{
			characterList.Add(new Character(user.UserNum, code, 0, DateTime.Now, DateTime.Now));
		}


		public bool IsHaveCharacter(int characterCode)
		{
			return characterList.Exists(c => c.characterCode == characterCode);
		}


		public void SetSkinList(List<Skin> skins)
		{
			skinList = new List<Skin>();
			foreach (CharacterSkinData characterSkinData in GameDB.character.GetDefaultSkinDataList())
			{
				AddSkin(characterSkinData.code);
			}

			List<Skin> list = new List<Skin>();
			if (skins != null)
			{
				using (List<Skin>.Enumerator enumerator2 = skins.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Skin skin = enumerator2.Current;
						if (skinList.Find(p => p.skinCode == skin.skinCode) == null)
						{
							list.Add(skin);
						}
					}
				}
			}

			skinList.AddRange(list);
		}


		public void AddSkin(int code)
		{
			skinList.Add(new Skin(user.UserNum, code));
		}


		public bool IsHaveSkin(int skinCode)
		{
			return skinList.Exists(c => c.skinCode == skinCode);
		}


		public void SetEmotionList(List<UserEmotion> emotions)
		{
			emotionList = emotions;
		}


		public bool IsHasEmotion(int emotionCode)
		{
			return emotionList.Find(x => x.emotionCode == emotionCode) != null;
		}


		public void SetEquipEmotionList(List<InventoryApi.UserEmoticonSlot> emotions)
		{
			equipEmotionList = emotions;
		}


		public InventoryApi.UserEmoticonSlot GetEquipEmotionList(EmotionPlateType type)
		{
			if (equipEmotionList == null)
			{
				return null;
			}

			return equipEmotionList.Find(x => x.slotType == type);
		}


		public void SetFreeRotation(List<int> freeRotations)
		{
			freeRotation = freeRotations;
		}


		public void SetRankPromotion(List<BattleUser> battleUsers)
		{
			this.battleUsers = battleUsers;
		}


		public void SetNormalMatchingPenaltyTime(DateTime matchingPenaltyTime)
		{
			normalMatchingPenaltyTime = matchingPenaltyTime.AddSeconds(1.0);
		}


		public void SetRankMatchingPenaltyTime(DateTime matchingPenaltyTime)
		{
			rankMatchingPenaltyTime = matchingPenaltyTime.AddSeconds(1.0);
		}


		public void SetCurrentRankingSeason(RankingSeason rankingSeason)
		{
			this.rankingSeason = rankingSeason;
		}


		public void SetDailyMissionRefreshFlag(bool dailyMissionRefreshFlag)
		{
			this.dailyMissionRefreshFlag = dailyMissionRefreshFlag;
		}


		public void SetServerTime(DateTime serverTime)
		{
			this.serverTime = serverTime;
		}


		public void SetClientTime()
		{
			clientTime = Time.realtimeSinceStartup;
		}


		public bool HasMatchingPenalty(MatchingMode matchingMode)
		{
			if (matchingMode == MatchingMode.Rank)
			{
				return HasMatchingPenaltyRankLevel() || HasMatchingPenaltyRankOpen() ||
				       HasMatchingPenaltyTime(matchingMode);
			}

			return HasMatchingPenaltyTime(matchingMode);
		}


		public bool HasMatchingPenaltyRankLevel()
		{
			return User.Level < 20;
		}


		public bool HasMatchingPenaltyRankCharacter()
		{
			return CharacterList.Count < 3;
		}


		public bool HasMatchingPenaltyRankOpen()
		{
			return rankingSeason == null || rankingSeason.startDtm > GetServerTime() ||
			       rankingSeason.endDtm < GetServerTime();
		}


		public bool HasMatchingPenaltyTime(MatchingMode matchingMode)
		{
			if (matchingMode == MatchingMode.Normal)
			{
				return DateTime.UtcNow < normalMatchingPenaltyTime;
			}

			return matchingMode == MatchingMode.Rank && DateTime.UtcNow < rankMatchingPenaltyTime;
		}


		public string GetMatchingPenaltyTime(MatchingMode matchingMode)
		{
			if (HasMatchingPenaltyTime(matchingMode))
			{
				TimeSpan timeSpan;
				if (matchingMode == MatchingMode.Normal)
				{
					timeSpan = normalMatchingPenaltyTime - GetServerTime();
				}
				else if (matchingMode == MatchingMode.Rank)
				{
					timeSpan = rankMatchingPenaltyTime - GetServerTime();
				}
				// co: dotPeek
				else
				{
					return string.Empty;
				}

				int num = (int) timeSpan.TotalSeconds / 60;
				int num2 = (int) timeSpan.TotalSeconds % 60;
				return string.Format("{0:D2}:{1:D2}", num, num2);
			}

			return "--:--";
		}


		public DateTime GetServerTime()
		{
			float num = Time.realtimeSinceStartup - clientTime;
			return serverTime.AddSeconds(num);
		}


		public RankingTierChangeType GetTierChageTypeMode(MatchingTeamMode matchingTeamMode)
		{
			if (battleUsers != null && battleUsers.Find(p => p.matchingTeamMode == matchingTeamMode) != null)
			{
				return battleUsers.Find(p => p.matchingTeamMode == matchingTeamMode).tierChangeType;
			}

			return RankingTierChangeType.None;
		}


		public bool CheckRankPenalty()
		{
			if (HasMatchingPenaltyRankLevel())
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Format("랭크 레벨 제한", 20), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return true;
			}

			if (HasMatchingPenaltyRankCharacter())
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Format("랭크 캐릭터 제한", 3), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return true;
			}

			if (HasMatchingPenaltyRankOpen())
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("랭크 시즌 종료 설명"), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return true;
			}

			return false;
		}
	}
}