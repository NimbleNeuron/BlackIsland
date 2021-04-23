using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class InfoMaker : BaseUI, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		[SerializeField] private LnText txtKeyCode = default;


		public string title = default;


		public string desc = default;


		public string keyCode = default;


		public Tooltip.TooltipMode mode;


		[ConditionalField("mode", Tooltip.TooltipMode.Fixed)]
		public Vector2 worldPos;


		[ConditionalField("mode", Tooltip.TooltipMode.Fixed)]
		public Tooltip.Pivot pivot;


		public bool localization;


		public bool useObjectWorldPos;


		[ConditionalField("useObjectWorldPos", true)]
		public Transform targetObject;


		protected override void OnDisable()
		{
			base.OnDisable();
			MonoBehaviourInstance<Tooltip>.inst.Hide(this);
		}


		public void OnPointerEnter(PointerEventData eventData)
		{
			if (localization)
			{
				if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(keyCode))
				{
					MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get(desc));
				}
				else
				{
					MonoBehaviourInstance<Tooltip>.inst.SetInfo(Ln.Get(title), keyCode, Ln.Get(desc));
				}
			}
			else if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(keyCode))
			{
				MonoBehaviourInstance<Tooltip>.inst.SetLabel(desc);
			}
			else
			{
				MonoBehaviourInstance<Tooltip>.inst.SetInfo(title, keyCode, desc);
			}

			if (mode == Tooltip.TooltipMode.Tracking)
			{
				MonoBehaviourInstance<Tooltip>.inst.ShowTracking(this);
				return;
			}

			if (mode == Tooltip.TooltipMode.Fixed)
			{
				if (useObjectWorldPos)
				{
					Vector2 vector = targetObject.position;
					vector += GameUtil.ConvertPositionOnScreenResolution(worldPos.x, worldPos.y);
					MonoBehaviourInstance<Tooltip>.inst.ShowFixed(this, vector, pivot);
					return;
				}

				MonoBehaviourInstance<Tooltip>.inst.ShowFixed(this,
					GameUtil.ConvertPositionOnScreenResolution(worldPos.x, worldPos.y), pivot);
			}
		}


		public void OnPointerExit(PointerEventData eventData)
		{
			MonoBehaviourInstance<Tooltip>.inst.Hide(this);
		}

		public void SetTextKeyCode(string keyCode)
		{
			if (txtKeyCode != null)
			{
				txtKeyCode.text = keyCode;
			}
		}
	}
}