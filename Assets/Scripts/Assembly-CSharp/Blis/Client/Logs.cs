using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Blis.Common;
using Blis.Server;
using Tftp.Net;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class Logs : MonoBehaviour
	{
		private static readonly AutoResetEvent TransferFinishedEvent = new AutoResetEvent(false);


		private readonly List<string> _logs = new List<string>();


		private Button _button;


		private GameObject _content;


		private GameServer _gameServer;


		private ScrollRect _scrollRect;

		private void Start()
		{
			GameObject gameObject = new GameObject("Server");
			gameObject.AddComponent<GameServer>();
			_scrollRect = GameObject.Find("LogScrollView").GetComponent<ScrollRect>();
			_content = GameObject.Find("LogContent");
			_button = GameObject.Find("LoadButton").GetComponent<Button>();
			_button.onClick.AddListener(OnClick);
			_gameServer = gameObject.GetComponent<GameServer>();
			StartCoroutine(LoadWorld());
		}


		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				_scrollRect.verticalNormalizedPosition = 0f;
			}
		}


		private void OnClick()
		{
			Log.V("Clicked!");
			ResWorldSnapshot worldSnapshot = RequestWorld(0);
			LoadWorld(worldSnapshot);
		}


		private IEnumerator LoadWorld()
		{
			BattleToken battleToken = new BattleToken
			{
				botCount = 15,
				botDifficulty = BotDifficulty.EASY,
				matchingMode = MatchingMode.Normal,
				matchingTeamMode = MatchingTeamMode.Solo
			};
			yield return SingletonMonoBehaviour<GameDBLoader>.inst.LoadCache(GameConstants.GetDataCacheFilePath());
			if (!SingletonMonoBehaviour<GameDBLoader>.inst.Result.success)
			{
				Log.E("Failed to load GameDB {0}, Error: ", SingletonMonoBehaviour<GameDBLoader>.inst.Result.reason);
				yield break;
			}

			_gameServer.GameService.LoadLevel(GameDB.level.DefaultLevel, battleToken, "Default");
		}


		private void LoadWorld(ResWorldSnapshot worldSnapshot)
		{
			for (int i = _content.transform.childCount - 1; i >= 0; i--)
			{
				Destroy(_content.transform.GetChild(i));
			}

			int currentSeq = worldSnapshot.currentSeq;
			_logs.Clear();
			worldSnapshot.snapshot.ForEach(delegate(SnapshotWrapper snapshotWrapper)
			{
				Log.V(snapshotWrapper.ToReflectionString());
				_logs.Add(snapshotWrapper.ToReflectionString());
			});
			int num = worldSnapshot.snapshot.Count<SnapshotWrapper>();
			AddLog(string.Format("currentSeq({0}) : totalCount({1})", currentSeq, num));
			foreach (string log in _logs.Skip(num - 200))
			{
				AddLog(log);
			}
		}


		private void AddLog(string log)
		{
			GameObject gameObject = new GameObject("logText");
			gameObject.transform.parent = _content.transform;
			Text text = gameObject.AddComponent<Text>();
			text.text = log.Replace("\n", "");
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			text.color = Color.black;
			text.font = Font.CreateDynamicFontFromOSFont("KBIZM", 25);
			text.fontSize = 25;
		}


		public ResWorldSnapshot RequestWorld(int seq)
		{
			ITftpTransfer tftpTransfer =
				new TftpClient("localhost", _gameServer.ListenPort + 1000).Download(string.Format("{0}.world", seq));
			tftpTransfer.OnProgress += OnTransferProgress;
			tftpTransfer.OnFinished += OnTransferFinished;
			tftpTransfer.OnError += OnTransferError;
			tftpTransfer.RetryCount = 0;
			Stream stream = new MemoryStream();
			tftpTransfer.Start(stream);
			TransferFinishedEvent.WaitOne(20);
			byte[] data = ((MemoryStream) stream).ToArray();
			return Serializer.Compression.Deserialize<ResWorldSnapshot>(data);
		}


		private void OnTransferProgress(ITftpTransfer transfer, TftpTransferProgress progress) { }


		private void OnTransferFinished(ITftpTransfer transfer)
		{
			OutputTransferStatus(transfer, "Finished");
		}


		private void OnTransferError(ITftpTransfer transfer, TftpTransferError error)
		{
			OutputTransferStatus(transfer, "Error: " + error);
		}


		private void OutputTransferStatus(ITftpTransfer transfer, string message)
		{
			Log.V("File Transfer (" + transfer.Filename + ") : " + message);
		}
	}
}