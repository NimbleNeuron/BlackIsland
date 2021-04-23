using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	[RequireComponent(typeof(GridLayoutGroup))]
	public class UITextGrid : BaseUI
	{
		[SerializeField] private GameObject textObjectPrefab = default;


		private readonly List<Text> textPool = new List<Text>();


		private int currentIndex = default;


		private GridLayoutGroup grid = default;


		[HideInInspector] public int Count => currentIndex;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			grid = GetComponent<GridLayoutGroup>();
		}


		private Text GetTextItem()
		{
			Text text;
			if (currentIndex >= textPool.Count)
			{
				text = Instantiate<GameObject>(textObjectPrefab, grid.transform).GetComponent<Text>();
				textPool.Add(text);
			}
			else
			{
				text = textPool[currentIndex];
			}

			currentIndex++;
			text.color = Color.white;
			text.gameObject.SetActive(true);
			return text;
		}


		public void PushText(string text)
		{
			GetTextItem().text = text;
		}


		public Text GetText(int index)
		{
			if (index < currentIndex)
			{
				return textPool[index];
			}

			return null;
		}


		public void Clear()
		{
			currentIndex = 0;
			textPool.ForEach(delegate(Text x) { x.gameObject.SetActive(false); });
		}
	}
}