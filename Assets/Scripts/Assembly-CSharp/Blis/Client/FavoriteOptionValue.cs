using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class FavoriteOptionValue : BaseControl
	{
		[SerializeField] private string optionTitle = default;


		[SerializeField] private string optionDesc = default;

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			MonoBehaviourInstance<Tooltip>.inst.SetInfo(Ln.Get(optionTitle), "", Ln.Get(optionDesc));
			Vector2 vector = transform.position;
			vector += GameUtil.ConvertPositionOnScreenResolution(30f, 50f);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			MonoBehaviourInstance<Tooltip>.inst.Hide(this);
		}
	}
}