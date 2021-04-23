using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ShaderChanger
{
	[MenuItem("Editor/Change Autodesk Materials")]
	private static void ChangeAutodeskMaterials()
	{
		string[] materialsFiles = AssetDatabase.FindAssets("t:Material")
			.Select(AssetDatabase.GUIDToAssetPath).ToArray();

		const string target = "368f2f3889a1e38499afe276851ba1a4";
		
		AssetDatabase.StartAssetEditing();

		int length = materialsFiles.Length;
		for (int i = 0; i < length; i++)
		{
			string material = materialsFiles[i];

			EditorUtility.DisplayProgressBar("Processing...",
				$"{material}", (float) i / length);
			
			string read = File.ReadAllText(material);

			if (read.Contains(target))
			{
				Debug.Log(material);

				string newData = read.Replace("m_Shader: {fileID: 4800000, guid: 368f2f3889a1e38499afe276851ba1a4, type: 3}", 
					"m_Shader: {fileID: 47, guid: 0000000000000000f000000000000000, type: 0}");
				File.WriteAllText(material, newData);

				AssetDatabase.ImportAsset(material);
			}
		}

		EditorUtility.ClearProgressBar();
		AssetDatabase.StopAssetEditing();
	}
}