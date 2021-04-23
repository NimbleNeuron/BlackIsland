using System.Collections.Generic;
using BIOutline;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class MouseOverManager
	{
		private readonly List<IHoverble> hover = new List<IHoverble>();

		public readonly Color HoverAllyColor = new Color(0.2f, 0.6f, 1f, 0.94f);


		public readonly Color HoverEnemyColor = new Color(1f, 0.2f, 0.3f, 0.94f);


		public readonly Color HoverNeutralColor = new Color(0.4f, 1f, 1f, 0.97f);


		public readonly Color InSightColor = new Color(0.7f, 0.7f, 0.7f, 0.99f);


		private LocalObject current;


		private void MouseOver(LocalObject localObject, HostileType? hostileType)
		{
			localObject.GetComponents<IHoverble>(hover);
			if (hover != null && hover.Count > 0)
			{
				for (int i = 0; i < hover.Count; i++)
				{
					hover[i].HoverOn();
				}
			}

			SelectionRenderer component = localObject.GetComponent<SelectionRenderer>();
			if (component != null)
			{
				component.SetHover(true);
			}

			SetOutlineColor(localObject, hostileType);
		}


		private void MouseOut(LocalObject localObject, HostileType? hostileType)
		{
			if (localObject == null)
			{
				return;
			}

			localObject.GetComponents<IHoverble>(hover);
			if (hover != null && hover.Count > 0)
			{
				for (int i = 0; i < hover.Count; i++)
				{
					hover[i].HoverOff();
				}
			}

			SelectionRenderer component = localObject.GetComponent<SelectionRenderer>();
			if (component != null)
			{
				component.SetHover(false);
			}

			SetOutlineColor(localObject, hostileType);
		}


		public void SetOutlineColor(LocalObject localObject, HostileType? hostileType)
		{
			SelectionRenderer selectionRenderer;
			if (!localObject.TryGetComponent<SelectionRenderer>(out selectionRenderer))
			{
				return;
			}

			if (selectionRenderer.outlineType == OutlineType.SelectLine)
			{
				return;
			}

			if (hostileType != null)
			{
				HostileType value = hostileType.Value;
				if (value == HostileType.Ally)
				{
					selectionRenderer.SetColor(HoverAllyColor);
					return;
				}

				if (value != HostileType.Enemy)
				{
					selectionRenderer.SetColor(HoverNeutralColor);
					return;
				}

				selectionRenderer.SetColor(HoverEnemyColor);
			}
			else
			{
				OutlineType outlineType = selectionRenderer.outlineType;
				if (outlineType == OutlineType.InSight)
				{
					selectionRenderer.SetColor(InSightColor);
					return;
				}

				selectionRenderer.SetColor(HoverNeutralColor);
			}
		}


		public void Update(LocalObject localObject, HostileType? hostileType)
		{
			if (localObject == null)
			{
				if (current != null)
				{
					MouseOut(current, hostileType);
					current = null;
				}

				return;
			}

			if (current == localObject)
			{
				return;
			}

			MouseOut(current, hostileType);
			current = localObject;
			if (localObject.ObjectType == ObjectType.AirSupplyItemBox &&
			    !localObject.GetComponent<LocalAirSupplyItemBox>().CanOpen)
			{
				current = null;
				return;
			}

			MouseOver(current, hostileType);
		}
	}
}