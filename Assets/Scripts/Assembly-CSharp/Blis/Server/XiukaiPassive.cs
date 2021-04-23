using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.XiukaiPassive)]
	public class XiukaiPassive : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeMakeItemProcessEvent = (Func<WorldPlayerCharacter, Item, Item>) Delegate.Combine(
				inst.OnBeforeMakeItemProcessEvent, new Func<WorldPlayerCharacter, Item, Item>(OnBeforeMakeItem));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterMakeItemProcessEvent = (Func<WorldPlayerCharacter, Item, Item>) Delegate.Combine(
				inst2.OnAfterMakeItemProcessEvent, new Func<WorldPlayerCharacter, Item, Item>(OnAfterMakeItem));
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return null;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeMakeItemProcessEvent = (Func<WorldPlayerCharacter, Item, Item>) Delegate.Remove(
				inst.OnBeforeMakeItemProcessEvent, new Func<WorldPlayerCharacter, Item, Item>(OnBeforeMakeItem));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterMakeItemProcessEvent = (Func<WorldPlayerCharacter, Item, Item>) Delegate.Remove(
				inst2.OnAfterMakeItemProcessEvent, new Func<WorldPlayerCharacter, Item, Item>(OnAfterMakeItem));
		}

		
		public Item OnBeforeMakeItem(WorldPlayerCharacter maker, Item makeItem)
		{
			if (maker.ObjectId != Caster.ObjectId)
			{
				return makeItem;
			}

			if (makeItem == null || makeItem.ItemData.itemType != ItemType.Consume)
			{
				return makeItem;
			}

			if (makeItem.ItemData.GetSubTypeData<ItemConsumableData>() == null)
			{
				return makeItem;
			}

			makeItem.AddRecoveryItem(Singleton<XiukaiSkillPassiveData>.inst.AddRecoveryPercent);
			makeItem.SetItemSpecialType(ItemMadeType.XiukaiMade);
			return makeItem;
		}

		
		public Item OnAfterMakeItem(WorldPlayerCharacter maker, Item makeItem)
		{
			if (maker.ObjectId != Caster.ObjectId)
			{
				return makeItem;
			}

			if (makeItem == null || makeItem.ItemData.itemType != ItemType.Consume)
			{
				return makeItem;
			}

			ItemConsumableData subTypeData = makeItem.ItemData.GetSubTypeData<ItemConsumableData>();
			if (subTypeData == null)
			{
				return makeItem;
			}

			int stack = 0;
			switch (subTypeData.itemGrade)
			{
				case ItemGrade.Uncommon:
					stack = Singleton<XiukaiSkillPassiveData>.inst.UncommonStack[SkillLevel];
					break;
				case ItemGrade.Rare:
					stack = Singleton<XiukaiSkillPassiveData>.inst.RareStack[SkillLevel];
					break;
				case ItemGrade.Epic:
					stack = Singleton<XiukaiSkillPassiveData>.inst.EpicStack[SkillLevel];
					break;
				case ItemGrade.Legend:
					stack = Singleton<XiukaiSkillPassiveData>.inst.LegendStack[SkillLevel];
					break;
			}

			PlaySkillAction(Caster, Info.skillData.PassiveSkillId, (int) subTypeData.itemGrade);
			AddState(Caster, Singleton<XiukaiSkillPassiveData>.inst.BuffState, stack);
			return makeItem;
		}
	}
}