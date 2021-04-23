using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class SkillSlot : Slot
	{
		private readonly List<SkillEvolutionLevel> skillEvolutionLevels = new List<SkillEvolutionLevel>();


		private readonly List<Image> skillLevels = new List<Image>();


		private ParticleSystem effect;


		private ParticleSystem evolutionEffect;


		private bool isEnable;


		private Image keyBg;


		private LnText keyText;


		private Image lockBg;


		private Image lockIcon;


		private bool playEvolutionEffect;


		private bool playUpgradeEffect;


		private SkillSlotIndex skillSlotIndex;


		private LnText skillStackText;


		private ParticleSystem upgradeEffect;


		private float waitingForCast;


		public bool IsEnable => isEnable;


		public void LateUpdate()
		{
			if (playUpgradeEffect)
			{
				if (!playEvolutionEffect && !upgradeEffect.isPlaying)
				{
					upgradeEffect.Play();
				}

				playUpgradeEffect = false;
			}

			if (playEvolutionEffect)
			{
				if (upgradeEffect.isPlaying)
				{
					upgradeEffect.Stop();
				}

				evolutionEffect.Play();
				playEvolutionEffect = false;
			}
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			keyBg = GameUtil.Bind<Image>(gameObject, "KeyCodeBg");
			keyText = GameUtil.Bind<LnText>(gameObject, "KeyCodeText");
			keyBg.gameObject.SetActive(false);
			keyText.gameObject.SetActive(false);
			lockBg = GameUtil.Bind<Image>(gameObject, "LockBg");
			Color color = lockBg.color;
			color.a = 0f;
			lockBg.color = color;
			lockIcon = GameUtil.Bind<Image>(gameObject, "Lock/Icon");
			GameUtil.Bind<Transform>(gameObject, "Level/Levels").GetComponentsInChildren<Image>(skillLevels);
			GameUtil.Bind<Transform>(gameObject, "EvolutionLevel")
				.GetComponentsInChildren<SkillEvolutionLevel>(skillEvolutionLevels);
			effect = GameUtil.Bind<ParticleSystem>(gameObject, "FX_BI_UIGlow_01");
			upgradeEffect = GameUtil.Bind<ParticleSystem>(gameObject, "UpgradeEffect");
			evolutionEffect = GameUtil.Bind<ParticleSystem>(gameObject, "EvolutionEffect");
			skillStackText = GameUtil.Bind<LnText>(gameObject, "StackCount");
			skillStackText.gameObject.SetActive(true);
			Cooldown.OnSkill = PlaySound;
		}


		public void SetSkillSlotIndex(SkillSlotIndex skillSlotIndex)
		{
			this.skillSlotIndex = skillSlotIndex;
		}


		public void SetIcon(string iconName)
		{
			if (image != null)
			{
				image.sprite = GameDB.skill.GetSkillIcon(iconName);
			}
		}


		public void SetKeyCode(string key, bool bgActiveFlag = true)
		{
			if (keyBg != null)
			{
				keyBg.gameObject.SetActive(bgActiveFlag);
			}

			if (keyText != null)
			{
				keyText.gameObject.SetActive(true);
				keyText.text = key;
			}
		}


		public void SetSkillLevel(int skillLevel)
		{
			for (int i = 0; i < skillLevels.Count; i++)
			{
				skillLevels[i].gameObject.SetActive(i < skillLevel);
			}
		}


		public void SetSkillEvolutionLevel(int skillLevel, int maxLevel)
		{
			for (int i = 0; i < skillEvolutionLevels.Count; i++)
			{
				if (maxLevel == 0)
				{
					skillEvolutionLevels[i].Active(false);
				}
				else
				{
					skillEvolutionLevels[i].Active(i < maxLevel);
					skillEvolutionLevels[i].Enable(i < skillLevel);
				}
			}
		}


		public void Enable()
		{
			if (cooldown != null && !cooldown.RemainCooldown() || !isEnable)
			{
				SetSpriteColor(Color.white);
			}

			isEnable = true;
		}


		public void Disable()
		{
			isEnable = false;
			SetSpriteColor(Color.gray);
		}


		public override void SetLock(bool isLock)
		{
			bool? isLock2 = this.isLock;
			if ((isLock2.GetValueOrDefault() == isLock) & (isLock2 != null))
			{
				return;
			}

			base.SetLock(isLock);
			if (lockIcon != null)
			{
				lockIcon.enabled = isLock;
			}

			if (lockBg != null)
			{
				Color color = lockBg.color;
				color.a = isLock ? 0.75f : 0f;
				lockBg.color = color;
			}
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (!eventData.dragging)
			{
				SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.ShowSkillTooltip(skillSlotIndex);
				SingletonMonoBehaviour<PlayerController>.inst.ShowSkillIndicator(skillSlotIndex);
			}
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			SingletonMonoBehaviour<PlayerController>.inst.HideSkillIndicator();
		}


		public void PlayEffect()
		{
			SetSpriteColor(Color.white);
			effect.Play();
		}


		private void PlaySound()
		{
			SkillId skillId = SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.GetSkillData(skillSlotIndex)
				.SkillId;
			if (!GameConstants.SKILL_COOLDOWN_SOUND.ContainsKey(skillId))
			{
				return;
			}

			Singleton<SoundControl>.inst.PlayUISound(GameConstants.SKILL_COOLDOWN_SOUND[skillId],
				ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
		}


		public void PlayUpgradeEffect()
		{
			playUpgradeEffect = true;
		}


		public void PlayEvolutionEffect()
		{
			playEvolutionEffect = true;
		}


		public void DrawSkillStack(int stack)
		{
			if (skillStackText != null)
			{
				if (stack <= 0)
				{
					skillStackText.text = null;
					return;
				}

				skillStackText.text = stack.ToString();
			}
		}


		public override void SetSprite(Sprite sprite)
		{
			if (image != null)
			{
				image.sprite = sprite;
			}
		}
	}
}