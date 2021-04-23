using System;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Common
{
	public class AreaUtil
	{
		public static AreaData UnknownDummyData = new AreaData(0, "unknown", 0);


		private static readonly RaycastHit[] hits = new RaycastHit[20];


		private static readonly Vector3 rayCastPositionAdjust = new Vector3(0f, 1f, 0f);

		public static AreaData GetCurrentAreaData(LevelData levelData, Vector3 position, int maskCode)
		{
			return GetCurrentAreaDataByMask(levelData, maskCode, GetCurrentAreaMask(position));
		}


		public static AreaData GetCurrentAreaDataByMask(LevelData levelData, int maskCode, int areaMask)
		{
			int num = areaMask & maskCode;
			if (Debug.isDebugBuild)
			{
				float num2 = Mathf.Log(num, 2f);
				if (num2 - Mathf.FloorToInt(num2) > 7f)
				{
					Log.W("userAreaMask: " + Convert.ToString(num, 2) + ", original: " + Convert.ToString(areaMask, 2));
				}
			}

			AreaData result;
			if (levelData.areaMaskCodeMap.TryGetValue(num, out result))
			{
				return result;
			}

			return UnknownDummyData;
		}


		public static int GetCurrentAreaMask(Vector3 position)
		{
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(position, out navMeshHit, 1f, 2147483647))
			{
				return navMeshHit.mask;
			}

			int num = Physics.RaycastNonAlloc(position + Vector3.up, Vector3.down, hits, 50f,
				GameConstants.LayerMask.GROUND_LAYER);
			for (int i = 0; i < num; i++)
			{
				GroundNavFlag groundNavFlag;
				if (hits[i].collider.TryGetComponent<GroundNavFlag>(out groundNavFlag))
				{
					return groundNavFlag.GetMaskCode;
				}
			}

			return 0;
		}
	}
}