using Blis.Common;
using UnityEngine;


public class BlackCurtainUI : MonoBehaviour
{
	
	private void Awake()
	{
		if (!SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene)
		{
			base.gameObject.SetActive(false);
		}
	}
}
