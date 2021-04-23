namespace Blis.Common
{
	public static class NetFactory
	{
		public static INetClient CreateClient()
		{
			return new LiteNetClient();
		}


		public static INetServer CreateServer()
		{
			return new LiteNetServer();
		}
	}
}