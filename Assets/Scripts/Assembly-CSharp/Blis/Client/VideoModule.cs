using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class VideoModule : MonoBehaviour
	{
		[SerializeField] protected Image blur;


		protected Canvas canvas;


		protected float dataVolume = 1f;


		protected bool isEnableCanvas;


		protected float lastVolume = 1f;

		protected virtual void Awake() { }


		protected virtual void Start()
		{
			LobbyCharacterTab lobbyTab =
				MonoBehaviourInstance<LobbyUI>.inst.GetLobbyTab<LobbyCharacterTab>(LobbyTab.InventoryTab);
			canvas = lobbyTab != null ? lobbyTab.GetComponent<Canvas>() : null;
		}


		protected virtual void Update()
		{
			if (canvas == null)
			{
				return;
			}

			SetVolume(dataVolume);
			if (canvas.enabled)
			{
				if (!isEnableCanvas)
				{
					isEnableCanvas = true;
					Enable();
					SetBlur(true);
				}
			}
			else if (isEnableCanvas)
			{
				if (Singleton<GameEventLogger>.inst.IsNotChina())
				{
					Singleton<SoundControl>.inst.SetVideoModeMute(false);
				}

				isEnableCanvas = false;
				Disable();
				SetBlur(false);
			}
		}


		protected virtual void OnEnable() { }


		protected virtual void OnDisable() { }


		private void SetBlur(bool isActive)
		{
			if (blur != null)
			{
				blur.gameObject.SetActive(isActive);
			}
		}


		public virtual void OnClick() { }


		public virtual void Enable()
		{
			SetBlur(true);
		}


		public virtual void Disable()
		{
			SetBlur(false);
		}


		public virtual void Clear() { }


		// [return: TupleElementNames(new string[]
		// {
		// 	"currentTime",
		// 	"lastTime",
		// 	"isPlaying"
		// })]
		// public virtual ValueTuple<double, double, bool> GetCurrentTime()
		// {
		// 	return new ValueTuple<double, double, bool>(0.0, 1.0, true);
		// }
		// co: tuple
		public virtual (double currentTime, double lastTime, bool isPlaying) GetCurrentTime()
		{
			return (0.0, 1.0, true);
		}


		public virtual bool IsPlaying()
		{
			return false;
		}


		public virtual void SetUrl(string url) { }


		public virtual bool IsHasUrl()
		{
			return false;
		}


		public virtual void SetVolume(float value)
		{
			dataVolume = value;
		}


		public virtual void PlayForURL(string url, float volume) { }
	}
}