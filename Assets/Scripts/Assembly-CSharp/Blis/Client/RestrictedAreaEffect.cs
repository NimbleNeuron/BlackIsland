using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class RestrictedAreaEffect : MonoBehaviourInstance<RestrictedAreaEffect>
	{
		public Transform levelRoot;


		private readonly Dictionary<int, List<RestrictedEffect>> effectMap =
			new Dictionary<int, List<RestrictedEffect>>();

		public void Init(LevelData levelData)
		{
			if (levelRoot == null)
			{
				return;
			}

			RestrictedEffect[] componentsInChildren = levelRoot.GetComponentsInChildren<RestrictedEffect>(false);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Init(levelData);
				int areaCode = componentsInChildren[i].GetAreaCode();
				if (areaCode == 0)
				{
					Log.E("[RestrictedAreaEffect] Effect[{1}] Position ({0}) isn't localted on navMesh.",
						componentsInChildren[i].transform.position, componentsInChildren[i].name);
				}
				else
				{
					if (!effectMap.ContainsKey(areaCode))
					{
						effectMap.Add(areaCode, new List<RestrictedEffect>());
					}

					effectMap[areaCode].Add(componentsInChildren[i]);
				}
			}
		}


		public void UpdateRestrictedArea(LevelData levelData, Dictionary<int, AreaRestrictionState> areaStateMap,
			float remainRestrictedTime)
		{
			foreach (int num in effectMap.Keys)
			{
				AreaRestrictionState areaState;
				if (areaStateMap.TryGetValue(num, out areaState))
				{
					using (List<RestrictedEffect>.Enumerator enumerator2 = effectMap[num].GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							RestrictedEffect restrictedEffect = enumerator2.Current;
							restrictedEffect.SetState(areaState, remainRestrictedTime);
						}

						continue;
					}
				}

				Log.W("[RestrictedAreaEffect] Area State Not Exist: {0}", num);
			}
		}
	}
}