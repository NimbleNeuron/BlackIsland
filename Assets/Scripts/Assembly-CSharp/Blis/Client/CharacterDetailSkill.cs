using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterDetailSkill : BasePage
	{
		private readonly Dictionary<SkillSlotIndex, SkillIcon> skills =
			new Dictionary<SkillSlotIndex, SkillIcon>(SingletonComparerEnum<SkillSlotIndexComparer, SkillSlotIndex>
				.Instance);


		private readonly List<SkillIcon> weaponSkillIcons = new List<SkillIcon>();
		private CharacterTabCharacterDetail.MasteryIcon masteryIcon;


		private int selectedCharacterCode;


		private Text skillCooldown;


		private Text skillCost;


		private Text skillDesc;


		private Text skillKeyCode;


		private Text skillName;


		private CanvasGroup skillStateCanvas;


		private VideoPlayerModule videoPlayerModule;


		private CanvasGroup wpSkillCanvas;


		private Text wpSkillCooldown;


		private Text wpSkillCost;


		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			skillName = GameUtil.Bind<Text>(gameObject, "SkillIName");
			skillKeyCode = GameUtil.Bind<Text>(skillName.gameObject, "Key");
			skillDesc = GameUtil.Bind<Text>(gameObject, "SkillDesc");
			skillStateCanvas = GameUtil.Bind<CanvasGroup>(gameObject, "SkillState");
			skillCost = GameUtil.Bind<Text>(skillStateCanvas.gameObject, "SkillCost");
			skillCooldown = GameUtil.Bind<Text>(skillStateCanvas.gameObject, "SkillCooldown");
			wpSkillCanvas = GameUtil.Bind<CanvasGroup>(gameObject, "WeaponSkillI");
			masteryIcon =
				new CharacterTabCharacterDetail.MasteryIcon(GameUtil
					.Bind<Transform>(wpSkillCanvas.gameObject, "MasteryIcon").gameObject);
			wpSkillCost = GameUtil.Bind<Text>(wpSkillCanvas.gameObject, "WpSkillState/WpSkillCost");
			wpSkillCooldown = GameUtil.Bind<Text>(wpSkillCanvas.gameObject, "WpSkillState/WpSkillCooldown");
			videoPlayerModule = GameUtil.Bind<VideoPlayerModule>(gameObject, "VideoPlayer");
			skills.Add(SkillSlotIndex.Active1,
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/Skills/Skill1").gameObject));
			skills.Add(SkillSlotIndex.Active2,
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/Skills/Skill2").gameObject));
			skills.Add(SkillSlotIndex.Active3,
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/Skills/Skill3").gameObject));
			skills.Add(SkillSlotIndex.Active4,
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/Skills/Skill4").gameObject));
			skills.Add(SkillSlotIndex.Passive,
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/Skills/Skill5").gameObject));
			weaponSkillIcons.Add(
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/WpSkills/Skill1").gameObject));
			weaponSkillIcons.Add(
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/WpSkills/Skill2").gameObject));
			weaponSkillIcons.Add(
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/WpSkills/Skill3").gameObject));
			weaponSkillIcons.Add(
				new SkillIcon(GameUtil.Bind<Transform>(gameObject, "Skill/WpSkills/Skill4").gameObject));
		}


		public void SetCharacterData(int characterCode, CharacterMasteryData masteryData)
		{
			selectedCharacterCode = characterCode;
			SetSkillIcon();
			SetMastery(masteryData);
		}


		private void SetSkillIcon()
		{
			if (skills != null && skills.Count > 0)
			{
				Dictionary<SkillSlotIndex, SkillSlotSet> defaultSkillSet =
					GameDB.skill.GetDefaultSkillSet(selectedCharacterCode, ObjectType.PlayerCharacter);
				foreach (KeyValuePair<SkillSlotIndex, SkillIcon> keyValuePair in skills)
				{
					SkillData skillData = GameDB.skill.GetSkillData(selectedCharacterCode, ObjectType.PlayerCharacter,
						defaultSkillSet[keyValuePair.Key], 1, 0);
					if (skillData == null)
					{
						keyValuePair.Value.Clear();
					}
					else
					{
						keyValuePair.Value.SetSkillData(skillData, keyValuePair.Key);
						keyValuePair.Value.SetName(LnUtil.GetSkillName(skillData.group));
						keyValuePair.Value.SetDesc(LnUtil.GetLobbySkillDesc(skillData.group));
						keyValuePair.Value.SetCost(LnUtil.GetCostText(skillData.CostType, skillData.CostKey,
							skillData.cost));
						if (skillData.cooldown == 0f)
						{
							keyValuePair.Value.SetCooldown("");
						}
						else
						{
							keyValuePair.Value.SetCooldown(Ln.Format("재사용 대기 시간 {0} 초", skillData.cooldown));
						}

						keyValuePair.Value.SetIcon(GameDB.skill.GetSkillIcon(skillData.Icon));
						SkillIcon value = keyValuePair.Value;
						value.onUpEventCallback = (SkillIcon.OnUpEventCallback) Delegate.Combine(
							value.onUpEventCallback,
							new SkillIcon.OnUpEventCallback(delegate(SkillIcon skillIcon)
							{
								OnUpCharacterSkillIcon(skillIcon);
							}));
					}
				}

				if (skills.ContainsKey(SkillSlotIndex.Active1))
				{
					if (skillName != null)
					{
						skillName.text = skills[SkillSlotIndex.Active1].GetName();
					}

					if (skillDesc != null)
					{
						skillDesc.text = skills[SkillSlotIndex.Active1].GetDesc();
					}

					if (skillCost != null)
					{
						skillCost.text = skills[SkillSlotIndex.Active1].GetCost();
					}

					if (skillCooldown != null)
					{
						skillCooldown.text = skills[SkillSlotIndex.Active1].GetCooldown();
					}

					skills[SkillSlotIndex.Active1].Press();
				}
			}
		}


		private void OnUpCharacterSkillIcon(SkillIcon skillIcon)
		{
			skillName.text = skillIcon.GetName();
			skillKeyCode.text = skillIcon.GetKeyCode() == "" ? "" : "[" + skillIcon.GetKeyCode() + "]";
			skillDesc.text = skillIcon.GetDesc();
			skillCost.text = skillIcon.GetCost();
			skillCooldown.text = skillIcon.GetCooldown();
			skillStateCanvas.alpha = 1f;
			wpSkillCanvas.alpha = 0f;
			wpSkillCanvas.interactable = false;
			wpSkillCanvas.blocksRaycasts = false;
			SetVideoPlayer(skillIcon.SkillData.group);
		}


		private void SetMastery(CharacterMasteryData masteryData)
		{
			if (masteryData != null)
			{
				List<MasteryType> list = new List<MasteryType>();
				list.Add(masteryData.weapon1);
				list.Add(masteryData.weapon2);
				list.Add(masteryData.weapon3);
				list.Add(masteryData.weapon4);
				weaponSkillIcons.ForEach(delegate(SkillIcon x) { x.Clear(); });
				int num = 0;
				using (List<MasteryType>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MasteryType mastery = enumerator.Current;
						SkillData skillData = GameDB.skill.GetSkillData(mastery, 1, 0);
						if (skillData == null)
						{
							weaponSkillIcons[num].Clear();
						}
						else
						{
							weaponSkillIcons[num].SetSkillData(skillData, SkillSlotIndex.WeaponSkill);
							weaponSkillIcons[num].SetName(LnUtil.GetSkillName(skillData.group));
							weaponSkillIcons[num].SetDesc(LnUtil.GetLobbySkillDesc(skillData.group));
							weaponSkillIcons[num]
								.SetCost(LnUtil.GetCostText(skillData.CostType, skillData.CostKey, skillData.cost));
							if (skillData.cooldown == 0f)
							{
								weaponSkillIcons[num].SetCooldown("");
							}
							else
							{
								weaponSkillIcons[num].SetCooldown(Ln.Format("재사용 대기 시간 {0} 초", skillData.cooldown));
							}

							weaponSkillIcons[num].SetIcon(GameDB.skill.GetSkillIcon(skillData.Icon));
							SkillIcon skillIcon2 = weaponSkillIcons[num];
							skillIcon2.onUpEventCallback = (SkillIcon.OnUpEventCallback) Delegate.Combine(
								skillIcon2.onUpEventCallback,
								new SkillIcon.OnUpEventCallback(delegate(SkillIcon skillIcon)
								{
									OnUpWeaponSkillIcon(mastery, skillIcon);
								}));
							num++;
						}
					}
				}
			}
		}


		private void OnUpWeaponSkillIcon(MasteryType masteryType, SkillIcon skillIcon)
		{
			skillName.text = skillIcon.GetName();
			skillKeyCode.text = skillIcon.GetKeyCode() == "" ? "" : "[" + skillIcon.GetKeyCode() + "]";
			skillDesc.text = skillIcon.GetDesc();
			wpSkillCost.text = skillIcon.GetCost();
			wpSkillCooldown.text = skillIcon.GetCooldown();
			masteryIcon.SetMastery(masteryType);
			skillStateCanvas.alpha = 0f;
			wpSkillCanvas.alpha = 1f;
			wpSkillCanvas.interactable = true;
			wpSkillCanvas.blocksRaycasts = true;
			SetVideoPlayer(skillIcon.SkillData.group);
		}


		private void SetVideoPlayer(int skillGroupId)
		{
			CharacterSkillVideoData data = GameDB.characterSkillVideoDB.GetData(skillGroupId);
			if (data != null)
			{
				videoPlayerModule.SetData(data.youTubeUrl, data.otherPlatFormUrl, data.volume);
				return;
			}

			videoPlayerModule.Clear();
		}


		private class SkillIcon
		{
				public delegate void OnUpEventCallback(SkillIcon skillIcon);


			private readonly EventTrigger eventTrigger;


			private readonly GameObject gameObject;


			private readonly Image icon;


			private readonly EventTrigger.TriggerEvent onUpEvent = new EventTrigger.TriggerEvent();


			private string cooldown;


			private string cost;


			private string desc;


			private GameInputEvent gameInputEvent;


			private string name;


			public OnUpEventCallback onUpEventCallback;


			private SkillData skillData;


			public SkillIcon(GameObject gameObject)
			{
				this.gameObject = gameObject;
				icon = gameObject.GetComponent<Image>();
				Toggle = gameObject.GetComponent<Toggle>();
				GameUtil.BindOrAdd<EventTrigger>(this.gameObject, ref eventTrigger);
				onUpEvent.AddListener(OnPointerUp);
				eventTrigger.triggers.Add(new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerUp,
					callback = onUpEvent
				});
			}


			public Toggle Toggle { get; }


			public SkillData SkillData => skillData;


			private void OnPointerUp(BaseEventData eventData)
			{
				OnUpEventCallback onUpEventCallback = this.onUpEventCallback;
				if (onUpEventCallback == null)
				{
					return;
				}

				onUpEventCallback(this);
			}


			public void SetSkillData(SkillData skillData, SkillSlotIndex skillSlotIndex)
			{
				this.skillData = skillData;
				switch (skillSlotIndex)
				{
					case SkillSlotIndex.Active1:
						gameInputEvent = GameInputEvent.Active1;
						break;
					case SkillSlotIndex.Active2:
						gameInputEvent = GameInputEvent.Active2;
						break;
					case SkillSlotIndex.Active3:
						gameInputEvent = GameInputEvent.Active3;
						break;
					case SkillSlotIndex.Active4:
						gameInputEvent = GameInputEvent.Active4;
						break;
					case SkillSlotIndex.WeaponSkill:
						gameInputEvent = GameInputEvent.WeaponSkill;
						break;
				}

				gameObject.SetActive(true);
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


			public string GetKeyCode()
			{
				string result = "";
				if (gameInputEvent != GameInputEvent.None)
				{
					result = Singleton<LocalSetting>.inst.GetKeyCode(gameInputEvent).ToString();
				}

				return result;
			}


			public void SetIcon(Sprite sprite)
			{
				icon.sprite = sprite;
			}


			public void Press()
			{
				Toggle.isOn = true;
				OnUpEventCallback onUpEventCallback = this.onUpEventCallback;
				if (onUpEventCallback == null)
				{
					return;
				}

				onUpEventCallback(this);
			}


			public void Clear()
			{
				skillData = null;
				gameInputEvent = GameInputEvent.None;
				name = string.Empty;
				desc = string.Empty;
				cost = string.Empty;
				cooldown = string.Empty;
				onUpEventCallback = null;
				gameObject.SetActive(false);
			}
		}
	}
}