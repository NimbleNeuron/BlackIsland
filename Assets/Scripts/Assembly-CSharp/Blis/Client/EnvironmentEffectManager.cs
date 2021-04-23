using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class EnvironmentEffectManager : MonoBehaviourInstance<EnvironmentEffectManager>
	{
		[SerializeField] private GameObject whiteHolePrefab = default;


		[SerializeField] private GameObject lastSafeZonePrefab = default;


		private readonly Dictionary<Vector3, EnvironmentEffect> effects =
			new Dictionary<Vector3, EnvironmentEffect>(SingletonComparerStruct<Vector3EqualityComparer, Vector3>
				.Instance);


		private LevelData currentLevel;

		public void Init(LevelData level)
		{
			currentLevel = level;
			List<CharacterSpawnPoint> characterSpawnPoints = currentLevel.characterSpawnPoints;
			for (int i = 0; i < characterSpawnPoints.Count; i++)
			{
				Vector3 position = characterSpawnPoints[i].transform.position;
				GameObject go = Instantiate<GameObject>(whiteHolePrefab, position, Quaternion.identity, transform);
				AddEnvironment(go);
			}

			if (lastSafeZonePrefab != null)
			{
				List<SecurityConsoleSpawnPoint> securityConsoleSpawnPoints = currentLevel.securityConsoleSpawnPoints;
				for (int j = 0; j < securityConsoleSpawnPoints.Count; j++)
				{
					Vector3 position2 = securityConsoleSpawnPoints[j].transform.position;
					GameObject go2 =
						Instantiate<GameObject>(lastSafeZonePrefab, position2, Quaternion.identity, transform);
					AddEnvironment(go2);
				}
			}
		}


		private void AddEnvironment(GameObject go)
		{
			EnvironmentEffect environmentEffect = null;
			GameUtil.Bind<EnvironmentEffect>(go, ref environmentEffect);
			if (environmentEffect != null)
			{
				if (effects.ContainsKey(go.transform.position))
				{
					Log.E("[AddEnvironment] Position Duplicated");
					return;
				}

				effects.Add(go.transform.position, environmentEffect);
			}
		}


		private IEnumerable<T> SampleEnvironment<T>(Vector3 pos, float sampleDistance) where T : EnvironmentEffect
		{
			float sqrDistance = Mathf.Pow(sampleDistance, 2f);
			foreach (Vector3 vector in effects.Keys)
			{
				if ((vector - pos).sqrMagnitude <= sqrDistance && effects[vector] is T)
				{
					yield return (T) effects[vector];
				}
			}
		}


		public void InvokeEvent<T>(Vector3 pos, float sampleDistance, string eventKey) where T : EnvironmentEffect
		{
			foreach (T t in SampleEnvironment<T>(pos, sampleDistance))
			{
				t.PlayAnimation(eventKey);
			}
		}
	}
}