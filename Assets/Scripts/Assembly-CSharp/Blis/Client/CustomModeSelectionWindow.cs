using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CustomModeSelectionWindow : BaseWindow
	{
		private readonly List<CustomModeSlot> customModeSlots = new List<CustomModeSlot>();
		private Button btnCreate;


		private MatchingTeamMode matchingTeamMode;


		private Transform modeParent;


		private Text txtCreate;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			modeParent = transform.FindRecursively("ModeParent");
			for (int i = 0; i < modeParent.childCount; i++)
			{
				CustomModeSlot customModeSlot =
					new CustomModeSlot(modeParent.GetChild(i).gameObject, i + MatchingTeamMode.Solo);
				customModeSlot.OnSelectMatchingTeamMode += SelectCustomMode;
				customModeSlots.Add(customModeSlot);
			}

			btnCreate = GameUtil.Bind<Button>(gameObject, "BTN_Create");
			txtCreate = GameUtil.Bind<Text>(btnCreate.gameObject, "TXT_Create");
			btnCreate.onClick.AddListener(delegate { CreateRoom(); });
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			SelectCustomMode(MatchingTeamMode.None);
		}


		private void SelectCustomMode(MatchingTeamMode matchingTeamMode)
		{
			this.matchingTeamMode = matchingTeamMode;
			foreach (CustomModeSlot customModeSlot in customModeSlots)
			{
				customModeSlot.OnSelect(matchingTeamMode);
			}

			if (matchingTeamMode == MatchingTeamMode.None)
			{
				btnCreate.enabled = false;
				txtCreate.color = new Color(0.565f, 0.565f, 0.565f);
				return;
			}

			btnCreate.enabled = true;
			txtCreate.color = Color.white;
		}


		private void CreateRoom()
		{
			MonoBehaviourInstance<MatchingService>.inst.CreateCustomGame(matchingTeamMode);
			Close();
		}


		public void ClickedClose()
		{
			Close();
		}


		protected override void OnClose()
		{
			base.OnClose();
			SelectCustomMode(MatchingTeamMode.None);
		}


		private class CustomModeSlot
		{
			private readonly Button btn;


			private readonly float defaultScale = 0.95f;


			private readonly EventTrigger eventTrigger;


			private readonly float focusScale = 1f;


			private readonly GameObject gameObject;


			private readonly Image imgSelected;


			private readonly MatchingTeamMode matchingTeamMode;


			private readonly EventTrigger.TriggerEvent onEnterEvent = new EventTrigger.TriggerEvent();


			private readonly EventTrigger.TriggerEvent onExitEvent = new EventTrigger.TriggerEvent();


			private MatchingTeamMode selectedMatchingTeamMode;


			public CustomModeSlot(GameObject gameObject, MatchingTeamMode matchingTeamMode)
			{
				CustomModeSlot customModeSlot = this;
				this.gameObject = gameObject;
				this.matchingTeamMode = matchingTeamMode;
				btn = gameObject.GetComponent<Button>();
				btn.onClick.AddListener(() =>
					customModeSlot.OnSelectMatchingTeamMode(matchingTeamMode));
				imgSelected = GameUtil.Bind<Image>(this.gameObject, "IMG_Select");
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

				// co: dotPeek
				// CustomModeSelectionWindow.CustomModeSlot <>4__this = this;
				// this.gameObject = gameObject;
				// this.matchingTeamMode = matchingTeamMode;
				// this.btn = gameObject.GetComponent<Button>();
				// this.btn.onClick.AddListener(delegate()
				// {
				// 	<>4__this.OnSelectMatchingTeamMode(matchingTeamMode);
				// });
				// this.imgSelected = GameUtil.Bind<Image>(this.gameObject, "IMG_Select");
				// GameUtil.BindOrAdd<EventTrigger>(this.gameObject, ref this.eventTrigger);
				// this.onEnterEvent.AddListener(new UnityAction<BaseEventData>(this.OnPointerEnter));
				// this.onExitEvent.AddListener(new UnityAction<BaseEventData>(this.OnPointerExit));
				// this.eventTrigger.triggers.Add(new EventTrigger.Entry
				// {
				// 	eventID = EventTriggerType.PointerEnter,
				// 	callback = this.onEnterEvent
				// });
				// this.eventTrigger.triggers.Add(new EventTrigger.Entry
				// {
				// 	eventID = EventTriggerType.PointerExit,
				// 	callback = this.onExitEvent
				// });
			}

			
			
			public event Action<MatchingTeamMode> OnSelectMatchingTeamMode = delegate { };


			public void OnSelect(MatchingTeamMode matchingTeamMode)
			{
				selectedMatchingTeamMode = matchingTeamMode;
				bool flag = this.matchingTeamMode == matchingTeamMode;
				imgSelected.gameObject.SetActive(flag);
				if (!flag)
				{
					gameObject.transform.localScale = new Vector3(defaultScale, defaultScale, defaultScale);
				}
			}


			private void OnPointerEnter(BaseEventData eventData)
			{
				gameObject.transform.localScale = new Vector3(focusScale, focusScale, focusScale);
			}


			private void OnPointerExit(BaseEventData eventData)
			{
				if (selectedMatchingTeamMode == matchingTeamMode)
				{
					return;
				}

				gameObject.transform.localScale = new Vector3(defaultScale, defaultScale, defaultScale);
			}
		}
	}
}