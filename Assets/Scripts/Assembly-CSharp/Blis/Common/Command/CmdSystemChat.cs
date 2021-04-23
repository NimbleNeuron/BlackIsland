using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdSystemChat, false)]
	public class CmdSystemChat : CommandPacket
	{
		[Key(5)] public int areaCode;


		[Key(3)] public int characterCode;


		[Key(8)] public bool isMonster = true;


		[Key(6)] public bool isNoticeColor;


		[Key(2)] public int senderObjectId;


		[Key(7)] public bool showTime = true;


		[Key(4)] public int targetCharacterCode;


		[Key(1)] public int targetObjectId;


		[Key(0)] public SystemChatType type;


		public override void Action(ClientService service)
		{
			SystemChatType systemChatType = type;
			if (systemChatType <= SystemChatType.PingMoving &&
			    MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.IsIgnoreUser(IgnoreType.Ping,
				    senderObjectId))
			{
				return;
			}

			string chatContent = GetChatContent(service);
			MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(chatContent, showTime, isNoticeColor);
		}


		private string GetChatContent(ClientService service)
		{
			string text = string.Empty;
			string playerNickName = service.GetPlayerNickName(senderObjectId);
			string text2 = isMonster
				? LnUtil.GetMonsterName(targetCharacterCode)
				: service.GetPlayerNickName(targetObjectId);
			switch (type)
			{
				case SystemChatType.MarkSelect:
				{
					LocalObject localObject =
						MonoBehaviourInstance<ClientService>.inst.World.Find<LocalObject>(targetObjectId);
					text = Ln.Format("마크특수지정", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(StringUtil.ColorStringType.Area, LnUtil.GetAreaName(areaCode)),
						localObject.GetLocalizedName(true));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				}
				case SystemChatType.MarkPosition:
					text = Ln.Format("마크지정", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(StringUtil.ColorStringType.Area, LnUtil.GetAreaName(areaCode)));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				case SystemChatType.PingSelectCharacter:
				{
					LocalCharacter target = service.World.Find<LocalCharacter>(targetObjectId);
					StringUtil.ColorStringType colorStringType = service.IsAlly(target)
						? StringUtil.ColorStringType.Ally
						: StringUtil.ColorStringType.Enemy;
					text = Ln.Format("핑대상지정", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(StringUtil.ColorStringType.Area, LnUtil.GetAreaName(areaCode)),
						StringUtil.GetColorString(colorStringType, text2),
						StringUtil.GetColorString(colorStringType, LnUtil.GetCharacterName(targetCharacterCode)));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				}
				case SystemChatType.PingSelectSpecial:
				{
					LocalObject localObject2 = service.World.Find<LocalObject>(targetObjectId);
					text = Ln.Format("핑특수지정", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(StringUtil.ColorStringType.Area, LnUtil.GetAreaName(areaCode)),
						localObject2.GetLocalizedName(true));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				}
				case SystemChatType.PingSelectObject:
				{
					LocalObject localObject3 = service.World.Find<LocalObject>(targetObjectId);
					text = Ln.Format("핑특수지정", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(StringUtil.ColorStringType.Area, LnUtil.GetAreaName(areaCode)),
						localObject3.GetLocalizedName(true));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				}
				case SystemChatType.PingEscape:
					text = Ln.Format("핑퇴각", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(StringUtil.ColorStringType.Area, LnUtil.GetAreaName(areaCode)));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				case SystemChatType.PingSupport:
					text = Ln.Format("핑지원", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(StringUtil.ColorStringType.Area, LnUtil.GetAreaName(areaCode)));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				case SystemChatType.PingWarning:
					text = Ln.Format("핑주의", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(StringUtil.ColorStringType.Area, LnUtil.GetAreaName(areaCode)));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				case SystemChatType.PingMoving:
					text = Ln.Format("핑이동", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(StringUtil.ColorStringType.Area, LnUtil.GetAreaName(areaCode)));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				case SystemChatType.PingInventoryItem:
				{
					ItemData itemData = GameDB.item.FindItemByCode(targetObjectId);
					text = Ln.Format("아이템보유", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(itemData.itemGrade.GetColor(), LnUtil.GetItemName(targetObjectId)));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				}
				case SystemChatType.PingPickItem:
				{
					ItemData itemData2 = GameDB.item.FindItemByCode(targetObjectId);
					text = Ln.Format("아이템지목", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(itemData2.itemGrade.GetColor(), LnUtil.GetItemName(targetObjectId)));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				}
				case SystemChatType.PingFindItem:
				{
					ItemData itemData3 = GameDB.item.FindItemByCode(targetObjectId);
					text = Ln.Format("아이템발견", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(StringUtil.ColorStringType.Area, LnUtil.GetAreaName(areaCode)),
						StringUtil.GetColorString(itemData3.itemGrade.GetColor(), LnUtil.GetItemName(targetObjectId)));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				}
				case SystemChatType.PingSearchItem:
				{
					ItemData itemData4 = GameDB.item.FindItemByCode(targetObjectId);
					text = Ln.Format("아이템탐색", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(itemData4.itemGrade.GetColor(), LnUtil.GetItemName(targetObjectId)));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				}
				case SystemChatType.PingForbid:
					text = Ln.Get("추가신호금지");
					break;
				case SystemChatType.BeginDyingCondition:
					if (isMonster)
					{
						text = Ln.Format("야생동물빈사알림", LnUtil.GetMonsterName(targetCharacterCode), playerNickName,
							LnUtil.GetCharacterName(characterCode));
						text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Notice, 0,
							text2.Length + LnUtil.GetMonsterName(targetCharacterCode).Length + 3);
					}
					else
					{
						text = Ln.Format("플레이어빈사알림", text2, LnUtil.GetCharacterName(targetCharacterCode),
							playerNickName, LnUtil.GetCharacterName(characterCode));
						text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Notice, 0,
							text2.Length + LnUtil.GetCharacterName(targetCharacterCode).Length + 3);
					}

					break;
				case SystemChatType.TeamUseHyperLoop:
					text = Ln.Format("아군하이퍼루프도착", playerNickName, LnUtil.GetCharacterName(characterCode),
						StringUtil.GetColorString(StringUtil.ColorStringType.Area, LnUtil.GetAreaName(areaCode)));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Ally, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				case SystemChatType.SurrenderGame:
					text = Ln.Format("게임포기알림", playerNickName, LnUtil.GetCharacterName(characterCode));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Notice, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
				case SystemChatType.PlayerDead:
					text = Ln.Format("플레이어사망알림", playerNickName, LnUtil.GetCharacterName(characterCode));
					text = StringUtil.GetColorConvertString(text, StringUtil.ColorStringType.Notice, 0,
						playerNickName.Length + LnUtil.GetCharacterName(characterCode).Length + 3);
					break;
			}

			return text;
		}
	}
}