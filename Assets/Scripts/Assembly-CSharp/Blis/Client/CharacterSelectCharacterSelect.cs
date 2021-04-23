using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using SuperScrollView;

namespace Blis.Client
{
	public class CharacterSelectCharacterSelect : BaseUI
	{
		private const int SLOT_COUNT = 6;


		private readonly int MarginCount = 2;


		private List<CharacterData> characterList = new List<CharacterData>();


		private bool initialized;


		private bool isOpen;


		private LoopListView2 listView;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			listView = GameUtil.Bind<LoopListView2>(gameObject, "ItemScrollView");
			initialized = false;
			isOpen = false;
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			InitListView();
		}


		private void InitListView()
		{
			if (initialized)
			{
				return;
			}

			initialized = true;
			characterList = GetCharacterDataList();
			listView.InitListView(GetItemCount(characterList.Count) + MarginCount, OnGetItemByIndex);
		}


		private List<CharacterData> GetCharacterDataList()
		{
			characterList.Clear();
			List<CharacterData> allCharacterData = GameDB.character.GetAllCharacterData();
			for (int i = 0; i < allCharacterData.Count; i++)
			{
				CharacterData characterData = allCharacterData[i];
				if (Lobby.inst.IsHaveCharacter(characterData.code) ||
				    MonoBehaviourInstance<MatchingService>.inst.IsFreeCharacter(characterData.code) ||
				    SingletonMonoBehaviour<KakaoPcService>.inst.BenefitByKakaoPcCafe)
				{
					characterList.Add(characterData);
				}
			}

			return characterList;
		}


		public void Open()
		{
			isOpen = true;
			gameObject.SetActive(true);
			characterList = GetCharacterDataList();
			UpdateCharacters(true);
		}


		public void Close()
		{
			isOpen = false;
			gameObject.SetActive(false);
		}


		public void UpdateCharacters(bool resetPos)
		{
			if (initialized)
			{
				listView.SetListItemCount(GetItemCount(characterList.Count) + MarginCount, resetPos);
				listView.RefreshAllShownItem();
			}
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (!isOpen)
			{
				return null;
			}

			if (!initialized)
			{
				return null;
			}

			if (index < 0)
			{
				return null;
			}

			int itemCount = GetItemCount(characterList.Count);
			if (itemCount + MarginCount < index)
			{
				return null;
			}

			int num = index - 1;
			LoopListViewItem2 loopListViewItem;
			if (index == 0 || itemCount <= num)
			{
				loopListViewItem = listView.NewListViewItem("Margin");
			}
			else
			{
				loopListViewItem = listView.NewListViewItem("CharacterSelectCharacterSlots");
				CharacterSelectCharacterSlots
					component = loopListViewItem.GetComponent<CharacterSelectCharacterSlots>();
				for (int i = 0; i < 6; i++)
				{
					int num2 = num * 6 + i;
					if (num2 < characterList.Count)
					{
						component.SetCharacter(i, characterList[num2]);
					}
					else
					{
						component.SetCharacter(i, null);
					}
				}
			}

			if (loopListViewItem != null)
			{
				loopListViewItem.gameObject.SetActive(true);
			}

			return loopListViewItem;
		}


		private int GetItemCount(int allCharacterCount)
		{
			int num = allCharacterCount / 6;
			if (allCharacterCount % 6 != 0)
			{
				num++;
			}

			return num;
		}
	}
}