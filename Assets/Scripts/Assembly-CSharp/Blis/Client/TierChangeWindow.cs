using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class TierChangeWindow : BaseWindow
	{
		private static readonly int IsPromotion = Animator.StringToHash("IsPromotion");


		private Image afterTierIcon;


		private Animator animator;


		private Image beforeTierIcon;


		private Transform loopEffectContainer;


		private Transform tierEffectContainer;


		private LnText txtChangeTier;


		private LnText txtGameMode;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			animator = transform.GetComponent<Animator>();
			beforeTierIcon = GameUtil.Bind<Image>(gameObject, "Frame/Mask/BeforeTier/Img_Tier/RankTierSlot");
			afterTierIcon = GameUtil.Bind<Image>(gameObject, "Frame/Mask/AfterTier/Img_Tier/RankTierSlot");
			txtGameMode = GameUtil.Bind<LnText>(gameObject, "Frame/Title/Txt_Mode");
			txtChangeTier = GameUtil.Bind<LnText>(gameObject, "Frame/Txt_RankInfo");
			tierEffectContainer = transform.Find("Frame/Mask/FrontEffect/TierEffect");
			loopEffectContainer = transform.Find("Frame/Mask/FrontEffect/RoopEffect");
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			SetGameMode();
			SetTierChange();
			SetAnimation();
		}


		protected override void OnClose()
		{
			base.OnClose();
			Singleton<SoundControl>.inst.SetBGMVolume(Singleton<LocalSetting>.inst.setting.bgmVolume);
		}


		private void SetGameMode()
		{
			txtGameMode.text = GlobalUserData.GetStringMatchingModeDetail();
		}


		private void SetTierChange()
		{
			if (Lobby.inst.User.LastBatchMode)
			{
				txtChangeTier.text = Ln.Format("배치고사 종료 설명", Lobby.inst.User.AfterTierType.GetName());
				beforeTierIcon.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(RankingTierType.Unrank);
			}
			else
			{
				if (Lobby.inst.User.BeforeTierChangeType == RankingTierChangeType.Promotion)
				{
					txtChangeTier.text = Ln.Format("랭크 승급 설명", Lobby.inst.User.AfterTierType.GetName());
				}
				else if (Lobby.inst.User.BeforeTierChangeType == RankingTierChangeType.Degrade)
				{
					txtChangeTier.text = Ln.Format("랭크 강등 설명", Lobby.inst.User.AfterTierType.GetName());
				}

				beforeTierIcon.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(Lobby.inst.User.BeforeTierType);
			}

			afterTierIcon.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetRankTierIconSprite(Lobby.inst.User.AfterTierType);
		}


		private void SetAnimation()
		{
			float bgmvolume = Singleton<SoundControl>.inst.BGMVolume >= 0.2f
				? 20f
				: Singleton<SoundControl>.inst.BGMVolume / Singleton<SoundControl>.inst.MasterVolume * 100f;
			Singleton<SoundControl>.inst.SetBGMVolume(bgmvolume);
			if (Lobby.inst.User.AfterTierType == RankingTierType.Unrank)
			{
				OnClose();
				return;
			}

			if (Lobby.inst.User.LastBatchMode ||
			    Lobby.inst.User.BeforeTierChangeType == RankingTierChangeType.Promotion)
			{
				Singleton<SoundControl>.inst.PlayUISound("Rank_TierPromotion");
				animator.SetBool(IsPromotion, true);
				for (int i = 0; i < tierEffectContainer.childCount; i++)
				{
					int afterTierType = (int) Lobby.inst.User.AfterTierType;
					if (i == afterTierType)
					{
						tierEffectContainer.GetChild(i).gameObject.SetActive(true);
						loopEffectContainer.GetChild(i).gameObject.SetActive(true);
					}
					else
					{
						tierEffectContainer.GetChild(i).gameObject.SetActive(false);
						loopEffectContainer.GetChild(i).gameObject.SetActive(false);
					}
				}
			}
			else
			{
				Singleton<SoundControl>.inst.PlayUISound("Rank_TierDemotion");
				animator.SetBool(IsPromotion, false);
			}

			animator.Play("Start");
		}


		public bool IsPlayAnimation()
		{
			return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
		}


		public void OnClickClose()
		{
			animator.Play(
				Lobby.inst.User.LastBatchMode || Lobby.inst.User.BeforeTierChangeType == RankingTierChangeType.Promotion
					? "ExitPromotion"
					: "ExitDemotion");
		}
	}
}