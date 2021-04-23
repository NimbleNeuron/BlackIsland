using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class FavoriteBoardArea : BaseControl
	{
		[SerializeField] private Text number = default;


		[SerializeField] private Text areaName = default;


		private int areaCode = default;

		
		
		public event Action<int> pointerEnter = delegate { };


		
		
		public event Action pointerExit = delegate { };


		public void SetAreaSlot(int index, int areaCode)
		{
			this.areaCode = areaCode;
			number.text = index.ToString();
			areaName.text = Ln.Get(LnType.Area_Name, areaCode.ToString());
			gameObject.SetActive(true);
		}


		public void SetHideAreaSlot()
		{
			gameObject.SetActive(false);
		}


		public override void OnPointerEnter(PointerEventData eventData)
		{
			pointerEnter(areaCode);
		}


		public override void OnPointerExit(PointerEventData eventData)
		{
			pointerExit();
		}
	}
}