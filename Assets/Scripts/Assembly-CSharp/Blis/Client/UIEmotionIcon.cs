using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIEmotionIcon : BaseTrackUI
	{
		private readonly List<Image> emotionIcons = new List<Image>();


		private readonly List<Renderer> renderers = new List<Renderer>();


		private bool isImmediatelyDestory;


		private string playingSoundName;


		protected override void OnDestroy()
		{
			ReleaseTracking();
			if (playingSoundName != null)
			{
				Singleton<SoundControl>.inst.StopFxSoundChild(transform, playingSoundName);
			}

			if (!isImmediatelyDestory && SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene)
			{
				GameUI inst = MonoBehaviourInstance<GameUI>.inst;
				if (inst != null)
				{
					EmotionPlateHud emotionPlateHud = inst.EmotionPlateHud;
					if (emotionPlateHud == null)
					{
						return;
					}

					emotionPlateHud.OnFinishEmotionIcon();
				}
			}
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			RebuildRenderer();
		}


		private void RebuildRenderer()
		{
			gameObject.GetComponentsInChildren<Image>(emotionIcons);
			gameObject.GetComponentsInChildren<Renderer>(renderers);
		}


		public void DestroyAfterSecond(float second)
		{
			isImmediatelyDestory = second == 0f;
			Destroy(gameObject, second);
		}


		public void Show()
		{
			if (emotionIcons != null)
			{
				for (int i = 0; i < emotionIcons.Count; i++)
				{
					emotionIcons[i].enabled = true;
				}
			}

			if (renderers != null)
			{
				for (int j = 0; j < renderers.Count; j++)
				{
					if (renderers[j] == null)
					{
						renderers.RemoveAt(j--);
					}
					else
					{
						renderers[j].enabled = true;
					}
				}
			}
		}


		public void Hide()
		{
			if (emotionIcons != null)
			{
				for (int i = 0; i < emotionIcons.Count; i++)
				{
					emotionIcons[i].enabled = false;
				}
			}

			if (renderers != null)
			{
				for (int j = 0; j < renderers.Count; j++)
				{
					if (renderers[j] == null)
					{
						renderers.RemoveAt(j--);
					}
					else
					{
						renderers[j].enabled = false;
					}
				}
			}
		}


		public void PlaySound(Transform parent, string name)
		{
			playingSoundName = name;
			if (name != null)
			{
				Singleton<SoundControl>.inst.PlayFXSoundChild(name, "EmotionIcon", 8, false, parent, false);
			}
		}
	}
}