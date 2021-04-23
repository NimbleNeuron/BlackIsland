using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterTabSkinList : BasePage
	{
		[SerializeField] private CharacterTabMasteryBtn masteryPrefab = default;


		[SerializeField] private GameObject cloneTarget = default;


		private readonly HashSet<WeaponType> currentWeaponType = new HashSet<WeaponType>();


		private readonly List<CharacterSkinData> resultData = new List<CharacterSkinData>();


		private readonly List<CharacterSelectSkinSlot> slot = new List<CharacterSelectSkinSlot>();


		private readonly List<CharacterTabMasteryBtn> weaponFilterButtons = new List<CharacterTabMasteryBtn>();


		private string currentInputText = string.Empty;


		private Button deleteInputTextButton = default;


		private GameObject empty = default;


		private Toggle hasCharacterToggle;


		private Toggle hasSkinToggle;


		private InputFieldExtension inputField = default;


		private bool isHasCharacter;


		private bool isHasSkin;


		private bool isInit;


		private CharacterTabMasteryBtn masteryAllButton;


		private RectTransform masteryParent;


		private ScrollRect scrollRect = default;


		private RectTransform scrollViewParent = default;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			Bind();
			AddEvent();
		}


		private void Bind()
		{
			inputField = GameUtil.Bind<InputFieldExtension>(gameObject, "Search/EditedTitle");
			deleteInputTextButton = GameUtil.Bind<Button>(gameObject, "Search/BtnDelete");
			scrollRect = GameUtil.Bind<ScrollRect>(gameObject, "SkinScrollView");
			scrollViewParent = GameUtil.Bind<RectTransform>(gameObject, "SkinScrollView/Viewport/Content");
			empty = GameUtil.Bind<RectTransform>(gameObject, "SkinScrollView/Viewport/Blank").gameObject;
			hasSkinToggle = GameUtil.Bind<Toggle>(gameObject, "HaveSkinCheck/CheckBox");
			hasCharacterToggle = GameUtil.Bind<Toggle>(gameObject, "HaveCharacterCheck/CheckBox");
			masteryParent = GameUtil.Bind<RectTransform>(gameObject, "Masterys");
			masteryAllButton =
				GameUtil.Bind<CharacterTabMasteryBtn>(masteryParent.gameObject, "CharacterTabMasteryBtn_All");
		}


		private void AddEvent()
		{
			inputField.onValueChanged.AddListener(delegate(string input)
			{
				if (currentInputText != input)
				{
					currentInputText = input;
					Refresh();
				}
			});
			deleteInputTextButton.onClick.AddListener(delegate { inputField.text = string.Empty; });
			hasCharacterToggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				isHasCharacter = isOn;
				Refresh();
			});
			hasSkinToggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				isHasSkin = isOn;
				Refresh();
			});
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			Refresh();
		}


		protected override void OnClosePage()
		{
			base.OnClosePage();
		}


		private void Refresh()
		{
			InitFilterObject();
			InitFilterData();
			RefreshCell();
		}


		private void InitFilterObject()
		{
			if (isInit)
			{
				return;
			}

			isInit = true;
			masteryAllButton.OnPointerClickEvent += delegate { ChangeMasteryFilter(WeaponType.None, null); };
			weaponFilterButtons.Add(masteryAllButton);
			List<WeaponTypeInfoData> list = GameDB.mastery.GetAllWeaponTypeInfoDatas().FindAll(x => x.shopFilter == 1);
			for (int i = 0; i < list.Count; i++)
			{
				CharacterTabMasteryBtn characterTabMasteryBtn =
					Instantiate<CharacterTabMasteryBtn>(masteryPrefab, masteryParent);
				if (characterTabMasteryBtn != null)
				{
					WeaponType weaponType = list[i].type;
					characterTabMasteryBtn.SetMastery(weaponType);
					characterTabMasteryBtn.OnPointerClickEvent += delegate { ChangeMasteryFilter(weaponType, null); };
					weaponFilterButtons.Add(characterTabMasteryBtn);
				}
			}

			ChangeMasteryFilter(WeaponType.None, null);
		}


		private void InitFilterData()
		{
			string searchName1 = LnUtil.GetSearchName(currentInputText);
			List<CharacterSkinData> allSkinDataList = GameDB.character.GetAllSkinDataList();
			List<FilterData> filterList = new List<FilterData>();
			for (int index = 0; index < allSkinDataList.Count; ++index)
			{
				CharacterSkinData characterSkinData = allSkinDataList[index];
				int code = characterSkinData.code;
				int characterCode = characterSkinData.characterCode;
				bool flag1 = Lobby.inst.IsHaveSkin(characterSkinData.code);
				bool flag2 = Lobby.inst.IsHaveCharacter(characterSkinData.characterCode);
				string searchName2 = LnUtil.GetSearchName(LnUtil.GetSkinName(code));
				if ((!isHasSkin || flag1) && (!isHasCharacter || flag2) &&
				    (string.IsNullOrEmpty(searchName1) || searchName2.Contains(searchName1)))
				{
					filterList.Add(new FilterData(code, characterCode));
				}
			}

			HashSet<MasteryType> masteryTypes = new HashSet<MasteryType>();
			foreach (WeaponType weaponType in currentWeaponType)
			{
				masteryTypes.Add(weaponType.GetWeaponMasteryType());
			}

			if (!masteryTypes.Contains(MasteryType.None))
			{
				filterList = filterList.FindAll(x =>
				{
					List<MasteryType> masteries =
						GameDB.mastery.GetCharacterMasteryData(x.characterCode).GetMasteries();
					for (int index = 0; index < masteries.Count; ++index)
					{
						if (masteryTypes.Contains(masteries[index]))
						{
							return true;
						}
					}

					return false;
				});
			}

			resultData.Clear();
			for (int i = 0; i < filterList.Count; ++i)
			{
				CharacterSkinData characterSkinData = allSkinDataList.Find(x =>
					x.purchaseType != SkinPurchaseType.FREE && x.code == filterList[i].skinCode);
				if (characterSkinData != null)
				{
					resultData.Add(characterSkinData);
				}
			}

			// co: dotPeek
			// CharacterTabSkinList.<>c__DisplayClass27_0 CS$<>8__locals1 = new CharacterTabSkinList.<>c__DisplayClass27_0();
			// string searchName = LnUtil.GetSearchName(this.currentInputText);
			// List<CharacterSkinData> allSkinDataList = GameDB.character.GetAllSkinDataList();
			// CS$<>8__locals1.filterList = new List<CharacterTabSkinList.FilterData>();
			// for (int k = 0; k < allSkinDataList.Count; k++)
			// {
			// 	CharacterSkinData characterSkinData = allSkinDataList[k];
			// 	int code = characterSkinData.code;
			// 	int characterCode = characterSkinData.characterCode;
			// 	bool flag = Lobby.inst.IsHaveSkin(characterSkinData.code);
			// 	bool flag2 = Lobby.inst.IsHaveCharacter(characterSkinData.characterCode);
			// 	string searchName2 = LnUtil.GetSearchName(LnUtil.GetSkinName(code));
			// 	if ((!this.isHasSkin || flag) && (!this.isHasCharacter || flag2) && (string.IsNullOrEmpty(searchName) || searchName2.Contains(searchName)))
			// 	{
			// 		CS$<>8__locals1.filterList.Add(new CharacterTabSkinList.FilterData(code, characterCode));
			// 	}
			// }
			// CS$<>8__locals1.masteryTypes = new HashSet<MasteryType>();
			// foreach (WeaponType weaponType in this.currentWeaponType)
			// {
			// 	CS$<>8__locals1.masteryTypes.Add(weaponType.GetWeaponMasteryType());
			// }
			// if (!CS$<>8__locals1.masteryTypes.Contains(MasteryType.None))
			// {
			// 	CS$<>8__locals1.filterList = CS$<>8__locals1.filterList.FindAll(delegate(CharacterTabSkinList.FilterData x)
			// 	{
			// 		List<MasteryType> masteries = GameDB.mastery.GetCharacterMasteryData(x.characterCode).GetMasteries();
			// 		for (int j = 0; j < masteries.Count; j++)
			// 		{
			// 			if (CS$<>8__locals1.masteryTypes.Contains(masteries[j]))
			// 			{
			// 				return true;
			// 			}
			// 		}
			// 		return false;
			// 	});
			// }
			// this.resultData.Clear();
			// int i;
			// int i2;
			// for (i = 0; i < CS$<>8__locals1.filterList.Count; i = i2)
			// {
			// 	CharacterSkinData characterSkinData2 = allSkinDataList.Find((CharacterSkinData x) => x.purchaseType != SkinPurchaseType.FREE && x.code == CS$<>8__locals1.filterList[i].skinCode);
			// 	if (characterSkinData2 != null)
			// 	{
			// 		this.resultData.Add(characterSkinData2);
			// 	}
			// 	i2 = i + 1;
			// }
		}


		private void RefreshCell()
		{
			if (resultData.Count > slot.Count)
			{
				int num = resultData.Count - slot.Count;
				for (int i = 0; i < num; i++)
				{
					GameObject gameObject = Instantiate<GameObject>(cloneTarget, scrollViewParent);
					CharacterSelectSkinSlot characterSelectSkinSlot =
						gameObject != null ? gameObject.GetComponent<CharacterSelectSkinSlot>() : null;
					if (characterSelectSkinSlot != null)
					{
						characterSelectSkinSlot.SetScrollRect(scrollRect);
						CharacterSelectSkinSlot characterSelectSkinSlot2 = characterSelectSkinSlot;
						characterSelectSkinSlot2.selectCallback =
							(CharacterSelectSkinSlot.SelectCallback) Delegate.Combine(
								characterSelectSkinSlot2.selectCallback,
								new CharacterSelectSkinSlot.SelectCallback(OnClickCell));
						slot.Add(characterSelectSkinSlot);
					}
				}
			}

			for (int j = 0; j < slot.Count; j++)
			{
				bool flag = j < resultData.Count;
				CharacterSelectSkinSlot characterSelectSkinSlot3 = slot[j];
				characterSelectSkinSlot3.gameObject.SetActive(flag);
				if (flag)
				{
					int index = j;
					characterSelectSkinSlot3.SetSlot(resultData[j], index);
					characterSelectSkinSlot3.SetNotReleaseableLock(!Lobby.inst.IsHaveSkin(resultData[j].code));
					characterSelectSkinSlot3.SetFocus(true);
				}
			}

			empty.SetActive(resultData.Count <= 0);
		}


		private void OnClickCell(int slotIndex)
		{
			MonoBehaviourInstance<LobbyUI>.inst.UIEvent.OnClickShowSkinInfo(resultData[slotIndex].code);
		}


		private void ChangeMasteryFilter(WeaponType weaponType, CharacterTabMasteryBtn selectedButton)
		{
			if (weaponType == WeaponType.None)
			{
				currentWeaponType.Clear();
				currentWeaponType.Add(weaponType);
			}
			else if (currentWeaponType.Contains(weaponType))
			{
				if (currentWeaponType.Contains(WeaponType.None))
				{
					currentWeaponType.Remove(WeaponType.None);
				}

				currentWeaponType.Remove(weaponType);
				if (currentWeaponType.Count <= 0)
				{
					currentWeaponType.Add(WeaponType.None);
				}
			}
			else
			{
				if (currentWeaponType.Contains(WeaponType.None))
				{
					currentWeaponType.Remove(WeaponType.None);
				}

				currentWeaponType.Add(weaponType);
			}

			if (currentWeaponType.Contains(WeaponType.None))
			{
				weaponFilterButtons.ForEach(delegate(CharacterTabMasteryBtn x)
				{
					x.SetSelected(x.WeaponType == WeaponType.None);
				});
			}
			else
			{
				weaponFilterButtons.ForEach(delegate(CharacterTabMasteryBtn x)
				{
					x.SetSelected(currentWeaponType.Contains(x.WeaponType));
				});
			}

			Refresh();
		}


		private struct FilterData
		{
			public FilterData(int skinCode, int characterCode)
			{
				this.skinCode = skinCode;
				this.characterCode = characterCode;
			}


			public readonly int skinCode;


			public readonly int characterCode;
		}
	}
}