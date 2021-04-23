namespace Blis.Common
{
	public static class ErrorTypeEx
	{
		public static bool IsGameError(this ErrorType errorType)
		{
			return errorType >= ErrorType.GAME_ERROR_BOUND;
		}
	}
}