using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class TeamMemberSlot : BaseControl
	{
		private Image alertMask;


		private ColorTweener alertTweener;


		private ColorTweener battelFrameColorTweener;


		private Image battleFrame;


		private Coroutine battleRoutine;


		private CanvasGroup canvasGroupInBattle;


		private float cooldown;


		private Image emotionIcon;


		private ColorTweener emotionIconTweener;


		private ITeamMemberSlotEventListener eventListener;


		private Image hpBar;


		private bool isDyingCondition;


		private bool isObserving;


		private Text level;


		private float maxCooldown;


		private int objectId;


		private float playBattleSoundTime;


		private Image portrait;


		private Image skillCooldown;


		private Image skillIcon;


		private Image spBar;


		private Image status;


		private Image teamColor;


		private int teamIndex;


		private Image unLinkBg;


		public int ObjectId => objectId;


		public int TeamIndex => teamIndex;


		private void Update()
		{
			if (0f < cooldown && 0f < maxCooldown)
			{
				float deltaTime = Time.deltaTime;
				cooldown -= deltaTime;
				SetUltimateSkillCooldown(cooldown, maxCooldown);
				if (cooldown <= 0f)
				{
					SetUltimateSkillReady(true);
				}
			}
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			portrait = GameUtil.Bind<Image>(gameObject, "Portrait");
			teamColor = GameUtil.Bind<Image>(gameObject, "TeamColor");
			canvasGroupInBattle = GameUtil.Bind<CanvasGroup>(gameObject, "InBattle");
			level = GameUtil.Bind<Text>(gameObject, "TeamColor/Level");
			hpBar = GameUtil.Bind<Image>(gameObject, "Gauge/HpBar/Forward");
			spBar = GameUtil.Bind<Image>(gameObject, "Gauge/SpBar/Forward");
			skillIcon = GameUtil.Bind<Image>(gameObject, "UltimateSkill/Icon");
			skillCooldown = GameUtil.Bind<Image>(gameObject, "UltimateSkill/Cooldown");
			skillCooldown.fillAmount = 0f;
			alertMask = GameUtil.Bind<Image>(gameObject, "Alert");
			alertMask.enabled = false;
			alertTweener = alertMask.GetComponent<ColorTweener>();
			alertTweener.StopAnimation();
			alertMask.color = alertTweener.from;
			battleFrame = GameUtil.Bind<Image>(gameObject, "BattleFrame");
			battleFrame.enabled = false;
			battelFrameColorTweener = battleFrame.GetComponent<ColorTweener>();
			battelFrameColorTweener.StopAnimation();
			battleFrame.color = battelFrameColorTweener.from;
			emotionIcon = GameUtil.Bind<Image>(gameObject, "EmotionIcon");
			emotionIcon.enabled = false;
			emotionIconTweener = emotionIcon.GetComponent<ColorTweener>();
			emotionIconTweener.OnAnimationFinish += delegate { emotionIcon.enabled = false; };
			status = GameUtil.Bind<Image>(gameObject, "Status");
			unLinkBg = GameUtil.Bind<Image>(gameObject, "UnlinkBg");
			Connected();
		}


		public void Show()
		{
			gameObject.SetActive(true);
		}


		public void Hide()
		{
			gameObject.SetActive(false);
		}


		public void SetTeamIndex(int teamIndex)
		{
			this.teamIndex = teamIndex;
		}


		public void SetEventListener(ITeamMemberSlotEventListener eventListener)
		{
			this.eventListener = eventListener;
		}


		public void Init(LocalPlayerCharacter teamMember)
		{
			objectId = teamMember.ObjectId;
			SetPortrait(teamMember.CharacterCode);
			SetTeamSlot(teamMember.TeamSlot);
			SetLevel(teamMember.Status.Level);
			SetHpBar(teamMember.Status.Hp, teamMember.Stat.MaxHp);
			SetSpBar(teamMember.Status.Sp, teamMember.Stat.MaxSp);
			DrawSkillIcon(teamMember);
			SkillSlotSet? skillSlotSet = teamMember.GetSkillSlotSet(SkillSlotIndex.Active4);
			if (skillSlotSet == null)
			{
				SetUltimateSkillReady(false);
			}
			else
			{
				int skillLevel = teamMember.GetSkillLevel(SkillSlotIndex.Active4);
				bool flag = false;
				if (0 < skillLevel)
				{
					flag = teamMember.CheckCooldown(skillSlotSet.Value);
				}

				SetUltimateSkillReady(flag);
				if (0 < skillLevel && !flag)
				{
					SetUltimateSkillCooldown(teamMember.GetSkillCooldown(skillSlotSet.Value),
						teamMember.GetSkillMaxCooldown(skillSlotSet.Value));
				}
			}

			isObserving = teamMember.IsObserving;
			isDyingCondition = teamMember.IsDyingCondition;
			if (teamMember.IsAlive)
			{
				if (teamMember.IsDyingCondition)
				{
					DyingCondition();
				}
				else
				{
					Alive();
				}
			}
			else
			{
				Dead();
			}

			if (teamMember.IsDisconnected)
			{
				Disconnected();
				return;
			}

			if (teamMember.IsObserving)
			{
				Observing();
				return;
			}

			Connected();
		}


		private void SetPortrait(int characterCode)
		{
			portrait.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterCommunitySprite(characterCode);
		}


		private void SetTeamSlot(int teamSlot)
		{
			teamColor.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(string.Format("Ico_Map_PointPin_{0:D2}",
					teamSlot));
		}


		public void SetLevel(int level)
		{
			this.level.text = level.ToString();
		}


		public void SetHpBar(int hp, int maxHp)
		{
			if (isDyingCondition)
			{
				maxHp = 500;
			}

			maxHp = Mathf.Max(1, maxHp);
			float fillAmount = Mathf.Clamp(hp / (float) maxHp, 0f, 1f);
			hpBar.fillAmount = fillAmount;
		}


		public void SetSpBar(int sp, int maxSp)
		{
			maxSp = Mathf.Max(1, maxSp);
			spBar.fillAmount = Mathf.Clamp(sp / (float) maxSp, 0f, 1f);
		}


		public void SetUltimateSkillReady(bool ready)
		{
			skillIcon.color = ready ? Color.white : Color.gray;
			skillCooldown.enabled = !ready;
		}


		public void SetUltimateSkillCooldown(float cooldown, float maxCooldown)
		{
			maxCooldown = Mathf.Max(1f, maxCooldown);
			this.cooldown = cooldown;
			this.maxCooldown = maxCooldown;
			skillCooldown.fillAmount = Mathf.Clamp(cooldown / maxCooldown, 0f, 1f);
		}


		public void DrawSkillIcon(LocalPlayerCharacter character)
		{
			SkillData skillData = character.GetSkillData(SkillSlotIndex.Active4);
			if (skillData != null)
			{
				skillIcon.sprite = GameDB.skill.GetSkillIcon(skillData.Icon);
			}
		}


		public void SetEmotionIcon(EmotionIconData emotionIconData)
		{
			emotionIcon.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetEmotionSprite(emotionIconData.sprite);
			emotionIcon.SetNativeSize();
			emotionIconTweener.StopAnimation();
			emotionIconTweener.PlayAnimation();
			emotionIcon.enabled = true;
		}


		public void AddUltimateSkillCooldown(float addCooldown)
		{
			cooldown += addCooldown;
		}


		public void HoldCooldownUltimateSkill()
		{
			skillCooldown.fillAmount = 1f;
		}


		public void Alive()
		{
			isDyingCondition = false;
			alertMask.enabled = false;
			alertTweener.StopAnimation();
			hpBar.color = GameConstants.TeamMode.MY_COLOR;
		}


		public void DyingCondition()
		{
			isDyingCondition = true;
			alertMask.enabled = true;
			alertTweener.PlayAnimation();
			hpBar.color = GameConstants.TeamMode.DYINGCONDITION_COLOR;
		}


		public void Dead()
		{
			alertMask.enabled = true;
			alertTweener.StopAnimation();
			alertMask.color = alertTweener.from;
		}


		public void Observing()
		{
			status.enabled = true;
			status.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Observe");
			unLinkBg.gameObject.SetActive(false);
		}


		public void Disconnected()
		{
			status.enabled = true;
			status.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Ico_Unlink");
			unLinkBg.gameObject.SetActive(true);
		}


		public void Connected()
		{
			if (isObserving)
			{
				Observing();
				return;
			}

			status.enabled = false;
			unLinkBg.gameObject.SetActive(false);
		}


		public void SetInBattleUI(bool isCombat)
		{
			canvasGroupInBattle.alpha = 1f;
			canvasGroupInBattle.gameObject.SetActive(isCombat);
		}


		public void SetInBattleCharacterUI()
		{
			if (battleRoutine != null)
			{
				StopCoroutine(battleRoutine);
				battleRoutine = null;
			}

			battleRoutine = this.StartThrowingCoroutine(CorBattleCharacterUI(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][CorBattleCharacterUI] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator CorBattleCharacterUI()
		{
			PlayBattleSound();
			canvasGroupInBattle.alpha = 1f;
			canvasGroupInBattle.gameObject.SetActive(true);
			battleFrame.enabled = true;
			battelFrameColorTweener.PlayAnimation();
			yield return new WaitForSeconds(5f);
			canvasGroupInBattle.alpha = 1f;
			canvasGroupInBattle.gameObject.SetActive(false);
			battleFrame.enabled = false;
			battelFrameColorTweener.StopAnimation();
			battleRoutine = null;
		}


		private void PlayBattleSound()
		{
			float time = Time.time;
			if (time > playBattleSoundTime)
			{
				Singleton<SoundControl>.inst.PlayUISound("TeamHit",
					ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam);
			}

			playBattleSoundTime = time + 10f;
		}


		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			if (eventData.dragging)
			{
				return;
			}

			PointerEventData.InputButton button = eventData.button;
			if (button != PointerEventData.InputButton.Left)
			{
				if (button != PointerEventData.InputButton.Right)
				{
					return;
				}

				ITeamMemberSlotEventListener teamMemberSlotEventListener = eventListener;
				if (teamMemberSlotEventListener == null)
				{
					return;
				}

				teamMemberSlotEventListener.OnSlotRightClick(this);
			}
			else if (eventData.clickCount == 1)
			{
				ITeamMemberSlotEventListener teamMemberSlotEventListener2 = eventListener;
				if (teamMemberSlotEventListener2 == null)
				{
					return;
				}

				teamMemberSlotEventListener2.OnSlotLeftClick(this);
			}
			else if (eventData.clickCount == 2)
			{
				eventData.clickCount = 0;
				ITeamMemberSlotEventListener teamMemberSlotEventListener3 = eventListener;
				if (teamMemberSlotEventListener3 == null)
				{
					return;
				}

				teamMemberSlotEventListener3.OnSlotDoubleClick(this);
			}
		}


		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				ITeamMemberSlotEventListener teamMemberSlotEventListener = eventListener;
				if (teamMemberSlotEventListener == null)
				{
					return;
				}

				teamMemberSlotEventListener.OnPointerDown(this);
			}
		}


		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			ITeamMemberSlotEventListener teamMemberSlotEventListener = eventListener;
			if (teamMemberSlotEventListener == null)
			{
				return;
			}

			teamMemberSlotEventListener.OnPointerUp(this);
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			ITeamMemberSlotEventListener teamMemberSlotEventListener = eventListener;
			if (teamMemberSlotEventListener == null)
			{
				return;
			}

			teamMemberSlotEventListener.OnPointerEnter(this);
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			ITeamMemberSlotEventListener teamMemberSlotEventListener = eventListener;
			if (teamMemberSlotEventListener != null)
			{
				teamMemberSlotEventListener.OnPointerExit(this);
			}

			MonoBehaviourInstance<Tooltip>.inst.Hide(GetParentWindow());
		}
	}
}