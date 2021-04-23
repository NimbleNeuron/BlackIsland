using System;
using Blis.Common;

namespace Blis.Server
{
	
	public class BattleTokenContext
	{
		
		public BattleTokenContext(RedisService redisService)
		{
			this.redisService = redisService;
		}

		
		public void GetBattleToken(string battleTokenKey, bool isReconnect, Action<bool, BattleToken> callback)
		{
			if (isReconnect && this.battleTokenKey != battleTokenKey)
			{
				throw new GameException(ErrorType.UserGameFinished);
			}
			if (this.battleToken == null)
			{
				this.redisService.GetBattleToken(battleTokenKey, delegate(BattleToken battleToken)
				{
					Log.W("[GetBattleToken] first get battle token!");
					this.battleToken = battleToken;
					this.battleTokenKey = battleTokenKey;
					callback(true, battleToken);
				});
				return;
			}
			if (this.battleTokenKey != battleTokenKey)
			{
				throw new GameException(ErrorType.InvalidBattleToken, "[BattleTokenContext] GetBattleToken is not matched. " + this.battleTokenKey + " != " + battleTokenKey);
			}
			callback(false, this.battleToken);
		}

		
		private RedisService redisService;

		
		private string battleTokenKey;

		
		private BattleToken battleToken;
	}
}
