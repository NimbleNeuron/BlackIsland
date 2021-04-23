using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TheraBytes.BetterUi
{
	[ExecuteAlways]
	[AddComponentMenu("Better UI/Layout/Anchor Override", 30)]
	public class AnchorOverride : UIBehaviour, IResolutionDependency
	{
		[SerializeField] private AnchorReferenceCollection anchorsFallback = new AnchorReferenceCollection();


		[SerializeField] private AnchorReferenceCollectionConfigCollection anchorsConfigs =
			new AnchorReferenceCollectionConfigCollection();


		private Canvas canvas;


		private AnchorReferenceCollection currentAnchors;


		private RectTransform RectTransform => transform as RectTransform;


		private void Update()
		{
			if (currentAnchors == null)
			{
				currentAnchors = anchorsConfigs.GetCurrentItem(anchorsFallback);
			}

			Vector2 anchorMin = RectTransform.anchorMin;
			Vector2 anchorMax = RectTransform.anchorMax;
			foreach (AnchorReference anchorReference in currentAnchors.Elements)
			{
				Rect rect;
				if (TryGetAnchor(anchorReference, out rect))
				{
					if (anchorReference.MinX != AnchorReference.ReferenceLocation.Disabled)
					{
						anchorMin.x = GetAnchorPosition(anchorReference, rect, anchorReference.MinX).x;
					}

					if (anchorReference.MaxX != AnchorReference.ReferenceLocation.Disabled)
					{
						anchorMax.x = GetAnchorPosition(anchorReference, rect, anchorReference.MaxX).x;
					}

					if (anchorReference.MinY != AnchorReference.ReferenceLocation.Disabled)
					{
						anchorMin.y = GetAnchorPosition(anchorReference, rect, anchorReference.MinY).y;
					}

					if (anchorReference.MaxY != AnchorReference.ReferenceLocation.Disabled)
					{
						anchorMax.y = GetAnchorPosition(anchorReference, rect, anchorReference.MaxY).y;
					}
				}
			}

			RectTransform.anchorMin = anchorMin;
			RectTransform.anchorMax = anchorMax;
		}


		protected override void OnEnable()
		{
			base.OnEnable();
			currentAnchors = anchorsConfigs.GetCurrentItem(anchorsFallback);
		}


		public void OnResolutionChanged()
		{
			currentAnchors = anchorsConfigs.GetCurrentItem(anchorsFallback);
			Update();
		}


		private static Vector2 GetAnchorPosition(AnchorReference a, Rect rect,
			AnchorReference.ReferenceLocation location)
		{
			Vector2 result = default;
			switch (location)
			{
				case AnchorReference.ReferenceLocation.Center:
					result = rect.center;
					break;
				case AnchorReference.ReferenceLocation.Pivot:
					result = rect.min +
					         new Vector2(a.Reference.pivot.x * rect.width, a.Reference.pivot.y * rect.height);
					break;
				case AnchorReference.ReferenceLocation.LowerLeft:
					result = rect.min;
					break;
				case AnchorReference.ReferenceLocation.UpperRight:
					result = rect.max;
					break;
				default:
					throw new NotImplementedException();
			}

			return result;
		}


		private bool TryGetAnchor(AnchorReference anchorRef, out Rect anchorObject)
		{
			anchorObject = default;
			if (anchorRef.Reference == null)
			{
				return false;
			}

			if (canvas == null)
			{
				canvas = transform.GetComponentInParent<Canvas>();
			}

			Rect rect = anchorRef.Reference.ToScreenRect(true, canvas);
			Vector2 min = rect.min;
			Vector2 max = rect.max;
			RectTransform rectTransform = transform.parent as RectTransform;
			Vector2 vector;
			Vector2 vector2;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, min, canvas.worldCamera,
				    out vector) &&
			    RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, max, canvas.worldCamera,
				    out vector2)
			)
			{
				Vector2 size = rectTransform.rect.size;
				vector = new Vector2(0.5f + vector.x / size.x, 0.5f + vector.y / size.y);
				vector2 = new Vector2(0.5f + vector2.x / size.x, 0.5f + vector2.y / size.y);
				anchorObject.min = vector;
				anchorObject.size = vector2 - vector;
				return true;
			}

			return false;
		}


		private bool IsParentOf(Transform _trm)
		{
			return _trm.parent == transform || !(_trm.parent == null) && IsParentOf(_trm.parent);
		}


		[Serializable]
		public class AnchorReference
		{
			public enum ReferenceLocation
			{
				Disabled,


				Center,


				Pivot,


				LowerLeft,


				UpperRight
			}


			[SerializeField] private RectTransform reference = default;


			[SerializeField] private ReferenceLocation minX = default;


			[SerializeField] private ReferenceLocation maxX = default;


			[SerializeField] private ReferenceLocation minY = default;


			[SerializeField] private ReferenceLocation maxY = default;


			
			public RectTransform Reference {
				get => reference;
				internal set => reference = value;
			}


			public ReferenceLocation MinX => minX;


			public ReferenceLocation MaxX => maxX;


			public ReferenceLocation MinY => minY;


			public ReferenceLocation MaxY => maxY;
		}


		[Serializable]
		public class AnchorReferenceCollection : IScreenConfigConnection
		{
			[SerializeField] private List<AnchorReference> elements = new List<AnchorReference>();


			[SerializeField] private string screenConfigName;


			public List<AnchorReference> Elements => elements;


			
			public string ScreenConfigName {
				get => screenConfigName;
				set => screenConfigName = value;
			}
		}


		[Serializable]
		public class AnchorReferenceCollectionConfigCollection : SizeConfigCollection<AnchorReferenceCollection> { }
	}
}