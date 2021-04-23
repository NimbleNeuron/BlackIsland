using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class MatchingConfirmPopup : BasePopup
	{
		private readonly Vector3 deco_L_from = new Vector3(-228f, 50f, 0f);
		private readonly Vector3 deco_L_to = new Vector3(-208f, 50f, 0f);
		private readonly string deco_off = "Img_Matching_Deco_01";
		private readonly string deco_on = "Img_Matching_Deco_02";
		private readonly Vector3 deco_R_from = new Vector3(228f, 50f, 0f);
		private readonly Vector3 deco_R_to = new Vector3(208f, 50f, 0f);
		private readonly float GaugeDeclineTime = 0.2f;
		private readonly float rotateDialDecoAngularSpeed = 8f;
		private Button acceptBtn;
		private ParticleSystem acceptEffect;
		private ParticleSystem acceptEffect_center;
		private LnText acceptLabel;
		private bool clickAcceptMatching;
		private Coroutine closeCoroutine;
		private Button declineBtn;
		private LnText declineLabel;
		private Image decoL;
		private PositionTweener decoLTweener;
		private Image decoR;
		private PositionTweener decoRTweener;
		private RectTransform dialDecoRotation;
		private ParticleSystem finishEffect;
		private Image gaugeBar;
		private float gaugeDeclineElapsedTime;
		private ParticleSystem gaugeEffect;
		private ParticleSystem gaugeEffect_static;
		private RectTransform gaugeEffectAnchor;
		private bool isAccept;
		private bool isDecline;
		private int lastSec;
		private Text mode;
		private bool rotateDialDeco;
		private float rotateDialDecoAngle;
		private float startFillAmount;
		private Text teamMode;
		private Text time;
		private float waitTime;

		private void Update()
		{
			if (!IsOpen)
			{
				return;
			}

			UpdateTime();
			UpdateDeclineTime();
			UpdateGaugeEffect();
			UpdateDial();
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			acceptBtn = GameUtil.Bind<Button>(gameObject, "BtnAccept");
			acceptLabel = GameUtil.Bind<LnText>(gameObject, "BtnAccept/Label");
			acceptBtn.onClick.AddListener(OnClickAcceptMatching);
			declineBtn = GameUtil.Bind<Button>(gameObject, "BtnDecline");
			declineLabel = GameUtil.Bind<LnText>(gameObject, "BtnDecline/Label");
			declineBtn.onClick.AddListener(OnClickDeclineMatching);
			dialDecoRotation = GameUtil.Bind<RectTransform>(gameObject, "DialDeco");
			decoL = GameUtil.Bind<Image>(gameObject, "Deco/Left");
			decoR = GameUtil.Bind<Image>(gameObject, "Deco/Right");
			decoLTweener = decoL.GetComponent<PositionTweener>();
			decoRTweener = decoR.GetComponent<PositionTweener>();
			mode = GameUtil.Bind<Text>(gameObject, "TXT_Mode");
			teamMode = GameUtil.Bind<Text>(gameObject, "TXT_TeamMode");
			time = GameUtil.Bind<Text>(gameObject, "TXT_Time");
			gaugeBar = GameUtil.Bind<Image>(gameObject, "Gauge/Gauge_Bar");
			gaugeEffectAnchor = GameUtil.Bind<RectTransform>(gameObject, "Gauge/Gauge_Effect");
			finishEffect = GameUtil.Bind<ParticleSystem>(gameObject, "Gauge/Fx_BI_UI_MatchingCircle_01");
			gaugeEffect =
				GameUtil.Bind<ParticleSystem>(gameObject, "Gauge/Gauge_Effect/Anchor/Fx_BI_UI_MatchingGauge_01");
			gaugeEffect_static = GameUtil.Bind<ParticleSystem>(gameObject, "Gauge/Fx_BI_UI_MatchingGauge_01");
			acceptEffect = GameUtil.Bind<ParticleSystem>(gameObject, "BtnAccept/Fx_BI_UI_MatchingComplete_01");
			acceptEffect_center = GameUtil.Bind<ParticleSystem>(gameObject, "Gauge/Fx_BI_UI_MatchingGlow_01");
			MonoBehaviourInstance<MatchingService>.inst.onCompleteMatchingEvent -= OnCompleteMatching;
			MonoBehaviourInstance<MatchingService>.inst.onCompleteMatchingEvent += OnCompleteMatching;
			MonoBehaviourInstance<MatchingService>.inst.onAcceptMatchingEvent -= OnAcceptMatching;
			MonoBehaviourInstance<MatchingService>.inst.onAcceptMatchingEvent += OnAcceptMatching;
			MonoBehaviourInstance<MatchingService>.inst.onDeclineMatchingEvent -= OnDeclineMatching;
			MonoBehaviourInstance<MatchingService>.inst.onDeclineMatchingEvent += OnDeclineMatching;
		}


		public override bool IgnoreEscapeInputWindow()
		{
			if (MonoBehaviourInstance<LobbyUI>.inst != null &&
			    MonoBehaviourInstance<LobbyUI>.inst.CharacterSelectWindow.IsOpen)
			{
				string format = "[MCP] IgnoreEscapeInputWindow() Already Open CharacterSelectWindow({0})";
				Lobby inst = Lobby.inst;
				LobbyState? lobbyState;
				if (inst == null)
				{
					lobbyState = null;
				}
				else
				{
					LobbyContext lobbyContext = inst.LobbyContext;
					lobbyState = lobbyContext != null ? new LobbyState?(lobbyContext.lobbyState) : null;
				}

				Debug.LogException(new Exception(string.Format(format, lobbyState)));
				return false;
			}

			return true;
		}


		private void OnClickAcceptMatching()
		{
			clickAcceptMatching = true;
			if (MonoBehaviourInstance<LobbyUI>.inst != null &&
			    MonoBehaviourInstance<LobbyUI>.inst.CharacterSelectWindow.IsOpen)
			{
				string format = "[MCP] OnClickAcceptMatching() Already Open CharacterSelectWindow({0})";
				Lobby inst = Lobby.inst;
				LobbyState? lobbyState;
				if (inst == null)
				{
					lobbyState = null;
				}
				else
				{
					LobbyContext lobbyContext = inst.LobbyContext;
					lobbyState = lobbyContext != null ? new LobbyState?(lobbyContext.lobbyState) : null;
				}

				Debug.LogException(new Exception(string.Format(format, lobbyState)));
				Close();
				return;
			}

			if (isAccept || isDecline)
			{
				return;
			}

			MonoBehaviourInstance<MatchingService>.inst.AcceptMatching();
			Singleton<SoundControl>.inst.PlayUISound("oui_btnHover_v2");
		}


		private void OnClickDeclineMatching()
		{
			if (MonoBehaviourInstance<LobbyUI>.inst != null &&
			    MonoBehaviourInstance<LobbyUI>.inst.CharacterSelectWindow.IsOpen)
			{
				string format = "[MCP] OnClickDeclineMatching() Already Open CharacterSelectWindow({0})";
				Lobby inst = Lobby.inst;
				LobbyState? lobbyState;
				if (inst == null)
				{
					lobbyState = null;
				}
				else
				{
					LobbyContext lobbyContext = inst.LobbyContext;
					lobbyState = lobbyContext != null ? new LobbyState?(lobbyContext.lobbyState) : null;
				}

				Debug.LogException(new Exception(string.Format(format, lobbyState)));
				Close();
				return;
			}

			if (isAccept || isDecline)
			{
				return;
			}

			MonoBehaviourInstance<MatchingService>.inst.DeclineMatching();
			Singleton<SoundControl>.inst.PlayUISound("oui_cancel_v1");
		}


		public void OnCompleteMatching()
		{
			if (MonoBehaviourInstance<LobbyUI>.inst != null &&
			    MonoBehaviourInstance<LobbyUI>.inst.CharacterSelectWindow.IsOpen)
			{
				string format = "[MCP] OnCompleteMatching() Already Open CharacterSelectWindow({0})";
				Lobby inst = Lobby.inst;
				LobbyState? lobbyState;
				if (inst == null)
				{
					lobbyState = null;
				}
				else
				{
					LobbyContext lobbyContext = inst.LobbyContext;
					lobbyState = lobbyContext != null ? new LobbyState?(lobbyContext.lobbyState) : null;
				}

				Debug.LogException(new Exception(string.Format(format, lobbyState)));
				return;
			}

			if (GlobalUserData.matchingMode == MatchingMode.Rank || GlobalUserData.matchingMode == MatchingMode.Normal)
			{
				Init();
				Open();
			}
		}


		private void OnAcceptMatching()
		{
			isAccept = true;
			EnableAcceptBtn(false);
			EnableDeclineBtn(false);
			PlayDail(false);
			PlayDeco(true);
			EnableAcceptEffect(true);
		}


		private void OnDeclineMatching()
		{
			isDecline = true;
			startFillAmount = gaugeBar.fillAmount;
			EnableAcceptBtn(false);
			EnableDeclineBtn(false);
			EnableFinishEffect(false);
			SetTime(string.Empty);
			PlayDail(false);
			StopCloseCoroutine();
			StartCloseCoroutine(3f);
		}


		private void Init()
		{
			EnableAcceptBtn(true);
			EnableDeclineBtn(true);
			InitDeco();
			SetModeName(string.Empty);
			SetTeamModeName(string.Empty);
			SetTime(string.Empty);
			SetGaugeBarFill(0f);
			SetRotationGaugeEffect(Quaternion.identity);
			EnableFinishEffect(false);
			EnableGaugeEffect(false);
			EnableAcceptEffect(false);
			isAccept = false;
			isDecline = false;
			waitTime = 0f;
			lastSec = 0;
			rotateDialDeco = false;
			rotateDialDecoAngle = 0f;
			gaugeDeclineElapsedTime = 0f;
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			StopCloseCoroutine();
			StartCloseCoroutine(20f);
			SetModeName(LnUtil.Get(MonoBehaviourInstance<MatchingService>.inst.MatchingMode));
			SetTeamModeName(LnUtil.Get(MonoBehaviourInstance<MatchingService>.inst.MatchingTeamMode));
			SetTime(GameConstants.MatchingAcceptWaitTime.ToString());
			PlayDail(true);
			PlayDeco(false);
			EnableFinishEffect(true);
			EnableGaugeEffect(true);
			Singleton<SoundControl>.inst.PlayUISound("10count");
			clickAcceptMatching = false;
		}


		protected override void OnClose()
		{
			base.OnClose();
			WindowController.FlashStopWindowEx();
			StopCloseCoroutine();
			Singleton<SoundControl>.inst.StopUIAudio("10count");
			RequestDelegate.request<MatchingApi.GetMatchingDeclineCountResult>(
				MatchingApi.GetMatchingDeclineCount(GlobalUserData.matchingMode), delegate(RequestDelegateError err,
					MatchingApi.GetMatchingDeclineCountResult res)
				{
					if (err != null)
					{
						return;
					}

					if (GlobalUserData.matchingMode == MatchingMode.Normal)
					{
						Lobby.inst.SetNormalMatchingPenaltyTime(res.matchingPenaltyTime);
					}

					if (GlobalUserData.matchingMode == MatchingMode.Rank)
					{
						Lobby.inst.SetRankMatchingPenaltyTime(res.matchingPenaltyTime);
					}

					if (!clickAcceptMatching && res.matchingDeclineCount == 1)
					{
						MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("매칭 거절 패널티 안내"), new Popup.Button
						{
							type = Popup.ButtonType.Confirm,
							text = Ln.Get("확인")
						});
					}
				});
		}


		private void UpdateTime()
		{
			if (isDecline)
			{
				return;
			}

			if (GameConstants.MatchingAcceptWaitTime < waitTime)
			{
				return;
			}

			waitTime += Time.deltaTime;
			gaugeBar.fillAmount = waitTime / GameConstants.MatchingAcceptWaitTime;
			int obj = GameConstants.MatchingAcceptWaitTime - Mathf.FloorToInt(waitTime);
			if (!lastSec.Equals(obj))
			{
				time.text = obj.ToString();
				lastSec = obj;
			}

			if (GameConstants.MatchingAcceptWaitTime <= waitTime)
			{
				EnableAcceptBtn(false);
				EnableDeclineBtn(false);
				EnableFinishEffect(false);
				EnableGaugeEffect(false);
				if (!isAccept)
				{
					Singleton<SoundControl>.inst.PlayUISound("oui_cancel_v1");
				}
			}
		}


		private void UpdateDeclineTime()
		{
			if (!isDecline)
			{
				return;
			}

			if (gaugeBar.fillAmount <= 0f)
			{
				return;
			}

			gaugeDeclineElapsedTime += Time.deltaTime;
			gaugeBar.fillAmount = Mathf.Lerp(startFillAmount, 0f,
				gaugeDeclineElapsedTime / (GaugeDeclineTime + startFillAmount * GaugeDeclineTime));
			if (gaugeBar.fillAmount <= 0f)
			{
				EnableGaugeEffect(false);
			}
		}


		private void UpdateGaugeEffect()
		{
			if (gaugeBar.fillAmount <= 0f)
			{
				return;
			}

			if (1f <= gaugeBar.fillAmount)
			{
				gaugeEffectAnchor.rotation = Quaternion.identity;
				return;
			}

			gaugeEffectAnchor.rotation = Quaternion.Euler(0f, 0f, gaugeBar.fillAmount * -360f);
		}


		private void UpdateDial()
		{
			if (!rotateDialDeco)
			{
				return;
			}

			rotateDialDecoAngle += Time.deltaTime * rotateDialDecoAngularSpeed;
			while (360f < rotateDialDecoAngle)
			{
				rotateDialDecoAngle -= 360f;
			}

			dialDecoRotation.rotation = Quaternion.Euler(0f, 0f, -rotateDialDecoAngle);
		}


		private void EnableAcceptBtn(bool enable)
		{
			acceptBtn.interactable = enable;
			acceptLabel.color = enable ? Color.white : Color.gray;
		}


		private void EnableDeclineBtn(bool enable)
		{
			declineBtn.interactable = enable;
			declineLabel.color = enable ? Color.white : Color.gray;
		}


		private void SetModeName(string name)
		{
			mode.text = name;
		}


		private void SetTeamModeName(string name)
		{
			teamMode.text = name;
		}


		private void SetTime(string time)
		{
			this.time.text = time;
		}


		private void SetGaugeBarFill(float fillAmount)
		{
			gaugeBar.fillAmount = fillAmount;
		}


		private void SetRotationGaugeEffect(Quaternion quaternion)
		{
			gaugeEffectAnchor.rotation = quaternion;
		}


		private void EnableFinishEffect(bool enable)
		{
			if (enable)
			{
				finishEffect.Play();
			}
			else if (finishEffect.isPlaying)
			{
				finishEffect.Stop();
			}

			if (finishEffect.gameObject.activeSelf != enable)
			{
				finishEffect.gameObject.SetActive(enable);
			}
		}


		private void EnableGaugeEffect(bool enable)
		{
			if (enable)
			{
				gaugeEffect.Play();
			}
			else if (gaugeEffect.isPlaying)
			{
				gaugeEffect.Stop();
			}

			if (gaugeEffect.gameObject.activeSelf != enable)
			{
				gaugeEffect.gameObject.SetActive(enable);
			}

			if (gaugeEffect_static.gameObject.activeSelf != enable)
			{
				gaugeEffect_static.gameObject.SetActive(enable);
			}
		}


		private void EnableAcceptEffect(bool enable)
		{
			if (enable)
			{
				acceptEffect.Play();
				acceptEffect_center.Play();
			}
			else
			{
				if (acceptEffect.isPlaying)
				{
					acceptEffect.Stop();
				}

				if (acceptEffect_center.isPlaying)
				{
					acceptEffect_center.Stop();
				}
			}

			if (acceptEffect.gameObject.activeSelf != enable)
			{
				acceptEffect.gameObject.SetActive(enable);
			}

			if (acceptEffect_center.gameObject.activeSelf != enable)
			{
				acceptEffect_center.gameObject.SetActive(enable);
			}
		}


		private void PlayDail(bool play)
		{
			rotateDialDeco = play;
		}


		private void PlayDeco(bool isOn)
		{
			decoL.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(isOn ? deco_on : deco_off);
			decoR.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(isOn ? deco_on : deco_off);
			if (isOn)
			{
				decoL.rectTransform.anchoredPosition = deco_L_from;
				decoR.rectTransform.anchoredPosition = deco_R_from;
				decoLTweener.from = deco_L_from;
				decoLTweener.to = deco_L_to;
				decoRTweener.from = deco_R_from;
				decoRTweener.to = deco_R_to;
			}
			else
			{
				decoL.rectTransform.anchoredPosition = deco_L_to;
				decoR.rectTransform.anchoredPosition = deco_R_to;
				decoLTweener.from = deco_L_to;
				decoLTweener.to = deco_L_from;
				decoRTweener.from = deco_R_to;
				decoRTweener.to = deco_R_from;
			}

			decoLTweener.PlayAnimation();
			decoRTweener.PlayAnimation();
		}


		private void InitDeco()
		{
			dialDecoRotation.rotation = Quaternion.identity;
			decoL.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(deco_off);
			decoR.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(deco_off);
			decoLTweener.StopAnimation();
			decoRTweener.StopAnimation();
			decoL.rectTransform.anchoredPosition = deco_L_to;
			decoR.rectTransform.anchoredPosition = deco_R_to;
		}


		private void StopCloseCoroutine()
		{
			if (closeCoroutine != null)
			{
				StopCoroutine(closeCoroutine);
			}
		}


		private void StartCloseCoroutine(float delay)
		{
			closeCoroutine = StartCoroutine(CoroutineUtil.DelayedAction(delay, Close));
		}
	}
}