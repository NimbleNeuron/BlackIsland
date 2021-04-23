using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Blis.Client
{
	public static class AnnounceVoice
	{
		private static readonly List<string> deadPlayerVoices = new List<string>
		{
			"Announce_Player_Kill_01",
			"Announce_Player_Kill_02"
		};


		private static readonly List<string> deadPlayerVoices3 = new List<string>
		{
			"Announce_Player_3Over_Kill_01",
			"Announce_Player_3Over_Kill_02"
		};


		private static readonly List<string> deadPlayerVoices5 = new List<string>
		{
			"Announce_Player_5Over_Kill_01",
			"Announce_Player_5Over_Kill_02"
		};


		private static readonly List<string> deadPlayerVoices7 = new List<string>
		{
			"Announce_Player_7Over_Kill_01",
			"Announce_Player_7Over_Kill_02"
		};


		private static readonly List<string> deadPlayerVoices8 = new List<string>
		{
			"Announce_Player_8Over_Kill_01",
			"Announce_Player_8Over_Kill_02"
		};


		private static readonly List<string> alivePlayerVoice2 = new List<string>
		{
			"Announcer_2_survivors_01",
			"Announcer_2_survivors_02"
		};


		private static readonly List<string> alivePlayerVoice3 = new List<string>
		{
			"Announcer_3_survivors_01",
			"Announcer_3_survivors_02"
		};


		private static readonly List<string> alivePlayerVoice5 = new List<string>
		{
			"Announcer_5_survivors_01",
			"Announcer_5_survivors_02"
		};


		private static readonly List<string> alivePlayerVoice10 = new List<string>
		{
			"Announcer_10_survivors_01",
			"Announcer_10_survivors_02"
		};


		private static readonly List<string> startGameVoices = new List<string>
		{
			"Announce_Start_Game_01",
			"Announce_Start_Game_02"
		};


		private static readonly List<string> winGameVoices = new List<string>
		{
			"Announce_Win_Game_01",
			"Announce_Win_Game_02"
		};


		private static readonly List<string> airSupplyNoticeVoices = new List<string>
		{
			"Announce_AirSupply_Ready_01",
			"Announce_AirSupply_Ready_02"
		};


		private static readonly List<string> airSupplyVoices = new List<string>
		{
			"Announce_AirSupply_Drop_01",
			"Announce_AirSupply_Drop_02"
		};


		private static readonly List<string> restrictVoices = new List<string>
		{
			"Announce_Restricted Area_01",
			"Announce_Restricted Area_02"
		};


		private static readonly List<string> restrictAccelVoices = new List<string>
		{
			"Announce_Restricted_Area_Fasten_01",
			"Announce_Restricted_Area_Fasten_02"
		};


		private static readonly List<string> finalRestrictVoices = new List<string>
		{
			"Announce_Final_Restrict_01",
			"Announce_Final_Restrict_02"
		};


		private static readonly List<string> finalSafeActiveVoices = new List<string>
		{
			"Announce_Final_Safe_Active_01",
			"Announce_Final_Safe_Active_02"
		};


		private static readonly List<string> finalSafePreDeactiveVoices = new List<string>
		{
			"Announce_Final_Pre_Deactive_01",
			"Announce_Final_Pre_Deactive_02",
			"Announce_Final_Pre_Deactive_03"
		};


		private static readonly List<string> finalSafeDeactiveVoices = new List<string>
		{
			"Announce_Final_Safe_Deactive_01",
			"Announce_Final_Safe_Deactive_02"
		};


		private static readonly List<string> tutorialVoices = new List<string>
		{
			"Announce_Tutorial_01",
			"Announce_Tutorial_02",
			"Announce_Tutorial_03",
			"Announce_Tutorial_04",
			"Announce_Tutorial_05",
			"Announce_Tutorial_06",
			"Announce_Tutorial_07",
			"Announce_Tutorial_08",
			"Announce_Tutorial_09",
			"Announce_Tutorial_10",
			"Announce_Tutorial_11",
			"Announce_Tutorial_12",
			"Announce_Tutorial_13",
			"Announce_Tutorial_14",
			"Announce_Tutorial_15",
			"Announce_Tutorial_16",
			"Announce_Tutorial_17",
			"Announce_Tutorial_18",
			"Announce_Tutorial_19",
			"Announce_Tutorial_20",
			"Announce_Tutorial_21",
			"Announce_Tutorial_22",
			"Announce_Tutorial_23"
		};


		private static readonly List<string> wicklineCreateExpectedVoices = new List<string>
		{
			"Announce_Wickline_01",
			"Announce_Wickline_02"
		};


		private static readonly List<string> wicklineAppearVoices = new List<string>
		{
			"Announce_WicklineLocation_01",
			"Announce_WicklineLocation_02"
		};


		private static readonly List<string> wicklineDeadVoices = new List<string>
		{
			"Announce_WicklineKilled_01",
			"Announce_WicklineKilled_02"
		};


		private static readonly List<string> treeOfLifeCreateExpectedVoices = new List<string>
		{
			"Announce_TreeOfLifePrep_01",
			"Announce_TreeOfLifePrep_02"
		};


		private static readonly List<string> treeOfLifeAppearVoices = new List<string>
		{
			"Announce_TreeOfLifeAppear_01",
			"Announce_TreeOfLifeAppear_02"
		};

		public static string GetAnnounceVoice(AnnounceVoiceType type, int extra = 0)
		{
			return GetAnnounceVoice(type, Singleton<LocalSetting>.inst.setting.voiceCountryCode, extra);
		}


		public static string GetDefaultLanguageVoice(AnnounceVoiceType type, int extra = 0)
		{
			return GetAnnounceVoice(type, SupportLanguage.English.GetAppLanguageCode(), extra);
		}


		private static string GetAnnounceVoice(AnnounceVoiceType type, string language, int extra = 0)
		{
			switch (type)
			{
				case AnnounceVoiceType.DeadPlayer:
					return DeadPlayer(language, extra);
				case AnnounceVoiceType.AlivePlayer:
					return AlivePlayer(language, extra);
				case AnnounceVoiceType.StartGame:
					return StartGame(language);
				case AnnounceVoiceType.WinGame:
					return WinGame(language);
				case AnnounceVoiceType.AirSupplyNotice:
					return AirSupplyNotice(language);
				case AnnounceVoiceType.AirSupply:
					return AirSupply(language);
				case AnnounceVoiceType.Restrict:
					return Restrict(language);
				case AnnounceVoiceType.RestrictAccel:
					return RestrictAccel(language);
				case AnnounceVoiceType.FinalRestrict:
					return FinalRestrict(language);
				case AnnounceVoiceType.FinalSafeActive:
					return FinalSafeActive(language);
				case AnnounceVoiceType.FinalSafePreDeactive:
					return FinalSafePreDeactive(language);
				case AnnounceVoiceType.FinalSafeDeactive:
					return FinalSafeDeactive(language);
				case AnnounceVoiceType.Tutorial:
					return Tutorial(language, extra);
				case AnnounceVoiceType.WicklineCreateExpected:
					return WicklineCreateExpected(language);
				case AnnounceVoiceType.WicklineAppear:
					return WicklineAppear(language);
				case AnnounceVoiceType.WicklineDead:
					return WicklineDead(language);
				case AnnounceVoiceType.TreeOfLifeCreateExpected:
					return TreeOfLifeCreateExpected(language);
				case AnnounceVoiceType.TreeOfLifeAppear:
					return TreeOfLifeAppear(language);
				default:
					throw new ArgumentOutOfRangeException("type", type, null);
			}
		}


		private static string DeadPlayer(string language, int count)
		{
			if (count <= 2)
			{
				return deadPlayerVoices.ElementAt(Random.Range(0, deadPlayerVoices.Count)) + "_" + language;
			}

			if (count <= 4)
			{
				return deadPlayerVoices3.ElementAt(Random.Range(0, deadPlayerVoices3.Count)) + "_" + language;
			}

			if (count <= 6)
			{
				return deadPlayerVoices5.ElementAt(Random.Range(0, deadPlayerVoices5.Count)) + "_" + language;
			}

			if (count == 7)
			{
				return deadPlayerVoices7.ElementAt(Random.Range(0, deadPlayerVoices7.Count)) + "_" + language;
			}

			return deadPlayerVoices8.ElementAt(Random.Range(0, deadPlayerVoices8.Count)) + "_" + language;
		}


		private static string AlivePlayer(string language, int count)
		{
			switch (count)
			{
				case 2:
					return alivePlayerVoice2.ElementAt(Random.Range(0, alivePlayerVoice2.Count)) + "_" + language;
				case 3:
					return alivePlayerVoice3.ElementAt(Random.Range(0, alivePlayerVoice3.Count)) + "_" + language;
				case 4:
					break;
				case 5:
					return alivePlayerVoice5.ElementAt(Random.Range(0, alivePlayerVoice5.Count)) + "_" + language;
				default:
					if (count == 10)
					{
						return alivePlayerVoice10.ElementAt(Random.Range(0, alivePlayerVoice10.Count)) + "_" + language;
					}

					break;
			}

			return string.Empty;
		}


		private static string StartGame(string language)
		{
			return startGameVoices.ElementAt(Random.Range(0, startGameVoices.Count)) + "_" + language;
		}


		private static string WinGame(string language)
		{
			return winGameVoices.ElementAt(Random.Range(0, winGameVoices.Count)) + "_" + language;
		}


		private static string AirSupplyNotice(string language)
		{
			return airSupplyNoticeVoices.ElementAt(Random.Range(0, airSupplyNoticeVoices.Count)) + "_" + language;
		}


		private static string AirSupply(string language)
		{
			return airSupplyVoices.ElementAt(Random.Range(0, airSupplyVoices.Count)) + "_" + language;
		}


		private static string Restrict(string language)
		{
			return restrictVoices.ElementAt(Random.Range(0, restrictVoices.Count)) + "_" + language;
		}


		private static string RestrictAccel(string language)
		{
			return restrictAccelVoices.ElementAt(Random.Range(0, restrictAccelVoices.Count)) + "_" + language;
		}


		private static string FinalRestrict(string language)
		{
			return finalRestrictVoices.ElementAt(Random.Range(0, finalRestrictVoices.Count)) + "_" + language;
		}


		private static string FinalSafeActive(string language)
		{
			return finalSafeActiveVoices.ElementAt(Random.Range(0, finalSafeActiveVoices.Count)) + "_" + language;
		}


		private static string FinalSafePreDeactive(string language)
		{
			return finalSafePreDeactiveVoices.ElementAt(Random.Range(0, finalSafePreDeactiveVoices.Count)) + "_" +
			       language;
		}


		private static string FinalSafeDeactive(string language)
		{
			return finalSafeDeactiveVoices.ElementAt(Random.Range(0, finalSafeDeactiveVoices.Count)) + "_" + language;
		}


		private static string Tutorial(string language, int extra)
		{
			return tutorialVoices.ElementAt(extra - 1) + "_" + language;
		}


		private static string WicklineCreateExpected(string language)
		{
			return wicklineCreateExpectedVoices.ElementAt(Random.Range(0, wicklineCreateExpectedVoices.Count)) + "_" +
			       language;
		}


		private static string WicklineAppear(string language)
		{
			return wicklineAppearVoices.ElementAt(Random.Range(0, wicklineAppearVoices.Count)) + "_" + language;
		}


		private static string WicklineDead(string language)
		{
			return wicklineDeadVoices.ElementAt(Random.Range(0, wicklineDeadVoices.Count)) + "_" + language;
		}


		private static string TreeOfLifeCreateExpected(string language)
		{
			return treeOfLifeCreateExpectedVoices.ElementAt(Random.Range(0, treeOfLifeCreateExpectedVoices.Count)) +
			       "_" + language;
		}


		private static string TreeOfLifeAppear(string language)
		{
			return treeOfLifeAppearVoices.ElementAt(Random.Range(0, treeOfLifeAppearVoices.Count)) + "_" + language;
		}
	}
}