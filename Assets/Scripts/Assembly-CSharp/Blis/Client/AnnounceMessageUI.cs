using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class AnnounceMessageUI : BaseUI
	{
		private const float TWEEN_DEFAULT_SPEED = 0.5f;


		private readonly List<Image> assistantMyTeamBg = new List<Image>();


		private readonly List<Image> assistantPortraits = new List<Image>();


		private CanvasGroup assistantRoot;


		private Image background;


		private CanvasGroup canvasGroup;


		private Image deadPortrait;


		private Image deadRoot;


		private CanvasGroup general;


		private bool isSkipping;


		private bool isWaitAnotherAnnounce;


		private Image KillMyTeamBg;


		private Image killPortrait;


		private Image killRoot;


		private Image killSymbol;


		private Image killWeaponImg;


		private Text message;


		private Queue<AnnounceMessageInfo> messageInfoQueue;


		private Image noticeImg;


		private Coroutine showMessage;


		private CanvasGroup tutorial;


		private Image tutorialBG;


		private Image tutorialImg;


		private Text tutorialMessage;


		private CanvasAlphaTweener tweener;

		
		public bool IsWaitAnotherAnnounce {
			set => isWaitAnotherAnnounce = value;
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			general = GameUtil.Bind<CanvasGroup>(gameObject, "General");
			message = GameUtil.Bind<Text>(general.gameObject, "Text");
			background = GameUtil.Bind<Image>(general.gameObject, "Bg");
			noticeImg = GameUtil.Bind<Image>(message.gameObject, "Icon");
			killSymbol = GameUtil.Bind<Image>(message.gameObject, "Symbol/1");
			tutorial = GameUtil.Bind<CanvasGroup>(gameObject, "Tutorial");
			tutorialMessage = GameUtil.Bind<Text>(tutorial.gameObject, "Text");
			tutorialBG = GameUtil.Bind<Image>(tutorial.gameObject, "Bg");
			tutorialImg = GameUtil.Bind<Image>(tutorialMessage.gameObject, "Icon");
			killRoot = GameUtil.Bind<Image>(gameObject, "PlayerKill/Kill");
			deadRoot = GameUtil.Bind<Image>(gameObject, "PlayerKill/Dead");
			killPortrait = GameUtil.Bind<Image>(gameObject, "PlayerKill/Kill/Portrait");
			KillMyTeamBg = GameUtil.Bind<Image>(gameObject, "PlayerKill/Kill/MyTeamBg");
			deadPortrait = GameUtil.Bind<Image>(gameObject, "PlayerKill/Dead/Portrait");
			killWeaponImg = GameUtil.Bind<Image>(gameObject, "PlayerKill/Weapon");
			assistantRoot = GameUtil.Bind<CanvasGroup>(gameObject, "PlayerKill/Assist");
			assistantPortraits.Add(GameUtil.Bind<Image>(assistantRoot.gameObject, "Assistant1/Portrait"));
			assistantPortraits.Add(GameUtil.Bind<Image>(assistantRoot.gameObject, "Assistant2/Portrait"));
			assistantPortraits.Add(GameUtil.Bind<Image>(assistantRoot.gameObject, "Assistant3/Portrait"));
			assistantMyTeamBg.Add(GameUtil.Bind<Image>(assistantRoot.gameObject, "Assistant1/MyTeamBg"));
			assistantMyTeamBg.Add(GameUtil.Bind<Image>(assistantRoot.gameObject, "Assistant2/MyTeamBg"));
			assistantMyTeamBg.Add(GameUtil.Bind<Image>(assistantRoot.gameObject, "Assistant3/MyTeamBg"));
			GameUtil.BindOrAdd<CanvasAlphaTweener>(gameObject, ref tweener);
			messageInfoQueue = new Queue<AnnounceMessageInfo>();
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			gameObject.SetActive(true);
		}


		public void ShowMessage(string announceMessage, Action announceVoice)
		{
			ShowMessage(AnnounceMessageType.General, announceMessage, 5f, Color.black, announceVoice);
		}


		public void ShowMessage(AnnounceMessageType type, string announceMessage, Action announceVoice)
		{
			ShowMessage(type, announceMessage, 5f, Color.black, announceVoice);
		}


		public void ShowMessage(string announceMessage, float duration, Color bgColor, Action announceVoice)
		{
			ShowMessage(AnnounceMessageType.General, announceMessage, duration, bgColor, announceVoice);
		}


		public void ShowMessageTutorial(string announceMessage, float duration, Color bgColor, bool isImmediately)
		{
			ShowMessage(AnnounceMessageType.Tutorial, announceMessage, duration, bgColor, null);
			if (isImmediately)
			{
				tweener.enabled = false;
				canvasGroup.alpha = 1f;
			}
		}


		public void ShowMessage(AnnounceMessageType type, string announceMessage, float duration, Color bgColor,
			Action announceVoice, WeaponType killWeaponType = WeaponType.None, int killCharacterCode = 0,
			int deadCharacterCode = 0, bool trapKill = false, int killPlayerObjectIds = 0,
			List<int> assistantObjectIds = null)
		{
			AnnounceMessageInfo item = AnnounceMessageInfo.Create(type, announceMessage, duration, bgColor,
				announceVoice, killWeaponType, killCharacterCode, deadCharacterCode, trapKill, killPlayerObjectIds,
				assistantObjectIds);
			if (showMessage != null)
			{
				messageInfoQueue.Enqueue(item);
				return;
			}

			if (isWaitAnotherAnnounce)
			{
				messageInfoQueue.Enqueue(item);
				return;
			}

			SetMessage(item);
		}


		private void SetMessage(AnnounceMessageInfo info)
		{
			ResetImage();
			if (info.type == AnnounceMessageType.Tutorial)
			{
				general.alpha = 0f;
				tutorial.alpha = 1f;
				tutorialMessage.text = info.message;
				tutorialBG.color = info.bgColor;
				tutorialImg.gameObject.SetActive(true);
			}
			else
			{
				general.alpha = 1f;
				tutorial.alpha = 0f;
				if (info.type != AnnounceMessageType.General)
				{
					if (info.type == AnnounceMessageType.Notice)
					{
						noticeImg.gameObject.SetActive(true);
					}
					else if (info.type == AnnounceMessageType.Kill)
					{
						killPortrait.sprite =
							SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterProfileSprite(
								info.killCharacterCode);
						deadPortrait.sprite =
							SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterProfileSprite(
								info.deadCharacterCode);
						if (info.trapKill)
						{
							killWeaponImg.sprite = SingletonMonoBehaviour<ResourceManager>.inst
								.GetTrapKillAnnounceWeaponSprite();
						}
						else
						{
							killWeaponImg.sprite =
								SingletonMonoBehaviour<ResourceManager>.inst.GetKillAnnounceWeaponSprite(
									info.killWeaponType);
						}

						killSymbol.gameObject.SetActive(true);
						killRoot.gameObject.SetActive(true);
						deadRoot.gameObject.SetActive(true);
						killWeaponImg.gameObject.SetActive(true);
						if (MonoBehaviourInstance<ClientService>.inst.IsTeamMode && info.assistantObjectIds != null &&
						    info.assistantObjectIds.Count > 0)
						{
							LocalPlayerCharacter settingTarget =
								MonoBehaviourInstance<ClientService>.inst.World.Find<LocalPlayerCharacter>(
									info.killPlayerObjectIds);
							SetMyTeamBg(KillMyTeamBg, settingTarget);
							SetAssistants(info.assistantObjectIds);
							assistantRoot.alpha = 1f;
						}
					}
				}

				message.text = info.message;
				background.color = info.bgColor;
			}

			Action announceVoice = info.announceVoice;
			if (announceVoice != null)
			{
				announceVoice();
			}

			SetMessage(info.duration);
		}


		private void SetAssistants(List<int> assistantObjectIds)
		{
			if (assistantObjectIds.Count > assistantPortraits.Count)
			{
				int num = assistantObjectIds.Count - assistantPortraits.Count;
				GameObject gameObject = assistantPortraits[0].transform.parent.gameObject;
				for (int i = 0; i < num; i++)
				{
					GameObject parent = Instantiate<GameObject>(gameObject, assistantRoot.transform);
					assistantPortraits.Add(GameUtil.Bind<Image>(parent, "Portrait"));
					assistantMyTeamBg.Add(GameUtil.Bind<Image>(parent, "MyTeamBg"));
				}
			}

			for (int j = 0; j < assistantObjectIds.Count; j++)
			{
				LocalPlayerCharacter localPlayerCharacter =
					MonoBehaviourInstance<ClientService>.inst.World.Find<LocalPlayerCharacter>(assistantObjectIds[j]);
				assistantPortraits[j].sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterProfileSprite(localPlayerCharacter
						.CharacterCode);
				assistantPortraits[j].transform.parent.gameObject.SetActive(true);
				SetMyTeamBg(assistantMyTeamBg[j], localPlayerCharacter);
			}
		}


		public void SetMyTeamBg(Image myTeamBg, LocalPlayerCharacter SettingTarget)
		{
			int myTeamNumber = MonoBehaviourInstance<ClientService>.inst.MyTeamNumber;
			int teamSlot = 0 < myTeamNumber ? SettingTarget.TeamSlot : 1;
			bool enabled = MonoBehaviourInstance<ClientService>.inst.MyObjectId == SettingTarget.ObjectId ||
			               0 < myTeamNumber && myTeamNumber == SettingTarget.TeamNumber;
			myTeamBg.color = GameConstants.TeamMode.GetTeamColor(teamSlot);
			myTeamBg.enabled = enabled;
		}


		private void SetMessage(float duration)
		{
			canvasGroup.alpha = 0f;
			showMessage = this.StartThrowingCoroutine(ShowMessage(duration),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][AnnounceMessage] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator ShowMessage(float duration)
		{
			float time = 0f;
			tweener.from = 0f;
			tweener.to = 1f;
			tweener.speed = 0.5f;
			tweener.PlayAnimation();
			bool finish = false;
			for (;;)
			{
				time += Time.deltaTime;
				if (duration - time <= 1f && !finish)
				{
					tweener.from = 1f;
					tweener.to = 0f;
					tweener.PlayAnimation();
					finish = true;
				}

				if (time > duration)
				{
					break;
				}

				yield return null;
			}

			Next();
		}


		public void Stop()
		{
			if (showMessage != null)
			{
				StopCoroutine(showMessage);
				showMessage = null;
				tweener.OnAnimationFinish -= OnFinishTween;
				tweener.StopAnimation();
			}

			canvasGroup.alpha = 0f;
		}


		public void SkipCurrentShowingMessage(bool isImmediately)
		{
			if (showMessage != null)
			{
				StopCoroutine(showMessage);
				tweener.OnAnimationFinish -= OnFinishTween;
				tweener.StopAnimation();
				if (isImmediately || isSkipping)
				{
					canvasGroup.alpha = 0f;
					isSkipping = false;
					Next();
					return;
				}

				isSkipping = true;
				tweener.from = canvasGroup.alpha;
				tweener.to = 0f;
				tweener.speed = 0.1f;
				tweener.OnAnimationFinish += OnFinishTween;
				tweener.PlayAnimation();
			}
		}


		private void OnFinishTween()
		{
			tweener.OnAnimationFinish -= OnFinishTween;
			isSkipping = false;
			Next();
		}


		public void Next()
		{
			if (messageInfoQueue.Count > 0)
			{
				SetMessage(messageInfoQueue.Dequeue());
				return;
			}

			canvasGroup.alpha = 0f;
			showMessage = null;
		}


		public bool IsShowing()
		{
			return canvasGroup.alpha != 0f;
		}


		private void ResetImage()
		{
			noticeImg.gameObject.SetActive(false);
			killSymbol.gameObject.SetActive(false);
			killRoot.gameObject.SetActive(false);
			deadRoot.gameObject.SetActive(false);
			killWeaponImg.gameObject.SetActive(false);
			assistantRoot.alpha = 0f;
			KillMyTeamBg.enabled = false;
			tweener.enabled = true;
			tutorialImg.gameObject.SetActive(false);
			for (int i = 0; i < assistantPortraits.Count; i++)
			{
				assistantPortraits[i].transform.parent.gameObject.SetActive(false);
			}
		}


		private class AnnounceMessageInfo
		{
			public Action announceVoice;


			public List<int> assistantObjectIds;


			public Color bgColor;


			public int deadCharacterCode;


			public float duration;


			public int killCharacterCode;


			public int killPlayerObjectIds;


			public WeaponType killWeaponType;


			public string message;


			public bool trapKill;


			public AnnounceMessageType type;

			private AnnounceMessageInfo() { }


			public static AnnounceMessageInfo Create(AnnounceMessageType type, string message, float duration,
				Color bgColor, Action announceVoice, WeaponType killWeaponType, int killCharacterCode,
				int deadCharacterCode, bool trapKill, int killPlayerObjectIds, List<int> assistantObjectIds)
			{
				return new AnnounceMessageInfo
				{
					type = type,
					message = message,
					duration = duration,
					bgColor = bgColor,
					killWeaponType = killWeaponType,
					killCharacterCode = killCharacterCode,
					deadCharacterCode = deadCharacterCode,
					trapKill = trapKill,
					announceVoice = announceVoice,
					killPlayerObjectIds = killPlayerObjectIds,
					assistantObjectIds = assistantObjectIds
				};
			}
		}
	}
}