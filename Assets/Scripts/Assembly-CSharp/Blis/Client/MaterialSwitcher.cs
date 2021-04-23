using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class MaterialSwitcher : MonoBehaviour
	{
		private const string PATH_IN_BUSH = "Materials/Trans/";


		private const string SUFFIX_IN_BUSH = "_Trans";


		private const string PATH_STEALTH = "Materials/Stealth/";


		private const string SUFFIX_STEALTH = "_Stealth";


		private MaterialSwitchType curMaterialType;


		private Material[] inBushMaterials;


		private Material[] originalMaterials;


		private Material[] stealthMaterials;


		private Renderer targetRenderer;

		private void Awake()
		{
			GameUtil.Bind<Renderer>(gameObject, ref targetRenderer);
			originalMaterials = targetRenderer.sharedMaterials;
			inBushMaterials = new Material[originalMaterials.Length];
			for (int i = 0; i < originalMaterials.Length; i++)
			{
				inBushMaterials[i] =
					Resources.Load<Material>("Materials/Trans/" + originalMaterials[i].name + "_Trans");
			}

			MaterialSwitchType materialSwitchType = curMaterialType;
			curMaterialType = MaterialSwitchType.Original;
			if (materialSwitchType != MaterialSwitchType.Original)
			{
				Switch(materialSwitchType);
			}
		}


		public void SetNewOrinalMaterials(int index)
		{
			originalMaterials[index] = targetRenderer.sharedMaterials[index];
			inBushMaterials[index] =
				Resources.Load<Material>("Materials/Trans/" + originalMaterials[index].name + "_Trans");
			MaterialSwitchType materialSwitchType = curMaterialType;
			curMaterialType = MaterialSwitchType.Original;
			if (materialSwitchType != MaterialSwitchType.Original)
			{
				Switch(materialSwitchType);
			}
		}


		public void Switch(MaterialSwitchType targetType)
		{
			if (targetType == curMaterialType)
			{
				return;
			}

			if (!CheckMaterialExist(targetType))
			{
				return;
			}

			curMaterialType = targetType;
			switch (targetType)
			{
				case MaterialSwitchType.Original:
					targetRenderer.sharedMaterials = originalMaterials;
					return;
				case MaterialSwitchType.InBush:
					targetRenderer.sharedMaterials = inBushMaterials;
					return;
				case MaterialSwitchType.Stealth:
					targetRenderer.sharedMaterials = stealthMaterials;
					return;
				default:
					return;
			}
		}


		private bool CheckMaterialExist(MaterialSwitchType targetType)
		{
			switch (targetType)
			{
				case MaterialSwitchType.Original:
					return true;
				case MaterialSwitchType.InBush:
					return true;
				case MaterialSwitchType.Stealth:
					if (stealthMaterials != null)
					{
						return true;
					}

					for (int i = 0; i < originalMaterials.Length; i++)
					{
						Material material =
							Resources.Load<Material>("Materials/Stealth/" + originalMaterials[i].name + "_Stealth");
						if (material == null)
						{
							return false;
						}

						if (stealthMaterials == null)
						{
							stealthMaterials = new Material[originalMaterials.Length];
						}

						stealthMaterials[i] = material;
					}

					return true;
				default:
					return false;
			}
		}
	}
}