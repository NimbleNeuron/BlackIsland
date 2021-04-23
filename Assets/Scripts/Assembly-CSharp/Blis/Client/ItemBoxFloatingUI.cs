using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class ItemBoxFloatingUI : MonoBehaviour, IHoverble
	{
		private LocalResourceItemBox localResourceItemBox;


		private UITrackingImage uiResourceImage;


		private UIResourceTimer uiResourceTimer;


		private void Update()
		{
			if (localResourceItemBox == null)
			{
				return;
			}

			if (localResourceItemBox.CollectibleData == null)
			{
				return;
			}

			if (localResourceItemBox.CollectibleData.castingActionType != CastingActionType.CollectibleOpenTreeOfLife &&
			    localResourceItemBox.CooldownUntil > Time.time)
			{
				int num = Mathf.CeilToInt(localResourceItemBox.CooldownUntil - Time.time);
				if (num > 0)
				{
					AllocTimerUI();
					uiResourceTimer.SetValue(num);
				}
			}
			else
			{
				FreeTimerUI();
			}
		}


		private void OnDestroy()
		{
			FreeTimerUI();
			FreeResourceImage();
		}


		public void HoverOn()
		{
			AllocResourceImage();
		}


		public void HoverOff()
		{
			FreeResourceImage();
		}

		public void Init(LocalResourceItemBox localItemBox)
		{
			localResourceItemBox = localItemBox;
		}


		private void AllocResourceImage()
		{
			if (uiResourceImage == null)
			{
				uiResourceImage = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UITrackingImage>();
				uiResourceImage.SetTrackingTarget(transform);
				uiResourceImage.SetTrackingOffset(Vector3.up * 2f);
				uiResourceImage.SetColor(Color.clear);
				CollectibleData collectibleData = localResourceItemBox.CollectibleData;
				if (collectibleData != null)
				{
					ItemData itemData = GameDB.item.FindItemByCode(collectibleData.itemCode);
					uiResourceImage.SetSprite(itemData.GetSprite());
					uiResourceImage.SetColor(Color.white);
					uiResourceImage.SetSize(new Vector2(65f, 35f));
				}
			}
		}


		private void FreeResourceImage()
		{
			if (MonoBehaviourInstance<GameUI>.inst != null && uiResourceImage != null)
			{
				MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UITrackingImage>(uiResourceImage);
				uiResourceImage = null;
			}
		}


		private void AllocTimerUI()
		{
			if (uiResourceTimer == null)
			{
				uiResourceTimer = MonoBehaviourInstance<GameUI>.inst.UITracker.Alloc<UIResourceTimer>();
				uiResourceTimer.SetTrackingTarget(transform);
				uiResourceTimer.SetTrackingOffset(Vector3.up * 2f);
			}
		}


		private void FreeTimerUI()
		{
			if (uiResourceTimer != null && MonoBehaviourInstance<GameUI>.inst != null)
			{
				MonoBehaviourInstance<GameUI>.inst.UITracker.Free<UIResourceTimer>(uiResourceTimer);
				uiResourceTimer = null;
			}
		}
	}
}