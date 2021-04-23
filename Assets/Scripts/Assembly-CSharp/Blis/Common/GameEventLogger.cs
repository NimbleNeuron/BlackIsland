using System;

namespace Blis.Common
{
	public class GameEventLogger : Singleton<GameEventLogger>
	{
		public string gameMode;


		public int gamePauseCount;


		public int matchMakingTime;


		public int maxBufferCount;


		public int noCommandCount;


		public string roomKey;


		public string serverName;


		public int warpCount;

		public GameEventLogger()
		{
			gameMode = "";
			serverName = "";
		}


		public void SetBufferCount(int count)
		{
			maxBufferCount = Math.Max(count, maxBufferCount);
		}


		public void SetMatchmakingTime(int matchMakingTime)
		{
			this.matchMakingTime = matchMakingTime;
		}


		public void SetRoomKey(string roomKey)
		{
			this.roomKey = roomKey;
		}


		public void IncNoCommandCount()
		{
			noCommandCount++;
		}


		public void SetGameMode(string gameMode)
		{
			this.gameMode = gameMode;
		}


		public void SetServerName(string serverName)
		{
			this.serverName = serverName;
		}


		public bool IsNotChina()
		{
			return !GlobalUserData.gaap;
		}
	}
}