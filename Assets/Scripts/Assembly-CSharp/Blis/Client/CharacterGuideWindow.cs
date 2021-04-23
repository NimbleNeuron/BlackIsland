using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterGuideWindow : BaseWindow
	{
		[SerializeField] private GameObject prefab = default;


		[SerializeField] private Text guideText = default;


		[SerializeField] private ToggleGroup toggleGroup = default;


		public WeaponMastery WeaponMastery;


		private readonly Dictionary<Toggle, int> toggles = new Dictionary<Toggle, int>();


		private int selectedCharacterCode;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			List<int> list = (from c in GameDB.character.GetAllCharacterData()
				select c.code).ToList<int>();
			Transform transform = toggleGroup.transform;
			if (transform.childCount < list.Count)
			{
				int num = list.Count - transform.childCount;
				for (int i = 0; i < num; i++)
				{
					Instantiate<GameObject>(prefab, transform).GetComponent<Toggle>().group = toggleGroup;
				}
			}

			for (int j = 0; j < transform.childCount; j++)
			{
				Toggle component = transform.GetChild(j).GetComponent<Toggle>();
				if (component != null)
				{
					component.GetComponent<CharacterGuide>().SetName(list[j]);
					toggles.Add(component, list[j]);
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
			if (isOn && selectedCharacterCode != characterCode)
			{
				SetCharacterCode(characterCode);
			}
		}


		private void FocusCharacter(int characterCode)
		{
			foreach (KeyValuePair<Toggle, int> keyValuePair in toggles)
			{
				if (keyValuePair.Value == characterCode && !keyValuePair.Key.isOn)
				{
					keyValuePair.Key.isOn = true;
				}
			}
		}


		public void SetCharacterCode(int selectedCharacterCode)
		{
			guideText.text = Ln.Get(LnType.CharacterGuide, string.Format("{0}", selectedCharacterCode));
			this.selectedCharacterCode = selectedCharacterCode;
			FocusCharacter(selectedCharacterCode);
			WeaponMastery.SetMasteryTypes(selectedCharacterCode);
		}
	}
}