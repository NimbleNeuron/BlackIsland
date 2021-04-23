using UnityEngine;

namespace Blis.Common
{
	[DisallowMultipleComponent]
	public class GroundNavFlag : MonoBehaviour
	{
		public enum MapAreaType
		{
			None = 0,
			Harbor = 1,
			Pond = 2,
			SandyBeach = 3,
			Uptown,
			Alley,
			Hotel,
			Downtown,
			Hospital,
			Temple,
			Archery = 10,
			Cemetery,
			Forest,
			Factory,
			Church,
			School,
			Laboratory,
			ReadyRoom
		}


		[SerializeField] private MapAreaType areaType = default;


		private int maskCode = default;


		public MapAreaType GetAreaType => areaType;


		public int GetMaskCode => maskCode;


		private void Awake()
		{
			AreaData areaData = GameDB.level.GetAreaData((int) areaType);
			if (areaData == null)
			{
				// Log.E("Can not find AreaData : " + areaType);
				Debug.LogWarning($"Can not find AreaData : {areaType}", gameObject);
				return;
			}

			maskCode = areaData.maskCode;
		}
	}
}