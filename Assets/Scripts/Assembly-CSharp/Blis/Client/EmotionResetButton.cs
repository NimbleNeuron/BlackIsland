using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class EmotionResetButton : MonoBehaviour
	{
		public void OnPointerEnter()
		{
			MonoBehaviourInstance<Tooltip>.inst.SetLabel(Ln.Get("설정을 초기화 합니다."));
			Vector2 vector = transform.position;
			vector += GameUtil.ConvertPositionOnScreenResolution(15f, 50f);
			MonoBehaviourInstance<Tooltip>.inst.ShowFixed(null, vector, Tooltip.Pivot.LeftTop);
		}


		public void OnPointerExit()
		{
			MonoBehaviourInstance<Tooltip>.inst.Hide();
		}
	}
}