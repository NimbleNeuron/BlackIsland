using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class PingHud : BaseUI, PingHudComponent.IEventDelegate
	{
		[SerializeField] private GameObject uiObject = default;


		[SerializeField] private Transform tr = default;


		private readonly Vector3 miniScale = new Vector3(0.5f, 0.5f, 0.5f);


		private PingType currentPingType = default;


		private PingHudComponent[] pingHudComponents = default;


		private float releaseSecond = default;


		private LocalObject targetObject = default;


		private Vector3 targetPosition = default;

		protected override void Awake()
		{
			pingHudComponents = GetComponentsInChildren<PingHudComponent>(true);
			for (int i = 0; i < pingHudComponents.Length; i++)
			{
				pingHudComponents[i].EventDelegate = this;
			}
		}


		private void Update()
		{
			releaseSecond += Time.deltaTime;
			if (releaseSecond > 0.15f && !uiObject.activeSelf)
			{
				UiActive(true, PingType.None);
			}

			if (uiObject.activeSelf && currentPingType != PingType.None)
			{
				CheckPosition();
			}

			if (Input.GetMouseButtonUp(0))
			{
				SendPing();
			}
		}


		void PingHudComponent.IEventDelegate.OnPointerEnter(PingType type)
		{
			if (type != PingType.None && !uiObject.activeSelf)
			{
				UiActive(true, type);
			}

			ActiveComponentUI(type);
		}


		private void CheckPosition()
		{
			tr.position = Input.mousePosition;
			Vector3 localPosition = tr.localPosition;
			if (localPosition.x > 0f && localPosition.y / localPosition.x < 1f &&
			    localPosition.y / localPosition.x > -1f)
			{
				ActiveComponentUI(PingType.Run);
				return;
			}

			if (localPosition.x < 0f && localPosition.y / localPosition.x < 1f &&
			    localPosition.y / localPosition.x > -1f)
			{
				ActiveComponentUI(PingType.Warning);
				return;
			}

			if (localPosition.y > 0f &&
			    (localPosition.y / localPosition.x > 1f || localPosition.y / localPosition.x < -1f))
			{
				ActiveComponentUI(PingType.Escape);
				return;
			}

			ActiveComponentUI(PingType.Help);
		}


		public void ActiveOnUI(Vector3 worldPos, Vector3 eventPos)
		{
			currentPingType = PingType.Select;
			targetPosition = worldPos;
			transform.position = eventPos;
			transform.localScale = miniScale;
			gameObject.SetActive(true);
			releaseSecond = 0f;
			UiActive(false, PingType.None);
		}


		public void Active(Vector3 worldPos, Vector3 hitGroundPoint, LocalObject hitTarget)
		{
			currentPingType = PingType.Select;
			targetPosition = worldPos;
			transform.position = Camera.main.WorldToScreenPoint(hitGroundPoint);
			transform.localScale = Vector3.one;
			targetObject = hitTarget;
			gameObject.SetActive(true);
			releaseSecond = 0f;
			UiActive(false, PingType.None);
		}


		private void Deactive()
		{
			gameObject.SetActive(false);
		}


		private void UiActive(bool active, PingType type)
		{
			uiObject.gameObject.SetActive(active);
			if (active)
			{
				currentPingType = type;
				ActiveComponentUI(type);
			}
		}


		public void SendPing()
		{
			if (!uiObject.activeSelf)
			{
				ReqPingTarget packet = new ReqPingTarget
				{
					pingType = PingType.Select,
					targetObjectId = targetObject == null ? 0 : targetObject.ObjectId,
					targetPosition = targetPosition
				};
				MonoBehaviourInstance<GameClient>.inst.Request(packet);
			}
			else if (uiObject.activeSelf && currentPingType != PingType.None)
			{
				ReqPingTarget packet2 = new ReqPingTarget
				{
					pingType = currentPingType,
					targetObjectId = targetObject == null ? 0 : targetObject.ObjectId,
					targetPosition = targetPosition
				};
				MonoBehaviourInstance<GameClient>.inst.Request(packet2);
			}

			Deactive();
		}


		private void ActiveComponentUI(PingType type)
		{
			currentPingType = type;
			for (int i = 0; i < pingHudComponents.Length; i++)
			{
				if (pingHudComponents[i].PingType == type)
				{
					pingHudComponents[i].SetHighlight(true);
				}
				else
				{
					pingHudComponents[i].SetHighlight(false);
				}
			}
		}
	}
}