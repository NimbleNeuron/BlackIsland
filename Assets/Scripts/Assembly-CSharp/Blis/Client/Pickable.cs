using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class Pickable : MonoBehaviour
	{
		private bool forcePickableDisable;


		private LocalObject obj;


		private int originLayer;


		public void Awake()
		{
			originLayer = gameObject.layer;
		}

		public LocalObject GetObject()
		{
			return obj;
		}


		public void Init(LocalObject obj, bool setLayer = true)
		{
			this.obj = obj;
			SetLayer(setLayer);
		}


		public void EnablePickable(bool enable)
		{
			if (forcePickableDisable)
			{
				enable = false;
			}

			SetLayer(enable);
		}


		public void ForcePickableDisable(bool forcePickableDisable)
		{
			this.forcePickableDisable = forcePickableDisable;
			EnablePickable(!forcePickableDisable);
		}


		private void SetLayer(bool enable)
		{
			if (enable)
			{
				gameObject.layer = GameConstants.LayerNumber.PICKABLE_OBJECT;
				return;
			}

			gameObject.layer = originLayer;
		}
	}
}