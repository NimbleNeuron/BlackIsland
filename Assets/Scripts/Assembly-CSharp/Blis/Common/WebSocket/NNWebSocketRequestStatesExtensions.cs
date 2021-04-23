namespace Blis.Common
{
	public static class NNWebSocketRequestStatesExtensions
	{
		public static bool IsFinal(this NNWebSocketRequestStates state)
		{
			return state >= NNWebSocketRequestStates.Done;
		}
		
		public static bool IsDone(this NNWebSocketRequestStates state)
		{
			return state == NNWebSocketRequestStates.Done;
		}
	}
}