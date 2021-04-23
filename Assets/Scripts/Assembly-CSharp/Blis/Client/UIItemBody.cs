using System.Text;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIItemBody : BaseControl
	{
		private readonly string GreenColor = "<color=#00B000>";


		private readonly string RedColor = "<color=#B00000>";


		private Text itemAbilities;


		private Text itemEffects;


		private Text itemStatus;


		private LayoutElement layoutElement;


		private LayoutGroup layoutGroup;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<LayoutGroup>(gameObject, ref layoutGroup);
			GameUtil.Bind<LayoutElement>(gameObject, ref layoutElement);
			itemStatus = GameUtil.Bind<Text>(gameObject, "ItemStatus");
			itemEffects = GameUtil.Bind<Text>(gameObject, "ItemEffects");
			itemAbilities = GameUtil.Bind<Text>(gameObject, "ItemAbilities");
		}


		public void UpdateUI(Item itemData)
		{
			UpdateCommonUI(itemData, itemData.ItemData);
		}


		public void UpdateUI(ItemData itemData)
		{
			UpdateCommonUI(null, itemData);
		}


		private void UpdateCommonUI(Item item, ItemData itemData)
		{
			layoutElement.ignoreLayout = false;
			transform.localScale = Vector3.one;
			UpdateStatusUI(item, itemData);
			UpdateEffectsUI(itemData);
			UpdateAbilitiesUI(itemData);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, layoutGroup.preferredHeight);
		}


		private void UpdateStatusUI(Item item, ItemData data)
		{
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			itemStatus.gameObject.SetActive(true);
			switch (data.itemType)
			{
				case ItemType.Weapon:
					UpdateWeaponUI(stringBuilder, data);
					goto IL_D8;
				case ItemType.Armor:
					UpdateArmorUI(stringBuilder, data);
					goto IL_D8;
				case ItemType.Special:
					UpdateSpecialUI(stringBuilder, item == null ? data : item.ItemData);
					goto IL_D8;
				case ItemType.Consume:
				{
					int heal;
					int num;
					int num2;
					if (item == null)
					{
						ItemConsumableData subTypeData = data.GetSubTypeData<ItemConsumableData>();
						heal = subTypeData.heal;
						num = subTypeData.hpRecover;
						num2 = subTypeData.spRecover;
					}
					else
					{
						heal = item.ItemData.GetSubTypeData<ItemConsumableData>().heal;
						num = item.GetRecovery(false);
						num2 = item.GetRecovery(true);
					}

					UpdateConsumeUI(stringBuilder, heal, num, num2);
					goto IL_D8;
				}
			}

			itemStatus.gameObject.SetActive(false);
			IL_D8:
			itemStatus.text = stringBuilder.ToString();
			itemStatus.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemStatus.preferredHeight);
		}


		private void UpdateEffectsUI(ItemData itemData)
		{
			string text = Ln.Get(string.Format("Item/Effects/{0}", itemData.code));
			if (string.IsNullOrEmpty(text) || text.Contains("LnErr"))
			{
				itemEffects.text = string.Empty;
				return;
			}

			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			stringBuilder.AppendLine(text);
			itemEffects.text = stringBuilder.ToString();
			itemEffects.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				itemEffects.preferredHeight);
		}


		private void UpdateAbilitiesUI(ItemData itemData)
		{
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			stringBuilder.Append(Ln.Get("최대 수량"));
			stringBuilder.Append(" : ");
			stringBuilder.AppendLine(itemData.stackable.ToString());
			ItemType itemType = itemData.itemType;
			if (itemType - ItemType.Weapon > 1)
			{
				if (itemType == ItemType.Consume)
				{
					stringBuilder.AppendLine(Ln.Get("좌클릭으로 사용 가능"));
				}
			}
			else
			{
				stringBuilder.AppendLine(Ln.Get("좌클릭으로 착용 가능"));
			}

			itemAbilities.text = stringBuilder.ToString();
			itemAbilities.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				itemAbilities.preferredHeight);
		}


		private string GetSignByValue(float value)
		{
			if (value > 0f)
			{
				return "+";
			}

			return "";
		}


		private void AppendStatText(StringBuilder builder, string statName, float value, string suffix = "",
			string forceToColor = null)
		{
			if (value == 0f)
			{
				return;
			}

			if (forceToColor == null)
			{
				builder.Append(value > 0f ? GreenColor : RedColor);
			}
			else
			{
				builder.Append(forceToColor);
			}

			statName = Ln.Get(statName);
			builder.Append(statName);
			builder.Append(" ");
			builder.Append(GetSignByValue(value));
			builder.Append(value);
			builder.AppendLine(suffix);
			builder.Append("</color>");
		}


		private void UpdateWeaponUI(StringBuilder builder, ItemData itemData)
		{
			ItemWeaponData subTypeData = itemData.GetSubTypeData<ItemWeaponData>();
			AppendStatText(builder, "공격력", subTypeData.attackPower);
			AppendStatText(builder, "방어력", subTypeData.defense);
			AppendStatText(builder, "최대 체력", subTypeData.maxHp);
			AppendStatText(builder, "초당 체력 재생", subTypeData.hpRegenRatio * 100f, "%");
			AppendStatText(builder, "초당 체력 재생", subTypeData.hpRegen);
			AppendStatText(builder, "초당 SP 재생", subTypeData.spRegenRatio * 100f, "%");
			AppendStatText(builder, "초당 SP 재생", subTypeData.spRegen);
			AppendStatText(builder, "공격 속도", subTypeData.attackSpeedRatio * 100f, "%");
			AppendStatText(builder, "이동 속도", subTypeData.moveSpeed);
			AppendStatText(builder, "시야", subTypeData.sightRange);
			AppendStatText(builder, "치명타 확률", subTypeData.criticalStrikeChance * 100f, "%");
			AppendStatText(builder, "치명타 피해량", subTypeData.criticalStrikeDamage * 100f, "%");
			AppendStatText(builder, "재사용 대기시간 감소", subTypeData.cooldownReduction * 100f, "%");
			AppendStatText(builder, "생명력 흡수", subTypeData.lifeSteal * 100f, "%");
			AppendStatText(builder, "일반 공격 사거리", subTypeData.attackRange);
			AppendStatText(builder, "기본 공격 적중 시 추가 피해", subTypeData.increaseBasicAttackDamage);
			AppendStatText(builder, "스킬 증폭", subTypeData.increaseSkillDamage);
			AppendStatText(builder, "스킬 증폭", subTypeData.increaseSkillDamageRatio * 100f, "%");
			if (subTypeData.decreaseRecoveryToBasicAttack != 0)
			{
				AppendStatText(builder, "기본 공격 피해 시 모든 치유 효과", -40f, "%", GreenColor);
			}

			if (subTypeData.decreaseRecoveryToSkill != 0)
			{
				AppendStatText(builder, "스킬 피해 시 모든 치유 효과", -40f, "%", GreenColor);
			}
		}


		private void UpdateArmorUI(StringBuilder builder, ItemData itemData)
		{
			ItemArmorData subTypeData = itemData.GetSubTypeData<ItemArmorData>();
			AppendStatText(builder, "공격력", subTypeData.attackPower);
			AppendStatText(builder, "방어력", subTypeData.defense);
			AppendStatText(builder, "최대 체력", subTypeData.maxHp);
			AppendStatText(builder, "최대 SP", subTypeData.maxSp);
			AppendStatText(builder, "초당 체력 재생", subTypeData.hpRegen);
			AppendStatText(builder, "초당 체력 재생", subTypeData.hpRegenRatio * 100f, "%");
			AppendStatText(builder, "초당 SP 재생", subTypeData.spRegen);
			AppendStatText(builder, "초당 SP 재생", subTypeData.spRegenRatio * 100f, "%");
			AppendStatText(builder, "공격 속도", subTypeData.attackSpeedRatio * 100f, "%");
			AppendStatText(builder, "이동 속도", subTypeData.moveSpeed);
			AppendStatText(builder, "시야", subTypeData.sightRange);
			AppendStatText(builder, "치명타 확률", subTypeData.criticalStrikeChance * 100f, "%");
			AppendStatText(builder, "치명타 피해량", subTypeData.criticalStrikeDamage * 100f, "%");
			AppendStatText(builder, "치명타 피해 감소", subTypeData.preventCriticalStrikeDamaged * 100f, "%");
			AppendStatText(builder, "재사용 대기시간 감소", subTypeData.cooldownReduction * 100f, "%");
			AppendStatText(builder, "생명력 흡수", subTypeData.lifeSteal * 100f, "%");
			AppendStatText(builder, "비전투중 이동속도", subTypeData.outOfCombatMoveSpeed);
			AppendStatText(builder, "기본 공격 적중 시 추가 피해", subTypeData.increaseBasicAttackDamage);
			AppendStatText(builder, "기본 공격 피해 감소", subTypeData.preventBasicAttackDamaged);
			AppendStatText(builder, "스킬 증폭", subTypeData.increaseSkillDamage);
			AppendStatText(builder, "스킬 증폭", subTypeData.increaseSkillDamageRatio * 100f, "%");
			AppendStatText(builder, "스킬 피해 감소", subTypeData.preventSkillDamagedRatio * 100f, "%");
			AppendStatText(builder, "일반 공격 사거리", subTypeData.attackRange);
			if (subTypeData.decreaseRecoveryToBasicAttack != 0)
			{
				AppendStatText(builder, "기본 공격 피해 시 모든 치유 효과", -40f, "%", GreenColor);
			}

			if (subTypeData.decreaseRecoveryToSkill != 0)
			{
				AppendStatText(builder, "스킬 피해 시 모든 치유 효과", -40f, "%", GreenColor);
			}
		}


		private void UpdateConsumeUI(StringBuilder builder, float heal, float hpRecover, float spRecover)
		{
			AppendStatText(builder, "체력 회복", heal);
			AppendStatText(builder, "체력 재생", hpRecover);
			AppendStatText(builder, "SP 재생", spRecover);
		}


		private void UpdateSpecialUI(StringBuilder builder, ItemData data)
		{
			ItemSpecialData subTypeData = data.GetSubTypeData<ItemSpecialData>();
			if (subTypeData.summonCode > 0)
			{
				SummonData summonData = GameDB.character.GetSummonData(subTypeData.summonCode);
				if (summonData != null)
				{
					AppendStatText(builder, "트랩 데미지", summonData.attackPower);
				}
			}
		}


		public void Clear()
		{
			layoutElement.ignoreLayout = true;
			transform.localScale = Vector3.zero;
		}
	}
}