using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class SlimCharacterSelectionView : BaseUI
	{
		[SerializeField] private GameObject characterCardPrefab = default;


		[SerializeField] private ToggleGroup toggleGroup = default;


		private readonly Dictionary<Toggle, int> toggles = new Dictionary<Toggle, int>();

		
		
		public event Action<int> OnCharacterSelected = delegate { };


		protected override void OnStartUI()
		{
			base.OnStartUI();
			Transform transform = toggleGroup.transform;
			List<int> list = (from c in GameDB.character.GetAllCharacterData()
				select c.code).ToList<int>();
			if (transform.childCount < list.Count)
			{
				int num = list.Count - transform.childCount;
				for (int i = 0; i < num; i++)
				{
					Instantiate<GameObject>(characterCardPrefab, transform);
				}
			}

			for (int j = 0; j < transform.childCount; j++)
			{
				Transform child = transform.GetChild(j);
				Toggle componentInChildren = child.GetComponentInChildren<Toggle>();
				LobbyCharacterCard component = child.GetComponent<LobbyCharacterCard>();
				if (j < list.Count)
				{
					componentInChildren.group = toggleGroup;
					toggles.Add(componentInChildren, list[j]);
					componentInChildren.isOn = false;
					component.SetCharacterCode(list[j], false, true);
				}
				else
				{
					component.SetCharacterCode(0, false, false);
				}
			}

			using (Dictionary<Toggle, int>.KeyCollection.Enumerator enumerator = toggles.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Toggle toggle = enumerator.Current;
					toggle.onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(toggles[toggle], isOn); });
				}
			}
		}


		private void OnToggleChange(int characterCode, bool isOn)
		{
			if (isOn)
			{
				OnCharacterSelected(characterCode);
			}
		}


		public void FocusCharacter(int characterCode)
		{
			foreach (KeyValuePair<Toggle, int> keyValuePair in toggles)
			{
				if (keyValuePair.Value == characterCode && !keyValuePair.Key.isOn)
				{
					keyValuePair.Key.isOn = true;
				}
			}
		}


		public void FocusCharacterPosition(int characterCode)
		{
			float num = 70f;
			int num2 = 6;
			int num3 = characterCode - num2;
			float y = num3 > 0 ? num * num3 : 0f;
			toggleGroup.transform.localPosition = new Vector3(0f, y, 0f);
		}
	}
}