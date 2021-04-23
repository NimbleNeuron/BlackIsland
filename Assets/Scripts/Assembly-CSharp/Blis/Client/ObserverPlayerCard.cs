using System;
using System.Collections;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ObserverPlayerCard : BaseUI
	{
		private Image alertMask;


		private ColorTweener alertTweener;


		private RectTransform battleFrame;


		private Coroutine battleRoutine;


		private Button btn;


		private RectTransform characterCard;


		private RectTransform content;


		private float cooldown;


		private Image hpBar;


		private bool isDyingCondition;


		private Text killCount;


		private Text level;


		private GameObject lost;


		private float maxCooldown;


		private Text nickname;


		private int objectId;


		private Action<ObserverPlayerCard> onClickEvent;


		private Image portrait;


		private GameObject select;


		private Image skillCooldown;


		private Image skillIcon;


		private Image spBar;


		private Image teamColor;


		private int teamNumber;


		private int teamSlot;


		public int ObjectId => objectId;


		public int TeamNumber => teamNumber;


		public int TeamSlot => teamSlot;


		public bool IsDead => lost.gameObject.activeSelf;


		private void LateUpdate()
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
			content = GameUtil.Bind<RectTransform>(this.gameObject, "Content");
			characterCard = GameUtil.Bind<RectTransform>(content.gameObject, "CharacterCard");
			GameObject gameObject = characterCard.gameObject;
			btn = GameUtil.Bind<Button>(gameObject, "Bg");
			btn.onClick.AddListener(OnClick);
			teamColor = GameUtil.Bind<Image>(gameObject, "LevelBg");
			level = GameUtil.Bind<Text>(gameObject, "LevelBg/Level");
			nickname = GameUtil.Bind<Text>(gameObject, "NameBg/UserNickname");
			portrait = GameUtil.Bind<Image>(gameObject, "Character");
			killCount = GameUtil.Bind<Text>(this.gameObject, "Kill/Txt_Count");
			alertMask = GameUtil.Bind<Image>(gameObject, "Alert");
			alertMask.enabled = false;
			alertTweener = alertMask.GetComponent<ColorTweener>();
			alertTweener.StopAnimation();
			alertMask.color = alertTweener.from;
			hpBar = GameUtil.Bind<Image>(gameObject, "Gauge/HpBar/Forward");
			spBar = GameUtil.Bind<Image>(gameObject, "Gauge/SpBar/Forward");
			skillIcon = GameUtil.Bind<Image>(gameObject, "UltimateSkill/Icon");
			skillCooldown = GameUtil.Bind<Image>(gameObject, "UltimateSkill/Cooldown");
			skillCooldown.fillAmount = 0f;
			battleFrame = GameUtil.Bind<RectTransform>(gameObject, "BattleFrame");
			lost = GameUtil.Bind<Transform>(gameObject, "Lost").gameObject;
			lost.SetActive(false);
			select = GameUtil.Bind<Transform>(this.gameObject, "Select").gameObject;
			select.SetActive(false);
		}


		public void Show()
		{
			gameObject.SetActive(true);
		}


		public void Hide()
		{
			gameObject.SetActive(false);
		}


		public void SetClickEvent(Action<ObserverPlayerCard> onClickEvent)
		{
			this.onClickEvent = onClickEvent;
		}


		public void Init(LocalPlayerCharacter character)
		{
			if (character == null)
			{
				objectId = 0;
				teamNumber = 0;
				teamSlot = 0;
				return;
			}

			objectId = character.ObjectId;
			teamNumber = character.TeamNumber;
			teamSlot = character.TeamSlot;
			isDyingCondition = character.IsDyingCondition;
			SetLevel(character.Status.Level);
			SetHpBar(character.Status.Hp, character.Stat.MaxHp);
			SetSpBar(character.Status.Sp, character.Stat.MaxSp);
			SetPortrait(character.CharacterCode);
			SetNickname(character.Nickname);
			SetKillCount(character.Status.PlayerKill);
			SetUltimate(character);
			SelectTeam(false);
			SetInBattle(false);
			if (!character.IsAlive)
			{
				Dead();
				return;
			}

			if (isDyingCondition)
			{
				DyingCondition();
				return;
			}

			Alive();
		}


		public void SetLevel(int level)
		{
			this.level.text = level.ToString();
		}


		private void SetPortrait(int characterCode)
		{
			portrait.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(characterCode);
		}


		private void SetNickname(string nickname)
		{
			this.nickname.text = nickname;
		}


		public void SetKillCount(int killCount)
		{
			this.killCount.text = killCount.ToString();
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


		private void SetUltimate(LocalPlayerCharacter character)
		{
			DrawUltimateIcon(character);
			SkillSlotSet? skillSlotSet = character.GetSkillSlotSet(SkillSlotIndex.Active4);
			if (skillSlotSet == null)
			{
				SetUltimateSkillReady(false);
				return;
			}

			int skillLevel = character.GetSkillLevel(SkillSlotIndex.Active4);
			bool flag = false;
			if (0 < skillLevel)
			{
				flag = character.CheckCooldown(skillSlotSet.Value);
			}

			SetUltimateSkillReady(flag);
			if (0 < skillLevel && !flag)
			{
				SetUltimateSkillCooldown(character.GetSkillCooldown(skillSlotSet.Value),
					character.GetSkillMaxCooldown(skillSlotSet.Value));
			}
		}


		public void DrawUltimateIcon(LocalPlayerCharacter character)
		{
			SkillData skillData = character.GetSkillData(SkillSlotIndex.Active4);
			if (skillData != null)
			{
				skillIcon.sprite = GameDB.skill.GetSkillIcon(skillData.Icon);
			}
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
			hpBar.fillAmount = 0f;
			spBar.fillAmount = 0f;
			if (!lost.activeSelf)
			{
				lost.SetActive(true);
			}
		}


		public void SetInBattle(bool isCombat) { }


		public void SetInBattleCharacter()
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
			battleFrame.gameObject.SetActive(true);
			yield return new WaitForSeconds(5f);
			battleFrame.gameObject.SetActive(false);
			battleRoutine = null;
		}


		public void SelectTeam(bool isSelect)
		{
			teamColor.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite(isSelect
				? string.Format("Ico_Map_PointPin_{0:D2}", teamSlot)
				: "Ico_Map_PointPin_04");
		}


		public void Select()
		{
			if (!select.activeSelf)
			{
				select.SetActive(true);
			}
		}


		public void Deselect()
		{
			if (select.activeSelf)
			{
				select.SetActive(false);
			}
		}


		public void SetUltimateSkillCooldown(float cooldown, float maxCooldown)
		{
			maxCooldown = Mathf.Max(1f, maxCooldown);
			this.cooldown = cooldown;
			this.maxCooldown = maxCooldown;
			skillCooldown.fillAmount = Mathf.Clamp(cooldown / maxCooldown, 0f, 1f);
		}


		public void AddUltimateSkillCooldown(float addCooldown)
		{
			cooldown += addCooldown;
		}


		public void HoldCooldownUltimateSkill()
		{
			skillCooldown.fillAmount = 1f;
		}


		public void SetUltimateSkillReady(bool ready)
		{
			skillIcon.color = ready ? Color.white : Color.gray;
			skillCooldown.enabled = !ready;
		}


		private void OnClick()
		{
			Action<ObserverPlayerCard> action = onClickEvent;
			if (action == null)
			{
				return;
			}

			action(this);
		}
	}
}