using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
	public class ResManager : MonoBehaviour
	{
		private static ResManager instance;


		public Sprite[] spriteObjArray;


		private readonly Dictionary<string, Sprite> spriteObjDict = new Dictionary<string, Sprite>();


		private string[] mWordList;


		public static ResManager Get {
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<ResManager>();
				}

				return instance;
			}
		}


		public int SpriteCount => spriteObjArray.Length;


		private void Awake()
		{
			instance = null;
			InitData();
		}


		private void InitData()
		{
			spriteObjDict.Clear();
			foreach (Sprite sprite in spriteObjArray)
			{
				spriteObjDict[sprite.name] = sprite;
			}
		}


		public Sprite GetSpriteByName(string spriteName)
		{
			Sprite result = null;
			if (spriteObjDict.TryGetValue(spriteName, out result))
			{
				return result;
			}

			return null;
		}


		public string GetRandomSpriteName()
		{
			int max = spriteObjArray.Length;
			int num = Random.Range(0, max);
			return spriteObjArray[num].name;
		}


		public Sprite GetSpriteByIndex(int index)
		{
			if (index < 0 || index >= spriteObjArray.Length)
			{
				return null;
			}

			return spriteObjArray[index];
		}


		public string GetSpriteNameByIndex(int index)
		{
			if (index < 0 || index >= spriteObjArray.Length)
			{
				return "";
			}

			return spriteObjArray[index].name;
		}
	}
}