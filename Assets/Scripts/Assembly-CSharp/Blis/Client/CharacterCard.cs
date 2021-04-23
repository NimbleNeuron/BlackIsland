using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterCard : BaseUI
	{
		private Image myTeamBg;


		private LnText playerName;


		private Image portrait;


		private RectTransform selectedWeapon;


		private long userId;


		private Image weaponType;


		public long UserId => userId;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			portrait = GameUtil.Bind<Image>(gameObject, "Character");
			myTeamBg = GameUtil.Bind<Image>(gameObject, "MyTeamBg");
			selectedWeapon = GameUtil.Bind<RectTransform>(gameObject, "WeaponTypeBg");
			selectedWeapon.localScale = Vector3.zero;
			weaponType = GameUtil.Bind<Image>(gameObject, "WeaponTypeBg/WeaponType");
			playerName = GameUtil.Bind<LnText>(gameObject, "NameBg/UserNickname");
		}


		public void ResetCard()
		{
			portrait.enabled = false;
			myTeamBg.enabled = false;
			weaponType.enabled = false;
			playerName.text = null;
		}


		public void SetUserId(long userId)
		{
			this.userId = userId;
		}


		public void SetCharacter(int characterCode, int skinIndex)
		{
			portrait.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(characterCode, skinIndex);
			portrait.enabled = true;
		}


		public void SetWeaponType(bool isMyTeam, int weaponCode)
		{
			if (isMyTeam)
			{
				ItemData itemData = GameDB.item.FindItemByCode(weaponCode);
				if (itemData != null)
				{
					WeaponTypeInfoData weaponTypeInfoData =
						GameDB.mastery.GetWeaponTypeInfoData(itemData.GetSubTypeData<ItemWeaponData>().weaponType);
					weaponType.sprite =
						SingletonMonoBehaviour<ResourceManager>.inst.GetWeaponMasterySprite(weaponTypeInfoData.type);
					weaponType.enabled = true;
				}
			}

			selectedWeapon.localScale = isMyTeam ? Vector3.one * 0.85f : Vector3.zero;
		}


		public void SetPlayerName(string name)
		{
			playerName.text = name;
		}


		public void SetMyTeam(bool isMyTeam, int teamSlot)
		{
			myTeamBg.color = GameConstants.TeamMode.GetTeamColor(teamSlot);
			myTeamBg.enabled = isMyTeam;
		}
	}
}