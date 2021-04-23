using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
	public class ListItem6 : MonoBehaviour
	{
		public List<ListItem5> mItemList;


		public void Init()
		{
			foreach (ListItem5 listItem in mItemList)
			{
				listItem.Init();
			}
		}
	}
}