using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class EmotionPlateHud : BaseUI, EmotionPlateHudComponent.IEventDelegate
	{
		private readonly List<EmotionPlateHudComponent>
			emotionPlateHudComponents = new List<EmotionPlateHudComponent>();


		private float centerButtonRadius;


		private GameObject componentObject;


		private int continousInputCount;


		private EmotionPlateType currentType = EmotionPlateType.Center;


		private int failCount;


		private float inputDelayTime;


		private bool isRequesting;


		private bool isShowing;


		private Transform plateHudCenter;


		private GameObject uiObject;


		private void Update()
		{
			if (inputDelayTime > 0f)
			{
				inputDelayTime -= Time.deltaTime;
			}

			if (!componentObject.activeSelf)
			{
				return;
			}

			if (!uiObject.activeSelf)
			{
				UiActive(true, EmotionPlateType.Center);
			}

			if (uiObject.activeSelf)
			{
				CheckCursorPosition();
			}

			KeyCode keyCode = MonoBehaviourInstance<GameInput>.inst.GetKeyCode(GameInputEvent.EmotionPlate);
			if (keyCode == KeyCode.None)
			{
				return;
			}

			if (Input.GetKeyUp(keyCode) || Input.GetMouseButtonUp(0))
			{
				if (isRequesting || inputDelayTime > 0f)
				{
					DeActive();
				}
				else
				{
					SendEmotionIcon();
				}
			}

			if (Input.GetMouseButtonUp(1) || Input.GetKeyDown(KeyCode.Escape))
			{
				DeActive();
			}
		}


		void EmotionPlateHudComponent.IEventDelegate.OnPointerEnter(EmotionPlateType type)
		{
			ActiveComponentUI(type);
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GetComponentsInChildren<EmotionPlateHudComponent>(true, emotionPlateHudComponents);
			uiObject = GameUtil.Bind<RectTransform>(gameObject, "Ui").gameObject;
			componentObject = GameUtil.Bind<RectTransform>(gameObject, "Components").gameObject;
			plateHudCenter = GameUtil.Bind<Transform>(gameObject, "PlateHudCenter");
			centerButtonRadius = GameUtil.Bind<RectTransform>(gameObject, "Ui/BtnCenter").rect.width / 2f;
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			EmotionLoad();
		}


		private void EmotionLoad()
		{
			RequestDelegate.request<InventoryApi.EmotionResult>(InventoryApi.GetInventoryEmotion(),
				delegate(RequestDelegateError err, InventoryApi.EmotionResult res)
				{
					if (err != null)
					{
						FailLoad();
						return;
					}

					SuccessLoad(res.userEmotionSlots);
				});
		}


		private void SuccessLoad(List<InventoryApi.UserEmoticonSlot> userEmotionSlots)
		{
			for (int i = 0; i < emotionPlateHudComponents.Count; i++)
			{
				emotionPlateHudComponents[i].EventDelegate = this;
				if (userEmotionSlots != null && userEmotionSlots.Count > i)
				{
					EmotionIconData emotionIconData =
						GameDB.emotionIcon.GetEmotionIconData(userEmotionSlots[i].emotionCode);
					emotionPlateHudComponents[i].SetEquippedEmotionIcon(emotionIconData);
				}
				else
				{
					emotionPlateHudComponents[i].SetEquippedEmotionIcon(null);
				}
			}
		}


		private void FailLoad()
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsGameStarted)
			{
				return;
			}

			if (5 <= failCount)
			{
				return;
			}

			failCount++;
			EmotionLoad();
		}


		private void CheckCursorPosition()
		{
			plateHudCenter.position = Input.mousePosition;
			Vector3 localPosition = plateHudCenter.localPosition;
			if (Mathf.Sqrt(Mathf.Pow(localPosition.x, 2f) + Mathf.Pow(localPosition.y, 2f)) < centerButtonRadius)
			{
				if (currentType != EmotionPlateType.Center)
				{
					ActiveComponentUI(EmotionPlateType.Center);
				}

				return;
			}

			float num = Mathf.Atan2(localPosition.y, localPosition.x) * 57.295776f;
			if (num < 0f)
			{
				num += 360f;
			}

			if (0f <= num && num < 22.5 || 337.5 <= num && num < 0f)
			{
				ActiveComponentUI(EmotionPlateType.Right);
				return;
			}

			if (22.5 <= num && num < 67.5)
			{
				ActiveComponentUI(EmotionPlateType.TopRight);
				return;
			}

			if (67.5 <= num && num < 112.5)
			{
				ActiveComponentUI(EmotionPlateType.Top);
				return;
			}

			if (112.5 <= num && num < 157.5)
			{
				ActiveComponentUI(EmotionPlateType.TopLeft);
				return;
			}

			if (157.5 <= num && num < 202.5)
			{
				ActiveComponentUI(EmotionPlateType.Left);
				return;
			}

			if (202.5 <= num && num < 247.5)
			{
				ActiveComponentUI(EmotionPlateType.BottomLeft);
				return;
			}

			if (247.5 <= num && num < 292.5)
			{
				ActiveComponentUI(EmotionPlateType.Bottom);
				return;
			}

			if (292.5 <= num && num < 337.5)
			{
				ActiveComponentUI(EmotionPlateType.BottomRight);
			}
		}


		public void Active(Vector3 hitGroundPoint)
		{
			componentObject.gameObject.SetActive(true);
			currentType = EmotionPlateType.Center;
			transform.position = Camera.main.WorldToScreenPoint(hitGroundPoint);
			transform.localScale = Vector3.one;
			UiActive(false, currentType);
		}


		private void DeActive()
		{
			componentObject.SetActive(false);
			uiObject.gameObject.SetActive(false);
		}


		public override bool IsActive()
		{
			return componentObject.activeSelf;
		}


		private void UiActive(bool active, EmotionPlateType type)
		{
			uiObject.gameObject.SetActive(active);
			if (active)
			{
				ActiveComponentUI(type);
			}
		}


		private void SendEmotionIcon()
		{
			if (!uiObject.activeSelf)
			{
				ReqEmotionIcon packet = new ReqEmotionIcon
				{
					emotionPlateType = EmotionPlateType.Center
				};
				MonoBehaviourInstance<GameClient>.inst.Request(packet);
				isRequesting = true;
			}
			else if (uiObject.activeSelf && !GetComponentIsEmpty(currentType))
			{
				ReqEmotionIcon packet2 = new ReqEmotionIcon
				{
					emotionPlateType = currentType
				};
				MonoBehaviourInstance<GameClient>.inst.Request(packet2);
				isRequesting = true;
			}
			else
			{
				isRequesting = false;
			}

			DeActive();
		}


		private bool GetComponentIsEmpty(EmotionPlateType type)
		{
			for (int i = 0; i < emotionPlateHudComponents.Count; i++)
			{
				if (emotionPlateHudComponents[i].Type == type)
				{
					return emotionPlateHudComponents[i].IsComponentEmpty();
				}
			}

			return true;
		}


		public void OnShowEmotionIcon(float inputDelayTime)
		{
			isRequesting = false;
			if (isShowing)
			{
				continousInputCount++;
			}
			else
			{
				continousInputCount = 0;
			}

			isShowing = true;
			if (continousInputCount >= 5)
			{
				this.inputDelayTime = 5f;
				continousInputCount = 0;
				MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(Ln.Get("감정표현제한"));
				return;
			}

			this.inputDelayTime = inputDelayTime;
		}


		public void OnFinishEmotionIcon()
		{
			isShowing = false;
		}


		private void ActiveComponentUI(EmotionPlateType type)
		{
			if (currentType == type)
			{
				return;
			}

			for (int i = 0; i < emotionPlateHudComponents.Count; i++)
			{
				if (emotionPlateHudComponents[i].Type == type)
				{
					emotionPlateHudComponents[i].SetHighlight(true);
				}
				else
				{
					emotionPlateHudComponents[i].SetHighlight(false);
				}
			}

			currentType = type;
		}
	}
}