using System.Collections.Generic;
using System.Text;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class WeaponMastery : BaseUI
	{
		public delegate void OnMasteryToggleChange(MasteryType masteryType);


		private const int minSelectLevel = 1;


		private const int maxSelectLevel = 20;


		[SerializeField] private List<Toggle> list_Toggles = new List<Toggle>();


		[SerializeField] private Text txt_AddOption_Desc = default;


		[SerializeField] private Text txt_Acquire_Title = default;


		[SerializeField] private Text txt_Acquire_Desc = default;


		[SerializeField] private Text txt_MasteryLevel = default;


		[SerializeField] private Transform progressBlock = default;


		[SerializeField] private RectTransform btn_Progress = default;


		private readonly List<WeaponMasterBlock> list_MasteryBlocks = new List<WeaponMasterBlock>();


		private readonly List<MasteryIcon> list_MasteryIcon = new List<MasteryIcon>();


		private List<MasteryType> list_MasteryType = new List<MasteryType>
		{
			MasteryType.OneHandSword,
			MasteryType.TwoHandSword,
			MasteryType.Axe,
			MasteryType.DualSword
		};


		public OnMasteryToggleChange onMasteryToggleChange;


		private int selectMasteryLevel = 1;


		private MasteryType selectMasteryType;


		public MasteryType SelectMasteryType => selectMasteryType;


		protected override void OnStartUI()
		{
			base.OnStartUI();
			if (0 < list_Toggles.Count)
			{
				list_Toggles[0].onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(0, isOn); });
			}

			if (1 < list_Toggles.Count)
			{
				list_Toggles[1].onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(1, isOn); });
			}

			if (2 < list_Toggles.Count)
			{
				list_Toggles[2].onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(2, isOn); });
			}

			if (3 < list_Toggles.Count)
			{
				list_Toggles[3].onValueChanged.AddListener(delegate(bool isOn) { OnToggleChange(3, isOn); });
			}

			InitMasterLevelBar();
			ResetMasteryIcons();
		}


		private void InitMasterLevelBar()
		{
			if (list_MasteryBlocks.Count == 0)
			{
				for (int i = 0; i < progressBlock.childCount; i++)
				{
					int level = i + 1;
					list_MasteryBlocks.Add(progressBlock.GetChild(i).GetComponent<WeaponMasterBlock>());
					list_MasteryBlocks[i].GetComponent<Button>().onClick.AddListener(delegate
					{
						ClickLevelButton(level);
					});
				}
			}
		}


		private void FillBlock(int level)
		{
			int num = 1;
			foreach (WeaponMasterBlock weaponMasterBlock in list_MasteryBlocks)
			{
				weaponMasterBlock.Clear();
				if (num < level)
				{
					weaponMasterBlock.Fill();
				}
				else if (num == level)
				{
					weaponMasterBlock.Fill();
					weaponMasterBlock.Focus();
				}

				num++;
			}
		}


		private void ResetMasteryIcons()
		{
			list_MasteryIcon.Clear();
			int num = 0;
			foreach (MasteryType masteryType in list_MasteryType)
			{
				MasteryIcon masteryIcon = new MasteryIcon(list_Toggles[num], masteryType);
				masteryIcon.icon.sprite = masteryIcon.masteryType.GetIcon();
				list_MasteryIcon.Add(masteryIcon);
				num++;
			}
		}


		public void SetMasteryTypes(int code)
		{
			list_MasteryType = GameDB.mastery.GetCharacterMasteryData(code).GetMasteries();
			ResetMasteryIcons();
			selectMasteryType = list_MasteryType[0];
			selectMasteryLevel = 1;
			FocusTabUI(selectMasteryType);
			UpdateTabUI();
			UpdateProgressbarUI();
			UpdateDescUI();
		}


		private void UpdateTabUI()
		{
			for (int i = 0; i < list_Toggles.Count; i++)
			{
				if (i < list_MasteryType.Count)
				{
					list_Toggles[i].gameObject.SetActive(true);
				}
				else
				{
					list_Toggles[i].gameObject.SetActive(false);
				}
			}
		}


		private void UpdateProgressbarUI(int level = 1)
		{
			txt_MasteryLevel.text = string.Format("{0}", level);
			FillBlock(level);
		}


		private void UpdateDescUI(int level = 1)
		{
			txt_Acquire_Title.text =
				Ln.Format("{0} 장착 시 증가 옵션", Ln.Get(string.Format("WeaponType/{0}", selectMasteryType)));
			txt_AddOption_Desc.text = Ln.Get(string.Format("MasteryType/{0}/Desc/{1}", selectMasteryType, level));
			HashSet<MasteryConditionType> hashSet = new HashSet<MasteryConditionType>();
			if (selectMasteryType != MasteryType.None)
			{
				List<MasteryExpData> list = GameDB.mastery.FindMasteryExpData(selectMasteryType);
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						MasteryConditionType conditionType = list[i].conditionType;
						hashSet.Add(conditionType);
					}
				}
			}

			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			foreach (MasteryConditionType masteryConditionType in hashSet)
			{
				stringBuilder.AppendLine(Ln.Get(string.Format("MasteryConditionType/{0}", masteryConditionType)));
			}

			txt_Acquire_Desc.text = stringBuilder.ToString();
		}


		private void OnToggleChange(int index, bool isOn)
		{
			if (index < 0 || list_MasteryType.Count <= index)
			{
				return;
			}

			MasteryType masteryType = list_MasteryType[index];
			if (isOn && selectMasteryType != masteryType)
			{
				UpdateWeaponMastery(masteryType);
				OnMasteryToggleChange onMasteryToggleChange = this.onMasteryToggleChange;
				if (onMasteryToggleChange == null)
				{
					return;
				}

				onMasteryToggleChange(masteryType);
			}
		}


		private void FocusTabUI(MasteryType masteryType)
		{
			foreach (MasteryIcon masteryIcon in list_MasteryIcon)
			{
				if (masteryIcon.masteryType == masteryType && !masteryIcon.toggle.isOn)
				{
					masteryIcon.toggle.isOn = true;
				}
			}
		}


		private void UpdateWeaponMastery(MasteryType selectMasteryType)
		{
			if (this.selectMasteryType == selectMasteryType)
			{
				return;
			}

			this.selectMasteryType = selectMasteryType;
			FocusTabUI(selectMasteryType);
			UpdateProgressbarUI(selectMasteryLevel);
			UpdateDescUI(selectMasteryLevel);
		}


		private void ClickLevelButton(int level)
		{
			if (selectMasteryLevel == level)
			{
				return;
			}

			selectMasteryLevel = level;
			UpdateProgressbarUI(level);
			UpdateDescUI(level);
		}


		public void ClickLevelUp()
		{
			if (selectMasteryLevel >= 20)
			{
				return;
			}

			selectMasteryLevel++;
			UpdateProgressbarUI(selectMasteryLevel);
			UpdateDescUI(selectMasteryLevel);
		}


		public void ClickLevelDown()
		{
			if (selectMasteryLevel <= 1)
			{
				return;
			}

			selectMasteryLevel--;
			UpdateProgressbarUI(selectMasteryLevel);
			UpdateDescUI(selectMasteryLevel);
		}

		private void Ref()
		{
			Reference.Use(btn_Progress);
		}


		private class MasteryIcon
		{
			private readonly EventTrigger eventTrigger;


			public readonly Image icon;


			public readonly MasteryType masteryType;


			private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


			private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


			public readonly Toggle toggle;

			public MasteryIcon(Toggle toggle, MasteryType masteryType)
			{
				this.toggle = toggle;
				this.masteryType = masteryType;
				icon = toggle.transform.Find("Image").GetComponent<Image>();
				icon.sprite = masteryType.GetIcon();
				GameUtil.BindOrAdd<EventTrigger>(toggle.gameObject, ref eventTrigger);
				eventTrigger.triggers.Clear();
				onEnterEvent.AddListener(OnPointerEnter);
				onExitEvent.AddListener(OnPointerExit);
				eventTrigger.triggers.Add(new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerEnter,
					callback = onEnterEvent
				});
				eventTrigger.triggers.Add(new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerExit,
					callback = onExitEvent
				});
			}


			private void OnPointerEnter(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get(string.Format("WeaponType/{0}", masteryType)));
				Vector2 vector = toggle.transform.position;
				vector += GameUtil.ConvertPositionOnScreenResolution(10f, 60f);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
			}


			private void OnPointerExit(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}
		}
	}
}