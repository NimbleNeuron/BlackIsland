using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class SkillUpgradeSlot : BaseControl
	{
		private readonly Color disableColor = new Color(0.3f, 0.3f, 0.3f);


		private ParticleSystem effect;


		private bool isMouseOnSlot;


		private SkillSlotIndex skillSlotIndex;


		private Button upperGradeButton;

		protected override void OnAwakeUI()
		{
			upperGradeButton = GameUtil.Bind<Button>(gameObject, "Btn");
			effect = gameObject.GetComponentInChildren<ParticleSystem>();
			Disable();
		}


		public void SetSkillSlotIndex(SkillSlotIndex skillSlotIndex)
		{
			this.skillSlotIndex = skillSlotIndex;
		}


		public void Enable(bool canUpgradeSkill)
		{
			gameObject.SetActive(true);
			upperGradeButton.image.color = canUpgradeSkill ? Color.white : disableColor;
			upperGradeButton.enabled = canUpgradeSkill;
			effect.Stop();
			if (canUpgradeSkill)
			{
				effect.Play();
			}
		}


		public void Disable()
		{
			gameObject.SetActive(false);
		}


		public bool GetEnableState()
		{
			return gameObject.activeSelf;
		}


		public void RefreshTooltip()
		{
			if (isMouseOnSlot && GetEnableState())
			{
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.ShowNextLevelSkillTooltip(skillSlotIndex);
			}
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (!eventData.dragging)
			{
				isMouseOnSlot = true;
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.ShowNextLevelSkillTooltip(skillSlotIndex);
			}
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			isMouseOnSlot = false;
			MonoBehaviourInstance<Tooltip>.inst.Hide(GetParentWindow());
		}


		public void SetOnClickEvent(Action<SkillSlotIndex> action)
		{
			upperGradeButton.onClick.RemoveAllListeners();
			upperGradeButton.onClick.AddListener(delegate
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