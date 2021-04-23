using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class MinimapIndicatorManager : MonoBehaviour
	{
		private MinimapIndicator currentIndicator;


		private UIMap uiMap;

		public void Init(UIMap uiMap)
		{
			this.uiMap = uiMap;
		}


		public void CreateIndicators()
		{
			GameObject gameObject =
				SingletonMonoBehaviour<ResourceManager>.inst.LoadIndicatorPrefab("MinimapRangeIndicator");
			if (gameObject == null)
			{
				return;
			}

			currentIndicator = Instantiate<GameObject>(gameObject, transform).GetComponent<MinimapIndicator>();
			currentIndicator.HideIndicator();
		}


		public MinimapIndicator GetCurrentIndicator()
		{
			return currentIndicator;
		}


		public void SetIndicator(float range)
		{
			if (range < 8f)
			{
				return;
			}

			try
			{
				if (uiMap == null)
				{
					throw new GameException("uiMap is Null");
				}

				PlayerController inst = SingletonMonoBehaviour<PlayerController>.inst;
				if (inst == null)
				{
					throw new GameException("playerController is Null");
				}

				if (currentIndicator == null)
				{
					throw new GameException("CurrentIndicator is Null");
				}

				Vector2 a = uiMap.WorldPositionToMiniMapPosition(inst.GetMyCharacterPos());
				Vector2 b = uiMap.WorldPositionToMiniMapPosition(inst.GetMyCharacterPos() + new Vector3(range, 0f, 0f));
				float scale = (a - b).magnitude * 2f;
				currentIndicator.SetScale(scale);
				ShowIndicator();
			}
			catch (Exception ex)
			{
				Log.V("[EXCEPTION] " + ex.Message + ": " + ex.StackTrace);
			}
		}


		public void ShowIndicator()
		{
			if (currentIndicator == null)
			{
				return;
			}

			currentIndicator.ShowIndicator();
		}


		public void HideIndicator()
		{
			if (currentIndicator == null)
			{
				return;
			}

			currentIndicator.HideIndicator();
		}
	}
}