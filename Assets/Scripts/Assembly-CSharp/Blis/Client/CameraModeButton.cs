using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CameraModeButton : BaseUI
	{
		[SerializeField] private Image image = default;

		protected override void OnStartUI()
		{
			base.OnStartUI();
			InstOnOnCameraModeChange(MonoBehaviourInstance<MobaCamera>.inst.Mode);
			MonoBehaviourInstance<MobaCamera>.inst.OnCameraModeChange += InstOnOnCameraModeChange;
		}


		private void InstOnOnCameraModeChange(MobaCameraMode mode)
		{
			if (mode == MobaCameraMode.Traveling)
			{
				image.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Btn_Camera");
				return;
			}

			if (mode == MobaCameraMode.Tracking)
			{
				image.sprite = SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Btn_Camera_Off");
			}
		}
	}
}