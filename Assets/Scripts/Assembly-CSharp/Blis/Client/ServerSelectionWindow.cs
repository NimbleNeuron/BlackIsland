using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ServerSelectionWindow : BaseWindow
	{
		[SerializeField] private GameObject serverSelectionItem = default;


		[SerializeField] private GameObject serverSelectionItem_CN = default;


		[SerializeField] private ToggleGroup serverList = default;


		[SerializeField] private Text accessDesc = default;


		private readonly List<RegionRecord> regionRecords = new List<RegionRecord>();


		private AccelerateChina accelerateChina;


		private Coroutine accessMoniteringCoroutine;


		private bool currentAccelerateChina;


		private MatchingRegion currentRegion;


		private bool tempAccelerateChina;


		private MatchingRegion tempRegion;

		protected override void OnOpen()
		{
			base.OnOpen();
			tempRegion = currentRegion;
			tempAccelerateChina = currentAccelerateChina;
			for (int i = 0; i < GameDB.platform.serverRegions.Count; i++)
			{
				MatchingRegion region = GameDB.platform.serverRegions[i].region;
				RegionRecord regionRecord =
					new RegionRecord(Instantiate<GameObject>(serverSelectionItem, serverList.transform), region);
				regionRecords.Add(regionRecord);
				if (regionRecord.ServerRegion == currentRegion)
				{
					regionRecord.SelectServer();
				}

				regionRecord.SetToggleGroup(serverList);
				regionRecord.SetToggleEvent(delegate(MatchingRegion selectedRegion) { tempRegion = selectedRegion; });
				regionRecord.UpdatePing();
				if (region == MatchingRegion.HongKong)
				{
					GameObject gameObject = Instantiate<GameObject>(serverSelectionItem_CN, serverList.transform);
					accelerateChina = new AccelerateChina(gameObject);
					accelerateChina.SetToggleEvent(delegate(bool isOn) { tempAccelerateChina = isOn; });
					accelerateChina.Update(currentAccelerateChina);
					gameObject.SetActive(Ln.GetCurrentLanguage() == SupportLanguage.ChineseSimplified);
				}
			}

			if (accessMoniteringCoroutine != null)
			{
				StopCoroutine(accessMoniteringCoroutine);
			}

			accessMoniteringCoroutine = this.StartThrowingCoroutine(CheckServerAccessable(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][CheckServerAccessable] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator CheckServerAccessable()
		{
			accessDesc.gameObject.SetActive(false);
			for (;;)
			{
				yield return new WaitForSeconds(1f);
				if (!IsOpen)
				{
					break;
				}

				if (regionRecords.Exists(x => x.CanAccess))
				{
					accessDesc.gameObject.SetActive(false);
				}
				else
				{
					accessDesc.gameObject.SetActive(true);
				}
			}
		}


		protected override void OnClose()
		{
			base.OnClose();
			Transform _tr = serverList.transform;
			for (int i = 0; i < _tr.childCount; i++)
			{
				Destroy(_tr.GetChild(i).gameObject);
			}

			regionRecords.Clear();
			accelerateChina = null;
			if (accessMoniteringCoroutine != null)
			{
				StopCoroutine(accessMoniteringCoroutine);
			}
		}


		public void OnClickSave()
		{
			MonoBehaviourInstance<LobbyService>.inst.SelectServerRegion(tempRegion);
			MonoBehaviourInstance<LobbyService>.inst.UpdateAccelerateChina(tempAccelerateChina);
			Close();
		}


		public void OnSelectServerRegion(MatchingRegion currentRegion)
		{
			this.currentRegion = currentRegion;
			for (int i = 0; i < regionRecords.Count; i++)
			{
				if (regionRecords[i].ServerRegion == currentRegion)
				{
					regionRecords[i].SelectServer();
					return;
				}
			}
		}


		public void OnChangeAccelerateChina(bool isOn)
		{
			currentAccelerateChina = isOn;
			AccelerateChina accelerateChina = this.accelerateChina;
			if (accelerateChina == null)
			{
				return;
			}

			accelerateChina.Update(isOn);
		}


		private class AccelerateChina
		{
			private readonly LnText text;


			private readonly Toggle toggle;

			public AccelerateChina(GameObject parent)
			{
				toggle = GameUtil.Bind<Toggle>(parent, "CheckBox");
				text = GameUtil.Bind<LnText>(parent, "Text");
				text.text = Ln.Get("중국지역가속안내");
			}


			public void Update(bool isOn)
			{
				toggle.isOn = isOn;
			}


			public void SetToggleEvent(UnityAction<bool> e)
			{
				toggle.onValueChanged.AddListener(delegate { e(toggle.isOn); });
			}
		}


		private class RegionRecord
		{
			private const string SIGNAL_01 = "Ico_Wifi_01";


			private const string SIGNAL_02 = "Ico_Wifi_02";


			private const string SIGNAL_03 = "Ico_Wifi_03";


			private const string SIGNAL_04 = "Ico_Wifi_04";


			private static readonly Color32 COLOR_BLACK = new Color32(190, 190, 190, byte.MaxValue);


			private static readonly Color32 COLOR_RED = new Color32(byte.MaxValue, 60, 0, byte.MaxValue);


			private readonly LnText ping;


			private readonly Image signal;


			private readonly LnText text;


			private readonly Toggle toggle;


			private bool canAccess;


			private LnText serverState;


			public RegionRecord(GameObject parent, MatchingRegion serverRegion)
			{
				ServerRegion = serverRegion;
				toggle = GameUtil.Bind<Toggle>(parent, "CheckBox");
				text = GameUtil.Bind<LnText>(parent, "Text");
				signal = GameUtil.Bind<Image>(parent, "Signal");
				ping = GameUtil.Bind<LnText>(parent, "Ping");
				serverState = GameUtil.Bind<LnText>(parent, "State");
				text.text = Ln.Get(LnType.ServerRegion, serverRegion.ToString());
			}


			public MatchingRegion ServerRegion { get; }


			public bool CanAccess => canAccess;


			public void UpdatePing()
			{
				ping.text = "???ms";
				ping.color = COLOR_BLACK;
				SingletonMonoBehaviour<PingUtil>.inst.GetPingMs(ServerRegion, UpdateInternal);
			}


			private void UpdateInternal(int ms)
			{
				if (ping.IsDestroyed())
				{
					return;
				}

				ping.text = string.Format("{0}ms", ms);
				if (ms < 0)
				{
					canAccess = false;
					ping.text = Ln.Get("확인 불가");
					ping.color = COLOR_RED;
					signal.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Wifi_01");
					return;
				}

				canAccess = true;
				ping.color = COLOR_BLACK;
				if (ms <= 100)
				{
					signal.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Wifi_04");
					return;
				}

				if (ms <= 200)
				{
					signal.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Wifi_03");
					return;
				}

				signal.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Wifi_02");
			}


			public void SetToggleGroup(ToggleGroup toggleGroup)
			{
				toggle.group = toggleGroup;
			}


			public void SetToggleEvent(UnityAction<MatchingRegion> e)
			{
				toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					if (isOn)
					{
						e(ServerRegion);
					}
				});
			}


			public void SelectServer()
			{
				toggle.isOn = true;
			}
		}
	}
}