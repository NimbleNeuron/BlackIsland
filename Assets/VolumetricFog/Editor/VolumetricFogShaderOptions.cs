using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace VolumetricFogAndMist {

    public class VolumetricFogShaderOptions {

        public bool pendingChanges;
        public ShaderAdvancedOption[] options;

        public void ReadOptions() {
            pendingChanges = false;
            // Populate known options
            options = new ShaderAdvancedOption[]
            {
                new ShaderAdvancedOption
                {
                    id = "FOG_ORTHO", name = "Orthographic Mode", description = "Enables support for orthographic camera projection."
                },
                new ShaderAdvancedOption
                {
                    id = "FOG_DEBUG",
                    name = "Debug Mode",
                    description = "Enables fog debug view."
                },
                new ShaderAdvancedOption
                {
                    id = "FOG_MASK",
                    name = "Geometry Mask",
                    description = "Enables mask defined by geometry volumes (meshes). Fog will only be visible inside the volumes."
                },
                new ShaderAdvancedOption
                {
                    id = "FOG_INVERTED_MASK",
                    name = "Geometry Mask (Inverted)",
                    description = "Enables mask defined by geometry volumes (meshes). Fog will NOT be visible through the volumes. Note: this option cannot be combined with the previous one."
                },
                new ShaderAdvancedOption
                {
                    id = "FOG_VOID_HEAVY_LOOP",
                    name = "Raymarched Void Area",
                    description = "Computes void within ray loop improving quality."
                },
                new ShaderAdvancedOption
                {
                    id = "FOG_OF_WAR_HEAVY_LOOP",
                    name = "Raymarched Fog Of War",
                    description = "Computes fog of war within ray loop improving quality."
                },
                new ShaderAdvancedOption
                {
                    id = "FOG_SMOOTH_SCATTERING",
                    name = "Smooth Scattering Rays",
                    description = "Adds blur passes to smooth light scattering rays."
                },
                new ShaderAdvancedOption
                {
                    id = "FOG_DIFFUSION",
                    name = "Sun Light Diffusion",
                    description = "Computes diffusion of Sun light across the fog."
                },
                new ShaderAdvancedOption
                {
                    id = "FOG_UNITY_DIR_SHADOWS",
                    name = "Native Directional Shadows",
                    description = "Use Unity directional shadow map to render shadows (Sun shadows option must be enabled)."
                },
new ShaderAdvancedOption
				{
					id = "FOG_DIR_SHADOWS_COOKIE",
					name = "Directional Light Cookie",
					description = "Enables directional light cookie sampling over fog shadow."
				},				
                new ShaderAdvancedOption
                {
                    id = "FOG_MAX_POINT_LIGHTS",
                    name = "",
                    description = "",
                    hasValue = true
                }
            };


            Shader shader = Shader.Find("VolumetricFogAndMist/VolumetricFog");
            if (shader != null) {
                string path = AssetDatabase.GetAssetPath(shader);
                string file = Path.GetDirectoryName(path) + "/VolumetricFogOptions.cginc";
                string[] lines = File.ReadAllLines(file, Encoding.UTF8);
                for (int k = 0; k < lines.Length; k++) {
                    for (int o = 0; o < options.Length; o++) {
                        if (lines[k].Contains(options[o].id)) {
                            options[o].enabled = !lines[k].StartsWith("//");
                            if (options[o].hasValue) {
                                string[] tokens = lines[k].Split(null);
                                if (tokens.Length > 2) {
                                    int.TryParse(tokens[2], out options[o].value);
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }


        public bool GetAdvancedOptionState(string optionId) {
            if (options == null)
                return false;
            for (int k = 0; k < options.Length; k++) {
                if (options[k].id.Equals(optionId)) {
                    return options[k].enabled;
                }
            }
            return false;
        }

        public void UpdateAdvancedOptionsFile() {
            // Reloads the file and updates it accordingly
            Shader shader = Shader.Find("VolumetricFogAndMist/VolumetricFog");
            if (shader != null) {
                string path = AssetDatabase.GetAssetPath(shader);
                string file = Path.GetDirectoryName(path) + "/VolumetricFogOptions.cginc";
                string[] lines = File.ReadAllLines(file, Encoding.UTF8);
                for (int k = 0; k < lines.Length; k++) {
                    for (int o = 0; o < options.Length; o++) {
                        if (lines[k].Contains(options[o].id)) {
                            if (options[o].hasValue) {
                                lines[k] = "#define " + options[o].id + " " + options[o].value;
                            } else {
                                if (options[o].enabled) {
                                    lines[k] = "#define " + options[o].id;
                                } else {
                                    lines[k] = "//#define " + options[o].id;
                                }
                            }
                            break;
                        }
                    }
                }
                File.WriteAllLines(file, lines, Encoding.UTF8);

                // Save VolumetricFog.cs change
                int maxPointLights = GetOptionValue("FOG_MAX_POINT_LIGHTS");
                bool enableSmoothScattering = GetAdvancedOptionState("FOG_SMOOTH_SCATTERING");
                bool useUnityShadowMap = GetAdvancedOptionState("FOG_UNITY_DIR_SHADOWS");
				bool useUnityDirectionalCookie = GetAdvancedOptionState("FOG_DIR_SHADOWS_COOKIE");
                bool enableDiffusion = GetAdvancedOptionState("FOG_DIFFUSION");
                file = Path.GetDirectoryName(path) + "/../../Scripts/VolumetricFogStaticParams.cs";
                if (!File.Exists(file)) {
                    Debug.LogError("VolumetricFogStaticParams.cs file not found!");
                } else {
                    lines = File.ReadAllLines(file, Encoding.UTF8);
                    for (int k = 0; k < lines.Length; k++) {
                        if (lines[k].Contains("MAX_POINT_LIGHTS")) {
                            lines[k] = "public const int MAX_POINT_LIGHTS = " + maxPointLights + ";";
                        } else if (lines[k].Contains("LIGHT_SCATTERING_BLUR_ENABLED")) {
                            string value = enableSmoothScattering ? "true" : "false";
                            lines[k] = "public const bool LIGHT_SCATTERING_BLUR_ENABLED = " + value + ";";
                        } else if (lines[k].Contains("USE_UNITY_SHADOW_MAP")) {
                            string value = useUnityShadowMap ? "true" : "false";
                            lines[k] = "public const bool USE_UNITY_SHADOW_MAP = " + value + ";";
                        } else if (lines[k].Contains("LIGHT_DIFFUSION_ENABLED")) {
                            string value = enableDiffusion ? "true" : "false";
                            lines[k] = "public const bool LIGHT_DIFFUSION_ENABLED = " + value + ";";
                        } else if (lines[k].Contains("USE_DIRECTIONAL_LIGHT_COOKIE")) {
							string value = useUnityDirectionalCookie ? "true" : "false";
							lines[k] = "public const bool USE_DIRECTIONAL_LIGHT_COOKIE = " + value + ";";
                        }
                    }
                    File.WriteAllLines(file, lines, Encoding.UTF8);
                }
            }

            pendingChanges = false;
            AssetDatabase.Refresh();
        }

        public int GetOptionValue(string id) {
            for (int k = 0; k < options.Length;k++) {
                if (options[k].hasValue && options[k].id.Equals(id)) {
                    return options[k].value;
                }
            }
            return 0;
        }

        public void SetOptionValue(string id, int value) {
            for (int k = 0; k < options.Length; k++) {
                if (options[k].hasValue && options[k].id.Equals(id)) {
                    options[k].value = value;
                }
            }
        }


    }

    public struct ShaderAdvancedOption {
        public string id;
        public string name;
        public string description;
        public bool enabled;
        public bool hasValue;
        public int value;
    }


}