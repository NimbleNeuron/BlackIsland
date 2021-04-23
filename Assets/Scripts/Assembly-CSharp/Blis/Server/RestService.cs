using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Blis.Server
{
	
	public class RestService : MonoBehaviourInstance<RestService>
	{
		
		
		private string apiProxyUrl
		{
			get
			{
				return ApiConstants.ApiProxyUrl;
			}
		}

		
		
		public BattleGame BattleGame
		{
			get
			{
				return this.battleGame;
			}
		}

		
		public void Init(string restRootUrl)
		{
			this.restRootUrl = restRootUrl;
		}

		
		public void InitBattleToken(string battleTokenKey, BattleToken battleToken)
		{
			this.battleGame = new BattleGame(battleTokenKey, battleToken);
			using (SHA256 sha = SHA256.Create())
			{
				byte[] bytes = Encoding.ASCII.GetBytes(this.secKey + battleTokenKey);
				byte[] value = sha.ComputeHash(bytes);
				this.battleHash = BitConverter.ToString(value).Replace("-", "");
			}
		}

		
		public void LogBattleGameStart(List<long> sessionUserNumList)
		{
			this.battleGame.startDtm = DateTime.Now;
			this.battleGame.userIds = sessionUserNumList;
			base.StartCoroutine(this.Request("POST", "/battle/games", this.battleGame));
		}

		
		public IEnumerator LogBattleGameFinish()
		{
			this.battleGame.endDtm = DateTime.Now;
			yield return base.StartCoroutine(this.Request("PUT", "/battle/games", this.battleGame));
		}

		
		public void LogBattleUserGame(WorldPlayerCharacter playerCharacter, int rewardACoin, long attackerUserId, float matchRating)
		{
			if (playerCharacter.PlayerSession.userId <= 0L)
			{
				return;
			}
			BattleUserGame battleUserGame = new BattleUserGame(this.battleGame, playerCharacter, rewardACoin, attackerUserId, matchRating);
			string path = string.Format("/battle/games/{0}/{1}", battleUserGame.userNum, this.battleGame.battleTokenKey);
			Log.W("BattleUserGame: {0}", Jsons.Serialize<BattleUserGame>(battleUserGame));
			base.StartCoroutine(this.Request("PUT", path, battleUserGame));
		}

		
		public IEnumerator LogBattleChat(BattleChatContext context)
		{
			if (context.messages.IsEmpty<BattleChatMessage>())
			{
				yield break;
			}
			string arg = context.startDtm.ToString("yyyy-MM-dd");
			long gameId = context.gameId;
			string path = string.Format("/chatLog/{0}/{1}", arg, gameId);
			yield return base.StartCoroutine(this.RequestToApiProxy("POST", path, context));
		}

		
		public CustomYieldInstruction WaitForComplete()
		{
			this.isStopRetry = true;
			return new WaitUntil(() => this.numRequests == 0);
		}

		
		private IEnumerator Request(string method, string path, object content)
		{
			return this.Request(method, this.restRootUrl, path, content);
		}

		
		private IEnumerator RequestToApiProxy(string method, string path, object content)
		{
			return this.Request(method, this.apiProxyUrl, path, content);
		}

		
		private IEnumerator Request(string method, string urlPrefix, string path, object content)
		{
			if (string.IsNullOrEmpty(urlPrefix))
			{
				Log.E("[RestService] urlPrefix is empty");
				yield break;
			}
			try
			{
				this.numRequests++;
				string json = Jsons.Serialize<object>(content);
				byte[] data = Encoding.UTF8.GetBytes(json);
				string url = urlPrefix + (urlPrefix.EndsWith("/") ? path.Substring(1) : path);
				int num;
				for (int retry = 0; retry <= this.maxRetry; retry = num)
				{
					UnityWebRequest request = new UnityWebRequest(url, method);
					UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(data);
					uploadHandlerRaw.contentType = "application/json; charset=utf-8";
					request.timeout = this.requestTimeout;
					request.uploadHandler = uploadHandlerRaw;
					yield return request.SendWebRequest();
					if (!request.isNetworkError && !request.isHttpError)
					{
						yield break;
					}
					Log.W("[RestService] error:{3}, method:{0}, url:{1}, content:{2}", new object[]
					{
						method,
						url,
						json,
						request.error
					});
					if (this.isStopRetry)
					{
						break;
					}
					yield return new WaitForSeconds(UnityEngine.Random.value * (float)(retry + 1));
					request = null;
					num = retry + 1;
				}
				Log.E("[RestService] ERROR method:{0}, url:{1}, content:{2}", new object[]
				{
					method,
					url,
					json
				});
				json = null;
				data = null;
				url = null;
			}
			finally
			{
				this.numRequests--;
			}
		}

		
		private readonly string secKey = "";

		
		private readonly int maxRetry = 2;

		
		private readonly int requestTimeout = 5;

		
		private string restRootUrl = "";

		
		private BattleGame battleGame;

		
		private string battleHash;

		
		private bool isStopRetry;

		
		private int numRequests;
	}
}
