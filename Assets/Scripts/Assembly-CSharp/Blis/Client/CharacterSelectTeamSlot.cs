using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterSelectTeamSlot : BaseUI
	{
		private readonly Color disableColor = new Color(0.2f, 0.2f, 0.2f);


		private Text nickName;


		private Image pick;


		private ParticleSystem pickEffect;


		private Image portrait;


		private Image teamColor;


		private int teamSlot;


		private Image unknown;


		private long userNum;


		private RectTransform weaponObj;


		private Image weaponType;


		public long UserNum => userNum;


		public int TeamSlot => teamSlot;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			portrait = GameUtil.Bind<Image>(gameObject, "Character");
			unknown = GameUtil.Bind<Image>(gameObject, "Unknown");
			pick = GameUtil.Bind<Image>(gameObject, "Pick");
			pickEffect = GameUtil.Bind<ParticleSystem>(gameObject, "Fx_Fbx_UI_Choice");
			nickName = GameUtil.Bind<Text>(gameObject, "NameBg/UserNickname");
			teamColor = GameUtil.Bind<Image>(gameObject, "MyTeamBg");
			weaponObj = GameUtil.Bind<RectTransform>(gameObject, "WeaponTypeBg");
			weaponObj.localScale = Vector3.zero;
			weaponType = GameUtil.Bind<Image>(weaponObj.gameObject, "WeaponType");
			weaponType.enabled = false;
		}


		public void SetSlot(MatchingService.MatchingUser userInfo)
		{
			userNum = userInfo.UserNum;
			teamSlot = userInfo.TeamSlot;
			Show();
			SetPortrait(userInfo.CharacterCode, userInfo.SkinCode);
			SetPick(userInfo.Pick);
			SetNickname(userInfo.NickName);
			SetTeamColor(userInfo.TeamSlot);
			SetWeaponType(userInfo.StartingDataCode);
		}


		private void SetPortrait(int characterCode, int skinCode)
		{
			if (characterCode == 0)
			{
				portrait.enabled = false;
				unknown.enabled = true;
				return;
			}

			portrait.enabled = true;
			unknown.enabled = false;
			int skinIndex = 0;
			CharacterSkinData skinData = GameDB.character.GetSkinData(skinCode);
			if (skinData != null)
			{
				skinIndex = skinData.index;
			}

			portrait.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(characterCode, skinIndex);
		}


		private void SetPick(bool pick)
		{
			this.pick.enabled = pick;
			portrait.color = pick ? Color.white : disableColor;
		}


		public void PlayPickEffect()
		{
			pickEffect.gameObject.SetActive(true);
			pickEffect.Play();
		}


		private void SetNickname(string nickName)
		{
			this.nickName.text = nickName;
		}


		private void SetTeamColor(int teamSlot)
		{
			teamColor.color = GameConstants.TeamMode.GetTeamColor(teamSlot);
		}


		private void SetWeaponType(int startingWeaponCode)
		{
			if (startingWeaponCode == 0)
			{
				weaponType.enabled = false;
				weaponObj.localScale = Vector3.zero;
				return;
			}

			ItemData itemData = null;
			RecommendStarting recommendStarting = GameDB.recommend.FindRecommendStarting(startingWeaponCode);
			if (recommendStarting != null)
			{
				itemData = GameDB.item.FindItemByCode(recommendStarting.startWeapon);
			}

			if (itemData != null)
			{
				WeaponTypeInfoData weaponTypeInfoData =
					GameDB.mastery.GetWeaponTypeInfoData(itemData.GetSubTypeData<ItemWeaponData>().weaponType);
				weaponType.sprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetWeaponMasterySprite(weaponTypeInfoData.type);
				weaponType.enabled = true;
				weaponObj.localScale = Vector3.one * 0.85f;
				return;
			}

			weaponType.enabled = false;
			weaponObj.localScale = Vector3.zero;
		}


		private void Show()
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
		}


		public void Hide()
		{
			userNum = 0L;
			teamSlot = 0;
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}
		}
	}
}