using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class FavoriteBoardCreate : BaseUI
	{
		private readonly List<Image> masteryIcons = new List<Image>();
		private int characterCode;


		private List<MasteryType> masteryTypes = new List<MasteryType>();


		private MasteryType selectMasteryType;


		private int slotId;


		private Transform toggleParent;


		private Toggle[] toggles;

		
		
		public event Action<Favorite> editAction = delegate { };


		
		
		public event Action<Favorite> bringAction = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			toggleParent = transform.FindRecursively("MasteryGroup");
			toggles = toggleParent.GetComponentsInChildren<Toggle>();
			for (int i = 0; i < toggles.Length; i++)
			{
				int index = i;
				toggles[i].onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(isOn, index); });
				Image item = GameUtil.Bind<Image>(toggles[i].gameObject, "IMG_Mastery");
				masteryIcons.Add(item);
			}
		}


		private void OnToggleChange(bool isOn, int index)
		{
			if (isOn)
			{
				selectMasteryType = masteryTypes[index];
			}
		}


		public void SetMasterys(int characterCode)
		{
			this.characterCode = characterCode;
			masteryTypes = GameDB.mastery.GetCharacterMasteryData(characterCode).GetMasteries();
			if (masteryTypes.Contains(MasteryType.DualSword))
			{
				masteryTypes.Remove(MasteryType.DualSword);
			}

			for (int i = 0; i < toggles.Length; i++)
			{
				if (i < masteryTypes.Count)
				{
					toggles[i].gameObject.SetActive(true);
					masteryIcons[i].sprite = masteryTypes[i].GetIcon();
				}
				else
				{
					toggles[i].gameObject.SetActive(false);
				}
			}

			toggles[0].isOn = true;
			selectMasteryType = masteryTypes[0];
		}


		public void SetSlotId(int slotid)
		{
			slotId = slotid;
		}


		public void ClickedCreateNewRoute()
		{
			Favorite obj = new Favorite(Lobby.inst.User.UserNum, characterCode, slotId, "",
				selectMasteryType.GetWeaponType(), new List<int>(), new List<int>(), -1L, "",
				false, false, BSERVersion.VERSION,
				RouteFilterType.ALL, 0, -1L);
			editAction(obj);
		}


		public void ClickedBringRoute()
		{
			Favorite obj = new Favorite(-1L, characterCode, slotId, "", selectMasteryType.GetWeaponType(),
				new List<int>(), new List<int>(), -1L, "", false, false, "", RouteFilterType.ALL, 0, -1L);
			bringAction(obj);
		}
	}
}