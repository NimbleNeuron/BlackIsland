using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterSelectSkinSelect : BaseUI
	{
		[SerializeField] private GameObject characterSelectSkinSlot = default;


		private List<CharacterSkinData> characterSkinList = new List<CharacterSkinData>();


		private ScrollRect scrollRect = default;


		private CharacterSelectSkinSlot[] skinSlots = default;


		private int slotIndex = default;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			scrollRect = GameUtil.Bind<ScrollRect>(gameObject, "SkinScrollView");
		}


		public void Open(int characterCode, int skinCode)
		{
			gameObject.SetActive(true);
			InitListView(characterCode, skinCode);
		}


		public void Close()
		{
			gameObject.SetActive(false);
		}


		private void InitListView(int characterCode, int skinCode)
		{
			slotIndex = 0;
			characterSkinList = GameDB.character.GetSkinDataList(characterCode);
			int childCount = scrollRect.content.childCount;
			int num = Mathf.Max(0, characterSkinList.Count - childCount);
			for (int i = 0; i < num; i++)
			{
				Instantiate<GameObject>(characterSelectSkinSlot, scrollRect.content);
			}

			skinSlots = scrollRect.content.GetComponentsInChildren<CharacterSelectSkinSlot>(true);
			for (int j = 0; j < skinSlots.Length; j++)
			{
				if (j < characterSkinList.Count)
				{
					CharacterSkinData characterSkinData = characterSkinList.ElementAt(j);
					bool flag = Lobby.inst.IsHaveSkin(characterSkinData.code);
					skinSlots[j].gameObject.SetActive(true);
					skinSlots[j].SetSlot(characterSkinData, j);
					skinSlots[j].OnSelectSlot(false);
					skinSlots[j].SetReleaseableLock(false);
					skinSlots[j].SetButtonInteractable(flag);
					skinSlots[j].SetNotReleaseableLock(!flag);
					if (flag)
					{
						skinSlots[j].selectCallback = OnClickSkin;
					}

					if (characterSkinData.code == skinCode)
					{
						skinSlots[j].OnSelectSlot(true);
					}
				}
				else
				{
					skinSlots[j].gameObject.SetActive(false);
				}
			}
		}


		private void OnClickSkin(int index)
		{
			MatchingService inst = MonoBehaviourInstance<MatchingService>.inst;
			if (inst == null)
			{
				return;
			}

			skinSlots[slotIndex].OnSelectSlot(false);
			slotIndex = index;
			skinSlots[slotIndex].OnSelectSlot(true);
			CharacterSkinData characterSkinData = characterSkinList[index];
			int skinCode = characterSkinData.code;
			if (characterSkinData.index == 0)
			{
				skinCode = 0;
			}

			inst.SelectSkin(skinCode);
		}
	}
}