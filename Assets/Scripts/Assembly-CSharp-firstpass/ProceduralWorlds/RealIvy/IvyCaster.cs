using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	public class IvyCaster : MonoBehaviour
	{
		public IvyPreset[] ivyPresets;


		public List<IvyController> ivys;


		public IvyController prefabIvyController;


		public void CastIvyByPresetName(string presetName, Vector3 position, Quaternion rotation)
		{
			IvyPreset presetByName = GetPresetByName(presetName);
			CastIvy(presetByName, position, rotation);
		}


		public void CastIvy(IvyPreset ivyPreset, Vector3 position, Quaternion rotation)
		{
			IvyController ivyController = GetFreeIvy();
			if (ivyController == null)
			{
				IvyController ivyController2 = Instantiate<IvyController>(prefabIvyController);
				ivyController2.transform.parent = transform;
				ivyController = ivyController2;
				ivys.Add(ivyController2);
			}

			ivyController.transform.position = position;
			ivyController.transform.rotation = rotation;
			ivyController.transform.Rotate(Vector3.right, -90f);
			ivyController.ivyParameters = ivyPreset.ivyParameters;
			ivyController.gameObject.SetActive(true);
			ivyController.StartGrowth();
		}


		public void CastRandomIvy(Vector3 position, Quaternion rotation)
		{
			int num = Random.Range(0, ivyPresets.Length);
			IvyPreset ivyPreset = ivyPresets[num];
			CastIvy(ivyPreset, position, rotation);
		}


		private IvyController GetFreeIvy()
		{
			IvyController result = null;
			for (int i = 0; i < ivys.Count; i++)
			{
				if (!ivys[i].gameObject.activeSelf)
				{
					result = ivys[i];
					break;
				}
			}

			return result;
		}


		private IvyPreset GetPresetByName(string presetName)
		{
			IvyPreset result = null;
			for (int i = 0; i < ivyPresets.Length; i++)
			{
				if (ivyPresets[i].name == presetName)
				{
					result = ivyPresets[i];
					break;
				}
			}

			return result;
		}
	}
}