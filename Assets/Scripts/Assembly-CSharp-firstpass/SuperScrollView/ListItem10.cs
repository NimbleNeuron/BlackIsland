using UnityEngine;

namespace SuperScrollView
{
	public class ListItem10 : MonoBehaviour
	{
		public ListItem9[] mItemList;


		public void Init()
		{
			ListItem9[] array = mItemList;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Init();
			}
		}
	}
}