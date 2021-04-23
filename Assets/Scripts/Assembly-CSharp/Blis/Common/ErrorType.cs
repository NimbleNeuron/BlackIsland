namespace Blis.Common
{
	public enum ErrorType
	{
		None,
		Internal,
		CommandQueueOverflowException,
		NetworkNotReachable,
		InvalidBattleToken = 1000,
		AlreadyExistUserId,
		GameStartedAlready,
		MatchingServerNone,
		MatchingTeamModeNone,
		UserGameFinished,
		UnavailableNickname = 1111,
		GAME_ERROR_BOUND = 10000,
		ObjectNotFound,
		NotEnoughInventory,
		NotEnoughItemBox,
		NotEnoughItem,
		InvalidItemCode,
		ItemNotFound,
		InvalidAction,
		NotEnoughSp,
		NotExistUserId,
		CharacterNotAlive,
		NotAvailableYet,
		CantConsumeItem,
		InvalidKey
	}
}