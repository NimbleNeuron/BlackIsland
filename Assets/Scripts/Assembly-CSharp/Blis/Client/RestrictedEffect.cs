using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public abstract class RestrictedEffect : MonoBehaviour
	{
		private int areaCode;

		public void Init(LevelData levelData)
		{
			AreaData currentAreaData = AreaUtil.GetCurrentAreaData(levelData, transform.position, 2147483640);
			areaCode = currentAreaData.code;
		}


		public int GetAreaCode()
		{
			return areaCode;
		}


		public abstract void SetState(AreaRestrictionState areaState, float time);
	}
}