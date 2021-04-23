using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ActionCostData
	{
		public readonly string castingAnimTrigger;


		public readonly int code;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly EffectCancelCondition effectCancelCondition;


		public readonly int sp;


		public readonly float time;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly CastingActionType type;

		[JsonConstructor]
		public ActionCostData(int code, CastingActionType type, int sp, float time, string castingAnimTrigger,
			EffectCancelCondition effectCancelCondition)
		{
			this.code = code;
			this.type = type;
			this.sp = sp;
			this.time = time;
			this.castingAnimTrigger = castingAnimTrigger;
			this.effectCancelCondition = effectCancelCondition;
		}


		public static CastingActionType GetActionCostType(bool rest)
		{
			if (!rest)
			{
				return CastingActionType.ToBattle;
			}

			return CastingActionType.ToRest;
		}


		public static CastingActionType GetActionCostType(ItemData itemData)
		{
			CastingActionType result = CastingActionType.None;
			switch (itemData.itemGrade)
			{
				case ItemGrade.Common:
					result = CastingActionType.CraftCommon;
					break;
				case ItemGrade.Uncommon:
					result = CastingActionType.CraftUnCommon;
					break;
				case ItemGrade.Rare:
					result = CastingActionType.CraftRare;
					break;
				case ItemGrade.Epic:
					result = CastingActionType.CraftEpic;
					break;
				case ItemGrade.Legend:
					result = CastingActionType.CraftLegend;
					break;
			}

			return result;
		}


		public static string GetAnimationTriggerName(CastingActionType type, int extraParam)
		{
			string result = "";
			switch (type)
			{
				case CastingActionType.ToRest:
				case CastingActionType.Resurrect:
					return "sleep";
				case CastingActionType.ToSearch:
					return result;
				case CastingActionType.ToBattle:
					return "awake";
				case CastingActionType.AirSupplyOpen:
					return "airDropOpen";
				case CastingActionType.BoxOpen:
					return "collect";
				case CastingActionType.CollectibleOpenWood:
				case CastingActionType.Resurrected:
					return "";
				case CastingActionType.CraftCommon:
				case CastingActionType.CraftUnCommon:
				case CastingActionType.CraftRare:
				case CastingActionType.CraftEpic:
				case CastingActionType.CraftLegend:
					return "craft";
				case CastingActionType.Hyperloop:
					return "craft";
			}

			Log.W("CastingAnimation[{0}] is not defined", "type");
			return "";
		}


		public string GetActionCastingName()
		{
			string result = "";
			CastingActionType castingActionType = type;
			switch (castingActionType)
			{
				case CastingActionType.ToRest:
					return "휴식 준비 중";
				case CastingActionType.ToSearch:
					return result;
				case CastingActionType.ToBattle:
					return "휴식 해제 중";
				case CastingActionType.AirSupplyOpen:
				case CastingActionType.BoxOpen:
					return "내용물 확인 중";
				case CastingActionType.CollectibleOpen:
				case CastingActionType.CollectibleOpenWater:
				case CastingActionType.CollectibleOpenStone:
				case CastingActionType.CollectibleOpenSeaFish:
				case CastingActionType.CollectibleOpenFreshWaterFish:
				case CastingActionType.CollectibleOpenPotato:
				case CastingActionType.CollectibleOpenTreeOfLife:
					break;
				case CastingActionType.CollectibleOpenWood:
					return "자원 채취 중";
				case CastingActionType.CraftCommon:
				case CastingActionType.CraftUnCommon:
				case CastingActionType.CraftRare:
				case CastingActionType.CraftEpic:
				case CastingActionType.CraftLegend:
					return "제작 중";
				default:
					if (castingActionType == CastingActionType.Resurrect)
					{
						return "부활 중";
					}

					if (castingActionType == CastingActionType.Resurrected)
					{
						return "부활 중";
					}

					break;
			}

			throw new ArgumentOutOfRangeException("type", type, null);
		}
	}
}