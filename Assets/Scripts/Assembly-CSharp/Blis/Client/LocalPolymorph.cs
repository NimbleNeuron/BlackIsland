using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class LocalPolymorph : MonoBehaviour
	{
		public void Init(LocalCharacter localCharacter)
		{
			if (localCharacter == null)
			{
				return;
			}

			ActiveOnHostileType activeOnHostileType = null;
			GameUtil.BindOrAdd<ActiveOnHostileType>(gameObject, ref activeOnHostileType);
			activeOnHostileType.Init(localCharacter);
		}
	}
}