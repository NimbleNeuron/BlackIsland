using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class StartConditionService : ServiceBase
	{
		
		public bool CheckAllUserReady()
		{
			if (this.game.GameStatus >= GameStatus.ReadyToStart)
			{
				Log.W("GameStartedAlready");
				return false;
			}
			GameStartCondition gameStartCondition = this.gameStartCondition;
			if (gameStartCondition != GameStartCondition.ManualReady)
			{
				if (gameStartCondition != GameStartCondition.FullCapacity)
				{
				}
				return this.game.Player.TotalSessionCount == this.capacity && this.game.Player.IsAllReady();
			}
			return this.game.Player.IsAllReady();
		}

		
		public void SetGameStartCondition(GameStartCondition gameStartCondition, int capacity)
		{
			if (this.game.GameStatus >= GameStatus.WaitForFirstJoin)
			{
				return;
			}
			this.game.SetGameStatus(GameStatus.WaitForFirstJoin);
			this.gameStartCondition = gameStartCondition;
			this.capacity = capacity;
			if (this.gameStartCondition == GameStartCondition.FullCapacity)
			{
				this.gameStartTimer = base.StartCoroutine(this.GameStartTimer());
			}
		}

		
		private IEnumerator GameStartTimer()
		{
			Log.W("Start Coroutine For GameStart Timeout: {0}", new object[]
			{
				this.game.GameStatus
			});
			yield return new WaitForSeconds(GameConstants.USER_READY_TIMEOUT);
			Log.W("Check GameStart Timeout: {0}", new object[]
			{
				this.game.GameStatus
			});
			if (this.game.GameStatus == GameStatus.LevelLoaded)
			{
				Log.W("StartGame although all user isn't coming");
				this.SetupGame();
			}
		}

		
		
		public DateTime SetupGameStartTime
		{
			get
			{
				return this.setupGameStartTime;
			}
		}

		
		public void SetupGame()
		{
			this.game.SetGameStatus(GameStatus.ReadyToStart);
			if (this.gameStartTimer != null)
			{
				base.StopCoroutine(this.gameStartTimer);
				this.gameStartTimer = null;
			}
			int gameStandbyTime = GameConstants.GameStandbyTime;
			this.setupGameStartTime = DateTime.Now;
			this.server.LogConnectionMap();
			this.server.LogSessionMap();
			Log.V("[GameServer] Send SetupGame.");
			this.server.Broadcast(new RpcSetupGame
			{
				standbySecond = gameStandbyTime
			}, NetChannel.ReliableOrdered);
			this.server.BuildTeam();
			this.game.Bot.SettingBotTeam();
			if (this.game.MatchingMode.IsTutorialMode())
			{
				this.game.Bot.SetTutorialStrategySheet();
			}
			else
			{
				this.game.Bot.SetStrategySheet();
			}
			base.StartCoroutine(this.DelayStartGame(gameStandbyTime));
		}

		
		private IEnumerator DelayStartGame(int count)
		{
			yield return new WaitForSeconds((float)count);
			if (this.game.GameStatus == GameStatus.ReadyToStart)
			{
				this.game.StartGame();
			}
		}

		
		private GameStartCondition gameStartCondition;

		
		private Coroutine gameStartTimer;

		
		private int capacity;

		
		private DateTime setupGameStartTime;
	}
}
