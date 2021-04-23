using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class Tooltip : MonoBehaviourInstance<Tooltip>
	{
		public enum Pivot
		{
			LeftBottom,

			LeftTop,

			RightBottom,

			RightTop
		}


		public enum TooltipMode
		{
			Tracking,

			Fixed
		}


		public enum TooltipPosition
		{
			None,

			EquipmentPosition,

			InventoryPosition,

			FavoritePosition,

			NavigationPosition,

			TargetInfoPosition,

			SkillInfoCenterPosition,

			LobbyMasterSkillInfoPosition,

			LobbyCharacterSkillInfoPosition,

			LobbyMasteryPosition
		}


		private const string Newline = "\n";


		[SerializeField] private List<Transform> tooltipPositions = default;


		private CanvasGroup _canvasGroup;


		private UITooltipComponent currentTooltipComponent;


		private BaseUI owner;


		private Coroutine showCanvas;


		private UIInfoLabel uiInfoLabel;


		private UIItemTooltip uiItemTooltip;


		private UILabelContainerTooltip uiLabelContainerTooltip;


		private UIMasteryInfo uiLobbyMasteryInfo;


		private UIMasteryInfo uiMasteryInfo;


		private UISkillTooltip uiSkillTooltip;

		protected override void _Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;
			uiItemTooltip = GameUtil.Bind<UIItemTooltip>(gameObject, "ItemSet");
			uiSkillTooltip = GameUtil.Bind<UISkillTooltip>(gameObject, "SkillGroup");
			uiMasteryInfo = GameUtil.Bind<UIMasteryInfo>(gameObject, "MasteryTooltip");
			uiInfoLabel = GameUtil.Bind<UIInfoLabel>(gameObject, "Info");
			uiLobbyMasteryInfo = GameUtil.Bind<UIMasteryInfo>(gameObject, "LobbyMasteryTooltip");
			uiLabelContainerTooltip = GameUtil.Bind<UILabelContainerTooltip>(gameObject, "LabelContainer");
			currentTooltipComponent = null;
			Hide();
		}


		public void SetItem(Item item, int amount, bool showCompare)
		{
			Clean();
			if (item == null)
			{
				Log.E("[ToolTip] SetItem(Item, int, bool) : Item is Null");
				return;
			}

			uiItemTooltip.SetItem(item, amount, showCompare);
			currentTooltipComponent = uiItemTooltip;
		}


		public void SetItem(ItemData itemData, int amount, bool showCompare)
		{
			Clean();
			if (itemData == null)
			{
				Log.E("[ToolTip] SetItem(ItemData, int, bool) : ItemData is Null");
				return;
			}

			uiItemTooltip.SetItem(itemData, amount, showCompare);
			currentTooltipComponent = uiItemTooltip;
		}


		public void SetWeaponSkill(WeaponType weaponType, SkillData skillData)
		{
			Clean();
			if (skillData == null)
			{
				Log.E("[ToolTip] SetWeaponSkill(WeaponType, SkillData) : SkillData is Null");
				return;
			}

			uiSkillTooltip.SetWeaponSkill(weaponType, skillData);
			currentTooltipComponent = uiSkillTooltip;
		}


		public void SetSkill(SkillData skillData, string keyCode, bool showLevel, float cooldownReduction = 0f)
		{
			Clean();
			if (skillData == null)
			{
				Log.E("[ToolTip] SetSkill(SkillData, string, bool) : SkillData is Null");
				return;
			}

			uiSkillTooltip.SetSkill(skillData, keyCode, showLevel, cooldownReduction);
			currentTooltipComponent = uiSkillTooltip;
		}


		public void SetSkill(LocalMovableCharacter self, SkillData skillData, SkillData nextSkillData,
			int evolutionLevel, string keyCode, bool addSkillInfo, float cooldownReduction)
		{
			Clean();
			if (skillData == null)
			{
				Log.E(
					"[ToolTip] SetSkill(LocalMovableCharacter, SkillData, SkillData, int, string, bool) : SkillData is Null");
				return;
			}

			uiSkillTooltip.SetSkill(self, skillData, nextSkillData, evolutionLevel, keyCode, addSkillInfo,
				cooldownReduction);
			currentTooltipComponent = uiSkillTooltip;
		}


		public void SetEvolutionSkill(LocalMovableCharacter self, SkillData skillData, int skillLevel, string keyCode,
			float cooldownReduction)
		{
			Clean();
			if (skillData == null)
			{
				Log.E("[ToolTip] SetEvolutionSkill(LocalMovableCharacter, SkillData, int, string) : SkillData is Null");
				return;
			}

			uiSkillTooltip.SetEvolutionSkill(self, skillData, skillLevel, keyCode, cooldownReduction);
			currentTooltipComponent = uiSkillTooltip;
		}


		public void SetStateEffect(CharacterStateData data, string caster, bool showLevel)
		{
			Clean();
			if (data == null)
			{
				Log.E(
					"[ToolTip] SetStateEffect(CharacterStateData, string, bool, params object[]) : CharacterStateData is Null");
				return;
			}

			uiSkillTooltip.SetStateEffect(data, caster, showLevel);
			currentTooltipComponent = uiSkillTooltip;
		}


		public void SetStateEffect(CharacterStateData data, string caster, bool showLevel, string customDesc)
		{
			Clean();
			if (data == null)
			{
				Log.E(
					"[ToolTip] SetStateEffect(CharacterStateData, string, bool, params object[]) : CharacterStateData is Null");
				return;
			}

			uiSkillTooltip.SetStateEffect(data, caster, showLevel, customDesc);
			currentTooltipComponent = uiSkillTooltip;
		}


		public void ShowStateEffectTooltip(CharacterStateValue characterState, SlotType slotType)
		{
			if (characterState == null)
			{
				return;
			}

			LocalCharacter localCharacter =
				MonoBehaviourInstance<ClientService>.inst.World.Find<LocalCharacter>(characterState.CasterId);
			CharacterStateData data = GameDB.characterState.GetData(characterState.code);
			if (characterState.StateType == StateType.ModeModifier)
			{
				int num = Mathf.FloorToInt(100f + localCharacter.Stat.GetValue(StatType.IncreaseModeDamageRatio) *
					100f);
				int num2 = Mathf.FloorToInt(100f - localCharacter.Stat.GetValue(StatType.PreventModeDamageRatio) *
					100f);
				int num3 = Mathf.FloorToInt(100f + localCharacter.Stat.GetValue(StatType.IncreaseModeHealRatio) * 100f);
				int num4 = Mathf.FloorToInt(
					100f + localCharacter.Stat.GetValue(StatType.IncreaseModeShieldRatio) * 100f);
				string text = string.Empty;
				if (num != 100)
				{
					text = Ln.Format("CharacterState/ModeModifier/IncreaseModeDamageRatio", num);
				}

				if (num2 != 100)
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += "\n";
					}

					text += Ln.Format("CharacterState/ModeModifier/PreventModeDamageRatio", num2);
				}

				if (num3 != 100)
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += "\n";
					}

					text += Ln.Format("CharacterState/ModeModifier/IncreaseModeHealRatio", num3);
				}

				if (num4 != 100)
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += "\n";
					}

					text += Ln.Format("CharacterState/ModeModifier/IncreaseModeShieldRatio", num4);
				}

				SetStateEffect(data, localCharacter != null ? localCharacter.Nickname : "", false, text);
			}
			else
			{
				SetStateEffect(data, localCharacter != null ? localCharacter.Nickname : "", false);
			}

			ShowFixed(null,
				slotType == SlotType.TargetInfo
					? TooltipPosition.TargetInfoPosition
					: TooltipPosition.SkillInfoCenterPosition);
		}


		public void SetMastery(MasteryType masteryType, int level, int exp, int maxExp)
		{
			Clean();
			uiMasteryInfo.SetMasteryInfo(masteryType, level, exp, maxExp);
			uiMasteryInfo.SetAcquisitionMethod(masteryType);
			currentTooltipComponent = uiMasteryInfo;
		}


		public void SetLobbyMastery(MasteryType masteryType, int level, int exp, int maxExp)
		{
			Clean();
			uiLobbyMasteryInfo.SetMasteryInfo(masteryType, level, exp, maxExp);
			uiLobbyMasteryInfo.SetAcquisitionMethod(masteryType);
			currentTooltipComponent = uiLobbyMasteryInfo;
		}


		public void SetLabel(string text, float width = 140f)
		{
			Clean();
			uiLabelContainerTooltip.SetLabel(text, width);
			currentTooltipComponent = uiLabelContainerTooltip;
		}


		public void SetInfo(string title, string key, string desc)
		{
			Clean();
			uiInfoLabel.SetTitle(title);
			uiInfoLabel.SetKey(key);
			uiInfoLabel.SetDesc(desc);
			currentTooltipComponent = uiInfoLabel;
		}


		public void ShowTracking(BaseUI owner)
		{
			if (currentTooltipComponent == null)
			{
				return;
			}

			this.owner = owner;
			currentTooltipComponent.ChangeMode(TooltipMode.Tracking);
			currentTooltipComponent.ChangeParent(transform);
			currentTooltipComponent.ChangePosition(Input.mousePosition);
			ShowInternal();
		}


		public void ShowFixed(BaseUI owner, TooltipPosition tooltipPosition)
		{
			if (currentTooltipComponent == null)
			{
				return;
			}

			this.owner = owner;
			currentTooltipComponent.ChangeMode(TooltipMode.Fixed);
			currentTooltipComponent.ChangeParent(tooltipPositions[(int) tooltipPosition]);
			currentTooltipComponent.ResetLocalPosition();
			currentTooltipComponent.ChangePivot(GetPivot(tooltipPosition));
			ShowInternal();
		}


		public void ShowFixed(BaseUI owner, SlotType slotType)
		{
			ShowFixed(owner, GetTooltipPosition(slotType));
		}


		public void ShowFixed(BaseUI owner, Vector2 position, Pivot pivot)
		{
			if (currentTooltipComponent == null)
			{
				return;
			}

			this.owner = owner;
			currentTooltipComponent.ChangeParent(transform);
			currentTooltipComponent.ChangePosition(position);
			currentTooltipComponent.ChangeMode(TooltipMode.Fixed);
			currentTooltipComponent.ChangePivot(pivot);
			ShowInternal();
		}


		public void ShowFixed(BaseUI owner, Pivot pivot)
		{
			if (currentTooltipComponent == null)
			{
				return;
			}

			this.owner = owner;
			currentTooltipComponent.ChangeMode(TooltipMode.Fixed);
			currentTooltipComponent.ChangePivot(pivot);
			ShowInternal();
		}


		private Pivot GetPivot(TooltipPosition tooltipPosition)
		{
			Pivot result;
			switch (tooltipPosition)
			{
				case TooltipPosition.EquipmentPosition:
					result = Pivot.LeftBottom;
					break;
				case TooltipPosition.InventoryPosition:
				case TooltipPosition.NavigationPosition:
					result = Pivot.RightBottom;
					break;
				case TooltipPosition.FavoritePosition:
					result = Pivot.RightTop;
					break;
				case TooltipPosition.TargetInfoPosition:
					result = Pivot.LeftTop;
					break;
				case TooltipPosition.SkillInfoCenterPosition:
					result = Pivot.LeftBottom;
					break;
				case TooltipPosition.LobbyMasterSkillInfoPosition:
					result = Pivot.LeftTop;
					break;
				case TooltipPosition.LobbyCharacterSkillInfoPosition:
					result = Pivot.LeftBottom;
					break;
				default:
					result = Pivot.LeftTop;
					break;
			}

			return result;
		}


		private TooltipPosition GetTooltipPosition(SlotType slotType)
		{
			TooltipPosition result = TooltipPosition.None;
			switch (slotType)
			{
				case SlotType.Equipment:
					result = TooltipPosition.EquipmentPosition;
					break;
				case SlotType.Inventory:
				case SlotType.ShortCut:
					result = TooltipPosition.InventoryPosition;
					break;
				case SlotType.Favorite:
					result = TooltipPosition.FavoritePosition;
					break;
				case SlotType.Navigation:
					result = TooltipPosition.NavigationPosition;
					break;
				case SlotType.TargetInfo:
					result = TooltipPosition.TargetInfoPosition;
					break;
			}

			return result;
		}


		private void ShowInternal()
		{
			if (showCanvas != null)
			{
				StopCoroutine(showCanvas);
			}

			if (_canvasGroup.alpha <= 0f)
			{
				showCanvas = this.StartThrowingCoroutine(
					CoroutineUtil.FrameDelayedAction(1, delegate { _canvasGroup.alpha = 1f; }),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][Tooltip] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
			}
		}


		private void Clean()
		{
			uiItemTooltip.Clear();
			uiSkillTooltip.Clear();
			uiLabelContainerTooltip.Clear();
			uiMasteryInfo.Clear();
			uiInfoLabel.Clear();
			uiLobbyMasteryInfo.Clear();
			currentTooltipComponent = null;
		}


		public void Hide()
		{
			Hide(null);
		}


		public void Hide(BaseUI parent)
		{
			if (owner == null || owner == parent)
			{
				if (showCanvas != null)
				{
					StopCoroutine(showCanvas);
				}

				_canvasGroup.alpha = 0f;
				if (currentTooltipComponent != null)
				{
					currentTooltipComponent.Clear();
				}
			}
		}
	}
}