using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class HudButton : BaseUI
	{
		[SerializeField] private InfoMaker restInfoMaker = default;


		[SerializeField] private InfoMaker camaraInfoMaker = default;


		[SerializeField] private InfoMaker combineWindowInfoMaker = default;


		[SerializeField] private InfoMaker mapInfoMaker = default;


		[SerializeField] private Button mapZoomIn = default;


		[SerializeField] private Button mapZoomOut = default;

		protected override void OnStartUI()
		{
			base.OnStartUI();
			SetRestKeyCode(Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.Rest),
				Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.Rest));
			SetCameraKeyCode(Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.ChangeCameraMode),
				Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.ChangeCameraMode));
			SetCombineWindowKeyCode(Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.OpenCombineWindow),
				Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.OpenCombineWindow));
			SetMapKeyCode(Singleton<LocalSetting>.inst.GetKeyCode(GameInputEvent.OpenMap),
				Singleton<LocalSetting>.inst.GetCombinationKeyCode(GameInputEvent.OpenMap));
			UpdateMiniMapButtons();
		}


		public void SetRestKeyCode(KeyCode keyCode, KeyCode[] combinations)
		{
			List<KeyCode> list = combinations.ToList<KeyCode>();
			list.Add(keyCode);
			string text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(list);
			restInfoMaker.keyCode = text;
			restInfoMaker.SetTextKeyCode(text);
		}


		public void SetCameraKeyCode(KeyCode keyCode, KeyCode[] combinations)
		{
			List<KeyCode> list = combinations.ToList<KeyCode>();
			list.Add(keyCode);
			string keyCode2 = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(list);
			camaraInfoMaker.keyCode = keyCode2;
		}


		public void SetCombineWindowKeyCode(KeyCode keyCode, KeyCode[] combinations)
		{
			List<KeyCode> list = combinations.ToList<KeyCode>();
			list.Add(keyCode);
			string text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(list);
			combineWindowInfoMaker.keyCode = text;
			combineWindowInfoMaker.SetTextKeyCode(text);
		}


		public void SetMapKeyCode(KeyCode keyCode, KeyCode[] combinations)
		{
			List<KeyCode> list = combinations.ToList<KeyCode>();
			list.Add(keyCode);
			string text = Singleton<LocalSetting>.inst.ConvertKeyCodeListToString(list);
			mapInfoMaker.keyCode = text;
			mapInfoMaker.SetTextKeyCode(text);
		}


		public void ToggleCombineWindow()
		{
			MonoBehaviourInstance<GameUI>.inst.ToggleWindow(MonoBehaviourInstance<GameUI>.inst.CombineWindow);
		}


		public void ToggleRest()
		{
			if (!MonoBehaviourInstance<GameUI>.inst.Caster.IsCasting())
			{
				SingletonMonoBehaviour<PlayerController>.inst.SwitchRest(!SingletonMonoBehaviour<PlayerController>.inst
					.IsRest());
			}
		}


		public void ToggleSettingWindow()
		{
			MonoBehaviourInstance<GameUI>.inst.ToggleWindow(MonoBehaviourInstance<GameUI>.inst.SettingWindow);
		}


		public void ToggleMapWindow()
		{
			MonoBehaviourInstance<GameUI>.inst.ToggleWindow(MonoBehaviourInstance<GameUI>.inst.MapWindow);
		}


		public void ToggleCameraLock()
		{
			if (MonoBehaviourInstance<MobaCamera>.inst.Mode == MobaCameraMode.Tracking)
			{
				MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Traveling);
			}
			else
			{
				MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Tracking);
			}

			Singleton<LocalSetting>.inst.SaveHoldCamera(MonoBehaviourInstance<MobaCamera>.inst.Mode ==
			                                            MobaCameraMode.Tracking);
		}


		public void SetWatchMode()
		{
			transform.Find("RestBtn").gameObject.SetActive(false);
			transform.Find("Bottom").gameObject.SetActive(false);
		}


		public void MapZoomIn()
		{
			if (!Singleton<LocalSetting>.inst.setting.miniMapZoomOut)
			{
				return;
			}

			Singleton<LocalSetting>.inst.setting.miniMapZoomOut = false;
			Singleton<LocalSetting>.inst.Save();
			MonoBehaviourInstance<GameUI>.inst.Minimap.ZoomIn();
			UpdateMiniMapButtons();
		}


		public void MapZoomOut()
		{
			if (Singleton<LocalSetting>.inst.setting.miniMapZoomOut)
			{
				return;
			}

			Singleton<LocalSetting>.inst.setting.miniMapZoomOut = true;
			Singleton<LocalSetting>.inst.Save();
			MonoBehaviourInstance<GameUI>.inst.Minimap.ZoomOut();
			UpdateMiniMapButtons();
		}


		public void OnClickMapZoomIn()
		{
			MapZoomIn();
		}


		public void OnClickMapZoomOut()
		{
			MapZoomOut();
		}


		private void UpdateMiniMapButtons()
		{
			mapZoomIn.interactable = Singleton<LocalSetting>.inst.setting.miniMapZoomOut;
			mapZoomOut.interactable = !Singleton<LocalSetting>.inst.setting.miniMapZoomOut;
		}


		public void EnableRestBtn(bool enable)
		{
			restInfoMaker.gameObject.SetActive(enable);
		}


		public void ShowTeleportMap() { }
	}
}