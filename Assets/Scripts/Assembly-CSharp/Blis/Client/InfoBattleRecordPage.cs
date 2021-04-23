using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class InfoBattleRecordPage : BasePage
	{
		[SerializeField] private GameObject battleRecordSlotPrefab = default;


		private readonly List<TabIcon> tabList = new List<TabIcon>();


		private GameObject iconBattle = default;


		private GameObject iconDate = default;


		private GameObject iconItems = default;


		private GameObject iconMonsterKill = default;


		private GameObject iconPlayer = default;


		private GameObject iconPlayerKill = default;


		private GameObject iconPlayerKillAssist = default;


		private GameObject iconRank = default;


		private GameObject iconTime = default;


		private GameObject noData = default;


		private ScrollRect scrollRect = default;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			Transform transform = this.transform.Find("Record/Cotent");
			scrollRect = GameUtil.Bind<ScrollRect>(transform.gameObject, "Scroll Rect");
			iconRank = transform.FindRecursively("IconRank").gameObject;
			iconPlayer = transform.FindRecursively("IconPlayer").gameObject;
			iconItems = transform.FindRecursively("IconItems").gameObject;
			iconPlayerKill = transform.FindRecursively("IconPlayerKill").gameObject;
			iconPlayerKillAssist = transform.FindRecursively("IconPlayerKillAssist").gameObject;
			iconMonsterKill = transform.FindRecursively("IconMonsterKill").gameObject;
			iconBattle = transform.FindRecursively("IconBattle").gameObject;
			iconTime = transform.FindRecursively("IconTime").gameObject;
			iconDate = transform.FindRecursively("IconDate").gameObject;
			noData = scrollRect.transform.FindRecursively("NoData").gameObject;
		}


		protected override void OnStartUI()
		{
			tabList.Add(new TabIcon(iconRank, Ln.Get("순위")));
			tabList.Add(new TabIcon(iconPlayer, Ln.Get("플레이어")));
			tabList.Add(new TabIcon(iconItems, Ln.Get("아이템")));
			tabList.Add(new TabIcon(iconPlayerKill, Ln.Get("플레이어 처치 수")));
			tabList.Add(new TabIcon(iconPlayerKillAssist, Ln.Get("어시스트 수")));
			tabList.Add(new TabIcon(iconMonsterKill, Ln.Get("야생동물 처치 수")));
			tabList.Add(new TabIcon(iconBattle, Ln.Get("전투")));
			tabList.Add(new TabIcon(iconTime, Ln.Get("플레이 타임")));
			tabList.Add(new TabIcon(iconDate, Ln.Get("플레이 날짜")));
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			if (scrollRect.content.childCount < InfoService.GetBattleUserGamesCount())
			{
				int num = InfoService.GetBattleUserGamesCount() - scrollRect.content.childCount;
				for (int i = 0; i < num; i++)
				{
					Instantiate<GameObject>(battleRecordSlotPrefab, scrollRect.content);
				}
			}

			BattleRecordSlot[] componentsInChildren = scrollRect.content.GetComponentsInChildren<BattleRecordSlot>();
			for (int j = 0; j < scrollRect.content.childCount; j++)
			{
				if (j < componentsInChildren.Length)
				{
					componentsInChildren[j].gameObject.SetActive(true);
					componentsInChildren[j].SetScrollRect(scrollRect);
					componentsInChildren[j].SetBattleRecord(InfoService.GetBattleUserGame(j));
					componentsInChildren[j].EnterMasterys = delegate(MasteryType masterys, Vector3 pos)
					{
						OnPointerEnterMasterys(masterys, pos);
					};
					componentsInChildren[j].ExitMasterys = delegate { OnPointerExitMasterys(); };
				}
				else
				{
					componentsInChildren[j].gameObject.SetActive(false);
				}
			}

			noData.SetActive(componentsInChildren.Length == 0);
		}


		private void OnPointerEnterMasterys(MasteryType masteryType, Vector3 position)
		{
			MonoBehaviourInstance<Tooltip>.inst.SetLabel(LnUtil.GetMasteryName(masteryType));
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, position, Tooltip.Pivot.LeftTop);
		}


		private void OnPointerExitMasterys()
		{
			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}


		private class TabIcon
		{
			private readonly EventTrigger eventTrigger;


			private readonly GameObject gameObject;


			private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


			private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


			private readonly string str;


			private readonly Vector2 tooltipPos;

			public TabIcon(GameObject gameObject, string str)
			{
				this.gameObject = gameObject;
				this.str = str;
				tooltipPos = gameObject.transform.position;
				tooltipPos += GameUtil.ConvertPositionOnScreenResolution(0f, 60f);
				GameUtil.BindOrAdd<EventTrigger>(this.gameObject, ref eventTrigger);
				onEnterEvent.AddListener(OnPointerEnter);
				onExitEvent.AddListener(OnPointerExit);
				eventTrigger.triggers.Add(new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerEnter,
					callback = onEnterEvent
				});
				eventTrigger.triggers.Add(new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerExit,
					callback = onExitEvent
				});
			}


			private void OnPointerEnter(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(str);
				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, tooltipPos, Tooltip.Pivot.LeftTop);
			}


			private void OnPointerExit(BaseEventData eventData)
			{
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}
		}
	}
}