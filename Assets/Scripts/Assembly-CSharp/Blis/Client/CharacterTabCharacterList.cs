using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterTabCharacterList : BasePage
	{
		[SerializeField] private CharacterTabMasteryBtn masteryPrefab = default;


		[SerializeField] private CharacterTabPortraitCard characterCardPrefab = default;


		private readonly List<CharacterTabPortraitCard> cards = new List<CharacterTabPortraitCard>();


		private readonly List<WeaponType> selectedWeaponTypes = new List<WeaponType>();


		private RectTransform blank;


		private CanvasGroup canvasGroup;


		private bool checkBoxHaveCharacter = default;


		private RectTransform content;


		private string editString = string.Empty;


		private Toggle holdCheckToggle = default;


		private InputFieldExtension inputField = default;


		private CharacterTabMasteryBtn masteryAllButtons;


		private List<CharacterTabMasteryBtn> masteryButtons;


		private RectTransform masterys;


		private int selectedCharacterCode;


		protected override void OnDestroy()
		{
			base.OnDestroy();
			NoticeService.onReceivedCharacter -= OnReceivedCharacter;
			ShopProductService.onReceivedDlcCharacter -= OnReceivedCharacter;
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			inputField = GameUtil.Bind<InputFieldExtension>(gameObject, "Search/EditedTitle");
			holdCheckToggle = GameUtil.Bind<Toggle>(gameObject, "HaveCharacterCheck/CheckBox");
			masterys = GameUtil.Bind<RectTransform>(gameObject, "Masterys");
			masteryAllButtons =
				GameUtil.Bind<CharacterTabMasteryBtn>(masterys.gameObject, "CharacterTabMasteryBtn_All");
			content = GameUtil.Bind<RectTransform>(gameObject, "CharacterScrollView/Viewport/Content");
			blank = GameUtil.Bind<RectTransform>(gameObject, "CharacterScrollView/Viewport/Blank");
			NoticeService.onReceivedCharacter += OnReceivedCharacter;
			ShopProductService.onReceivedDlcCharacter += OnReceivedCharacter;
			inputField.onValueChanged.AddListener(OnInputValueChange);
			holdCheckToggle.onValueChanged.AddListener(ClickedHaveCharacterCheckBox);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			SetMasteryButtons();
		}


		public void Initialized()
		{
			holdCheckToggle.isOn = true;
			checkBoxHaveCharacter = true;
			selectedWeaponTypes.Clear();
			selectedWeaponTypes.Add(WeaponType.None);
			inputField.text = string.Empty;
			inputField.textComponent.text = string.Empty;
		}


		public void Open()
		{
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			OpenPage();
			Filtering();
		}


		public void Close()
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			ClosePage();
		}


		private void SetCharacterList(List<CharacterCardData> characterCardDataList)
		{
			if (characterCardDataList == null || !characterCardDataList.Any<CharacterCardData>())
			{
				content.gameObject.SetActive(false);
				SetBlank(true);
				return;
			}

			content.gameObject.SetActive(true);
			SetBlank(false);
			int num = Mathf.Max(0, characterCardDataList.Count<CharacterCardData>() - content.childCount);
			for (int i = 0; i < num; i++)
			{
				Instantiate<CharacterTabPortraitCard>(characterCardPrefab, content);
			}

			for (int j = 0; j < content.childCount; j++)
			{
				CharacterTabPortraitCard component = content.GetChild(j).GetComponent<CharacterTabPortraitCard>();
				if (j < characterCardDataList.Count<CharacterCardData>())
				{
					component.SetCharacterCode(characterCardDataList[j].characterCode, characterCardDataList[j].have,
						characterCardDataList[j].freeRotation);
					component.SetListener(
						MonoBehaviourInstance<LobbyUI>.inst.GetLobbyTab<LobbyCharacterTab>(LobbyTab.InventoryTab));
					cards.Add(component);
				}
				else if (component != null)
				{
					component.SetCharacterCode(0, false, false);
				}
			}
		}


		private void Filtering()
		{
			string searchString = LnUtil.GetSearchName(editString);
			List<MasteryType> masteryTypes = new List<MasteryType>();
			foreach (WeaponType selectedWeaponType in selectedWeaponTypes)
			{
				masteryTypes.Add(selectedWeaponType.GetWeaponMasteryType());
			}

			List<CharacterCardData> source =
				new List<CharacterCardData>();
			List<int> list = GameDB.character.GetAllCharacterData()
				.Select<CharacterData, int>(c => c.code).ToList<int>();
			for (int index = 0; index < list.Count; ++index)
			{
				int characterCode = list[index];
				bool have = Lobby.inst.IsHaveCharacter(characterCode);
				bool freeRotation = Lobby.inst.FreeRotation.Exists(x => x == characterCode);
				string searchName = LnUtil.GetSearchName(LnUtil.GetCharacterName(characterCode));
				if (!checkBoxHaveCharacter || have)
				{
					source.Add(new CharacterCardData(characterCode, have, freeRotation,
						searchName));
				}
			}

			if (searchString != string.Empty)
			{
				source = source.FindAll(
					p =>
						p.searchName.Contains(searchString));
			}

			if (!masteryTypes.Contains(MasteryType.None))
			{
				source = source.FindAll(p =>
				{
					CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(p.characterCode);
					foreach (MasteryType mastery in characterMasteryData.GetMasteries())
					{
						if (masteryTypes.Contains(mastery))
						{
							return true;
						}
					}

					int num = (int) characterMasteryData.GetMasteries()
						.Find(x => masteryTypes.Contains(x));
					return false;
				});
			}

			SetCharacterList(source
				.OrderByDescending<CharacterCardData, bool>(
					p =>
						Lobby.inst.IsHaveCharacter(p.characterCode))
				.ThenBy<CharacterCardData, int>(
					p => p.characterCode)
				.ToList<CharacterCardData>());

			// co: dotPeek
			// string searchString = LnUtil.GetSearchName(this.editString);
			// List<MasteryType> masteryTypes = new List<MasteryType>();
			// foreach (WeaponType weaponType in this.selectedWeaponTypes)
			// {
			// 	masteryTypes.Add(weaponType.GetWeaponMasteryType());
			// }
			// List<CharacterTabCharacterList.CharacterCardData> list = new List<CharacterTabCharacterList.CharacterCardData>();
			// List<int> list2 = (from c in GameDB.character.GetAllCharacterData()
			// select c.code).ToList<int>();
			// for (int i = 0; i < list2.Count; i++)
			// {
			// 	int characterCode = list2[i];
			// 	bool flag = Lobby.inst.IsHaveCharacter(characterCode);
			// 	bool freeRotation = Lobby.inst.FreeRotation.Exists((int x) => x == characterCode);
			// 	string searchName = LnUtil.GetSearchName(LnUtil.GetCharacterName(characterCode));
			// 	if (!this.checkBoxHaveCharacter || flag)
			// 	{
			// 		list.Add(new CharacterTabCharacterList.CharacterCardData(characterCode, flag, freeRotation, searchName));
			// 	}
			// }
			// if (searchString != string.Empty)
			// {
			// 	list = list.FindAll((CharacterTabCharacterList.CharacterCardData p) => p.searchName.Contains(searchString));
			// }
			// if (!masteryTypes.Contains(MasteryType.None))
			// {
			// 	Predicate<MasteryType> <>9__6;
			// 	list = list.FindAll(delegate(CharacterTabCharacterList.CharacterCardData p)
			// 	{
			// 		CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(p.characterCode);
			// 		foreach (MasteryType item in characterMasteryData.GetMasteries())
			// 		{
			// 			if (masteryTypes.Contains(item))
			// 			{
			// 				return true;
			// 			}
			// 		}
			// 		List<MasteryType> masteries = characterMasteryData.GetMasteries();
			// 		Predicate<MasteryType> match;
			// 		if ((match = <>9__6) == null)
			// 		{
			// 			match = (<>9__6 = ((MasteryType x) => masteryTypes.Contains(x)));
			// 		}
			// 		masteries.Find(match);
			// 		return false;
			// 	});
			// }
			// this.SetCharacterList((from p in list
			// orderby Lobby.inst.IsHaveCharacter(p.characterCode) descending, p.characterCode
			// select p).ToList<CharacterTabCharacterList.CharacterCardData>());
		}


		private void SetBlank(bool enable)
		{
			blank.gameObject.SetActive(enable);
		}


		private void ClickedHaveCharacterCheckBox(bool holdFlag)
		{
			checkBoxHaveCharacter = holdFlag;
			Filtering();
		}


		private void OnInputValueChange(string input)
		{
			if (editString != input)
			{
				editString = input;
				Filtering();
			}
		}


		public void ClearInputField()
		{
			inputField.text = string.Empty;
			inputField.textComponent.text = string.Empty;
			Filtering();
		}


		private void SetMasteryButtons()
		{
			List<WeaponTypeInfoData> list = GameDB.mastery.GetAllWeaponTypeInfoDatas().FindAll(x => x.shopFilter == 1);
			masteryButtons = masterys.GetComponentsInChildren<CharacterTabMasteryBtn>()
				.ToList<CharacterTabMasteryBtn>();
			int num = Mathf.Max(0, list.Count - (masteryButtons.Count - 1));
			for (int i = 0; i < num; i++)
			{
				CharacterTabMasteryBtn characterTabMasteryBtn =
					Instantiate<CharacterTabMasteryBtn>(masteryPrefab, masterys);
				WeaponType weaponType = list[i].type;
				characterTabMasteryBtn.SetMastery(weaponType);
				characterTabMasteryBtn.OnPointerClickEvent += delegate(BaseControl control, PointerEventData ed)
				{
					ChangeMasteryFilter(weaponType, (CharacterTabMasteryBtn) control);
				};
				masteryButtons.Add(characterTabMasteryBtn);
			}

			masteryAllButtons.SetMastery(WeaponType.None);
			masteryAllButtons.OnPointerClickEvent += delegate(BaseControl control, PointerEventData ed)
			{
				if (!((CharacterTabMasteryBtn) control).IsActiveSelected())
				{
					ChangeMasteryFilter(WeaponType.None, null);
				}
			};
			ChangeMasteryFilter(WeaponType.None, null);
			masteryAllButtons.SetSelected(true);
		}


		private void ChangeMasteryFilter(WeaponType weaponType, CharacterTabMasteryBtn selectedButton)
		{
			if (weaponType != WeaponType.None)
			{
				if (selectedWeaponTypes.Contains(WeaponType.None))
				{
					selectedWeaponTypes.Clear();
					masteryAllButtons.SetSelected(false);
				}

				if (!selectedButton.IsActiveSelected() && !selectedWeaponTypes.Contains(weaponType))
				{
					selectedWeaponTypes.Add(weaponType);
					selectedButton.SetSelected(true);
				}
				else if (selectedButton.IsActiveSelected() && selectedWeaponTypes.Contains(weaponType))
				{
					selectedWeaponTypes.Remove(weaponType);
					selectedButton.SetSelected(false);
				}
			}

			if (weaponType == WeaponType.None || selectedWeaponTypes.Count == 0)
			{
				selectedWeaponTypes.Clear();
				selectedWeaponTypes.Add(WeaponType.None);
				masteryButtons.ForEach(delegate(CharacterTabMasteryBtn x) { x.SetSelected(false); });
				masteryAllButtons.SetSelected(true);
			}

			Filtering();
		}


		private void OnReceivedCharacter(int characterCode)
		{
			Filtering();
		}


		public void buySuccessCallback(int code)
		{
			Filtering();
		}


		private class CharacterCardData
		{
			public readonly int characterCode;


			public readonly bool freeRotation;


			public readonly bool have;


			public readonly string searchName;

			public CharacterCardData(int characterCode, bool have, bool freeRotation, string searchName)
			{
				this.characterCode = characterCode;
				this.have = have;
				this.freeRotation = freeRotation;
				this.searchName = searchName;
			}
		}
	}
}