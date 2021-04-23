using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class StressTestScene : MonoBehaviour
	{
		public InputField host;


		public InputField botCount;


		public Text console;

		private void Awake()
		{
			SingletonMonoBehaviour<GameDBLoader>.inst.Load(GameConstants.GetDataCacheFilePath());
		}


		public void OnDummyCreate()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = host.text;
			string[] array = host.text.Split(':');
			string text = array[0];
			int port = int.Parse(array[1]);
			int num = int.Parse(botCount.text);
			for (int i = 0; i < num; i++)
			{
				gameObject.AddComponent<DummyClient>().Connect(text, port, 0);
			}
		}
	}
}