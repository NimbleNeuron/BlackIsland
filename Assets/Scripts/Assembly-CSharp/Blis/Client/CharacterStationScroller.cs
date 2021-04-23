using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class CharacterStationScroller : UIBehaviour, IDragHandler, IEventSystemHandler, IScrollHandler
	{
		public CameraControl cameraControl;


		public float speed = 1f;

		protected override void Start()
		{
			base.Start();
			cameraControl = SingletonMonoBehaviour<LobbyCharacterStation>.inst.CameraControl;
		}


		public void OnDrag(PointerEventData eventData)
		{
			if (!SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
			{
				return;
			}

			if (MonoBehaviourInstance<LobbyService>.inst == null)
			{
				return;
			}

			if (SingletonMonoBehaviour<LobbyCharacterStation>.inst.IsPlayingAppearAnim())
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Left)
			{
				SingletonMonoBehaviour<LobbyCharacterStation>.inst.Rotate(-Vector3.up * eventData.delta.x * speed);
			}
		}


		public void OnScroll(PointerEventData eventData)
		{
			float num = cameraControl.currentZoom;
			num -= Input.GetAxis("Mouse ScrollWheel") * cameraControl.zoomSpeed;
			cameraControl.SetZoom(num);
		}
	}
}