using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class MinimapMatrixMaker : MonoBehaviour
	{
		public Transform f;


		public Transform t;


		public Transform worldPos;


		public string spawnPointPath = "Default";

		[ContextMenu("ShowLocalToWorldMatrix")]
		public void Show()
		{
			Log.E(transform.localToWorldMatrix.ToString());
		}


		[ContextMenu("ShowWorldToLocalMatrix")]
		public void Show2()
		{
			Log.E(transform.worldToLocalMatrix.ToString());
		}


		[ContextMenu("Show Length")]
		public void ASDF()
		{
			Log.E((f.position - t.position).magnitude.ToString());
		}


		[ContextMenu("ToLocal")]
		public void ToLocal()
		{
			SingletonMonoBehaviour<ResourceManager>.inst.GetSpawnPoints<CharacterSpawnPoint>(spawnPointPath +
				"/InitialCharacterSpawnPoints");
			Log.E(((IEnumerable<CharacterSpawnPoint>) null).Where<CharacterSpawnPoint>(x => !x.IsUsed)
				.ToList<CharacterSpawnPoint>().Count.ToString());

			// SingletonMonoBehaviour<ResourceManager>.inst.GetSpawnPoints<CharacterSpawnPoint>(this.spawnPointPath + "/InitialCharacterSpawnPoints");
			// Log.E(Enumerable.Where((IEnumerable<TSource>)null, (CharacterSpawnPoint x) => !x.IsUsed).ToList<CharacterSpawnPoint>().Count.ToString());
		}
	}
}