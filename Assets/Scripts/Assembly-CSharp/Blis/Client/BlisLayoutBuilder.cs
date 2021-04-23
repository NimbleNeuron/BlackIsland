using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class BlisLayoutBuilder : MonoBehaviourInstance<BlisLayoutBuilder>
	{
		private Dictionary<RectTransform, RebuildCall> rebuildMap;


		private RectTransform rectTransform;


		private void LateUpdate()
		{
			if (rebuildMap.Count > 0)
			{
				List<RebuildCall> list = rebuildMap.Values.ToList<RebuildCall>();
				list.Sort();
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Rebuild();
				}

				list.Clear();
				rebuildMap.Clear();
			}
		}

		protected override void _Awake()
		{
			base._Awake();
			rectTransform = (RectTransform) transform;
			rebuildMap = new Dictionary<RectTransform, RebuildCall>();
		}


		public void Rebuild(RectTransform layoutTransform)
		{
			if (!rebuildMap.ContainsKey(layoutTransform))
			{
				RebuildCall value = new RebuildCall(layoutTransform, GetDepth(layoutTransform));
				rebuildMap.Add(layoutTransform, value);
			}
		}


		private int GetDepth(Transform transform)
		{
			Transform parent = transform.parent;
			if (parent == null)
			{
				throw new Exception("This transform is not BlisLayoutBuilder's child.");
			}

			if (parent == rectTransform)
			{
				return 0;
			}

			return GetDepth(parent) + 1;
		}


		private struct RebuildCall : IComparable<RebuildCall>
		{
			public RebuildCall(RectTransform transform, int depth)
			{
				this.transform = transform;
				this.depth = depth;
			}


			public void Rebuild()
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
			}


			public int CompareTo(RebuildCall other)
			{
				return other.depth.CompareTo(depth);
			}


			public readonly RectTransform transform;


			public readonly int depth;
		}
	}
}