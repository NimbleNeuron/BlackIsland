using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Blis.Client
{
	public class MatchingSocket : BlisWebSocket
	{
		private IMatchingNotificationHandler _notificationHandler;
		private Status status;

		public void Init(string matchingHost, IMatchingNotificationHandler notificationHandler)
		{
			base.Init(matchingHost);
			_notificationHandler = notificationHandler;
		}
		
		protected override void OnNotification(NNWebSocketMessage msg)
		{
			Log.V("[MatchingSocket] Notification: {0}", msg.method);

			string method = msg.method;

			switch (method)
			{
				case "UpdateCustomGameRoom":
					_notificationHandler.OnUpdateCustomGameRoom(
						((JToken) ParseNotificationParam<Dictionary<string, object>>(msg)["roomInfo"])
						.ToObject<CustomGameRoom>());
					break;
				
				case "AcceptMatching":
					_notificationHandler.OnAcceptMatching();
					break;
				
				case "StandBy":
					_notificationHandler.OnStandBy();
					break;
				
				case "GameStart":
					_notificationHandler.OnStartGame(ParseNotificationParam<MatchingResult>(msg));
					break;
				
				case "CustomChatMessage":
					_notificationHandler.OnCustomGameRoomChatMessage(ParseNotificationParam<MatchingChatMessage>(msg));
					break;
				
				case "CharacterSelectPhase":
					Dictionary<string, object> notificationParam1 =
						ParseNotificationParam<Dictionary<string, object>>(msg);
					_notificationHandler.OnCharacterSelectPhase(
						((JToken) notificationParam1["matchingTeamMembers"]).ToObject<List<MatchingTeamMember>>(),
						((JToken) notificationParam1["freeCharacters"]).ToObject<List<long>>(),
						((JToken) notificationParam1["banCharacters"]).ToObject<List<long>>());
					break;
				
				case "MatchingComplete":
					if (status == Status.Exited) break;
					
					status = Status.Matched;
					_notificationHandler.OnCompleteMatching();
					break;
				
				case "SelectCharacter":
					Dictionary<string, object> notificationParam2 =
						ParseNotificationParam<Dictionary<string, object>>(msg);
					_notificationHandler.OnCharacterSelect((long) notificationParam2["userNum"],
						(int) (long) notificationParam2["characterCode"], (int) (long) notificationParam2["weapon"],
						(int) (long) notificationParam2["skinCode"]);
					break;
				
				case "ObserverCharacterSelectPhase":
					_notificationHandler.OnObserverCharacterSelectPhase(
						((JToken) ParseNotificationParam<Dictionary<string, object>>(msg)["matchingTeamList"])
						.ToObject<List<MatchingTeam>>());
					break;
				
				case "OtherMemberNotJoinMatching":
					if (status == Status.Exited) break;
					
					status = Status.WaitingExit;
					StartCoroutine(DelayExit(_notificationHandler.OnOtherMemberNotJoinMatching));
					break;
				
				case "SelectSkin":
					Dictionary<string, object> notificationParam3 =
						ParseNotificationParam<Dictionary<string, object>>(msg);
					_notificationHandler.OnSkinSelect((long) notificationParam3["userNum"],
						(int) (long) notificationParam3["skinCode"]);
					break;
				
				case "StopMatching":
					if (status == Status.Exited) break;
					
					status = Status.WaitingExit;
					StartCoroutine(DelayExit(_notificationHandler.OnStopMatching));
					break;
				
				case "CustomGameRoomEmpty":
					status = Status.WaitingExit;
					StartCoroutine(DelayExit(_notificationHandler.OnCustomGameRoomEmpty));
					break;
				
				case "DodgeMatching":
					status = Status.Matching;
					_notificationHandler.OnDodgeMatching();
					break;
				
				case "ChatMessage":
					_notificationHandler.OnChatMessage(ParseNotificationParam<MatchingChatMessage>(msg));
					break;
				
				case "PickCharacter":
					Dictionary<string, object> notificationParam4 =
						ParseNotificationParam<Dictionary<string, object>>(msg);
					_notificationHandler.OnCharacterPick((long) notificationParam4["pickUserNum"],
						(int) (long) notificationParam4["skinCode"]);
					break;
				
				case "KickOutMatching":
					status = Status.WaitingExit;
					StartCoroutine(DelayExit(_notificationHandler.OnKickOutMatching));
					break;
				
				case "CancelPickCharacter":
					_notificationHandler.OnCancelCharacterPick(
						(long) ParseNotificationParam<Dictionary<string, object>>(msg)["pickUserNum"]);
					break;
				
				case "ExitMatchingOtherUser":
					status = Status.WaitingExit;
					StartCoroutine(DelayExit(_notificationHandler.OnExitMatchingOtherUser));
					break;
				
				case "KickOutDeclineMatching":
					status = Status.WaitingExit;
					StartCoroutine(DelayExit(_notificationHandler.OnKickOutDeclineMatching));
					break;
				
				case "KickedOut":
					status = Status.WaitingExit;
					StartCoroutine(DelayExit(_notificationHandler.OnKickedOutCustomGameRoom));
					break;
				
				case "StartMatching":
					if (status == Status.WaitForStartMatching) break;
					
					status = Status.Matching;
					_notificationHandler.OnStartMatching();
					break;
				
				case "DodgeCustomMatching":
					status = Status.InCustomRoom;
					_notificationHandler.OnDodgeCustomMatching();
					break;
				
				case "Error":
					OnError(Ln.Get(
						((BlisWebSocketError) (long) ParseNotificationParam<Dictionary<string, object>>(msg)["error"])
						.ToString()));
					Close();
					break;
				
				default: break;
			}
			
			// ISSUE: reference to a compiler-generated method
			// switch ((ulong) method.GetHashCode())
			// {
			// 	case 29973850:
			// 		if (!(method == "UpdateCustomGameRoom"))
			// 		{
			// 			break;
			// 		}
			//
			// 		_notificationHandler.OnUpdateCustomGameRoom(
			// 			((JToken) ParseNotificationParam<Dictionary<string, object>>(msg)["roomInfo"])
			// 			.ToObject<CustomGameRoom>());
			// 		break;
			// 	case 174034126:
			// 		if (!(method == "AcceptMatching"))
			// 		{
			// 			break;
			// 		}
			//
			// 		_notificationHandler.OnAcceptMatching();
			// 		break;
			// 	case 290421726:
			// 		if (!(method == "StandBy"))
			// 		{
			// 			break;
			// 		}
			//
			// 		_notificationHandler.OnStandBy();
			// 		break;
			// 	case 773798957:
			// 		if (!(method == "GameStart"))
			// 		{
			// 			break;
			// 		}
			//
			// 		_notificationHandler.OnStartGame(ParseNotificationParam<MatchingResult>(msg));
			// 		break;
			// 	case 801632935:
			// 		if (!(method == "CustomChatMessage"))
			// 		{
			// 			break;
			// 		}
			//
			// 		_notificationHandler.OnCustomGameRoomChatMessage(
			// 			ParseNotificationParam<MatchingChatMessage>(msg));
			// 		break;
			// 	case 1220543855:
			// 		if (!(method == "CharacterSelectPhase"))
			// 		{
			// 			break;
			// 		}
			//
			// 		Dictionary<string, object> notificationParam1 =
			// 			ParseNotificationParam<Dictionary<string, object>>(msg);
			// 		_notificationHandler.OnCharacterSelectPhase(
			// 			((JToken) notificationParam1["matchingTeamMembers"]).ToObject<List<MatchingTeamMember>>(),
			// 			((JToken) notificationParam1["freeCharacters"]).ToObject<List<long>>(),
			// 			((JToken) notificationParam1["banCharacters"]).ToObject<List<long>>());
			// 		break;
			// 	case 1269936433:
			// 		if (!(method == "MatchingComplete") || status == Status.Exited)
			// 		{
			// 			break;
			// 		}
			//
			// 		status = Status.Matched;
			// 		_notificationHandler.OnCompleteMatching();
			// 		break;
			// 	case 1411044056:
			// 		if (!(method == "SelectCharacter"))
			// 		{
			// 			break;
			// 		}
			//
			// 		Dictionary<string, object> notificationParam2 =
			// 			ParseNotificationParam<Dictionary<string, object>>(msg);
			// 		_notificationHandler.OnCharacterSelect((long) notificationParam2["userNum"],
			// 			(int) (long) notificationParam2["characterCode"], (int) (long) notificationParam2["weapon"],
			// 			(int) (long) notificationParam2["skinCode"]);
			// 		break;
			// 	case 1563530349:
			// 		if (!(method == "ObserverCharacterSelectPhase"))
			// 		{
			// 			break;
			// 		}
			//
			// 		_notificationHandler.OnObserverCharacterSelectPhase(
			// 			((JToken) ParseNotificationParam<Dictionary<string, object>>(msg)["matchingTeamList"])
			// 			.ToObject<List<MatchingTeam>>());
			// 		break;
			// 	case 1734132195:
			// 		if (!(method == "OtherMemberNotJoinMatching") || status == Status.Exited)
			// 		{
			// 			break;
			// 		}
			//
			// 		status = Status.WaitingExit;
			// 		StartCoroutine(DelayExit(_notificationHandler.OnOtherMemberNotJoinMatching));
			// 		break;
			// 	case 1972982416:
			// 		if (!(method == "SelectSkin"))
			// 		{
			// 			break;
			// 		}
			//
			// 		Dictionary<string, object> notificationParam3 =
			// 			ParseNotificationParam<Dictionary<string, object>>(msg);
			// 		_notificationHandler.OnSkinSelect((long) notificationParam3["userNum"],
			// 			(int) (long) notificationParam3["skinCode"]);
			// 		break;
			// 	case 2357251546:
			// 		if (!(method == "StopMatching") || status == Status.Exited)
			// 		{
			// 			break;
			// 		}
			//
			// 		status = Status.WaitingExit;
			// 		StartCoroutine(DelayExit(_notificationHandler.OnStopMatching));
			// 		break;
			// 	case 2783468700:
			// 		if (!(method == "CustomGameRoomEmpty"))
			// 		{
			// 			break;
			// 		}
			//
			// 		status = Status.WaitingExit;
			// 		StartCoroutine(DelayExit(_notificationHandler.OnCustomGameRoomEmpty));
			// 		break;
			// 	case 2904760399:
			// 		if (!(method == "DodgeMatching"))
			// 		{
			// 			break;
			// 		}
			//
			// 		status = Status.Matching;
			// 		_notificationHandler.OnDodgeMatching();
			// 		break;
			// 	case 2926586898:
			// 		if (!(method == "ChatMessage"))
			// 		{
			// 			break;
			// 		}
			//
			// 		_notificationHandler.OnChatMessage(ParseNotificationParam<MatchingChatMessage>(msg));
			// 		break;
			// 	case 2971983575:
			// 		if (!(method == "PickCharacter"))
			// 		{
			// 			break;
			// 		}
			//
			// 		Dictionary<string, object> notificationParam4 =
			// 			ParseNotificationParam<Dictionary<string, object>>(msg);
			// 		_notificationHandler.OnCharacterPick((long) notificationParam4["pickUserNum"],
			// 			(int) (long) notificationParam4["skinCode"]);
			// 		break;
			// 	case 2983817814:
			// 		if (!(method == "KickOutMatching"))
			// 		{
			// 			break;
			// 		}
			//
			// 		status = Status.WaitingExit;
			// 		StartCoroutine(DelayExit(_notificationHandler.OnKickOutMatching));
			// 		break;
			// 	case 3016764313:
			// 		if (!(method == "CancelPickCharacter"))
			// 		{
			// 			break;
			// 		}
			//
			// 		_notificationHandler.OnCancelCharacterPick(
			// 			(long) ParseNotificationParam<Dictionary<string, object>>(msg)["pickUserNum"]);
			// 		break;
			// 	case 3468182155:
			// 		if (!(method == "ExitMatchingOtherUser") || status == Status.Exited)
			// 		{
			// 			break;
			// 		}
			//
			// 		status = Status.WaitingExit;
			// 		StartCoroutine(DelayExit(_notificationHandler.OnExitMatchingOtherUser));
			// 		break;
			// 	case 3511714570:
			// 		if (!(method == "KickOutDeclineMatching"))
			// 		{
			// 			break;
			// 		}
			//
			// 		status = Status.WaitingExit;
			// 		StartCoroutine(DelayExit(_notificationHandler.OnKickOutDeclineMatching));
			// 		break;
			// 	case 3969424418:
			// 		if (!(method == "KickedOut"))
			// 		{
			// 			break;
			// 		}
			//
			// 		status = Status.WaitingExit;
			// 		StartCoroutine(DelayExit(_notificationHandler.OnKickedOutCustomGameRoom));
			// 		break;
			// 	case 4047638176:
			// 		if (!(method == "StartMatching") || status != Status.WaitForStartMatching)
			// 		{
			// 			break;
			// 		}
			//
			// 		status = Status.Matching;
			// 		_notificationHandler.OnStartMatching();
			// 		break;
			// 	case 4082281516:
			// 		if (!(method == "DodgeCustomMatching"))
			// 		{
			// 			break;
			// 		}
			//
			// 		status = Status.InCustomRoom;
			// 		_notificationHandler.OnDodgeCustomMatching();
			// 		break;
			// 	case 4086144241:
			// 		if (!(method == "Error"))
			// 		{
			// 			break;
			// 		}
			//
			// 		OnError(Ln.Get(
			// 			((BlisWebSocketError) (long) ParseNotificationParam<Dictionary<string, object>>(msg)["error"])
			// 			.ToString()));
			// 		Close();
			// 		break;
			//
			//
			// 	// co: dotPeek    
			// 	// Log.V("[MatchingSocket] Notification: {0}", msg.method);
			// 	// string method = msg.method;
			// 	// uint num = <PrivateImplementationDetails>.ComputeStringHash(method);
			// 	// if (num <= 2357251546U)
			// 	// {
			// 	// 	if (num <= 1220543855U)
			// 	// 	{
			// 	// 		if (num <= 290421726U)
			// 	// 		{
			// 	// 			if (num != 29973850U)
			// 	// 			{
			// 	// 				if (num != 174034126U)
			// 	// 				{
			// 	// 					if (num != 290421726U)
			// 	// 					{
			// 	// 						return;
			// 	// 					}
			// 	// 					if (!(method == "StandBy"))
			// 	// 					{
			// 	// 						return;
			// 	// 					}
			// 	// 					this._notificationHandler.OnStandBy();
			// 	// 					return;
			// 	// 				}
			// 	// 				else
			// 	// 				{
			// 	// 					if (!(method == "AcceptMatching"))
			// 	// 					{
			// 	// 						return;
			// 	// 					}
			// 	// 					this._notificationHandler.OnAcceptMatching();
			// 	// 					return;
			// 	// 				}
			// 	// 			}
			// 	// 			else
			// 	// 			{
			// 	// 				if (!(method == "UpdateCustomGameRoom"))
			// 	// 				{
			// 	// 					return;
			// 	// 				}
			// 	// 				CustomGameRoom customGameRoom = ((JObject)base.ParseNotificationParam<Dictionary<string, object>>(msg)["roomInfo"]).ToObject<CustomGameRoom>();
			// 	// 				this._notificationHandler.OnUpdateCustomGameRoom(customGameRoom);
			// 	// 				return;
			// 	// 			}
			// 	// 		}
			// 	// 		else if (num != 773798957U)
			// 	// 		{
			// 	// 			if (num != 801632935U)
			// 	// 			{
			// 	// 				if (num != 1220543855U)
			// 	// 				{
			// 	// 					return;
			// 	// 				}
			// 	// 				if (!(method == "CharacterSelectPhase"))
			// 	// 				{
			// 	// 					return;
			// 	// 				}
			// 	// 				Dictionary<string, object> dictionary = base.ParseNotificationParam<Dictionary<string, object>>(msg);
			// 	// 				this._notificationHandler.OnCharacterSelectPhase(((JArray)dictionary["matchingTeamMembers"]).ToObject<List<MatchingTeamMember>>(), ((JArray)dictionary["freeCharacters"]).ToObject<List<long>>(), ((JArray)dictionary["banCharacters"]).ToObject<List<long>>());
			// 	// 				return;
			// 	// 			}
			// 	// 			else
			// 	// 			{
			// 	// 				if (!(method == "CustomChatMessage"))
			// 	// 				{
			// 	// 					return;
			// 	// 				}
			// 	// 				MatchingChatMessage message = base.ParseNotificationParam<MatchingChatMessage>(msg);
			// 	// 				this._notificationHandler.OnCustomGameRoomChatMessage(message);
			// 	// 				return;
			// 	// 			}
			// 	// 		}
			// 	// 		else
			// 	// 		{
			// 	// 			if (!(method == "GameStart"))
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			this._notificationHandler.OnStartGame(base.ParseNotificationParam<MatchingResult>(msg));
			// 	// 			return;
			// 	// 		}
			// 	// 	}
			// 	// 	else if (num <= 1563530349U)
			// 	// 	{
			// 	// 		if (num != 1269936433U)
			// 	// 		{
			// 	// 			if (num != 1411044056U)
			// 	// 			{
			// 	// 				if (num != 1563530349U)
			// 	// 				{
			// 	// 					return;
			// 	// 				}
			// 	// 				if (!(method == "ObserverCharacterSelectPhase"))
			// 	// 				{
			// 	// 					return;
			// 	// 				}
			// 	// 				Dictionary<string, object> dictionary2 = base.ParseNotificationParam<Dictionary<string, object>>(msg);
			// 	// 				this._notificationHandler.OnObserverCharacterSelectPhase(((JArray)dictionary2["matchingTeamList"]).ToObject<List<MatchingTeam>>());
			// 	// 				return;
			// 	// 			}
			// 	// 			else
			// 	// 			{
			// 	// 				if (!(method == "SelectCharacter"))
			// 	// 				{
			// 	// 					return;
			// 	// 				}
			// 	// 				Dictionary<string, object> dictionary3 = base.ParseNotificationParam<Dictionary<string, object>>(msg);
			// 	// 				this._notificationHandler.OnCharacterSelect((long)dictionary3["userNum"], (int)((long)dictionary3["characterCode"]), (int)((long)dictionary3["weapon"]), (int)((long)dictionary3["skinCode"]));
			// 	// 				return;
			// 	// 			}
			// 	// 		}
			// 	// 		else
			// 	// 		{
			// 	// 			if (!(method == "MatchingComplete"))
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			if (this.status == MatchingSocket.Status.Exited)
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			this.status = MatchingSocket.Status.Matched;
			// 	// 			this._notificationHandler.OnCompleteMatching();
			// 	// 			return;
			// 	// 		}
			// 	// 	}
			// 	// 	else if (num != 1734132195U)
			// 	// 	{
			// 	// 		if (num != 1972982416U)
			// 	// 		{
			// 	// 			if (num != 2357251546U)
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			if (!(method == "StopMatching"))
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			if (this.status == MatchingSocket.Status.Exited)
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			this.status = MatchingSocket.Status.WaitingExit;
			// 	// 			base.StartCoroutine(this.DelayExit(new Action(this._notificationHandler.OnStopMatching)));
			// 	// 			return;
			// 	// 		}
			// 	// 		else
			// 	// 		{
			// 	// 			if (!(method == "SelectSkin"))
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			Dictionary<string, object> dictionary4 = base.ParseNotificationParam<Dictionary<string, object>>(msg);
			// 	// 			this._notificationHandler.OnSkinSelect((long)dictionary4["userNum"], (int)((long)dictionary4["skinCode"]));
			// 	// 			return;
			// 	// 		}
			// 	// 	}
			// 	// 	else
			// 	// 	{
			// 	// 		if (!(method == "OtherMemberNotJoinMatching"))
			// 	// 		{
			// 	// 			return;
			// 	// 		}
			// 	// 		if (this.status == MatchingSocket.Status.Exited)
			// 	// 		{
			// 	// 			return;
			// 	// 		}
			// 	// 		this.status = MatchingSocket.Status.WaitingExit;
			// 	// 		base.StartCoroutine(this.DelayExit(new Action(this._notificationHandler.OnOtherMemberNotJoinMatching)));
			// 	// 		return;
			// 	// 	}
			// 	// }
			// 	// else if (num <= 3016764313U)
			// 	// {
			// 	// 	if (num <= 2926586898U)
			// 	// 	{
			// 	// 		if (num != 2783468700U)
			// 	// 		{
			// 	// 			if (num != 2904760399U)
			// 	// 			{
			// 	// 				if (num != 2926586898U)
			// 	// 				{
			// 	// 					return;
			// 	// 				}
			// 	// 				if (!(method == "ChatMessage"))
			// 	// 				{
			// 	// 					return;
			// 	// 				}
			// 	// 				MatchingChatMessage msg2 = base.ParseNotificationParam<MatchingChatMessage>(msg);
			// 	// 				this._notificationHandler.OnChatMessage(msg2);
			// 	// 				return;
			// 	// 			}
			// 	// 			else
			// 	// 			{
			// 	// 				if (!(method == "DodgeMatching"))
			// 	// 				{
			// 	// 					return;
			// 	// 				}
			// 	// 				this.status = MatchingSocket.Status.Matching;
			// 	// 				this._notificationHandler.OnDodgeMatching();
			// 	// 				return;
			// 	// 			}
			// 	// 		}
			// 	// 		else
			// 	// 		{
			// 	// 			if (!(method == "CustomGameRoomEmpty"))
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			this.status = MatchingSocket.Status.WaitingExit;
			// 	// 			base.StartCoroutine(this.DelayExit(new Action(this._notificationHandler.OnCustomGameRoomEmpty)));
			// 	// 			return;
			// 	// 		}
			// 	// 	}
			// 	// 	else if (num != 2971983575U)
			// 	// 	{
			// 	// 		if (num != 2983817814U)
			// 	// 		{
			// 	// 			if (num != 3016764313U)
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			if (!(method == "CancelPickCharacter"))
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			Dictionary<string, object> dictionary5 = base.ParseNotificationParam<Dictionary<string, object>>(msg);
			// 	// 			this._notificationHandler.OnCancelCharacterPick((long)dictionary5["pickUserNum"]);
			// 	// 			return;
			// 	// 		}
			// 	// 		else
			// 	// 		{
			// 	// 			if (!(method == "KickOutMatching"))
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			this.status = MatchingSocket.Status.WaitingExit;
			// 	// 			base.StartCoroutine(this.DelayExit(new Action(this._notificationHandler.OnKickOutMatching)));
			// 	// 			return;
			// 	// 		}
			// 	// 	}
			// 	// 	else
			// 	// 	{
			// 	// 		if (!(method == "PickCharacter"))
			// 	// 		{
			// 	// 			return;
			// 	// 		}
			// 	// 		Dictionary<string, object> dictionary6 = base.ParseNotificationParam<Dictionary<string, object>>(msg);
			// 	// 		this._notificationHandler.OnCharacterPick((long)dictionary6["pickUserNum"], (int)((long)dictionary6["skinCode"]));
			// 	// 		return;
			// 	// 	}
			// 	// }
			// 	// else if (num <= 3969424418U)
			// 	// {
			// 	// 	if (num != 3468182155U)
			// 	// 	{
			// 	// 		if (num != 3511714570U)
			// 	// 		{
			// 	// 			if (num != 3969424418U)
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			if (!(method == "KickedOut"))
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			this.status = MatchingSocket.Status.WaitingExit;
			// 	// 			base.StartCoroutine(this.DelayExit(new Action(this._notificationHandler.OnKickedOutCustomGameRoom)));
			// 	// 			return;
			// 	// 		}
			// 	// 		else
			// 	// 		{
			// 	// 			if (!(method == "KickOutDeclineMatching"))
			// 	// 			{
			// 	// 				return;
			// 	// 			}
			// 	// 			this.status = MatchingSocket.Status.WaitingExit;
			// 	// 			base.StartCoroutine(this.DelayExit(new Action(this._notificationHandler.OnKickOutDeclineMatching)));
			// 	// 			return;
			// 	// 		}
			// 	// 	}
			// 	// 	else
			// 	// 	{
			// 	// 		if (!(method == "ExitMatchingOtherUser"))
			// 	// 		{
			// 	// 			return;
			// 	// 		}
			// 	// 		if (this.status == MatchingSocket.Status.Exited)
			// 	// 		{
			// 	// 			return;
			// 	// 		}
			// 	// 		this.status = MatchingSocket.Status.WaitingExit;
			// 	// 		base.StartCoroutine(this.DelayExit(new Action(this._notificationHandler.OnExitMatchingOtherUser)));
			// 	// 		return;
			// 	// 	}
			// 	// }
			// 	// else if (num != 4047638176U)
			// 	// {
			// 	// 	if (num != 4082281516U)
			// 	// 	{
			// 	// 		if (num != 4086144241U)
			// 	// 		{
			// 	// 			return;
			// 	// 		}
			// 	// 		if (!(method == "Error"))
			// 	// 		{
			// 	// 			return;
			// 	// 		}
			// 	// 		base.OnError(Ln.Get(((BlisWebSocketError)((long)base.ParseNotificationParam<Dictionary<string, object>>(msg)["error"])).ToString()));
			// 	// 		base.Close();
			// 	// 		return;
			// 	// 	}
			// 	// 	else
			// 	// 	{
			// 	// 		if (!(method == "DodgeCustomMatching"))
			// 	// 		{
			// 	// 			return;
			// 	// 		}
			// 	// 		this.status = MatchingSocket.Status.InCustomRoom;
			// 	// 		this._notificationHandler.OnDodgeCustomMatching();
			// 	// 		return;
			// 	// 	}
			// 	// }
			// 	// else
			// 	// {
			// 	// 	if (!(method == "StartMatching"))
			// 	// 	{
			// 	// 		return;
			// 	// 	}
			// 	// 	if (this.status != MatchingSocket.Status.WaitForStartMatching)
			// 	// 	{
			// 	// 		return;
			// 	// 	}
			// 	// 	this.status = MatchingSocket.Status.Matching;
			// 	// 	this._notificationHandler.OnStartMatching();
			// 	// 	return;
			// 	// }
			// }
		}

		public void EnterMatching(string matchingTokenKey, long userNum, Action callback)
		{
			Log.V("[MatchingSocket] Connect to {0}", host);
			
			Connect(host, onConnect, onFailed);

			void onConnect()
			{
				status = Status.WaitForStartMatching;
				WebSocketRequest req = Req(
					nameof(EnterMatching), 
					(object) nameof(matchingTokenKey),
					(object) matchingTokenKey,
					(object) nameof(userNum),
					(object) userNum
					);
				
				Send<object>(req, response);

				void response(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "EnterNormalMatching");
					}
					else
					{
						Action action = callback;
						if (action == null)
						{
							return;
						}

						action();
					}
				}
			}

			void onFailed()
			{
				if (MonoBehaviourInstance<LobbyUI>.inst)
					MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnLobbyStateUpdate(LobbyState.Ready);
			}
		}


		public void ExitMatching(Action callback)
		{
			if (status != Status.Matching)
			{
				Log.W("[MatchingSocket.ExitMatching] MatchingStatus is 'Matching'");
				return;
			}

			Send<object>(Req("ExitMatching", Array.Empty<object>()),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "ExitMatching");
						return;
					}

					Action callback2 = callback;
					if (callback2 == null)
					{
						return;
					}

					callback2();
				});
		}


		public void AcceptMatching()
		{
			if (status != Status.Matched)
			{
				Log.W(string.Format("[MatchingSocket] AcceptMatching | status is not 'Matched' | status: {0}", status));
				return;
			}

			Send<object>(Req("AcceptMatching", Array.Empty<object>()), delegate(BlisWebSocketRequest req, object res)
			{
				if (res == null)
				{
					HandleError(req, "AcceptMatching");
				}
			});
		}


		public void DeclineMatching()
		{
			if (status != Status.Matched)
			{
				Log.W(string.Format("[MatchingSocket] DeclineMatching | status is not 'Matched' | status: {0}",
					status));
				return;
			}

			Send<object>(Req("DeclineMatching", Array.Empty<object>()), delegate(BlisWebSocketRequest req, object res)
			{
				if (res == null)
				{
					HandleError(req, "DeclineMatching");
				}
			});
		}


		public void SelectCharacter(int characterCode, int weaponCode, int skinCode)
		{
			if (status != Status.Matched)
			{
				Log.W(string.Format("[MatchingSocket] SelectCharacter | status is not 'Matched' | status: {0}",
					status));
				return;
			}

			Send<object>(
				Req("PostMatchedSelectCharacter", "characterCode", characterCode, "weaponCode", weaponCode, "skinCode",
					skinCode), delegate(BlisWebSocketRequest req, object res)
				{
					if (res == null)
					{
						HandleError(req, "PostMatchedSelectCharacter");
					}
				});
		}


		public void SelectSkin(int skinCode)
		{
			if (status != Status.Matched)
			{
				Log.W(string.Format("[MatchingSocket] SelectCharacter | status is not 'Matched' | status: {0}",
					status));
				return;
			}

			Send<object>(Req("PostMatchedSelectSkin", "skinCode", skinCode),
				delegate(BlisWebSocketRequest req, object res)
				{
					if (res == null)
					{
						HandleError(req, "PostMatchedSelectSkin");
					}
				});
		}


		public void PickCharacter()
		{
			if (status != Status.Matched)
			{
				Log.W(string.Format("[MatchingSocket] PickCharacter | status is not 'Matched' | status: {0}", status));
				return;
			}

			Send<object>(Req("PostMatchedPickCharacter", Array.Empty<object>()),
				delegate(BlisWebSocketRequest req, object res)
				{
					if (res == null)
					{
						HandleError(req, "PostMatchedPickCharacter");
					}
				});
		}


		public void CancelPickCharacter()
		{
			if (status != Status.Matched)
			{
				Log.W(string.Format("[MatchingSocket] CancelPickCharacter | status is not 'Matched' | status: {0}",
					status));
				return;
			}

			Send<object>(Req("PostMatchingCancelPickCharacter", Array.Empty<object>()),
				delegate(BlisWebSocketRequest req, object res)
				{
					if (res == null)
					{
						HandleError(req, "PostMatchingCancelPickCharacter");
					}
				});
		}


		public void SelectRoute(long route)
		{
			if (status != Status.Matched)
			{
				Log.W(string.Format("[MatchingSocket] SelectRoute | status is not 'Matched' | status: {0}", status));
				return;
			}

			Send<object>(Req("PostMatchedSelectRoute", "route", route), delegate(BlisWebSocketRequest req, object res)
			{
				if (res == null)
				{
					HandleError(req, "PostMatchedSelectRoute");
				}
			});
		}


		protected override IEnumerator DelayExit(Action callback)
		{
			yield return new WaitForSeconds(0.5f);
			if (status != Status.WaitingExit)
			{
				Log.V("[CustomGameWebSocket] ExitMatching - Blocked by {0}", status);
				yield break;
			}

			exitCallback = callback;
			Close();
		}


		protected override bool OnClose()
		{
			if (status == Status.WaitingExit)
			{
				status = Status.Exited;
				return true;
			}

			return false;
		}


		public void SendChatMessage(string message)
		{
			if (status != Status.Matched)
			{
				Log.W("[MatchingSocket.SendChatMessage] Status is not 'Matched'");
				return;
			}

			Send<object>(Req("PostMatchingChatting", "message", message),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "PostMatchingChatting");
					}
				});
		}


		public void SendCustomChatting(string message)
		{
			if (status != Status.InCustomRoom)
			{
				Log.W("[MatchingSocket.SendCustomChatting] Status is not 'Matched'");
				return;
			}

			Send<object>(Req("CustomGameChatMessage", "message", message),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "CustomGameChatMessage");
					}
				});
		}


		public void CreateCustomGameRoom(string tokenKey, long userNum, Action<CustomGameRoom> callback)
		{
			Log.V("[CustomGameWebSocket] Connect to {0}", host);
			Connect(host, () => Send<Dictionary<string, object>>(
				Req(nameof(CreateCustomGameRoom), (object) "customGameTokenKey", (object) tokenKey,
					(object) nameof(userNum), (object) userNum), (request, result) =>
				{
					if (result == null)
					{
						HandleError(request, nameof(CreateCustomGameRoom));
					}
					else
					{
						status = Status.InCustomRoom;
						CustomGameRoom customGameRoom = ((JToken) result["roomInfo"]).ToObject<CustomGameRoom>();
						Action<CustomGameRoom> action = callback;
						if (action == null)
						{
							return;
						}

						action(customGameRoom);
					}
				}), () => MonoBehaviourInstance<LobbyUI>.inst?.UIEvent.OnLobbyStateUpdate(LobbyState.Ready));
		}
		
		public void EnterCustomGameRoom(string tokenKey, string customRoomKey, long userNum,
			Action<CustomGameRoom> callback)
		{
			Log.V("[CustomGameWebSocket] Connect to {0}", host);
			
			WebSocketRequest req = Req(nameof(EnterCustomGameRoom),
				(object) nameof(tokenKey),
				(object) tokenKey,
				(object) "roomKey",
				(object) customRoomKey,
				(object) nameof(userNum),
				(object) userNum);
			
			Connect(host, onConnect, onFailed);

			void onConnect()
			{
				Send<Dictionary<string, object>>(req, (request, result) =>
				{
					if (result == null)
					{
						HandleError(request, nameof(EnterCustomGameRoom));
					}
					else
					{
						status = Status.InCustomRoom;
						CustomGameRoom customGameRoom = ((JToken) result["roomInfo"]).ToObject<CustomGameRoom>();
						Action<CustomGameRoom> action = callback;
						if (action == null)
						{
							return;
						}

						action(customGameRoom);
					}
				});
			}

			void onFailed()
			{
				if (MonoBehaviourInstance<LobbyUI>.inst)
					MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnLobbyStateUpdate(LobbyState.Ready);
			}
		}

		public void LeaveCustomGameRoom(Action callback)
		{
			if (callback == null)
			{
				throw new NullReferenceException();
			}

			Log.V("[CustomGameWebSocket.LeaveCustomMatchRoom]");
			if (status != Status.InCustomRoom)
			{
				Log.W("[CustomGameWebSocket.LeaveCustomMatchRoom] MatchingStatus is not 'InCustomRoom'");
				return;
			}

			status = Status.WaitingExit;
			Send<object>(Req("LeaveCustomGameRoom", Array.Empty<object>()),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "LeaveCustomGameRoom");
						return;
					}

					StartCoroutine(DelayExit(callback));
				});
		}


		public void KickUserCustomGameRoom(long kickUserNum)
		{
			if (status != Status.InCustomRoom)
			{
				Log.W("[CustomGameWebSocket.KickUserCustomGameRoom] Status is not 'InCustomRoom'");
				return;
			}

			Send<object>(Req("KickCustomGameRoom", "kickUserNum", kickUserNum),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "KickCustomGameRoom");
					}
				});
		}


		public void AddBotCustomGameRoom(int slotNum, int characterCode, string name)
		{
			if (status != Status.InCustomRoom)
			{
				Log.W("[CustomGameWebSocket.AddBotCustomGameRoom] MatchingStatus is not 'InCustomRoom'");
				return;
			}

			Send<object>(Req("AddBotCustomGameRoom", "slotNum", slotNum, "characterCode", characterCode, "name", name),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "AddBotCustomGameRoom");
					}
				});
		}


		public void RemoveBotCustomGameRoom(int slotNum)
		{
			if (status != Status.InCustomRoom)
			{
				Log.W("[CustomGameWebSocket.RemoveBotCustomGameRoom] MatchingStatus is not 'InCustomRoom'");
				return;
			}

			Send<object>(Req("RemoveBotCustomGameRoom", "slotNum", slotNum),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "RemoveBotCustomGameRoom");
					}
				});
		}


		public void UpdateAcceleration(bool isOn)
		{
			if (status != Status.InCustomRoom)
			{
				Log.W("[CustomGameWebSocket.UpdateAcceleration] Status is not 'InCustomRoom'");
				return;
			}

			Send<object>(Req("UpdateAccelerationCustomGameRoom", "isOn", isOn),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "UpdateAccelerationCustomGameRoom");
					}
				});
		}


		public void MoveSlot(int to)
		{
			if (status != Status.InCustomRoom)
			{
				Log.W("[CustomGameWebSocket.MoveSlot] Status is not 'InCustomRoom'");
				return;
			}

			Send<object>(Req("MoveSlotCustomGameRoom", "targetSlot", to),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "MoveSlotCustomGameRoom");
					}
				});
		}
		
		public void MoveObserverSlot()
		{
			if (status != Status.InCustomRoom)
			{
				Log.W("[CustomGameWebSocket.MoveObserverSlot] Status is not 'InCustomRoom'");
				return;
			}

			Send<object>(Req("MoveObserverSlot", Array.Empty<object>()),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "MoveObserverSlot");
					}
				});
		}


		public void MoveUserToObserverSlot(long moveUserNum)
		{
			if (status != Status.InCustomRoom)
			{
				Log.W("[CustomGameWebSocket.MoveObserverSlot] Status is not 'InCustomRoom'");
				return;
			}

			Send<object>(Req("MoveUserToObserverSlot", "moveUserNum", moveUserNum),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "MoveUserToObserverSlot");
					}
				});
		}


		public void StartCustomGame()
		{
			if (status != Status.InCustomRoom)
			{
				Log.W("[CustomGameWebSocket.StartCustomGame] Status is not 'InCustomRoom'");
				return;
			}

			Send<object>(Req("StartCustomGame", Array.Empty<object>()),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "StartCustomGame");
					}
				});
		}


		private enum Status
		{
			Idle,

			WaitForStartMatching,

			Matching,

			Matched,

			InCustomRoom,

			WaitingExit,

			Exited
		}
	}
}