using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class HyperLinker : UIBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler,
		IPointerClickHandler
	{
		private readonly Dictionary<string, HyperLinkBlock> hyperlinks = new Dictionary<string, HyperLinkBlock>();


		private RectTransform canvasRect;


		private Camera mainCamera;


		private RectTransform rectTransform;


		private Text targetText;

		protected override void Awake()
		{
			rectTransform = (RectTransform) transform;
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
		}


		protected override void OnEnable()
		{
			base.OnEnable();
			targetText = GetComponent<Text>();
			if (targetText == null)
			{
				enabled = false;
			}

			canvasRect = MonoBehaviourInstance<GameUI>.inst.transform as RectTransform;
		}


		public void OnPointerClick(PointerEventData eventData)
		{
			foreach (KeyValuePair<string, HyperLinkBlock> keyValuePair in hyperlinks)
			{
				Vector2 localPos =
					GameUIUtility.ScreenToRectPos(mainCamera, canvasRect, rectTransform, eventData.position);
				localPos.x *= rectTransform.lossyScale.x;
				if (keyValuePair.Value.Contains(localPos))
				{
					keyValuePair.Value.cbEvent();
					break;
				}
			}
		}


		public void OnPointerEnter(PointerEventData eventData) { }


		public void OnPointerExit(PointerEventData eventData) { }


		public void AddHyperLink(string regex, Action callback)
		{
			if (callback != null)
			{
				if (hyperlinks.ContainsKey(regex))
				{
					hyperlinks.Remove(regex);
				}

				hyperlinks.Add(regex, new HyperLinkBlock(GetBoundsList(regex), callback));
			}
		}


		public void ClearLinks()
		{
			hyperlinks.Clear();
		}


		private List<Bounds> GetBoundsList(string regexString)
		{
			Match match = new Regex(regexString).Match(targetText.text);
			if (match.Length > 0)
			{
				targetText.cachedTextGenerator.Populate(targetText.text,
					targetText.GetGenerationSettings(targetText.rectTransform.rect.size));
				UICharInfo[] charactersArray = targetText.cachedTextGenerator.GetCharactersArray();
				List<Bounds> list = new List<Bounds>();
				float num = float.MaxValue;
				for (int i = match.Index; i < match.Index + match.Length; i++)
				{
					if (charactersArray[i].charWidth > 0f)
					{
						Vector2 cursorPos = charactersArray[i].cursorPos;
						Vector2 vector = new Vector2(charactersArray[i].charWidth, targetText.font.lineHeight);
						Vector2 vector2 = new Vector2(cursorPos.x + vector.x * 0.5f, cursorPos.y - vector.y * 0.5f);
						if (Math.Abs(num - charactersArray[i].cursorPos.y) > Mathf.Epsilon)
						{
							list.Add(new Bounds(vector2, vector));
							num = charactersArray[i].cursorPos.y;
						}
						else
						{
							Bounds value = list[list.Count - 1];
							value.Encapsulate(vector2 * 2f - cursorPos);
							list[list.Count - 1] = value;
						}
					}
				}

				return list;
			}

			return null;
		}


		private IEnumerator MonitoringHoverEvent()
		{
			for (;;)
			{
				foreach (KeyValuePair<string, HyperLinkBlock> keyValuePair in hyperlinks)
				{
					Vector2 b = Camera.main.ScreenToViewportPoint(Input.mousePosition);
					Vector2 v = canvasRect.sizeDelta * b;
					Vector2 localPos = transform.InverseTransformPoint(v);
					keyValuePair.Value.Contains(localPos);
				}
			}
		}


		private class HyperLinkBlock
		{
			private readonly List<Bounds> boundsList;
			public readonly Action cbEvent;

			public HyperLinkBlock(List<Bounds> boundsList, Action cbEvent)
			{
				this.boundsList = boundsList;
				this.cbEvent = cbEvent;
			}


			public bool Contains(Vector2 localPos)
			{
				for (int i = 0; i < boundsList.Count; i++)
				{
					if (boundsList[i].Contains(localPos))
					{
						return true;
					}
				}

				return false;
			}
		}
	}
}