using System;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ObserverViewCharacterSlot : BaseUI, IPointerClickHandler, IEventSystemHandler
	{
		private readonly Color disableColor = new Color(0.2f, 0.2f, 0.2f);


		private GameObject blank;


		private LnText characterName;


		private Image notSelect;


		private Action<int> onClickEvent;


		private Image pick;


		private ParticleSystem pickEffect;


		private LnText playerName;


		private Image portrait;


		private Image teamColor;


		private int teamSlot;


		private long userNum;


		private GameObject weapon;


		private Image weaponType;


		public long UserNum => userNum;


		public void OnPointerClick(PointerEventData eventData)
		{
			Action<int> action = onClickEvent;
			if (action == null)
			{
				return;
			}

			action(teamSlot);
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			playerName = GameUtil.Bind<LnText>(gameObject, "Content/PlayerName");
			characterName = GameUtil.Bind<LnText>(gameObject, "Content/CharacterName/Text");
			portrait = GameUtil.Bind<Image>(gameObject, "Content/Portrait/IMG_Character");
			portrait.enabled = false;
			notSelect = GameUtil.Bind<Image>(gameObject, "Content/Portrait/IMG_NotSelect");
			blank = GameUtil.Bind<Transform>(gameObject, "Content/Portrait/IMG_Blank").gameObject;
			blank.transform.localScale = Vector3.zero;
			weapon = GameUtil.Bind<Transform>(gameObject, "Content/WeaponTypeBg").gameObject;
			weapon.transform.localScale = Vector3.zero;
			weaponType = GameUtil.Bind<Image>(weapon, "WeaponType");
			teamColor = GameUtil.Bind<Image>(gameObject, "TeamColor");
			teamColor.enabled = false;
			pick = GameUtil.Bind<Image>(gameObject, "Content/Pick");
			pickEffect = GameUtil.Bind<ParticleSystem>(gameObject, "Content/Fx_Fbx_UI_Choice");
		}


		public void SetClickEvent(Action<int> action)
		{
			onClickEvent = action;
		}


		public void Blank()
		{
			userNum = 0L;
			playerName.text = null;
			characterName.text = null;
			portrait.enabled = false;
			notSelect.enabled = false;
			blank.transform.localScale = Vector3.one;
			weapon.transform.localScale = Vector3.zero;
			teamColor.enabled = false;
			onClickEvent = null;
		}


		public void SetMatchingUser(MatchingService.MatchingUser userInfo)
		{
			userNum = userInfo.UserNum;
			SetPlayerName(userInfo.NickName);
			SetCharacter(userInfo.CharacterCode);
			SetSkin(userInfo.CharacterCode, userInfo.SkinCode);
			SetWeaponType(userInfo.StartingDataCode);
			SetTeamColor(userInfo.TeamSlot);
			SetPick(userInfo.Pick);
		}


		public void Select()
		{
			if (userNum == 0L)
			{
				return;
			}

			teamColor.enabled = true;
		}


		public void Deselect()
		{
			teamColor.enabled = false;
		}


		private void SetPlayerName(string nickname)
		{
			playerName.text = nickname;
		}


		public void SetCharacter(int characterCode)
		{
			blank.transform.localScale = Vector3.zero;
			if (characterCode <= 0)
			{
				NotSelectedCharacter();
				return;
			}

			notSelect.enabled = false;
			characterName.text = LnUtil.GetCharacterName(characterCode);
			portrait.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(characterCode);
			portrait.enabled = true;
		}


		public void SetSkin(int characterCode, int skinCode)
		{
			int skinIndex = 0;
			CharacterSkinData skinData = GameDB.character.GetSkinData(skinCode);
			if (skinData != null)
			{
				skinIndex = skinData.index;
			}

			portrait.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(characterCode, skinIndex);
		}


		public void SetPick(bool pick)
		{
			this.pick.enabled = pick;
			portrait.color = pick ? Color.white : disableColor;
		}


		private void NotSelectedCharacter()
		{
			portrait.enabled = false;
			notSelect.enabled = true;
			characterName.text = null;
		}


		public void SetWeaponType(int startingDataCode)
		{
			if (startingDataCode <= 0)
			{
				weapon.transform.localScale = Vector3.zero;
				return;
			}

			ItemData itemData = null;
			RecommendStarting recommendStarting = GameDB.recommend.FindRecommendStarting(startingDataCode);
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
				weapon.transform.localScale = Vector3.one;
				return;
			}

			weaponType.enabled = false;
			weapon.transform.localScale = Vector3.zero;
		}


		private void SetTeamColor(int teamSlot)
		{
			this.teamSlot = teamSlot;
			teamColor.color = GameConstants.TeamMode.GetTeamColor(teamSlot);
		}


		public void PlayPickEffect()
		{
			pickEffect.gameObject.SetActive(true);
			pickEffect.Play();
		}
	}
}