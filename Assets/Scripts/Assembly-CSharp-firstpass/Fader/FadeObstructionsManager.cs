using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FadeObstructionsManager : MonoBehaviour
{
	private static FadeObstructionsManager instance;


	public Camera Camera;


	public float FadeOutSeconds = 1f;


	public float FadeInSeconds = 1f;


	public float RayRadius = 0.25f;


	public float FinalAlpha = 0.1f;


	public LayerMask LayerMask = -1;


	private readonly List<FadeObject> HiddenObjects = new List<FadeObject>();


	private readonly List<GameObject> objectsInWay = new List<GameObject>();


	private readonly List<GameObject> ShouldBeVisibleObjects = new List<GameObject>();


	private bool loggedCameraError;


	public static FadeObstructionsManager Instance => instance;


	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			return;
		}

		Debug.LogError("There should be only one FadeObstructingObjects component in your scene");
	}


	private void Update()
	{
		if (Camera == null)
		{
			if (!loggedCameraError)
			{
				Debug.LogError("You need to set the camera for Fade Obstructing Objects to work", this);
				loggedCameraError = true;
			}

			return;
		}

		loggedCameraError = false;
		objectsInWay.Clear();
		foreach (GameObject gameObject in ShouldBeVisibleObjects)
		{
			Vector3 value = gameObject.transform.position - Camera.transform.position;
			foreach (RaycastHit raycastHit in Physics.SphereCastAll(
				new Ray(Camera.transform.position, Vector3.Normalize(value)), RayRadius, value.magnitude,
				LayerMask.value))
			{
				Collider collider = raycastHit.collider;
				if (!ShouldBeVisibleObjects.Contains(collider.gameObject) &&
				    !(collider.gameObject.GetComponent<Renderer>() == null))
				{
					objectsInWay.Add(collider.gameObject);
				}
			}
		}

		FadeObstructions(objectsInWay);
	}


	public bool IsHidden(GameObject go)
	{
		return HiddenObjects.FirstOrDefault(x => x.GameObject == go) != null;
	}


	public void RegisterShouldBeVisible(GameObject shouldBeVisible)
	{
		ShouldBeVisibleObjects.Add(shouldBeVisible);
		shouldBeVisible.AddComponent<NotifyFadeSystem>();
	}


	public void UnRegisterShouldBeVisible(GameObject shouldBeVisible)
	{
		ShouldBeVisibleObjects.Remove(shouldBeVisible);
	}


	private void FadeObstructions(List<GameObject> objectsInWay)
	{
		using (List<GameObject>.Enumerator enumerator = objectsInWay.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				GameObject go = enumerator.Current;
				if (HiddenObjects.FirstOrDefault(x => x.GameObject == go) == null)
				{
					FadeObjectOptions component = go.GetComponent<FadeObjectOptions>();
					if (!(component != null) || !component.OverrideFinalAlpha || component.FinalAlpha != 1f)
					{
						FadeObject fadeObject = new FadeObject
						{
							GameObject = go,
							TransparencyLevel = 1f
						};
						fadeObject.Options = component;
						HiddenObjects.Add(fadeObject);
						go.AddComponent<NotifyFadeSystem>();
					}
				}
			}
		}

		HiddenObjects.RemoveAll(delegate(FadeObject x)
		{
			float num = x.Options != null && x.Options.OverrideFadeOutSeconds
				? x.Options.FadeOutSeconds
				: FadeOutSeconds;
			float num2 = x.Options != null && x.Options.OverrideFadeInSeconds ? x.Options.FadeInSeconds : FadeInSeconds;
			using (List<GameObject>.Enumerator enumerator2 = objectsInWay.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current == x.GameObject)
					{
						float num3 = x.Options != null && x.Options.OverrideFinalAlpha
							? x.Options.FinalAlpha
							: FinalAlpha;
						if (x.TransparencyLevel > num3)
						{
							x.TransparencyLevel -= Time.deltaTime * (1f / num);
							if (x.TransparencyLevel <= num3)
							{
								x.TransparencyLevel = num3;
							}

							foreach (Material material in x.GameObject.GetComponent<Renderer>().materials)
							{
								material.color = new Color(material.color.r, material.color.g, material.color.b,
									x.TransparencyLevel);
							}

							if (x.TransparencyLevel == num3 && x.TransparencyLevel == 0f)
							{
								x.GameObject.GetComponent<Renderer>().enabled = false;
							}
						}

						return false;
					}
				}
			}

			if (x.TransparencyLevel < 1f)
			{
				if (x.TransparencyLevel == 0f)
				{
					x.GameObject.GetComponent<Renderer>().enabled = true;
				}

				x.TransparencyLevel += Time.deltaTime * (1f / num2);
				if (x.TransparencyLevel > 1f)
				{
					x.TransparencyLevel = 1f;
				}

				foreach (Material material2 in x.GameObject.GetComponent<Renderer>().materials)
				{
					material2.color = new Color(material2.color.r, material2.color.g, material2.color.b,
						x.TransparencyLevel);
				}

				return false;
			}

			Destroy(x.GameObject.GetComponent<NotifyFadeSystem>());
			return true;
		});
	}


	internal void RemoveFadingObject(GameObject gameObject)
	{
		HiddenObjects.RemoveAll(x => x.GameObject == gameObject);
	}


	private class FadeObject
	{
		
		public GameObject GameObject { get; set; }


		
		public FadeObjectOptions Options { get; set; }


		
		public float TransparencyLevel { get; set; }
	}


	private class NotifyFadeSystem : MonoBehaviour
	{
		private void OnDestroy()
		{
			Instance.RemoveFadingObject(gameObject);
			Instance.UnRegisterShouldBeVisible(gameObject);
		}
	}
}