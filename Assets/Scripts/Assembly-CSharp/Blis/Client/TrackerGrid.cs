using System.Collections.Generic;
using UnityEngine;

namespace Blis.Client
{
	public class TrackerGrid
	{
		private readonly List<Item> sorted = new List<Item>();


		private readonly List<Item> tracks = new List<Item>();

		public void AddGridItem(BaseTrackUI ui)
		{
			if (!tracks.Exists(x => x.obj == ui))
			{
				tracks.Add(new Item(ui, ui.transform.position, ui.rectTransform.rect.size));
			}
		}


		public void RemoveGridItem(BaseTrackUI ui)
		{
			tracks.RemoveAll(x => x.obj == ui);
		}


		public void UpdateGrid()
		{
			sorted.Clear();
			for (int i = 0; i < tracks.Count; i++)
			{
				tracks[i].Reset();
				UpdatePosition(tracks[i]);
				sorted.Add(tracks[i]);
			}
		}


		private Vector2 UpdatePosition(Item item, int loopCount = 0)
		{
			if (loopCount++ < 3)
			{
				Item item2 = null;
				for (int i = 0; i < sorted.Count; i++)
				{
					if (IsOverlap(sorted[i], item, RectTransform.Axis.Horizontal) &&
					    IsOverlap(sorted[i], item, RectTransform.Axis.Vertical))
					{
						item2 = sorted[i];
						break;
					}
				}

				if (item2 != null)
				{
					Vector2 vector = (item.size + item2.size) * 0.5f;
					Vector2 vector2 = item.position - item.OriPosition;
					Vector2 position = item.position;
					Vector2 vector5;
					if (Mathf.Abs(vector2.x) < Mathf.Epsilon)
					{
						item.position.x = item2.position.x - vector.x;
						Vector2 vector3 = UpdatePosition(item, loopCount);
						item.position = position;
						item.position.x = item2.position.x + vector.x;
						Vector2 vector4 = UpdatePosition(item, loopCount);
						vector5 = (item.OriPosition - vector3).sqrMagnitude < (item.OriPosition - vector4).sqrMagnitude
							? vector3
							: vector4;
					}
					else if (vector2.x > 0f)
					{
						item.position.x = item2.position.x + vector.x;
						vector5 = UpdatePosition(item, loopCount);
					}
					else
					{
						item.position.x = item2.position.x - vector.x;
						vector5 = UpdatePosition(item, loopCount);
					}

					item.position = position;
					Vector2 vector8;
					if (Mathf.Abs(vector2.y) < Mathf.Epsilon)
					{
						item.position.y = item2.position.y - vector.y;
						Vector2 vector6 = UpdatePosition(item, loopCount);
						item.position = position;
						item.position.y = item2.position.y + vector.y;
						Vector2 vector7 = UpdatePosition(item, loopCount);
						vector8 = (item.OriPosition - vector6).sqrMagnitude < (item.OriPosition - vector7).sqrMagnitude
							? vector6
							: vector7;
					}
					else if (vector2.y > 0f)
					{
						item.position.y = item2.position.y + vector.y;
						vector8 = UpdatePosition(item, loopCount);
					}
					else
					{
						item.position.y = item2.position.y - vector.y;
						vector8 = UpdatePosition(item, loopCount);
					}

					item.position =
						(item.OriPosition - vector5).sqrMagnitude < (item.OriPosition - vector8).sqrMagnitude
							? vector5
							: vector8;
				}
			}

			return item.position;
		}


		private bool IsOverlap(Item a, Item b, RectTransform.Axis axis)
		{
			if (axis == RectTransform.Axis.Horizontal)
			{
				if (Mathf.Abs(a.position.x - b.position.x) < (a.size.x + b.size.x) * 0.5f)
				{
					return true;
				}
			}
			else if (Mathf.Abs(a.position.y - b.position.y) < (a.size.y + b.size.y) * 0.5f)
			{
				return true;
			}

			return false;
		}


		private class Item
		{
			public readonly BaseTrackUI obj;


			public readonly Vector2 size;


			public Vector2 position;


			public Item(BaseTrackUI obj, Vector2 position, Vector2 size)
			{
				this.obj = obj;
				this.position = position;
				OriPosition = position;
				this.size = size;
			}


			public Vector2 OriPosition { get; }


			public void Reset()
			{
				position = OriPosition;
			}
		}
	}
}