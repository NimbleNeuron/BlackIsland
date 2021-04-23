using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UISkillHeader : BaseControl
	{
		private LayoutElement layoutElement;


		private Text skillCoolTime;


		private Text skillCost;


		private Image skillIcon;


		private Text skillKey;


		private Text skillName;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<LayoutElement>(gameObject, ref layoutElement);
			skillIcon = GameUtil.Bind<Image>(gameObject, "IMG_SkillIcon");
			skillName = GameUtil.Bind<Text>(gameObject, "TextZone/Hor1/Name");
			skillKey = GameUtil.Bind<Text>(gameObject, "TextZone/Hor1/Key");
			skillCost = GameUtil.Bind<Text>(gameObject, "TextZone/Hor2/Cost");
			skillCoolTime = GameUtil.Bind<Text>(gameObject, "TextZone/Hor2/CoolTime");
		}


		public void SetSkillData(SkillData skillData, string keyCode, bool showLevel, float cooldownReduction)
		{
			SetSprite(GameDB.skill.GetSkillIcon(skillData.Icon));
			SetName(LnUtil.GetSkillName(skillData.group), skillData.level, showLevel);
			SetCost(skillData.CostType, skillData.CostKey, skillData.cost);
			SetSkillKey(keyCode);
			SetCooldown(skillData.cooldown * (1f - cooldownReduction));
		}


		public void SetSkillDataInPreview(string icon, string name, string desc)
		{
			SetSprite(GameDB.skill.GetSkillIcon(icon));
			skillName.text = name;
			skillKey.text = "";
			skillCost.text = desc;
			skillCoolTime.text = "";
		}


		public void SetSprite(Sprite sprite)
		{
			skillIcon.sprite = sprite;
			layoutElement.ignoreLayout = false;
			transform.localScale = Vector3.one;
		}


		public void SetName(string name, int level, bool showLevel)
		{
			if (showLevel && 0 < level)
			{
				skillName.text = name + " (" + Ln.Format("스킬툴팁레벨", level) + ")";
				return;
			}

			skillName.text = name ?? "";
		}


		public void SetCost(SkillCostType costType, int costKey, int cost)
		{
			skillCost.text = LnUtil.GetCostText(costType, costKey, cost);
		}


		public void SetCost(string desc)
		{
			skillCost.text = desc;
		}


		public void SetSkillKey(string keyCode)
		{
			skillKey.text = keyCode == "" ? keyCode : "[" + keyCode + "]";
		}


		public void SetCooldown(float cooldown)
		{
			if (cooldown > 0f)
			{
				skillCoolTime.text = Ln.Format("재사용 대기 시간 {0} 초", GetCoolTimeText(cooldown));
				return;
			}

			skillCoolTime.text = "";
		}


		private string GetCoolTimeText(float cooldown)
		{
			return (Mathf.RoundToInt(cooldown * 100f) / 100f).ToString();
		}


		public void Clear()
		{
			layoutElement.ignoreLayout = true;
			transform.localScale = Vector3.zero;
		}
	}
}