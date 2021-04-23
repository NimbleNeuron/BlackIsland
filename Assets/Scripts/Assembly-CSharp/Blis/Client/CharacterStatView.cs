using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterStatView : BaseUI, ILnEventHander
	{
		[SerializeField] private Text characterName = default;


		[SerializeField] private UIProgress health = default;


		[SerializeField] private UIProgress stamina = default;


		[SerializeField] private UIProgress attack = default;


		[SerializeField] private UIProgress defence = default;


		[SerializeField] private LayoutGroup masterysGroup = default;


		[SerializeField] private Text tooltipName = default;


		[SerializeField] private Text tooltip = default;


		[SerializeField] private Text tooltipCost = default;


		[SerializeField] private Text tooltipCoolDown = default;


		private readonly List<MasteryIcon> masteryIcons = new List<MasteryIcon>();


		private readonly Dictionary<SkillSlotIndex, SkillIcon> skills =
			new Dictionary<SkillSlotIndex, SkillIcon>(SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>
				.Instance);


		private int selectedCharacterCode;


		public void OnLnDataChange()
		{
			tooltipName.text = null;
			tooltip.text = null;
			tooltipCost.text = null;
			tooltipCoolDown.text = null;
			if (selectedCharacterCode > 0)
			{
				OnSelectCharacter(selectedCharacterCode);
			}
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			for (int i = 0; i < masterysGroup.transform.childCount; i++)
			{
				if (masterysGroup.transform.GetChild(i).gameObject.name != "Tip")
				{
					masteryIcons.Add(new MasteryIcon(masterysGroup.transform.GetChild(i).gameObject));
				}
			}

			skills.Add(SkillSlotIndex.Active1,
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/Skills/Skill1").gameObject));
			skills.Add(SkillSlotIndex.Active2,
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/Skills/Skill2").gameObject));
			skills.Add(SkillSlotIndex.Active3,
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/Skills/Skill3").gameObject));
			skills.Add(SkillSlotIndex.Active4,
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/Skills/Skill4").gameObject));
			skills.Add(SkillSlotIndex.Passive,
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/Skill5").gameObject));
		}


		public void OnSelectCharacter(int characterCode)
		{
			selectedCharacterCode = characterCode;
			if (tooltipName != null)
			{
				tooltipName.text = null;
			}

			if (tooltip != null)
			{
				tooltip.text = null;
			}

			if (tooltipCost != null)
			{
				tooltipCost.text = null;
			}

			if (tooltipCoolDown != null)
			{
				tooltipCoolDown.text = null;
			}

			CharacterData characterData = GameDB.character.GetCharacterData(characterCode);
			if (characterData != null)
			{
				characterName.text = LnUtil.GetCharacterName(characterCode);
				if (health != null)
				{
					health.SetValue(characterData.maxHp, 800);
					health.SetLabel(characterData.maxHp.ToString());
				}

				if (stamina != null)
				{
					stamina.SetValue(characterData.maxSp, 500);
					stamina.SetLabel(characterData.maxSp.ToString());
				}

				if (attack != null)
				{
					attack.SetValue((int) Math.Truncate(characterData.attackPower), 100);
					attack.SetLabel(characterData.attackPower.ToString());
				}

				if (defence != null)
				{
					defence.SetValue((int) Math.Truncate(characterData.defense), 50);
					defence.SetLabel(characterData.defense.ToString());
				}
			}

			CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(characterCode);
			if (characterMasteryData != null)
			{
				List<MasteryType> list = new List<MasteryType>();
				list.Add(characterMasteryData.weapon1);
				list.Add(characterMasteryData.weapon2);
				list.Add(characterMasteryData.weapon3);
				list.Add(characterMasteryData.weapon4);
				ClearMasteryIcons();
				int num = 0;
				while (num < masteryIcons.Count && num < list.Count)
				{
					masteryIcons[num].SetMastery(list[num]);
					num++;
				}
			}

			if (skills != null && skills.Count > 0)
			{
				Dictionary<SkillSlotIndex, SkillSlotSet> defaultSkillSet =
					GameDB.skill.GetDefaultSkillSet(characterCode, ObjectType.PlayerCharacter);
				foreach (KeyValuePair<SkillSlotIndex, SkillIcon> keyValuePair in skills)
				{
					SkillData skillData = GameDB.skill.GetSkillData(characterCode, ObjectType.PlayerCharacter,
						defaultSkillSet[keyValuePair.Key], 1, 0);
					if (skillData != null)
					{
						keyValuePair.Value.SetSkilLData(skillData, keyValuePair.Key);
						keyValuePair.Value.SetName(LnUtil.GetSkillName(skillData.group));
						keyValuePair.Value.SetDesc(LnUtil.GetLobbySkillDesc(skillData.group));
						keyValuePair.Value.SetCost(LnUtil.GetCostText(skillData.CostType, skillData.CostKey,
							skillData.cost));
						keyValuePair.Value.SetCooldown(Ln.Format("재사용 대기 시간 {0} 초", skillData.cooldown));
						keyValuePair.Value.SetIcon(GameDB.skill.GetSkillIcon(skillData.Icon));
					}
				}

				if (skills.ContainsKey(SkillSlotIndex.Active1))
				{
					if (tooltipName != null)
					{
						tooltipName.text = skills[SkillSlotIndex.Active1].GetName();
					}

					if (tooltip != null)
					{
						tooltip.text = skills[SkillSlotIndex.Active1].GetDesc();
					}

					if (tooltipCost != null)
					{
						tooltipCost.text = skills[SkillSlotIndex.Active1].GetCost();
					}

					if (tooltipCoolDown != null)
					{
						tooltipCoolDown.text = skills[SkillSlotIndex.Active1].GetCooldown();
					}

					skills[SkillSlotIndex.Active1].Press();
				}
			}
		}


		public void OnClickCharacterGuide()
		{
			MonoBehaviourInstance<LobbyUI>.inst.CharacterGuideWindow.Open();
			MonoBehaviourInstance<LobbyUI>.inst.CharacterGuideWindow.SetCharacterCode(selectedCharacterCode);
		}


		private void ClearMasteryIcons()
		{
			masteryIcons.ForEach(delegate(MasteryIcon x) { x.SetMastery(MasteryType.None); });
		}


		private class MasteryIcon
		{
			private readonly EventTrigger eventTrigger;


			private readonly GameObject gameObject;


			private readonly Image glow;


			private readonly Image icon;


			private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


			private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


			private SkillData skillData;


			private WeaponType weaponType;

			public MasteryIcon(GameObject gameObject)
			{
				this.gameObject = gameObject;
				icon = GameUtil.Bind<Image>(gameObject, "Icon");
				glow = GameUtil.Bind<Image>(gameObject, "Glow");
				glow.gameObject.SetActive(false);
				GameUtil.BindOrAdd<EventTrigger>(this.gameObject, ref eventTrigger);
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


			public void SetMastery(MasteryType masteryType)
			{
				if (masteryType != MasteryType.None)
				{
					skillData = GameDB.skill.GetSkillData(masteryType, 1, 0);
					icon.sprite = masteryType.GetIcon();
					gameObject.SetActive(true);
					weaponType = masteryType.GetWeaponType();
					return;
				}

				skillData = null;
				icon.sprite = null;
				gameObject.SetActive(false);
				weaponType = WeaponType.None;
			}


			private void OnPointerEnter(BaseEventData eventData)
			{
				glow.gameObject.SetActive(true);
				MonoBehaviourInstance<Tooltip>.inst.SetWeaponSkill(weaponType, skillData);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null,
					Tooltip.TooltipPosition.LobbyMasterSkillInfoPosition);
			}


			private void OnPointerExit(BaseEventData eventData)
			{
				glow.gameObject.SetActive(false);
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}
		}


		private class SkillIcon
		{
			private readonly EventTrigger eventTrigger;


			private readonly GameObject gameObject;


			private readonly Image icon;


			private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


			private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();
			private string cooldown;


			private string cost;


			private string desc;


			private GameInputEvent gameInputEvent;


			private string name;


			private SkillData skillData;


			public SkillIcon(GameObject gameObject)
			{
				this.gameObject = gameObject;
				icon = gameObject.GetComponent<Image>();
				Toggle = gameObject.GetComponent<Toggle>();
				Toggle.graphic.GetComponent<Image>().enabled = false;
				GameUtil.BindOrAdd<EventTrigger>(this.gameObject, ref eventTrigger);
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


			public Toggle Toggle { get; }


			private void OnPointerEnter(BaseEventData eventData)
			{
				string keyCode = "";
				if (gameInputEvent != GameInputEvent.None)
				{
					keyCode = Singleton<LocalSetting>.inst.GetKeyCode(gameInputEvent).ToString();
				}

				Toggle.isOn = true;
				Toggle.graphic.GetComponent<Image>().enabled = true;
				MonoBehaviourInstance<Tooltip>.inst.SetSkill(skillData, keyCode, false);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null,
					Tooltip.TooltipPosition.LobbyCharacterSkillInfoPosition);
			}


			private void OnPointerExit(BaseEventData eventData)
			{
				Toggle.group.SetAllTogglesOff();
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}


			public void SetSkilLData(SkillData skillData, SkillSlotIndex skillSlotIndex)
			{
				this.skillData = skillData;
				switch (skillSlotIndex)
				{
					case SkillSlotIndex.Active1:
						gameInputEvent = GameInputEvent.Active1;
						return;
					case SkillSlotIndex.Active2:
						gameInputEvent = GameInputEvent.Active2;
						return;
					case SkillSlotIndex.Active3:
						gameInputEvent = GameInputEvent.Active3;
						return;
					case SkillSlotIndex.Active4:
						gameInputEvent = GameInputEvent.Active4;
						return;
					default:
						return;
				}
			}


			public void SetName(string name)
			{
				this.name = name;
			}


			public string GetName()
			{
				return name;
			}


			public void SetDesc(string desc)
			{
				this.desc = desc;
			}


			public string GetDesc()
			{
				return desc;
			}


			public void SetCost(string cost)
			{
				this.cost = cost;
			}


			public string GetCost()
			{
				return cost;
			}


			public void SetCooldown(string cooldown)
			{
				this.cooldown = cooldown;
			}


			public string GetCooldown()
			{
				return cooldown;
			}


			public void SetIcon(Sprite sprite)
			{
				icon.sprite = sprite;
			}


			public void Press() { }
		}
	}
}