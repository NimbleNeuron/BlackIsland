using UnityEngine;

namespace Blis.Common
{
	public class NavAreaTestAgent : MonoBehaviour
	{
		public string currentArea = "";

		private void Update()
		{
			AreaData currentAreaData =
				AreaUtil.GetCurrentAreaData(GameDB.level.DefaultLevel, transform.position, 2147483640);
			if (currentAreaData != null)
			{
				currentArea = currentAreaData.name;
				return;
			}

			currentArea = "알 수 없음";
		}
	}
}