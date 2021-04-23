namespace Blis.Common
{
	public enum BlisWebSocketError
	{
		UnknownMatchingError = 1,
		NoBattleHost = 1001,
		OutOfService,
		InvalidMatchingMode = 2000,
		AlreadyJoinedMatching,
		EmptyMatchingToken = 2003,
		InvalidCommand,
		NotMatchingUser,
		InvalidMatchingToken,
		AlreadyJoinCustomGame,
		NotExistCustomGame,
		NotJoinCustomGame,
		InvalidCustomGameKey,
		InvalidParameter,
		NotOwnerUser,
		CannotJoinFullRoom,
		CannotAddBotFullRoom,
		CannotStartGameLackOfUserCount,
		CannotJoinCustomGameStarted,
		AlreadyCustomGameStarted,
		InvalidSlot,
		CannotStartInvalidBotTeam,
		CannotStartInvalidTeamInfo,
		CannotMoveBotTeam
	}
}