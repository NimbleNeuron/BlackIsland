using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIBullet : BaseUI
	{
		[SerializeField] private List<GameObject> bulletBgList;


		[SerializeField] private List<GameObject> bulletList;

		protected override void Awake()
		{
			base.Awake();
			bulletBgList = new List<GameObject>();
			bulletList = new List<GameObject>();
			Transform child = transform.GetChild(0);
			for (int i = 0; i < child.childCount; i++)
			{
				GameObject gameObject = child.GetChild(i).gameObject;
				bulletBgList.Add(gameObject);
			}

			Transform child2 = transform.GetChild(1);
			for (int j = 0; j < child2.childCount; j++)
			{
				GameObject gameObject2 = child2.GetChild(j).gameObject;
				bulletList.Add(gameObject2);
			}
		}


		public void Setup(int remainBullet, int capacity)
		{
			for (int i = 0; i < bulletBgList.Count; i++)
			{
				bulletBgList[i].SetActive(i < capacity);
			}

			for (int j = 0; j < bulletList.Count; j++)
			{
				bulletList[j].SetActive(j < remainBullet);
			}
		}


		public void UpdateBullet(int remainBullet)
		{
			for (int i = 0; i < bulletList.Count; i++)
			{
				bulletList[i].SetActive(i < remainBullet);
			}
		}


		public void UpdateSprite(Sprite image, Sprite bgImage)
		{
			for (int i = 0; i < bulletList.Count; i++)
			{
				bulletList[i].GetComponent<Image>().sprite = image;
			}

			for (int j = 0; j < bulletBgList.Count; j++)
			{
				bulletBgList[j].GetComponent<Image>().sprite = bgImage;
			}
		}
	}
}