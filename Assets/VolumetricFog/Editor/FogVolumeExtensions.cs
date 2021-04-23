using UnityEngine;
using UnityEditor;

namespace VolumetricFogAndMist {
    public class FogVolumeExtensions : MonoBehaviour {

        [MenuItem("GameObject/Create Other/Volumetric Fog/Transition Volume")]
        static void CreateFogVolume(MenuCommand menuCommand) {
            GameObject fogVolume = Resources.Load<GameObject>("Prefabs/FogVolume");
            if (fogVolume == null) {
                Debug.LogError("Could not load FogVolume from Resources/Prefabs folder!");
                return;
            }
            GameObject newFogVolume = Instantiate(fogVolume);
            newFogVolume.name = "Volumetric Fog Volume";

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newFogVolume, menuCommand.context as GameObject);

            // Break prefab link
#if UNITY_2018_3_OR_NEWER
#else
            PrefabUtility.DisconnectPrefabInstance(newFogVolume);
#endif

            // Register root object for undo.
            Undo.RegisterCreatedObjectUndo(newFogVolume, "Create Volumetric Fog Volume");
            Selection.activeObject = newFogVolume;

            // Enables fog volumes in fog component
            VolumetricFog fog = VolumetricFog.instance;
            if (fog != null)
                fog.useFogVolumes = true;
        }

        [MenuItem("GameObject/Create Other/Volumetric Fog/Fog Area (Box)")]
        static void CreateFogAreaBox(MenuCommand menuCommand) {
            GameObject fogArea = Resources.Load<GameObject>("Prefabs/FogBoxArea");
            if (fogArea == null) {
                Debug.LogError("Could not load FogBoxArea from Resources/Prefabs folder!");
                return;
            }
            GameObject newFogArea = Instantiate(fogArea);
            newFogArea.name = "Volumetric Fog Area Box";

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newFogArea, menuCommand.context as GameObject);

            // Break prefab link
#if UNITY_2018_3_OR_NEWER
#else
			PrefabUtility.DisconnectPrefabInstance(newFogArea);
#endif


            // Register root object for undo.
            Undo.RegisterCreatedObjectUndo(newFogArea, "Create Volumetric Fog Area (Box)");
            Selection.activeObject = newFogArea;
        }


        [MenuItem("GameObject/Create Other/Volumetric Fog/Fog Area (Spherical)")]
        static void CreateFogAreaSphere(MenuCommand menuCommand) {
            GameObject fogArea = Resources.Load<GameObject>("Prefabs/FogSphereArea");
            if (fogArea == null) {
                Debug.LogError("Could not load FogSphereArea from Resources/Prefabs folder!");
                return;
            }
            GameObject newFogArea = Instantiate(fogArea);
            newFogArea.name = "Volumetric Fog Area Sphere";

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(newFogArea, menuCommand.context as GameObject);

            // Break prefab link
#if UNITY_2018_3_OR_NEWER
#else
			PrefabUtility.DisconnectPrefabInstance(newFogArea);
#endif

            // Register root object for undo.
            Undo.RegisterCreatedObjectUndo(newFogArea, "Create Volumetric Fog Area (Sphere)");
            Selection.activeObject = newFogArea;
        }

        [MenuItem("GameObject/Create Other/Volumetric Fog/Fog Area (Spherical, 2 layers)")]
        static void CreateFogCloud2Layer(MenuCommand menuCommand) {
            GameObject fogArea = Resources.Load<GameObject>("Prefabs/FogSphereArea");
            if (fogArea == null) {
                Debug.LogError("Could not load FogSphereArea from Resources/Prefabs folder!");
                return;
            }

            GameObject root = new GameObject("Volumetric Fog Cloud Layers");
            Vector3 pos = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
            pos.y += 70;
            root.transform.position = pos;
            VolumetricFog fog = InstantiateCloudLayer(fogArea, 1).GetComponent<VolumetricFog>();
            fog.transform.SetParent(root.transform, false);
            fog.transform.localScale = new Vector3(2000, 60f, 2000);
            fog.density = 1f;
            fog.height = 60f;
            fog.noiseStrength = 0.778f;
            fog.noiseScale = 4.37f;
            fog.noiseSparse = 0.418f;
            fog.noiseFinalMultiplier = 1f;
            fog.distance = 0;
            fog.distanceFallOff = 0f;

            fog = InstantiateCloudLayer(fogArea, 2).GetComponent<VolumetricFog>();
            fog.transform.SetParent(root.transform, false);
            fog.transform.localPosition += new Vector3(0, 50, 0);
            fog.transform.localScale = new Vector3(2000, 50, 2000);
            fog.density = 1f;
            fog.height = 50f;
            fog.noiseStrength = 1f;
            fog.noiseScale = 5.57f;
            fog.noiseSparse = 0.458f;
            fog.noiseFinalMultiplier = 1f;
            fog.distance = 0;
            fog.distanceFallOff = 0f;


            Selection.activeObject = root;
        }

        static GameObject InstantiateCloudLayer(GameObject fogArea, int layerIndex) {
            GameObject newFogArea = Instantiate(fogArea);
            newFogArea.name = "Volumetric Fog Area Layer " + layerIndex;

            // Break prefab link
#if UNITY_2018_3_OR_NEWER
#else
			PrefabUtility.DisconnectPrefabInstance(newFogArea);
#endif
            return newFogArea;

        }


        [MenuItem("GameObject/Create Other/Volumetric Fog/Fog Of War Hole")]
        static void CreateFogOfWarHole(MenuCommand menuCommand) {
            GameObject hole = new GameObject("Fog Of War Hole");
            hole.transform.localScale = new Vector3(35f, 1f, 35f);
            hole.AddComponent<FogOfWarHole>();
            Undo.RegisterCreatedObjectUndo(hole, "Create Fog Of War Hole");
            Selection.activeObject = hole;
        }
    }

}
