using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class RewardWindow : BaseWindow
	{
		public delegate void LevelUpEvent();


		private const float gaugeSpeed = 3f;


		private const float gaugeCoolTime = 0.3f;


		public float arrowSpeed = 10f;


		private readonly Queue<RewardInfo> rewardInfoQueue = new Queue<RewardInfo>();


		private readonly List<RewardSlot> rewardSlots = new List<RewardSlot>();


		private Button btnClose;


		private RectTransform imgArrow;


		private GameObject imgBoard;


		private Image imgXPGauge;


		private GameObject levelUp;


		private GameObject levelUpArrowEffect;


		public LevelUpEvent levelUpEvent;


		private GameObject levelUpGaugeEffect;


		private Transform rewardSlotParent;


		private ScaleTweener scaleTweenerCurrentLevel;


		private LnText txtAnimationXP;


		private LnText txtCurrentLevel;


		private LnText txtLevelUp;


		private LnText txtPrevLevel;


		private LnText txtTitle;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			imgBoard = transform.FindRecursively("IMG_Board").gameObject;
			txtTitle = GameUtil.Bind<LnText>(this.gameObject, "TXT_Title");
			rewardSlotParent = transform.FindRecursively("RewardSlotParent");
			for (int i = 0; i < rewardSlotParent.childCount; i++)
			{
				GameObject gameObject = rewardSlotParent.GetChild(i).gameObject;
				rewardSlots.Add(new RewardSlot(gameObject));
			}

			levelUp = transform.FindRecursively("LevelUp").gameObject;
			txtAnimationXP = GameUtil.Bind<LnText>(this.gameObject, "LevelUp/TXT_AnimationXP");
			txtPrevLevel = GameUtil.Bind<LnText>(this.gameObject, "LevelUp/IMG_XPGauge/TXT_PrevLevel");
			txtCurrentLevel =
				GameUtil.Bind<LnText>(this.gameObject, "LevelUp/IMG_XPGauge/CurrentLevel/TXT_CurrentLevel");
			GameUtil.BindOrAdd<ScaleTweener>(txtCurrentLevel.gameObject, ref scaleTweenerCurrentLevel);
			imgXPGauge = GameUtil.Bind<Image>(this.gameObject, "LevelUp/IMG_XPGauge");
			levelUpGaugeEffect = transform.FindRecursively("LevelUpGaugeEffect").gameObject;
			imgArrow = GameUtil.Bind<RectTransform>(this.gameObject, "IMG_Arrow");
			txtLevelUp = GameUtil.Bind<LnText>(this.gameObject, "TXT_LevelUp");
			levelUpArrowEffect = transform.FindRecursively("LevelUpArrowEffect").gameObject;
			btnClose = GameUtil.Bind<Button>(this.gameObject, "BTN_Close");
			btnClose.onClick.AddListener(delegate { ClickedClose(); });
		}


		private void ClickedClose()
		{
			if (rewardInfoQueue.Count > 0)
			{
				ResetUI();
				ShowReward(0.3f);
				return;
			}

			if (levelUpEvent != null)
			{
				ResetUI();
				levelUpEvent();
				levelUpEvent = null;
				return;
			}

			Close();
		}


		public RewardItemInfo GetTutorialRewardItemInfo(TutorialRewardData tutorialRewardData)
		{
			int num = tutorialRewardData.collectionCode > 0 ? 3 : 0;
			int itemValue = num == 3 ? tutorialRewardData.collectionCode : tutorialRewardData.amount;
			return new RewardItemInfo((RewardItemType) num, itemValue);
		}


		public void AddReward(RewardInfo rewardInfo)
		{
			rewardInfoQueue.Enqueue(rewardInfo);
		}


		public void ShowReward(float delay = 0f)
		{
			RewardInfo rewardInfo = rewardInfoQueue.Dequeue();
			this.StartThrowingCoroutine(Reward(rewardInfo, delay),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][ShowReward] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public void SetLevelUpReward(int prevLevel, int currentLevel, RewardType rewardType, int gainXP)
		{
			int num = 0;
			for (int i = prevLevel; i < currentLevel; i++)
			{
				num += GameDB.user.GetRewardAP(i);
			}

			this.StartThrowingCoroutine(LevelUpReward(prevLevel, currentLevel, num, gainXP),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][SetLevelUpReward] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator LevelUpReward(int prevLevel, int currentLevel, int gainAP, int gainXP)
		{
			yield return new WaitForSeconds(1f);
			yield return this.StartThrowingCoroutine(AnimationLevelUp(prevLevel, currentLevel, gainXP),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][LevelUpReward] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			RewardInfo rewardInfo = new RewardInfo(RewardType.LevelUp, new List<RewardItemInfo>
			{
				new RewardItemInfo(RewardItemType.ACoin, gainAP)
			});
			AddReward(rewardInfo);
			ShowReward();
		}


		private IEnumerator Reward(RewardInfo rewardInfo, float delay = 0f)
		{
			yield return new WaitForSeconds(delay);
			yield return this.StartThrowingCoroutine(AnimationReward(rewardInfo),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][Reward] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
		}


		private IEnumerator AnimationLevelUp(int prevLevel, int currentLevel, int gainXP)
		{
			Singleton<SoundControl>.inst.PlayUISound("oui_LevelUp_v1");
			levelUp.SetActive(true);
			txtAnimationXP.text = "+ 0 XP";
			txtPrevLevel.text = prevLevel.ToString();
			txtCurrentLevel.text = (prevLevel + 1).ToString();
			int repeatCount = currentLevel - prevLevel;
			this.StartThrowingCoroutine(AnimationTotalXP(gainXP, repeatCount),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][AnimationTotalXP] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			int num;
			for (int i = 0; i < repeatCount; i = num + 1)
			{
				yield return this.StartThrowingCoroutine(AnimationXPGauge(prevLevel + i + 1),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][AnimationXPGauge] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
				num = i;
			}

			imgBoard.SetActive(false);
			imgBoard.transform.localScale = new Vector3(0f, 1f, 1f);
			levelUp.SetActive(false);
			yield return new WaitForSeconds(0.2f);
			imgArrow.gameObject.SetActive(true);
			this.StartThrowingCoroutine(AnimationArrow(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][AnimationArrow] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			yield return new WaitForSeconds(0.4f);
			txtLevelUp.text = currentLevel.ToString();
			txtLevelUp.gameObject.SetActive(true);
			levelUpArrowEffect.SetActive(true);
			yield return new WaitForSeconds(0.4f);
			levelUpArrowEffect.SetActive(false);
		}


		private IEnumerator AnimationTotalXP(int targetXP, int repeatCount)
		{
			float timer = 0f;
			float aniTime = 1.3f * repeatCount;
			while (timer < aniTime)
			{
				timer += Time.deltaTime * 3f;
				int money = (int) (targetXP * (timer / aniTime));
				txtAnimationXP.text = "+ " + StringUtil.AssetToString(money) + " XP";
				yield return null;
			}

			txtAnimationXP.text = "+ " + StringUtil.AssetToString(targetXP) + " XP";
		}


		private IEnumerator AnimationTotalXP(int prevXP, int targetXP, int repeatCount)
		{
			float timer = 0f;
			float aniTime = 1.3f * repeatCount;
			while (timer < aniTime)
			{
				timer += Time.deltaTime * 3f;
				int money = prevXP + (int) (targetXP * (timer / aniTime));
				txtAnimationXP.text = "+ " + StringUtil.AssetToString(money) + " XP";
				yield return null;
			}

			txtAnimationXP.text = "+ " + StringUtil.AssetToString(targetXP) + " XP";
		}


		private IEnumerator AnimationXPGauge(int targetLevel)
		{
			float timer = 0f;
			imgXPGauge.fillAmount = 0f;
			txtPrevLevel.text = (targetLevel - 1).ToString();
			txtCurrentLevel.text = targetLevel.ToString();
			while (timer < 1f)
			{
				timer += Time.deltaTime * 3f;
				imgXPGauge.fillAmount = timer;
				yield return null;
			}

			levelUpGaugeEffect.SetActive(false);
			levelUpGaugeEffect.SetActive(true);
			imgXPGauge.fillAmount = 1f;
			scaleTweenerCurrentLevel.PlayAnimation();
			yield return new WaitForSeconds(0.3f);
		}


		private IEnumerator AnimationReward(RewardInfo rewardInfo)
		{
			imgBoard.SetActive(true);
			yield return new WaitForSeconds(0.1f);
			Singleton<SoundControl>.inst.PlayUISound("oui_Reward_v1");
			int num = 0;
			foreach (RewardItemInfo rewardItemInfo in rewardInfo.itemInfos)
			{
				rewardSlots[num].ShowRewardSlot(rewardItemInfo);
				num++;
			}

			btnClose.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.1f);
			SetTitle(rewardInfo.rewardType);
		}


		private IEnumerator AnimationArrow()
		{
			float targetHeight = 1550f;
			float startWidth = imgArrow.rect.width;
			float height = imgArrow.rect.height;
			while (height < targetHeight)
			{
				height += Time.deltaTime * arrowSpeed;
				imgArrow.sizeDelta = new Vector2(startWidth, height);
				yield return null;
			}
		}


		private void SetTitle(RewardType rewardType)
		{
			switch (rewardType)
			{
				case RewardType.LevelUp:
					txtTitle.text = Ln.Get("레벨업 보상!");
					break;
				case RewardType.DailyMission:
				case RewardType.MailBox:
					txtTitle.text = Ln.Get("일일 보상!");
					break;
				case RewardType.TutotialClear:
					txtTitle.text = Ln.Get("튜토리얼 보상");
					break;
				case RewardType.DlcReward:
					txtTitle.text = Ln.Get("DLC 보상!");
					break;
			}

			txtTitle.gameObject.SetActive(true);
		}


		protected override void OnClose()
		{
			base.OnClose();
			ResetUI();
			MonoBehaviourInstance<LobbyUI>.inst.NicknameCheckProcess();
			MonoBehaviourInstance<LobbyUI>.inst.TierChangeCheckProcess();
		}


		private void ResetUI()
		{
			foreach (RewardSlot rewardSlot in rewardSlots)
			{
				rewardSlot.HideRewardSlot();
			}

			imgBoard.transform.localScale = new Vector3(0f, 1f, 1f);
			txtTitle.GetComponent<CanvasGroup>().alpha = 0f;
			txtLevelUp.transform.localScale = new Vector3(2f, 2f, 2f);
			txtLevelUp.GetComponent<CanvasGroup>().alpha = 0.5f;
			imgBoard.SetActive(false);
			txtTitle.gameObject.SetActive(false);
			txtLevelUp.gameObject.SetActive(false);
			levelUpGaugeEffect.SetActive(false);
			levelUp.SetActive(false);
			txtAnimationXP.text = "";
			txtPrevLevel.text = "";
			txtCurrentLevel.text = "";
			imgArrow.localPosition = new Vector3(0f, -900f, 0f);
			imgArrow.sizeDelta = new Vector2(320f, 410f);
			imgArrow.gameObject.SetActive(false);
			levelUpArrowEffect.SetActive(false);
			btnClose.gameObject.SetActive(false);
		}


		private class RewardSlot
		{
			private readonly GameObject gameObject;


			private readonly Image imgBG;


			private readonly Image imgRewardIcon;


			private readonly LnText txtRewardValue;

			public RewardSlot(GameObject gameObject)
			{
				this.gameObject = gameObject;
				imgBG = GameUtil.Bind<Image>(gameObject, "IMG_BG");
				imgRewardIcon = GameUtil.Bind<Image>(gameObject, "IMG_RewardIcon");
				txtRewardValue = GameUtil.Bind<LnText>(gameObject, "TXT_RewardValue");
			}


			public void ShowRewardSlot(RewardItemInfo rewardItemInfo)
			{
				imgBG.enabled = EnableBG(rewardItemInfo.itemType);
				imgRewardIcon.sprite = GetRewardIcon(rewardItemInfo.itemType, rewardItemInfo.itemValue);
				txtRewardValue.text = GetRewardValueString(rewardItemInfo.itemType, rewardItemInfo.itemValue);
				gameObject.SetActive(true);
			}


			public void HideRewardSlot()
			{
				gameObject.transform.localScale = new Vector3(1f, 0f, 1f);
				gameObject.SetActive(false);
			}


			private bool EnableBG(RewardItemType rewardItemType)
			{
				switch (rewardItemType)
				{
					case RewardItemType.ACoin:
						return false;
					case RewardItemType.XP:
						return false;
					case RewardItemType.Character:
						return true;
					case RewardItemType.Skin:
						return true;
				}

				return false;
			}


			private Sprite GetRewardIcon(RewardItemType rewardItemType, int rewardItemValue)
			{
				switch (rewardItemType)
				{
					case RewardItemType.ACoin:
						return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_ACoin_2");
					case RewardItemType.NP:
						return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_NP_02");
					case RewardItemType.XP:
						return SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_XP_02");
					case RewardItemType.Character:
						return SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(
							rewardItemValue);
					case RewardItemType.Skin:
					{
						CharacterSkinData skinData = GameDB.character.GetSkinData(rewardItemValue);
						return SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(
							skinData.characterCode, skinData.index);
					}
					default:
						return null;
				}
			}


			private string GetRewardValueString(RewardItemType rewardItemType, int rewardItemValue)
			{
				switch (rewardItemType)
				{
					case RewardItemType.ACoin:
					case RewardItemType.NP:
					case RewardItemType.XP:
						return StringUtil.AssetToString(rewardItemValue);
					case RewardItemType.Character:
						return Ln.Get(LnType.Character_Name, rewardItemValue.ToString());
					case RewardItemType.Skin:
						return LnUtil.GetSkinName(rewardItemValue);
					default:
						return "";
				}
			}
		}
	}
}