﻿using UnityEngine;

namespace UnityStandardAssets.Water
{
	[ExecuteInEditMode]
	public class WaterTile : MonoBehaviour
	{
		public PlanarReflection reflection;


		public WaterBase waterBase;


		public void Start()
		{
			AcquireComponents();
		}


		public void OnWillRenderObject()
		{
			if (reflection)
			{
				reflection.WaterTileBeingRendered(transform, Camera.current);
			}

			if (waterBase)
			{
				waterBase.WaterTileBeingRendered(transform, Camera.current);
			}
		}


		private void AcquireComponents()
		{
			if (!reflection)
			{
				if (transform.parent)
				{
					reflection = transform.parent.GetComponent<PlanarReflection>();
				}
				else
				{
					reflection = transform.GetComponent<PlanarReflection>();
				}
			}

			if (!waterBase)
			{
				if (transform.parent)
				{
					waterBase = transform.parent.GetComponent<WaterBase>();
					return;
				}

				waterBase = transform.GetComponent<WaterBase>();
			}
		}
	}
}