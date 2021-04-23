using UnityEngine.EventSystems;

namespace Blis.Client
{
	public interface IMapSlicePointerEventHandler
	{
		void OnPointerMapEnter(int areaCode);


		void OnPointerMapExit(int areaCode);


		void OnPointerMapClick(int areaCode, PointerEventData.InputButton mouseButton);
	}
}