using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class SkillEvolutionSlot : BaseControl
	{
		private Button btn;


		private Image img;


		private SkillSlotIndex skillSlotIndex;

		protected override void OnAwakeUI()
		{
			img = GameUtil.Bind<Image>(gameObject, "Btn");
			btn = GameUtil.Bind<Button>(gameObject, "Btn");
			Disable();
		}


		public void SetSkillSlotIndex(SkillSlotIndex skillSlotIndex)
		{
			this.skillSlotIndex = skillSlotIndex;
		}


		public void Enable(bool canUpgradeSkill)
		{
			gameObject.SetActive(true);
			img.color = canUpgradeSkill ? Color.white : Color.gray;
			btn.enabled = canUpgradeSkill;
		}


		public void Disable()
		{
			gameObject.SetActive(false);
		}


		public bool GetEnableState()
		{
			return gameObject.activeSelf;
		}


		public void SetIcon(string iconName)
		{
			img.sprite = GameDB.skill.GetSkillIcon(iconName);
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (!eventData.dragging)
			{
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.ShowEvolutionSkillTooltip(skillSlotIndex);
			}
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			MonoBehaviourInstance<Tooltip>.inst.Hide(GetParentWindow());
		}


		public void SetOnClickEvent(Action<SkillSlotIndex> action)
		{
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(delegate
			{
				Action<SkillSlotIndex> action2 = action;
				if (action2 == null)
				{
					return;
				}

				action2(skillSlotIndex);
			});
		}
	}
}