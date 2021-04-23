using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UISkillTooltip : UITooltipComponent
	{
		[SerializeField] private UISkillBody uiSkillTitleHeader = default;


		[SerializeField] private UISkillHeader uiSkillHeader = default;


		[SerializeField] private UISkillBody uiSkillBody = default;


		[SerializeField] private UISkillAdd uiSkillAdd = default;

		public void SetWeaponSkill(WeaponType weaponType, SkillData skillData)
		{
			Clear();
			uiSkillTitleHeader.SetDesc(Ln.Get(string.Format("WeaponType/{0}", weaponType)));
			uiSkillHeader.SetSkillDataInPreview(skillData.Icon, LnUtil.GetSkillName(skillData.group), Ln.Get("무기 스킬"));
			uiSkillBody.SetDesc(Ln.Get(string.Format("Skill/LobbyDesc/{0}", skillData.group)));
			transform.localScale = Vector3.one;
		}


		public void SetSkill(SkillData skillData, string keyCode, bool showLevel, float cooldownReduction)
		{
			Clear();
			uiSkillHeader.SetSkillData(skillData, keyCode, showLevel, cooldownReduction);
			uiSkillBody.SetDesc(Ln.Get(string.Format("Skill/LobbyDesc/{0}", skillData.group)));
			GetComponent<VerticalLayoutGroup>().padding.right = skillData.cooldown > 0f ? 10 : 0;
			transform.localScale = Vector3.one;
		}


		public void SetSkill(LocalMovableCharacter self, SkillData skillData, SkillData nextSkillData,
			int evolutionLevel, string keyCode, bool addSkillInfo, float cooldownReduction)
		{
			Clear();
			uiSkillHeader.SetSkillData(skillData, keyCode, true, cooldownReduction);
			uiSkillBody.SetDesc(self.GetSkillTooltip(skillData, evolutionLevel));
			if (addSkillInfo)
			{
				uiSkillAdd.SetSkillData(self.GetNextLevelTooltipParam(skillData, evolutionLevel),
					self.GetNextLevelTooltipValue(skillData, nextSkillData, evolutionLevel));
			}

			GetComponent<VerticalLayoutGroup>().padding.right = skillData.cooldown > 0f ? 10 : 0;
			transform.localScale = Vector3.one;
		}


		public void SetEvolutionSkill(LocalMovableCharacter self, SkillData skillData, int skillLevel, string keyCode,
			float cooldownReduction)
		{
			Clear();
			uiSkillHeader.SetSkillData(skillData, keyCode, true, cooldownReduction);
			uiSkillHeader.SetName(LnUtil.GetSkillEvolutionName(skillData.RepresentGroup), skillLevel, true);
			uiSkillHeader.SetCost(skillData.CostType, skillData.CostKey, skillData.cost);
			uiSkillHeader.SetSkillKey("[" + keyCode + "]");
			uiSkillHeader.SetCooldown(skillData.cooldown);
			uiSkillBody.SetDesc(LnUtil.GetSkillEvolutionDesc(skillData.group));
			GetComponent<VerticalLayoutGroup>().padding.right = skillData.cooldown > 0f ? 10 : 0;
			transform.localScale = Vector3.one;
		}


		public void SetStateEffect(CharacterStateData data, string caster, bool showLevel)
		{
			Clear();
			uiSkillHeader.SetSprite(GameDB.skill.GetSkillIcon(data.GroupData.iconName));
			uiSkillHeader.SetName(LnUtil.GetCharacterStateName(data.group), data.level, showLevel);
			uiSkillHeader.SetCost(string.IsNullOrEmpty(caster) ? "" : Ln.Get("시전자") + " : " + caster);
			uiSkillHeader.SetSkillKey("");
			uiSkillHeader.SetCooldown(0f);
			uiSkillBody.SetDesc(Ln.Get(string.Format("CharacterState/Group/Desc/{0}", data.group)));
			transform.localScale = Vector3.one;
		}


		public void SetStateEffect(CharacterStateData data, string caster, bool showLevel, string customDesc)
		{
			Clear();
			uiSkillHeader.SetSprite(GameDB.skill.GetSkillIcon(data.GroupData.iconName));
			uiSkillHeader.SetName(LnUtil.GetCharacterStateName(data.group), data.level, showLevel);
			uiSkillHeader.SetCost(string.IsNullOrEmpty(caster) ? "" : Ln.Get("시전자") + " : " + caster);
			uiSkillHeader.SetSkillKey("");
			uiSkillHeader.SetCooldown(0f);
			uiSkillBody.SetDesc(customDesc);
			transform.localScale = Vector3.one;
		}


		public override void Clear()
		{
			base.Clear();
			transform.localScale = Vector3.zero;
			uiSkillHeader.Clear();
			uiSkillBody.Clear();
			uiSkillTitleHeader.Clear();
			uiSkillAdd.Clear();
		}
	}
}