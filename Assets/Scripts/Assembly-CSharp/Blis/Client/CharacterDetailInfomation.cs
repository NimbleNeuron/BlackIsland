using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CharacterDetailInfomation : BasePage
	{
		private UIProgress attack;


		private UIProgress defence;


		private Text guideDesc;


		private UIProgress health;


		private int selectedCharacterCode;


		private EventTrigger showAcquisitionMethod;


		private UIProgress stamina;


		private WeaponMastery weaponMastery;


		private Transform wpMasteryInfoIcon;


		private Text wpTypeTipDesc;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			health = GameUtil.Bind<UIProgress>(gameObject, "Stat/Stat1/Guage");
			stamina = GameUtil.Bind<UIProgress>(gameObject, "Stat/Stat2/Guage");
			attack = GameUtil.Bind<UIProgress>(gameObject, "Stat/Stat3/Guage");
			defence = GameUtil.Bind<UIProgress>(gameObject, "Stat/Stat4/Guage");
			guideDesc = GameUtil.Bind<Text>(gameObject, "PlayGuideScrollView/Viewport/Content/Desc");
			wpTypeTipDesc = GameUtil.Bind<Text>(gameObject, "WpTypeTipScrollView/Viewport/Content/Desc");
			wpMasteryInfoIcon = GameUtil.Bind<Transform>(gameObject, "WpMastery/Text/AddInfo");
			weaponMastery = GameUtil.Bind<WeaponMastery>(gameObject, "WeaponMastery");
			showAcquisitionMethod = GameUtil.Bind<EventTrigger>(gameObject, "WpMastery/Text/AddInfo");
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener(delegate(BaseEventData data) { ShowTooltip((PointerEventData) data); });
			showAcquisitionMethod.triggers.Add(entry);
			EventTrigger.Entry entry2 = new EventTrigger.Entry();
			entry2.eventID = EventTriggerType.PointerExit;
			entry2.callback.AddListener(delegate(BaseEventData data) { HideTooltip((PointerEventData) data); });
			showAcquisitionMethod.triggers.Add(entry2);
		}


		public void SetCharacterData(int characterCode, CharacterMasteryData masteryData)
		{
			selectedCharacterCode = characterCode;
			guideDesc.text = Ln.Get(LnType.CharacterGuide, string.Format("{0}", selectedCharacterCode));
			wpTypeTipDesc.text = Ln.Get(LnType.CharacterGuide,
				string.Format("{0}/{1}", selectedCharacterCode, masteryData.weapon1));
			SetStatProgress();
			this.weaponMastery.SetMasteryTypes(selectedCharacterCode);
			WeaponMastery weaponMastery = this.weaponMastery;
			weaponMastery.onMasteryToggleChange = (WeaponMastery.OnMasteryToggleChange) Delegate.Combine(
				weaponMastery.onMasteryToggleChange,
				new WeaponMastery.OnMasteryToggleChange(delegate(MasteryType masteryType)
				{
					wpTypeTipDesc.text = Ln.Get(LnType.CharacterGuide,
						string.Format("{0}/{1}", selectedCharacterCode, masteryType));
				}));
		}


		private void SetStatProgress()
		{
			CharacterData characterData = GameDB.character.GetCharacterData(selectedCharacterCode);
			if (characterData != null)
			{
				if (health != null)
				{
					health.SetValue(characterData.maxHp, 800);
					health.SetLabel(characterData.maxHp.ToString());
				}

				if (stamina != null)
				{
					stamina.SetValue(characterData.maxSp, 500);
					stamina.SetLabel(characterData.maxSp.ToString());
				}

				if (attack != null)
				{
					attack.SetValue((int) Math.Truncate(characterData.attackPower), 100);
					attack.SetLabel(characterData.attackPower.ToString());
				}

				if (defence != null)
				{
					defence.SetValue((int) Math.Truncate(characterData.defense), 50);
					defence.SetLabel(characterData.defense.ToString());
				}
			}
		}


		private void ShowTooltip(PointerEventData data)
		{
			Vector2 vector = wpMasteryInfoIcon.transform.position;
			vector += GameUtil.ConvertPositionOnScreenResolution(30f, 0f);
			MonoBehaviourInstance<Tooltip>.inst.SetLobbyMastery(weaponMastery.SelectMasteryType, 1, 0, 0);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
		}


		private void HideTooltip(PointerEventData data)
		{
			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}
	}
}