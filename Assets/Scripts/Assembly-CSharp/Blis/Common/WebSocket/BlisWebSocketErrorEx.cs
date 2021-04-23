using System;

namespace Blis.Common
{
	
	public static class BlisWebSocketErrorEx
	{
		
		public static bool CloseOnError(this BlisWebSocketError error)
		{
			if (error <= BlisWebSocketError.NoBattleHost)
			{
				if (error != BlisWebSocketError.UnknownMatchingError)
				{
					if (error != BlisWebSocketError.NoBattleHost)
					{
						goto IL_88;
					}

					return false;
				}
			}
			else if (error != BlisWebSocketError.OutOfService)
			{
				switch (error)
				{
					case BlisWebSocketError.InvalidMatchingMode:
					case BlisWebSocketError.AlreadyJoinedMatching:
					case BlisWebSocketError.EmptyMatchingToken:
					case BlisWebSocketError.InvalidCommand:
					case BlisWebSocketError.NotMatchingUser:
					case BlisWebSocketError.InvalidMatchingToken:
					case BlisWebSocketError.AlreadyJoinCustomGame:
					case BlisWebSocketError.NotExistCustomGame:
					case BlisWebSocketError.NotJoinCustomGame:
					case BlisWebSocketError.InvalidCustomGameKey:
					case BlisWebSocketError.InvalidParameter:
					case BlisWebSocketError.NotOwnerUser:
					case BlisWebSocketError.CannotJoinFullRoom:
					case BlisWebSocketError.CannotJoinCustomGameStarted:
						break;
					case (BlisWebSocketError) 2002:
						goto IL_88;
					case BlisWebSocketError.CannotAddBotFullRoom:
					case BlisWebSocketError.CannotStartGameLackOfUserCount:
					case BlisWebSocketError.AlreadyCustomGameStarted:
					case BlisWebSocketError.InvalidSlot:
					case BlisWebSocketError.CannotStartInvalidBotTeam:
					case BlisWebSocketError.CannotStartInvalidTeamInfo:
					case BlisWebSocketError.CannotMoveBotTeam:
						return false;
					default:
						goto IL_88;
				}
			}

			return true;
			IL_88:
			throw new ArgumentOutOfRangeException("error", error, null);
		}
	}
}