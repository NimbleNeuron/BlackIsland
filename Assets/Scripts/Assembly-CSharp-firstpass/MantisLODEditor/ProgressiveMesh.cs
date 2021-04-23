using System;
using System.Collections.Generic;
using UnityEngine;

namespace MantisLODEditor
{
	[Serializable]
	public class ProgressiveMesh : ScriptableObject
	{
		public int[] triangles;


		public string[] uuids;


		public Dictionary<string, Lod_Mesh[]> lod_meshes;
	}
}