using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class ChatCommandService
	{
		private readonly string MUTE_CHAT_ALL_KEY = Ln.Get("전체 채팅무시");


		private readonly string MUTE_CHAT_KEY = Ln.Get("채팅무시 대상");


		private readonly string MUTE_PING_ALL_KEY = Ln.Get("전체 핑무시");


		private readonly string MUTE_PING_KEY = Ln.Get("핑무시 대상");


		private readonly string UN_MUTE_CHAT_ALL_KEY = Ln.Get("전체 채팅무시 해제");


		private readonly string UN_MUTE_CHAT_KEY = Ln.Get("채팅무시 대상 해제");


		private readonly string UN_MUTE_PING_ALL_KEY = Ln.Get("전체 핑무시 해제");


		private readonly string UN_MUTE_PING_KEY = Ln.Get("핑무시 대상 해제");


		private readonly string WHISPER_KEY = Ln.Get("귓속말 태그");

		public bool IsChatCommand(string chatContent)
		{
			string[] array = chatContent.Split(' ');
			int num = array.Length;
			if (num != 1)
			{
				if (num != 2)
				{
					return false;
				}

				string text = array[0];
				string nickName = array[1];
				List<int> findAliveTeamMemberId = GetFindAliveTeamMemberId(nickName);
				if (text.Equals(MUTE_CHAT_KEY))
				{
					MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SendIgnoreRequest(IgnoreType.Chat,
						true, findAliveTeamMemberId,
						GetIgnoreCallBack(IgnoreType.Chat, findAliveTeamMemberId, true, true));
				}
				else if (text.Equals(UN_MUTE_CHAT_KEY))
				{
					MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SendIgnoreRequest(IgnoreType.Chat,
						false, findAliveTeamMemberId,
						GetIgnoreCallBack(IgnoreType.Chat, findAliveTeamMemberId, false, true));
				}
				else if (text.Equals(MUTE_PING_KEY))
				{
					MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SendIgnoreRequest(IgnoreType.Ping,
						true, findAliveTeamMemberId,
						GetIgnoreCallBack(IgnoreType.Ping, findAliveTeamMemberId, true, true));
				}
				else
				{
					if (!text.Equals(UN_MUTE_PING_KEY))
					{
						return false;
					}

					MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SendIgnoreRequest(IgnoreType.Ping,
						false, findAliveTeamMemberId,
						GetIgnoreCallBack(IgnoreType.Ping, findAliveTeamMemberId, false, true));
				}
			}
			else
			{
				string text2 = array[0];
				List<int> aliveTeamMemberIds = GetAliveTeamMemberIds();
				if (text2.Equals(MUTE_CHAT_ALL_KEY))
				{
					MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SendIgnoreRequest(IgnoreType.Chat,
						true, aliveTeamMemberIds, GetIgnoreCallBack(IgnoreType.Chat, aliveTeamMemberIds, true, false));
				}
				else if (text2.Equals(UN_MUTE_CHAT_ALL_KEY))
				{
					MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SendIgnoreRequest(IgnoreType.Chat,
						false, aliveTeamMemberIds,
						GetIgnoreCallBack(IgnoreType.Chat, aliveTeamMemberIds, false, false));
				}
				else if (text2.Equals(MUTE_PING_ALL_KEY))
				{
					MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SendIgnoreRequest(IgnoreType.Ping,
						true, aliveTeamMemberIds, GetIgnoreCallBack(IgnoreType.Ping, aliveTeamMemberIds, true, false));
				}
				else
				{
					if (!text2.Equals(UN_MUTE_PING_ALL_KEY))
					{
						return false;
					}

					MonoBehaviourInstance<ClientService>.inst.IgnoreTargetService.SendIgnoreRequest(IgnoreType.Ping,
						false, aliveTeamMemberIds,
						GetIgnoreCallBack(IgnoreType.Ping, aliveTeamMemberIds, false, false));
				}
			}

			return true;
		}


		private List<int> GetAliveTeamMemberIds()
		{
			List<int> list = new List<int>();
			List<PlayerContext> list2 = MonoBehaviourInstance<ClientService>.inst
				.GetTeamMember(MonoBehaviourInstance<ClientService>.inst.MyTeamNumber).FindAll(x =>
					x.Character.ObjectId != MonoBehaviourInstance<ClientService>.inst.MyObjectId &&
					x.Character.IsAlive);
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(list2[i].Character.ObjectId);
			}

			return list;
		}


		private List<int> GetFindAliveTeamMemberId(string nickName)
		{
			PlayerContext playerContext = MonoBehaviourInstance<ClientService>.inst
				.GetTeamMember(MonoBehaviourInstance<ClientService>.inst.MyTeamNumber)
				.Find(x => x.Character.IsAlive && x.nickname.Equals(nickName));
			if (playerContext == null)
			{
				return null;
			}

			return new List<int>
			{
				playerContext.Character.ObjectId
			};
		}


		private int GetFindAlivePlayerId(string nickName)
		{
			foreach (PlayerContext playerContext in MonoBehaviourInstance<ClientService>.inst.GetPlayers())
			{
				if (playerContext.Character.ObjectId != MonoBehaviourInstance<ClientService>.inst.MyObjectId &&
				    playerContext.Character.IsAlive && playerContext.nickname.Equals(nickName))
				{
					return playerContext.Character.ObjectId;
				}
			}

			return -1;
		}


		private Action<bool> GetIgnoreCallBack(IgnoreType ignoreType, List<int> objectIds, bool isIgnore, bool isSingle)
		{
			return delegate
			{
				string param_ = string.Empty;
				if (isSingle)
				{
					param_ = MonoBehaviourInstance<ClientService>.inst.GetPlayerNickName(objectIds[0]);
				}

				IgnoreType ignoreType2 = ignoreType;
				if (ignoreType2 != IgnoreType.Ping)
				{
					if (ignoreType2 == IgnoreType.Chat)
					{
						if (isIgnore)
						{
							MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(
								isSingle ? Ln.Format("채팅무시 대상 피드백", param_) : Ln.Get("전체 채팅무시 피드백"), false, false);
							return;
						}

						MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(
							isSingle ? Ln.Format("채팅무시 대상 해제 피드백", param_) : Ln.Get("전체 채팅무시 해제 피드백"), false, false);
					}
				}
				else
				{
					if (isIgnore)
					{
						MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(
							isSingle ? Ln.Format("핑무시 대상 피드백", param_) : Ln.Get("전체 핑무시 피드백"), false, false);
						return;
					}

					MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(
						isSingle ? Ln.Format("핑무시 대상 해제 피드백", param_) : Ln.Get("전체 핑무시 해제 피드백"), false, false);
				}
			};
		}
	}
}