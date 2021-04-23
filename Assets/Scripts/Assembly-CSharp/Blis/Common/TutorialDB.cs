using System.Collections.Generic;
using System.Linq;
using Blis.Client;

namespace Blis.Common
{
	public class TutorialDB
	{
		private readonly Dictionary<int, CharacterSettingData> characterSettingMap =
			new Dictionary<int, CharacterSettingData>();


		private readonly Dictionary<TutorialType, RecommendArea> recommendAreaMap =
			new Dictionary<TutorialType, RecommendArea>();


		private readonly Dictionary<TutorialType, List<RecommendItem>> recommendItemMap =
			new Dictionary<TutorialType, List<RecommendItem>>();


		private readonly Dictionary<TutorialType, TutorialSettingData> settingMap =
			new Dictionary<TutorialType, TutorialSettingData>();


		private readonly Dictionary<int, List<TutorialMessageBoxData>> dicTutorialMessageBoxDatas =
			new Dictionary<int, List<TutorialMessageBoxData>>();


		private readonly List<TutorialDialogueData> tutorialDialogueDataList = new List<TutorialDialogueData>();


		private readonly List<TutorialQuestData> tutorialQuestDataList = new List<TutorialQuestData>();


		private List<TutorialRewardData> tutorialRewardList;

		public TutorialDB()
		{
			InitSettingData();
			int group = 1;
			string title = "Tutorial/Tab/1";
			string subTitle = "TutorialMessageBox/BasicGuide/Step1/Title";
			string desc = "TutorialMessageBox/BasicGuide/Step1/Comment";
			string imgName = "";
			Dictionary<int, TutorialItemDataInfo> dictionary = new Dictionary<int, TutorialItemDataInfo>();
			dictionary[203101] = new TutorialItemDataInfo("현재 지역에서 획득가능", false, true);
			dictionary[101104] = new TutorialItemDataInfo("보유 아이템", true, false);
			AddMessegeBoxData(group, new TutorialMessageBoxData(title, subTitle, desc, imgName, dictionary));
			AddMessegeBoxData(2,
				new TutorialMessageBoxData("Tutorial/Tab/1", "TutorialMessageBox/BasicGuide/Step2/Title",
					"TutorialMessageBox/BasicGuide/Step2/Comment", "Img_Tutorial_Screenshot_03", null));
			AddMessegeBoxData(3,
				new TutorialMessageBoxData("Tutorial/Tab/1", "TutorialMessageBox/BasicGuide/Step3/Title",
					"TutorialMessageBox/BasicGuide/Step3/Comment", "Img_Tutorial_Screenshot_04", null));
			AddMessegeBoxData(4,
				new TutorialMessageBoxData("Tutorial/Tab/2", "TutorialMessageBox/Trace/Step1/Title",
					"TutorialMessageBox/Trace/Step1/Comment", "Img_Tutorial_Screenshot_05", null));
			AddMessegeBoxData(5,
				new TutorialMessageBoxData("Tutorial/Tab/3", "TutorialMessageBox/Hunt/Step1/Title",
					"TutorialMessageBox/Hunt/Step1/Comment", "Img_Tutorial_Screenshot_06", null));
			AddMessegeBoxData(6,
				new TutorialMessageBoxData("Tutorial/Tab/3", "TutorialMessageBox/Hunt/Step2/Title",
					"TutorialMessageBox/Hunt/Step2/Comment", "Img_Tutorial_Screenshot_07", null));
			AddMessegeBoxData(7,
				new TutorialMessageBoxData("Tutorial/Tab/3", "TutorialMessageBox/Hunt/Step3/Title",
					"TutorialMessageBox/Hunt/Step3/Comment", "Img_Tutorial_Screenshot_08", null));
			AddMessegeBoxData(8,
				new TutorialMessageBoxData("Tutorial/Tab/4", "TutorialMessageBox/PowerUp/Step1/Title",
					"TutorialMessageBox/PowerUp/Step1/Comment", "Img_Tutorial_Screenshot_09", null));
			AddMessegeBoxData(9,
				new TutorialMessageBoxData("Tutorial/Tab/4", "TutorialMessageBox/PowerUp/Step2/Title",
					"TutorialMessageBox/PowerUp/Step2/Comment", "Img_Tutorial_Screenshot_10", null));
			AddMessegeBoxData(10,
				new TutorialMessageBoxData("Tutorial/Tab/5", "TutorialMessageBox/FinalSurvival/Step1/Title",
					"TutorialMessageBox/FinalSurvival/Step1/Comment", "Img_Tutorial_Screenshot_11", null));
			AddMessegeBoxData(11,
				new TutorialMessageBoxData("Tutorial/Tab/1", "TutorialMessageBox/BasicGuide/Step4/Title",
					"TutorialMessageBox/BasicGuide/Step4/Comment", "Img_Tutorial_Screenshot_11", null));
			tutorialDialogueDataList.Add(new TutorialDialogueData(1, 1, "TutorialDialogue/BasicGuide/1"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(2, 1, "TutorialDialogue/BasicGuide/2"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(3, 1, "TutorialDialogue/BasicGuide/3"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(4, 1, "TutorialDialogue/BasicGuide/4"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(5, 1, "TutorialDialogue/BasicGuide/5"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(6, 1, "TutorialDialogue/BasicGuide/6"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(7, 1, "TutorialDialogue/BasicGuide/7"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(8, 1, "TutorialDialogue/BasicGuide/8"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(9, 1, "TutorialDialogue/BasicGuide/9"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(10, 1, "TutorialDialogue/BasicGuide/10"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(11, 1, "TutorialDialogue/BasicGuide/11"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(12, 1, "TutorialDialogue/BasicGuide/12"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(13, 1, "TutorialDialogue/BasicGuide/13"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(14, 1, "TutorialDialogue/BasicGuide/14"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(15, 1, "TutorialDialogue/Trace/1"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(16, 1, "TutorialDialogue/Trace/2"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(17, 1, "TutorialDialogue/Trace/3"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(18, 1, "TutorialDialogue/Trace/4"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(19, 1, "TutorialDialogue/Trace/5"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(20, 1, "TutorialDialogue/Trace/6"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(21, 1, "TutorialDialogue/Trace/7"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(22, 2, "TutorialDialogue/Hunt/1"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(23, 2, "TutorialDialogue/Hunt/2"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(24, 1, "TutorialDialogue/PowerUp/1"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(25, 1, "TutorialDialogue/PowerUp/2"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(26, 1, "TutorialDialogue/PowerUp/3"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(27, 1, "TutorialDialogue/PowerUp/4"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(28, 1, "TutorialDialogue/PowerUp/5"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(29, 1, "TutorialDialogue/PowerUp/6"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(30, 2, "TutorialDialogue/FinalSurvival/1"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(31, 2, "TutorialDialogue/FinalSurvival/2"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(32, 2, "TutorialDialogue/FinalSurvival/3"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(33, 2, "TutorialDialogue/FinalSurvival/4"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(34, 2, "TutorialDialogue/FinalSurvival/5"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(35, 1, "TutorialDialogue/BasicGuide/15"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(36, 1, "TutorialDialogue/BasicGuide/16"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(37, 1, "TutorialDialogue/PowerUp/7"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(38, 1, "TutorialDialogue/PowerUp/8"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(39, 1, "TutorialDialogue/PowerUp/9"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(40, 1, "TutorialDialogue/PowerUp/10"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(41, 1, "TutorialDialogue/PowerUp/11"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(42, 1, "TutorialDialogue/PowerUp/12"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(43, 1, "TutorialDialogue/PowerUp/13"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(44, 1, "TutorialDialogue/PowerUp/14"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(45, 1, "TutorialDialogue/PowerUp/15"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(46, 1, "TutorialDialogue/PowerUp/16"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(47, 2, "TutorialDialogue/FinalSurvival/6"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(48, 2, "TutorialDialogue/FinalSurvival/7"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(49, 2, "TutorialDialogue/FinalSurvival/8"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(50, 1, "TutorialDialogue/PowerUp/17"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(51, 1, "TutorialDialogue/PowerUp/18",
				"Img_Tutorial_Screenshot_10"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(52, 1, "TutorialDialogue/PowerUp/19"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(53, 1, "TutorialDialogue/PowerUp/20"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(54, 1, "TutorialDialogue/PowerUp/21"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(55, 1, "TutorialDialogue/PowerUp/22"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(56, 1, "TutorialDialogue/PowerUp/23"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(57, 2, "TutorialDialogue/FinalSurvival/9"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(58, 2, "TutorialDialogue/FinalSurvival/10"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(59, 2, "TutorialDialogue/FinalSurvival/11"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(60, 2, "TutorialDialogue/FinalSurvival/12"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(61, 2, "TutorialDialogue/FinalSurvival/13"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(62, 2, "TutorialDialogue/FinalSurvival/14"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(63, 2, "TutorialDialogue/FinalSurvival/15"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(64, 2, "TutorialDialogue/FinalSurvival/16"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(65, 2, "TutorialDialogue/FinalSurvival/17"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(66, 2, "TutorialDialogue/FinalSurvival/18"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(67, 2, "TutorialDialogue/FinalSurvival/19"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(68, 2, "TutorialDialogue/FinalSurvival/20"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(69, 1, "TutorialDialogue/PowerUp/24"));
			tutorialDialogueDataList.Add(new TutorialDialogueData(70, 1, "TutorialDialogue/PowerUp/25",
				"Img_Tutorial_Screenshot_12"));
		}


		public void InitQuestDataListBasicGuide()
		{
			tutorialQuestDataList.Clear();
			tutorialQuestDataList.Add(new TutorialQuestData(1,
				new TutorialMainQuestData(TutorialType.BasicGuide, 0, 1, "TutorialQuest/BasicGuide/Main/Title/1",
					"TutorialQuest/BasicGuide/Main/Comment/1", 1), new List<TutorialSubQuestData>
				{
					new TutorialSubQuestData(1, "TutorialQuest/BasicGuide/Sub/Title/1", 2, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.GainScrapMetal, false),
						new TutorialQuestInfo(TutorialQuestType.GainBettery, false)
					}),
					new TutorialSubQuestData(2, "TutorialQuest/BasicGuide/Sub/Title/2", -1, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.GainBranch, false)
					}),
					new TutorialSubQuestData(3, "TutorialQuest/BasicGuide/Sub/Title/3", -1, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.GainArmyKnife, false)
					}),
					new TutorialSubQuestData(4, "TutorialQuest/BasicGuide/Sub/Title/4", -1, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.MoveToUptown, false)
					}),
					new TutorialSubQuestData(5, "TutorialQuest/BasicGuide/Sub/Title/5", -1, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.GainPianoWire, false)
					})
				}));
		}


		public void InitQuestDataListTrace()
		{
			tutorialQuestDataList.Clear();
			tutorialQuestDataList.Add(new TutorialQuestData(2,
				new TutorialMainQuestData(TutorialType.Trace, 0, 1, "TutorialQuest/Trace/Main/Title/1",
					"TutorialQuest/Trace/Main/Comment/1", 4), new List<TutorialSubQuestData>
				{
					new TutorialSubQuestData(1, "TutorialQuest/Trace/Sub/Title/1", -1, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.RunCCTV, false)
					})
				}));
		}


		public void InitQuestDataListHunt()
		{
			tutorialQuestDataList.Clear();
			tutorialQuestDataList.Add(new TutorialQuestData(3,
				new TutorialMainQuestData(TutorialType.Hunt, 4, 2, "TutorialQuest/Hunt/Main/Title/1",
					"TutorialQuest/Hunt/Main/Comment/1", 5), new List<TutorialSubQuestData>
				{
					new TutorialSubQuestData(1, "TutorialQuest/Hunt/Sub/Title/1", 6, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.ReloadWeapon, false)
					}),
					new TutorialSubQuestData(2, "TutorialQuest/Hunt/Sub/Title/2", 7, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.Resting, false)
					}),
					new TutorialSubQuestData(3, "TutorialQuest/Hunt/Sub/Title/3", -1, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.OpenMonsterItemBox, false)
					})
				}));
		}


		public void InitQuestDataListPowerUp()
		{
			tutorialQuestDataList.Clear();
			tutorialQuestDataList.Add(new TutorialQuestData(4,
				new TutorialMainQuestData(TutorialType.PowerUp, 2, 1, "TutorialQuest/PowerUp/Main/Title/2",
					"TutorialQuest/PowerUp/Main/Comment/1", 8), new List<TutorialSubQuestData>
				{
					new TutorialSubQuestData(1, "TutorialQuest/PowerUp/Sub/Title/1", 9, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.UseHyperloop, false)
					}),
					new TutorialSubQuestData(2, "TutorialQuest/PowerUp/Sub/Title/2", -1, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.DiscardItem, false)
					}),
					new TutorialSubQuestData(3, "TutorialQuest/Hunt/Sub/Title/2", 7, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.Resting, false)
					})
				}));
		}


		public void InitQuestDataListFinalSurvival()
		{
			tutorialQuestDataList.Clear();
			tutorialQuestDataList.Add(new TutorialQuestData(5,
				new TutorialMainQuestData(TutorialType.FinalSurvival, 0, 2, "TutorialQuest/FinalSurvival/Main/Title/1",
					"TutorialQuest/FinalSurvival/Main/Comment/1", 10), new List<TutorialSubQuestData>
				{
					new TutorialSubQuestData(1, "TutorialQuest/FinalSurvival/Sub/Title/1", -1,
						new List<TutorialQuestInfo>
						{
							new TutorialQuestInfo(TutorialQuestType.OpenMasteryWindow, false)
						}),
					new TutorialSubQuestData(2, "TutorialQuest/FinalSurvival/Sub/Title/2", -1,
						new List<TutorialQuestInfo>
						{
							new TutorialQuestInfo(TutorialQuestType.RifleWeaponMasteryLevelSeven, false)
						}),
					new TutorialSubQuestData(3, "TutorialQuest/FinalSurvival/Sub/Title/3", -1,
						new List<TutorialQuestInfo>
						{
							new TutorialQuestInfo(TutorialQuestType.LearnWeaponSkill, false)
						}),
					new TutorialSubQuestData(4, "TutorialQuest/Hunt/Sub/Title/2", 7, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.Resting, false)
					}),
					new TutorialSubQuestData(5, "TutorialQuest/PowerUp/Sub/Title/2", -1, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.DiscardItem, false)
					}),
					new TutorialSubQuestData(6, "TutorialQuest/FinalSurvival/Sub/Title/4", -1,
						new List<TutorialQuestInfo>
						{
							new TutorialQuestInfo(TutorialQuestType.OpenMapWindow, false)
						}),
					new TutorialSubQuestData(7, "TutorialQuest/Hunt/Sub/Title/1", 6, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.ReloadWeapon, false)
					}),
					new TutorialSubQuestData(8, "TutorialQuest/PowerUp/Sub/Title/1", 9, new List<TutorialQuestInfo>
					{
						new TutorialQuestInfo(TutorialQuestType.UseHyperloop, false)
					}),
					new TutorialSubQuestData(9, "TutorialQuest/FinalSurvival/Sub/Title/5", -1,
						new List<TutorialQuestInfo>
						{
							new TutorialQuestInfo(TutorialQuestType.GainGunpowder, false)
						}),
					new TutorialSubQuestData(10, "TutorialQuest/FinalSurvival/Sub/Title/6", -1,
						new List<TutorialQuestInfo>
						{
							new TutorialQuestInfo(TutorialQuestType.GainPianoWire, false)
						}),
					new TutorialSubQuestData(11, "TutorialQuest/FinalSurvival/Sub/Title/7", -1,
						new List<TutorialQuestInfo>
						{
							new TutorialQuestInfo(TutorialQuestType.GainScrapMetal, false),
							new TutorialQuestInfo(TutorialQuestType.GainBettery, false),
							new TutorialQuestInfo(TutorialQuestType.GainOil, false)
						}),
					new TutorialSubQuestData(12, "TutorialQuest/FinalSurvival/Sub/Title/8", -1,
						new List<TutorialQuestInfo>
						{
							new TutorialQuestInfo(TutorialQuestType.GainGatlingGun, false)
						})
				}));
		}


		public void SetData<T>(List<T> data)
		{
			if (typeof(T) == typeof(TutorialRewardData))
			{
				tutorialRewardList = data.Cast<TutorialRewardData>().ToList<TutorialRewardData>();
			}
		}


		private void AddMessegeBoxData(int group, TutorialMessageBoxData tmData)
		{
			if (dicTutorialMessageBoxDatas.ContainsKey(group))
			{
				dicTutorialMessageBoxDatas[group].Add(tmData);
				return;
			}

			List<TutorialMessageBoxData> list = new List<TutorialMessageBoxData>();
			list.Add(tmData);
			dicTutorialMessageBoxDatas.Add(group, list);
		}


		public TutorialRewardData GetTutorialRewardData(int tutorialKey)
		{
			return tutorialRewardList.Find(x => x.tutorialKey == tutorialKey);
		}


		public List<TutorialMessageBoxData> GetTutorialMessageBoxDatas(int group)
		{
			return dicTutorialMessageBoxDatas[group];
		}


		public TutorialQuestData GetTurorialQuestData(int code)
		{
			return tutorialQuestDataList.Find(x => x.code == code);
		}


		public TutorialDialogueData GetTutorialDialogueData(int code)
		{
			return tutorialDialogueDataList.Find(x => x.code == code);
		}


		private void InitSettingData()
		{
			AddSettingData(TutorialType.BasicGuide, 1, BotDifficulty.TUTORIAL1, new List<int>
				{
					10
				}, DayNight.Day, false, false, false, false, false, false, TutorialFinishConditionType.CraftItem,
				101404, 1,
				new List<int>
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					15,
					16
				});
			AddSettingData(TutorialType.Trace, 2, BotDifficulty.TUTORIAL2, new List<int>
				{
					20,
					21,
					22
				}, DayNight.Day, false, false, false, false, false, false, TutorialFinishConditionType.KillPlayer, 0, 3,
				new List<int>
				{
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15,
					16
				});
			AddSettingData(TutorialType.Hunt, 3, BotDifficulty.NONE, new List<int>(), DayNight.Day, false, false, false,
				false, false, false, TutorialFinishConditionType.KillMonster, 0, 5, new List<int>
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					11,
					12,
					13,
					14,
					15,
					16
				});
			AddSettingData(TutorialType.PowerUp, 4, BotDifficulty.NONE, new List<int>(), DayNight.Day, false, false,
				false, false, false, false, TutorialFinishConditionType.CraftTrackingItem, 0, 3, new List<int>
				{
					1,
					2,
					3,
					5,
					6,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15,
					16
				});
			AddSettingData(TutorialType.FinalSurvival, 5, BotDifficulty.EASY, new List<int>
			{
				50,
				51,
				52,
				53,
				54,
				55,
				56,
				57,
				58,
				59,
				60,
				61,
				62,
				63
			}, DayNight.Day, true, true, false, false, true, true, TutorialFinishConditionType.Win, 0, 1, new List<int>
			{
				16
			});
			AddCharacterSettingData(1, 1, ObjectType.PlayerCharacter, 13, 1, new[]
				{
					13
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 101104, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(2, 1, ObjectType.PlayerCharacter, 1, 1, new[]
				{
					1
				}, 15, 1, 1, 5, 1, 1, 1, 1, 7, 10, 10, 10, 10, 1, 3, 3, 3, 1, 0, 105402, 1, 201409, 202401, 203502, 0,
				205405, 302316, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(3, 2, ObjectType.PlayerCharacter, 10, 1, new[]
				{
					10
				}, 5, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 0, 0, 116101, 1, 201409, 202412, 203408, 204406,
				205404, 302316, 3, 301302, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(4, 1, ObjectType.PlayerCharacter, 7, 1, new[]
				{
					4,
					7
				}, 8, 1, 1, 1, 1, 5, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 1, 1, 102404, 1, 201406, 0, 203405, 204302, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(5, 2, ObjectType.PlayerCharacter, 11, 1, new[]
				{
					11
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 117101, 1, 0, 0, 0, 0, 0, 302110, 2, 301102,
				3,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(10, 4, ObjectType.BotPlayerCharacter, 13, 2, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15,
					16
				}, 20, 10, 10, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 4, 4, 4, 3, 3, 0, 104403, 1, 201406, 0, 203408, 204502, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(20, 6, ObjectType.BotPlayerCharacter, 1, 0, new[]
				{
					1
				}, 12, 12, 12, 1, 1, 20, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 1, 0, 114304, 1, 201409, 202209, 0, 204403,
				205206, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(21, 4, ObjectType.BotPlayerCharacter, 1, 0, new[]
				{
					1
				}, 7, 8, 8, 1, 1, 1, 1, 20, 1, 1, 1, 1, 1, 1, 2, 2, 2, 1, 0, 108301, 1, 201411, 202303, 0, 204403,
				205206,
				302205, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(22, 7, ObjectType.BotPlayerCharacter, 1, 0, new[]
				{
					1
				}, 7, 8, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 1, 0, 110404, 1, 201411, 202413, 203302, 204403,
				205205, 302205, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(50, 1, ObjectType.BotPlayerCharacter, 1, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 105103, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(51, 2, ObjectType.BotPlayerCharacter, 2, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 116101, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(52, 3, ObjectType.BotPlayerCharacter, 3, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 107101, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(53, 4, ObjectType.BotPlayerCharacter, 4, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 104101, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(54, 11, ObjectType.BotPlayerCharacter, 5, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 102101, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(55, 7, ObjectType.BotPlayerCharacter, 6, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 110102, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(56, 1, ObjectType.BotPlayerCharacter, 7, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 101104, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(57, 2, ObjectType.BotPlayerCharacter, 8, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 117101, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(58, 3, ObjectType.BotPlayerCharacter, 9, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 120101, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(59, 4, ObjectType.BotPlayerCharacter, 10, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 108102, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(60, 6, ObjectType.BotPlayerCharacter, 12, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 114101, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(61, 11, ObjectType.BotPlayerCharacter, 13, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 102101, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(62, 1, ObjectType.BotPlayerCharacter, 14, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 101104, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddCharacterSettingData(63, 2, ObjectType.BotPlayerCharacter, 15, 0, new[]
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13,
					14,
					15
				}, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 117101, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			AddRecommendItem(TutorialType.BasicGuide, 1, 1, MasteryType.OneHandSword, RecommendItemType.Key, 101404);
			AddRecommendItem(TutorialType.Hunt, 1, 2, MasteryType.Pistol, RecommendItemType.Key, 116202);
			AddRecommendItem(TutorialType.PowerUp, 1, 1, MasteryType.TwoHandSword, RecommendItemType.Key, 102411);
			AddRecommendItem(TutorialType.PowerUp, 1, 1, MasteryType.TwoHandSword, RecommendItemType.Key, 202303);
			AddRecommendItem(TutorialType.PowerUp, 1, 1, MasteryType.TwoHandSword, RecommendItemType.Key, 204411);
			AddRecommendItem(TutorialType.PowerUp, 1, 1, MasteryType.TwoHandSword, RecommendItemType.Key, 205302);
			AddRecommendItem(TutorialType.FinalSurvival, 1, 2, MasteryType.AssaultRifle, RecommendItemType.Key, 117403);
			AddRecommendItem(TutorialType.FinalSurvival, 1, 2, MasteryType.AssaultRifle, RecommendItemType.Key, 202412);
			AddRecommendItem(TutorialType.FinalSurvival, 1, 2, MasteryType.AssaultRifle, RecommendItemType.Key, 201406);
			AddRecommendItem(TutorialType.FinalSurvival, 1, 2, MasteryType.AssaultRifle, RecommendItemType.Key, 203301);
			AddRecommendItem(TutorialType.FinalSurvival, 1, 2, MasteryType.AssaultRifle, RecommendItemType.Key, 204405);
			AddRecommendItem(TutorialType.FinalSurvival, 1, 2, MasteryType.AssaultRifle, RecommendItemType.Key, 205202);
			AddRecommendArea(TutorialType.BasicGuide, 1, 13, 14, 0, 0, 0, false, 1, MasteryType.OneHandSword);
			AddRecommendArea(TutorialType.Trace, 2, 1, 0, 0, 0, 0, false, 1, MasteryType.Axe);
			AddRecommendArea(TutorialType.Hunt, 3, 10, 0, 0, 0, 0, false, 2, MasteryType.Pistol);
			AddRecommendArea(TutorialType.PowerUp, 4, 7, 4, 0, 0, 0, false, 1, MasteryType.TwoHandSword);
			AddRecommendArea(TutorialType.FinalSurvival, 5, 11, 14, 13, 8, 2, false, 2, MasteryType.AssaultRifle);
		}


		private void AddSettingData(TutorialType type, int playerSettingDataCode, BotDifficulty botDifficulty,
			List<int> botSettingDataCode, DayNight beginDayNight, bool enableAirSupply, bool enableMonsterReSpawn,
			bool enableBattleTimer, bool enableHyperloop, bool enableSecurityConsole, bool enableChangeCameraMode,
			TutorialFinishConditionType finishConditionType, int finishConditionKey, int finishConditionValue,
			List<int> restrictedAreas)
		{
			settingMap.Add(type,
				new TutorialSettingData(type, playerSettingDataCode, botDifficulty, botSettingDataCode, beginDayNight,
					enableAirSupply, enableMonsterReSpawn, enableBattleTimer, enableHyperloop, enableSecurityConsole,
					enableChangeCameraMode, finishConditionType, finishConditionKey, finishConditionValue,
					restrictedAreas));
		}


		private void AddCharacterSettingData(int code, int characterCode, ObjectType objectType, int startingArea,
			int startingIndex, int[] walkableAreaCodes, int characterLevel, int weaponMasteryLevel1,
			int weaponMasteryLevel2, int weaponMasteryLevel3, int weaponMasteryLevel4, int explorationMasteryLevel1,
			int explorationMasteryLevel2, int explorationMasteryLevel3, int explorationMasteryLevel4,
			int survivalMasteryLevel1, int survivalMasteryLevel2, int survivalMasteryLevel3, int survivalMasteryLevel4,
			int skillLevelPassive, int skillLevelActive1, int skillLevelActive2, int skillLevelActive3,
			int skillLevelActive4, int skillPoint, int equipmentWeapon, int equipmentWeaponCount, int equipmentHead,
			int equipmentChest, int equipmentArm, int equipmentLeg, int equipmentTrinket, int inventoryItem1,
			int inventoryItemCount1, int inventoryItem2, int inventoryItemCount2, int inventoryItem3,
			int inventoryItemCount3, int inventoryItem4, int inventoryItemCount4, int inventoryItem5,
			int inventoryItemCount5, int inventoryItem6, int inventoryItemCount6, int inventoryItem7,
			int inventoryItemCount7, int inventoryItem8, int inventoryItemCount8, int inventoryItem9,
			int inventoryItemCount9, int inventoryItem10, int inventoryItemCount10)
		{
			characterSettingMap.Add(code,
				new CharacterSettingData(code, characterCode, objectType, startingArea, startingIndex,
					walkableAreaCodes, characterLevel, weaponMasteryLevel1, weaponMasteryLevel2, weaponMasteryLevel3,
					weaponMasteryLevel4, explorationMasteryLevel1, explorationMasteryLevel2, explorationMasteryLevel3,
					explorationMasteryLevel4, survivalMasteryLevel1, survivalMasteryLevel2, survivalMasteryLevel3,
					survivalMasteryLevel4, skillLevelPassive, skillLevelActive1, skillLevelActive2, skillLevelActive3,
					skillLevelActive4, skillPoint, equipmentWeapon, equipmentWeaponCount, equipmentHead, equipmentChest,
					equipmentArm, equipmentLeg, equipmentTrinket, inventoryItem1, inventoryItemCount1, inventoryItem2,
					inventoryItemCount2, inventoryItem3, inventoryItemCount3, inventoryItem4, inventoryItemCount4,
					inventoryItem5, inventoryItemCount5, inventoryItem6, inventoryItemCount6, inventoryItem7,
					inventoryItemCount7, inventoryItem8, inventoryItemCount8, inventoryItem9, inventoryItemCount9,
					inventoryItem10, inventoryItemCount10));
		}


		private void AddRecommendItem(TutorialType tutorialType, int code, int characterCode, MasteryType mastery,
			RecommendItemType itemType, int itemCode)
		{
			if (!recommendItemMap.ContainsKey(tutorialType))
			{
				recommendItemMap.Add(tutorialType, new List<RecommendItem>());
			}

			recommendItemMap[tutorialType].Add(new RecommendItem(code, characterCode, mastery, itemType, itemCode));
		}


		private void AddRecommendArea(TutorialType tutorialType, int code, int area1Code, int area2Code, int area3Code,
			int area4Code, int area5Code, bool recommend, int characterCode, MasteryType masteryType)
		{
			recommendAreaMap[tutorialType] = new RecommendArea(code, area1Code, area2Code, area3Code, area4Code,
				area5Code, characterCode, masteryType, recommend);
		}


		public TutorialSettingData GetTutorialSettingData(TutorialType tutorialType)
		{
			return settingMap[tutorialType];
		}


		public CharacterSettingData GetCharacterSettingData(int code)
		{
			return characterSettingMap[code];
		}


		public List<ItemData> GetRecommendItems(TutorialType tutorialType, RecommendItemType recommendItemType)
		{
			List<ItemData> list = new List<ItemData>();
			if (recommendItemMap.ContainsKey(tutorialType))
			{
				list.AddRange(from x in recommendItemMap[tutorialType]
					where x.recommendItemType == recommendItemType
					select x
					into recommendItem
					select GameDB.item.FindItemByCode(recommendItem.itemCode)
					into itemData
					where itemData != null
					select itemData);
			}

			return list;
		}


		public RecommendArea GetRecommendArea(TutorialType tutorialType)
		{
			return recommendAreaMap[tutorialType];
		}
	}
}