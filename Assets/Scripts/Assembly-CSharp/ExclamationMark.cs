using Blis.Client;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;


public class ExclamationMark : BaseUI
{
	
	public void UpdateExcalmation(ItemData itemData)
	{
		if (itemData != null)
		{
			this.desc = Ln.Get(string.Format("Item/Help/{0:000000}", itemData.code));
			if (this.desc.Equals("") || this.desc.Contains("LnErr"))
			{
				base.transform.gameObject.SetActive(false);
				return;
			}
			base.transform.gameObject.SetActive(true);
		}
	}

	
	public void OnPointerEnter(BaseEventData data)
	{
		MonoBehaviourInstance<Tooltip>.inst.SetLabel(this.desc, 140f);
		Vector2 position = base.transform.position;
		position.y += GameUtil.ConvertPositionOnScreenResolution(0f, 65f).y;
		MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, position, Tooltip.Pivot.LeftTop);
	}

	
	public void OnPointerExit(BaseEventData data)
	{
		MonoBehaviourInstance<Tooltip>.inst.Hide();
	}

	
	private string desc = "";
}
