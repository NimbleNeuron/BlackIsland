using System.Collections.Generic;
using UnityEngine;

namespace Blis.Client
{
	public class ExtraPointDisplayManager : MonoBehaviour
	{
		private static ExtraPointDisplayManager _instance;


		[SerializeField] private List<ExtraPointDisplayData> extraPointDisplayDatas = new List<ExtraPointDisplayData>();


		public static ExtraPointDisplayManager instance {
			get
			{
				if (_instance == null)
				{
					GameObject gameObject =
						Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ExtraPointDisplayManager"));
					DontDestroyOnLoad(gameObject);
					_instance = gameObject.GetComponent<ExtraPointDisplayManager>();
				}

				return _instance;
			}
		}


		public ExtraPointDisplayData GetData(int characterCode)
		{
			foreach (ExtraPointDisplayData extraPointDisplayData in extraPointDisplayDatas)
			{
				if (extraPointDisplayData.CharacterCode == characterCode)
				{
					return extraPointDisplayData;
				}
			}

			return null;
		}
	}
}