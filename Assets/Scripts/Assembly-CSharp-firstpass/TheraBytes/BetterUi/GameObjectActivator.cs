using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Helpers/Game Object Activator", 30)]
	public class GameObjectActivator : UIBehaviour, IResolutionDependency
	{
		[SerializeField] private Settings settingsFallback = new Settings();


		[SerializeField] private SettingsConfigCollection customSettings = new SettingsConfigCollection();


		public Settings CurrentSettings => customSettings.GetCurrentItem(settingsFallback);


		protected override void OnEnable()
		{
			base.OnEnable();
			Apply();
		}


		public void OnResolutionChanged()
		{
			Apply();
		}


		public void Apply()
		{
			foreach (GameObject gameObject in CurrentSettings.ActiveObjects)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
			}

			foreach (GameObject gameObject2 in CurrentSettings.InactiveObjects)
			{
				if (gameObject2 != null)
				{
					gameObject2.SetActive(false);
				}
			}
		}


		[Serializable]
		public class Settings : IScreenConfigConnection
		{
			public List<GameObject> ActiveObjects = new List<GameObject>();


			public List<GameObject> InactiveObjects = new List<GameObject>();


			[SerializeField] private string screenConfigName;


			
			public string ScreenConfigName {
				get => screenConfigName;
				set => screenConfigName = value;
			}
		}


		[Serializable]
		public class SettingsConfigCollection : SizeConfigCollection<Settings> { }
	}
}