using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class CharacterSelectWeaponSelect : BaseUI, ICharacterSelectWeaponListener
	{
		private readonly List<CharacterSelectWeaponSelectSlot>
			weaponSlots = new List<CharacterSelectWeaponSelectSlot>();


		public void OnClickWeapon(int startingDataCode)
		{
			try
			{
				MatchingService inst = MonoBehaviourInstance<MatchingService>.inst;
				if (inst == null)
				{
					throw new GameException("MatchingService is Null");
				}

				inst.SelectWeapon(startingDataCode);
			}
			catch (Exception ex)
			{
				Log.V("[EXCEPTION] " + ex.Message + ": " + ex.StackTrace);
			}
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GetComponentsInChildren<CharacterSelectWeaponSelectSlot>(weaponSlots);
		}


		public void Clear()
		{
			for (int i = 0; i < weaponSlots.Count; i++)
			{
				weaponSlots[i].SetListener(null);
				weaponSlots[i].Hide();
			}
		}


		public void UpdateWeapons(int characterCode, int startingDataCode)
		{
			SetWeapons(characterCode);
			SelectWeapon(startingDataCode);
		}


		public void SetWeapons(int characterCode)
		{
			CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(characterCode);
			SetWeapon(0, characterCode, characterMasteryData.weapon1);
			SetWeapon(1, characterCode, characterMasteryData.weapon2);
			SetWeapon(2, characterCode, characterMasteryData.weapon3);
		}


		private void SetWeapon(int index, int characterCode, MasteryType weaponMasteryType)
		{
			if (weaponSlots == null)
			{
				return;
			}

			if (index < 0 || weaponSlots.Count <= index)
			{
				return;
			}

			int startingDataCode = 0;
			ItemData itemData = null;
			MasteryType masteryType = MasteryType.None;
			if (weaponMasteryType != MasteryType.None)
			{
				RecommendStarting recommendStarting =
					GameDB.recommend.FindStartingData(characterCode, weaponMasteryType);
				if (recommendStarting != null)
				{
					startingDataCode = recommendStarting.code;
					itemData = GameDB.item.FindItemByCode(recommendStarting.startWeapon);
					masteryType = recommendStarting.mastery;
				}
			}

			if (itemData == null || masteryType == MasteryType.None)
			{
				weaponSlots[index].SetListener(null);
				weaponSlots[index].Hide();
				return;
			}

			string itemName = LnUtil.GetItemName(itemData.code);
			string weaponDesc = Ln.Get(string.Format("MasteryType/{0}/Desc", masteryType));
			weaponSlots[index].SetWeapon(startingDataCode, itemName, weaponDesc, itemData.GetSprite());
			weaponSlots[index].SetListener(this);
			weaponSlots[index].SetLink(index);
			weaponSlots[index].Show((index + 1) * 0.05f);
		}


		public void SelectWeapon(int startingDataCode)
		{
			if (weaponSlots == null)
			{
				return;
			}

			for (int i = 0; i < weaponSlots.Count; i++)
			{
				if (weaponSlots[i].StartingDataCode == startingDataCode)
				{
					weaponSlots[i].Select();
				}
				else
				{
					weaponSlots[i].Deselect();
				}
			}
		}


		public void PickMyCharacter()
		{
			if (weaponSlots == null)
			{
				return;
			}

			for (int i = 0; i < weaponSlots.Count; i++)
			{
				weaponSlots[i].Lock();
			}
		}


		public void CancelPickMyCharacter()
		{
			if (weaponSlots == null)
			{
				return;
			}

			for (int i = 0; i < weaponSlots.Count; i++)
			{
				weaponSlots[i].UnLock();
			}
		}
	}
}