//------------------------------------------------------------------------------------------------------------------
// Volumetric Fog & Mist
// Created by Ramiro Oliva (Kronnect)
//------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

#if GAIA_PRESENT
using Gaia;
#endif


namespace VolumetricFogAndMist
{
	public enum FOG_PRESET
	{
		Clear = 0,
		Mist = 10,
		WindyMist = 11,
		LowClouds = 20,
		SeaClouds = 21,
		GroundFog = 30,
		FrostedGround = 31,
		FoggyLake = 32,
		Fog = 41,
		HeavyFog = 42,
		SandStorm1 = 50,
		Smoke = 51,
		ToxicSwamp = 52,
		SandStorm2 = 53,
		WorldEdge = 200,
		Custom = 1000
	}

	public enum SPSR_BEHAVIOUR
	{
		AutoDetectInEditor = 0,
		ForcedOn = 1,
		ForcedOff = 2
	}

	public enum TRANSPARENT_MODE
	{
		None = 0,
		Blend = 1
	}

	public enum COMPUTE_DEPTH_SCOPE
	{
		OnlyTreeBillboards = 0,
		EverythingInLayer = 1,
		TreeBillboardsAndTransparentObjects = 2
	}

	public enum LIGHTING_MODEL
	{
		Classic = 0,
		Natural = 1,
		SingleLight = 2
	}

	public enum SUN_SHADOWS_BAKE_MODE
	{
		Realtime = 0,
		Discrete = 1
	}

	public enum FOG_VOID_TOPOLOGY
	{
		Sphere = 0,
		Box = 1
	}

	public enum FOG_AREA_TOPOLOGY
	{
		Sphere = 1,
		Box = 2
	}

	public enum FOG_AREA_SORTING_MODE
	{
		DistanceToCamera = 0,
		Altitude = 1,
		Fixed = 2
	}

	public enum FOG_AREA_FOLLOW_MODE
	{
		FullXYZ = 0,
		RestrictToXZPlane = 1
	}

	public enum FOG_VISIBILITY_SCOPE
	{
		Global = 0,
		Volume = 1
	}

	public struct FOG_TEMPORARY_PROPERTIES
	{
		public Color color;
		public float density;
	}

	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Rendering/Volumetric Fog & Mist")]
	[HelpURL("http://kronnect.com/taptapgo")]
	public partial class VolumetricFog : MonoBehaviour
	{
		public const string SKW_FOG_DISTANCE_ON = "FOG_DISTANCE_ON";
		public const string SKW_LIGHT_SCATTERING = "FOG_SCATTERING_ON";
		public const string SKW_FOG_AREA_BOX = "FOG_AREA_BOX";
		public const string SKW_FOG_AREA_SPHERE = "FOG_AREA_SPHERE";
		public const string SKW_FOG_VOID_BOX = "FOG_VOID_BOX";
		public const string SKW_FOG_VOID_SPHERE = "FOG_VOID_SPHERE";
		public const string SKW_FOG_HAZE_ON = "FOG_HAZE_ON";
		public const string SKW_FOG_OF_WAR_ON = "FOG_OF_WAR_ON";
		public const string SKW_FOG_BLUR = "FOG_BLUR_ON";
		public const string SKW_SUN_SHADOWS = "FOG_SUN_SHADOWS_ON";
		public const string SKW_FOG_USE_XY_PLANE = "FOG_USE_XY_PLANE";
		public const string SKW_FOG_COMPUTE_DEPTH = "FOG_COMPUTE_DEPTH";
		public const string SKW_POINT_LIGHTS = "FOG_POINT_LIGHTS";

		private const string DEPTH_CAM_NAME = "VFMDepthCamera";
		private const string DEPTH_SUN_CAM_NAME = "VFMDepthSunCamera";
		private const string VFM_BUILD_FIRST_INSTALL = "VFMFirstInstall";
		private const string VFM_BUILD_HINT = "VFMBuildHint106RC2";

		private static VolumetricFog _fog;


		[HideInInspector] public bool
			isDirty;

        /// <summary>
        ///     If this fog instances is being rendered
        /// </summary>
        public bool isRendering;

		public VolumetricFog fogRenderer;
		private VolumetricFog[] allFogRenderers;

		[NonSerialized] public float distanceToCameraMin, distanceToCameraMax, distanceToCamera, distanceToCameraYAxis;

		[NonSerialized] public FOG_TEMPORARY_PROPERTIES temporaryProperties;

		public static VolumetricFog instance {
			get
			{
				if (_fog == null)
				{
					if (Camera.main != null)
					{
						_fog = Camera.main.GetComponent<VolumetricFog>();
					}

					if (_fog == null)
					{
						foreach (Camera camera in Camera.allCameras)
						{
							_fog = camera.GetComponent<VolumetricFog>();
							if (_fog != null)
							{
								break;
							}
						}
					}
				}

				return _fog;
			}
		}


		// State variables
		public Camera fogCamera => mainCamera;

        /// <summary>
        ///     Returns the number of fog instances being rendered
        /// </summary>
        /// <value>The rendering instances count.</value>
        public int renderingInstancesCount => _renderingInstancesCount;

        /// <summary>
        ///     Returns a list of actual rendering fog instances
        /// </summary>
        /// <value>The rendering instances.</value>
        public List<VolumetricFog> renderingInstances { get; } = new List<VolumetricFog>();

        /// <summary>
        ///     Returns a list of registered fog instances
        /// </summary>
        /// <value>The instances.</value>
        public List<VolumetricFog> instances { get; } = new List<VolumetricFog>();

		public bool hasCamera {
			get
			{
				if (!_hasCameraChecked)
				{
					_hasCamera = GetComponent<Camera>() != null;
					_hasCameraChecked = true;
				}

				return _hasCamera;
			}
		}

		#region General settings

		[SerializeField] private FOG_PRESET
			_preset = FOG_PRESET.Mist;

		public FOG_PRESET preset {
			get => _preset;
			set
			{
				if (value != _preset)
				{
					_preset = value;
					UpdatePreset();
					isDirty = true;
				}
			}
		}


		[SerializeField] private VolumetricFogProfile
			_profile;

		public VolumetricFogProfile profile {
			get => _profile;
			set
			{
				if (value != _profile)
				{
					_profile = value;
					if (_profile != null)
					{
						_profile.Load(this);
						_preset = FOG_PRESET.Custom;
					}

					isDirty = true;
				}
			}
		}

		[SerializeField] private bool
			_profileSync;

		public bool profileSync {
			get => _profileSync;
			set
			{
				if (value != _profileSync)
				{
					_profileSync = value;
					isDirty = true;
				}
			}
		}

		[SerializeField] private bool
			_useFogVolumes = false;

		public bool useFogVolumes {
			get => _useFogVolumes;
			set
			{
				if (value != _useFogVolumes)
				{
					_useFogVolumes = value;
					isDirty = true;
				}
			}
		}

		[SerializeField] private bool
			_debugPass = false;

		public bool debugDepthPass {
			get => _debugPass;
			set
			{
				if (value != _debugPass)
				{
					_debugPass = value;
					isDirty = true;
				}
			}
		}


		[SerializeField] private bool
			_showInSceneView = true;

		public bool showInSceneView {
			get => _showInSceneView;
			set
			{
				if (value != _showInSceneView)
				{
					_showInSceneView = value;
					isDirty = true;
				}
			}
		}


		[SerializeField] private TRANSPARENT_MODE _transparencyBlendMode = TRANSPARENT_MODE.None;

		public TRANSPARENT_MODE transparencyBlendMode {
			get => _transparencyBlendMode;
			set
			{
				if (value != _transparencyBlendMode)
				{
					_transparencyBlendMode = value;
					UpdateRenderComponents();
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] [Range(0, 1f)] private float
			_transparencyBlendPower = 1.0f;

		public float transparencyBlendPower {
			get => _transparencyBlendPower;
			set
			{
				if (value != _transparencyBlendPower)
				{
					_transparencyBlendPower = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] private LayerMask
			_transparencyLayerMask = -1;

		public LayerMask transparencyLayerMask {
			get => _transparencyLayerMask;
			set
			{
				if (_transparencyLayerMask != value)
				{
					_transparencyLayerMask = value;
					isDirty = true;
				}
			}
		}


		[SerializeField] private FOG_VISIBILITY_SCOPE _visibilityScope = FOG_VISIBILITY_SCOPE.Global;

		public FOG_VISIBILITY_SCOPE visibilityScope {
			get => _visibilityScope;
			set
			{
				if (_visibilityScope != value)
				{
					_visibilityScope = value;
					isDirty = true;
				}
			}
		}


		[SerializeField] private Bounds _visibilityVolume = new Bounds(Vector3.zero, new Vector3(1000, 1000, 1000));

		public Bounds visibilityVolume {
			get => _visibilityVolume;
			set
			{
				if (_visibilityVolume != value)
				{
					_visibilityVolume = value;
					isDirty = true;
				}
			}
		}


		[SerializeField] private LIGHTING_MODEL _lightingModel = LIGHTING_MODEL.Classic;

		public LIGHTING_MODEL lightingModel {
			get => _lightingModel;
			set
			{
				if (value != _lightingModel)
				{
					_lightingModel = value;
					UpdateMaterialProperties();
					UpdateTexture();
					isDirty = true;
				}
			}
		}


		[SerializeField] private bool
			_enableMultipleCameras = false;

		public bool enableMultipleCameras {
			get => _enableMultipleCameras;
			set
			{
				if (value != _enableMultipleCameras)
				{
					_enableMultipleCameras = value;
					UpdateMultiCameraSetup();
					isDirty = true;
				}
			}
		}


		[SerializeField] private bool
			_computeDepth = false;

		public bool computeDepth {
			get => _computeDepth;
			set
			{
				if (value != _computeDepth)
				{
					_computeDepth = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private COMPUTE_DEPTH_SCOPE
			_computeDepthScope = COMPUTE_DEPTH_SCOPE.OnlyTreeBillboards;

		public COMPUTE_DEPTH_SCOPE computeDepthScope {
			get => _computeDepthScope;
			set
			{
				if (value != _computeDepthScope)
				{
					_computeDepthScope = value;
					if (_computeDepthScope == COMPUTE_DEPTH_SCOPE.TreeBillboardsAndTransparentObjects)
					{
						_transparencyBlendMode = TRANSPARENT_MODE.None;
					}

					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_transparencyCutOff = 0.1f;

		public float transparencyCutOff {
			get => _transparencyCutOff;
			set
			{
				if (value != _transparencyCutOff)
				{
					_transparencyCutOff = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private bool
			_renderBeforeTransparent;

		public bool renderBeforeTransparent {
			get => _renderBeforeTransparent;
			set
			{
				if (value != _renderBeforeTransparent)
				{
					_renderBeforeTransparent = value;
					if (_renderBeforeTransparent)
					{
						_transparencyBlendMode = TRANSPARENT_MODE.None;
					}

					UpdateRenderComponents();
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private GameObject
			_sun;

		public GameObject sun {
			get => _sun;
			set
			{
				if (value != _sun)
				{
					_sun = value;
					UpdateSun();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 0.5f)] private float
			_timeBetweenTextureUpdates = 0.2f;

		public float timeBetweenTextureUpdates {
			get => _timeBetweenTextureUpdates;
			set
			{
				if (value != _timeBetweenTextureUpdates)
				{
					_timeBetweenTextureUpdates = value;
					isDirty = true;
				}
			}
		}

		[SerializeField] private bool
			_sunCopyColor = true;

		public bool sunCopyColor {
			get => _sunCopyColor;
			set
			{
				if (value != _sunCopyColor)
				{
					_sunCopyColor = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		#endregion

		#region Fog Geometry settings

		[SerializeField] [Range(0, 1.25f)] private float
			_density = 1.0f;

		public float density {
			get => _density;
			set
			{
				if (value != _density)
				{
					_preset = FOG_PRESET.Custom;
					_density = value;
					UpdateMaterialProperties();
					UpdateTextureAlpha();
					UpdateTexture();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_noiseStrength = 0.8f;

		public float noiseStrength {
			get => _noiseStrength;
			set
			{
				if (value != _noiseStrength)
				{
					_preset = FOG_PRESET.Custom;
					_noiseStrength = value;
					UpdateMaterialProperties();
					UpdateTextureAlpha();
					UpdateTexture();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(1f, 2f)] private float
			_noiseFinalMultiplier = 1f;

		public float noiseFinalMultiplier {
			get => _noiseFinalMultiplier;
			set
			{
				if (value != _noiseFinalMultiplier)
				{
					_preset = FOG_PRESET.Custom;
					_noiseFinalMultiplier = value;
					UpdateMaterialProperties();
					UpdateTextureAlpha();
					UpdateTexture();
					isDirty = true;
				}
			}
		}


		[SerializeField] [Range(-0.3f, 2f)] private float
			_noiseSparse;

		public float noiseSparse {
			get => _noiseSparse;
			set
			{
				if (value != _noiseSparse)
				{
					_preset = FOG_PRESET.Custom;
					_noiseSparse = value;
					UpdateMaterialProperties();
					UpdateTextureAlpha();
					UpdateTexture();
					isDirty = true;
				}
			}
		}


		[SerializeField] [Range(0, 1000f)] private float
			_distance = 0f;

		public float distance {
			get => _distance;
			set
			{
				if (value != _distance)
				{
					_preset = FOG_PRESET.Custom;
					_distance = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_maxFogLength = 1000f;

		public float maxFogLength {
			get => _maxFogLength;
			set
			{
				if (value != _maxFogLength)
				{
					_maxFogLength = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_maxFogLengthFallOff = 0f;

		public float maxFogLengthFallOff {
			get => _maxFogLengthFallOff;
			set
			{
				if (value != _maxFogLengthFallOff)
				{
					_maxFogLengthFallOff = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 5f)] private float
			_distanceFallOff = 0f;

		public float distanceFallOff {
			get => _distanceFallOff;
			set
			{
				if (value != _distanceFallOff)
				{
					_preset = FOG_PRESET.Custom;
					_distanceFallOff = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0.0001f, 500f)]
		private float
			_height = 4.0f;

		public float height {
			get => _height;
			set
			{
				if (value != _height)
				{
					_preset = FOG_PRESET.Custom;
					_height = Mathf.Max(value, 0.0001f);
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_heightFallOff = 0.6f;

		public float heightFallOff {
			get => _heightFallOff;
			set
			{
				if (value != _heightFallOff)
				{
					_preset = FOG_PRESET.Custom;
					_heightFallOff = Mathf.Clamp01(value);
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] private float
			_deepObscurance = 1f;

		public float deepObscurance {
			get => _deepObscurance;
			set
			{
				if (value != _deepObscurance && value >= 0)
				{
					_preset = FOG_PRESET.Custom;
					_deepObscurance = Mathf.Clamp01(value);
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_baselineHeight = 0f;

		public float baselineHeight {
			get => _baselineHeight;
			set
			{
				if (value != _baselineHeight)
				{
					_preset = FOG_PRESET.Custom;
					_baselineHeight = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private bool
			_baselineRelativeToCamera = false;

		public bool baselineRelativeToCamera {
			get => _baselineRelativeToCamera;
			set
			{
				if (value != _baselineRelativeToCamera)
				{
					_preset = FOG_PRESET.Custom;
					_baselineRelativeToCamera = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_baselineRelativeToCameraDelay = 0;

		public float baselineRelativeToCameraDelay {
			get => _baselineRelativeToCameraDelay;
			set
			{
				if (value != _baselineRelativeToCameraDelay)
				{
					_baselineRelativeToCameraDelay = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_noiseScale = 1;

		public float noiseScale {
			get => _noiseScale;
			set
			{
				if (value != _noiseScale && value >= 0.2f)
				{
					_preset = FOG_PRESET.Custom;
					_noiseScale = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		#endregion

		#region Fog Style settings

		[SerializeField] [Range(0, 1.05f)] private float
			_alpha = 1.0f;

		public float alpha {
			get => _alpha;
			set
			{
				if (value != _alpha)
				{
					_preset = FOG_PRESET.Custom;
					_alpha = value;
					currentFogAlpha = _alpha;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private Color
			_color = new Color(0.89f, 0.89f, 0.89f, 1);

		public Color color {
			get => _color;
			set
			{
				if (value != _color)
				{
					_preset = FOG_PRESET.Custom;
					_color = value;
					currentFogColor = _color;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private Color
			_specularColor = new Color(1, 1, 0.8f, 1);

		public Color specularColor {
			get => _specularColor;
			set
			{
				if (value != _specularColor)
				{
					_preset = FOG_PRESET.Custom;
					_specularColor = value;
					currentFogSpecularColor = _specularColor;
					UpdateTexture();
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_specularThreshold = 0.6f;

		public float specularThreshold {
			get => _specularThreshold;
			set
			{
				if (value != _specularThreshold)
				{
					_preset = FOG_PRESET.Custom;
					_specularThreshold = value;
					UpdateTexture();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_specularIntensity = 0.2f;

		public float specularIntensity {
			get => _specularIntensity;
			set
			{
				if (value != _specularIntensity)
				{
					_preset = FOG_PRESET.Custom;
					_specularIntensity = value;
					UpdateMaterialProperties();
					UpdateTexture();
					isDirty = true;
				}
			}
		}

		[SerializeField] private Vector3
			_lightDirection = new Vector3(1, 0, -1);

		public Vector3 lightDirection {
			get => _lightDirection;
			set
			{
				if (value != _lightDirection)
				{
					_preset = FOG_PRESET.Custom;
					_lightDirection = value;
					UpdateMaterialProperties();
					UpdateTexture();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_lightIntensity = 0.2f;

		public float lightIntensity {
			get => _lightIntensity;
			set
			{
				if (value != _lightIntensity)
				{
					_preset = FOG_PRESET.Custom;
					_lightIntensity = value;
					UpdateMaterialProperties();
					UpdateTexture();
					isDirty = true;
				}
			}
		}

		[SerializeField] private Color
			_lightColor = Color.white;

		public Color lightColor {
			get => _lightColor;
			set
			{
				if (value != _lightColor)
				{
					_preset = FOG_PRESET.Custom;
					_lightColor = value;
					currentLightColor = _lightColor;
					UpdateMaterialProperties();
					UpdateTexture();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(1, 5)] private int
			_updateTextureSpread = 1;

		public int updateTextureSpread {
			get => _updateTextureSpread;
			set
			{
				if (value != _updateTextureSpread)
				{
					_updateTextureSpread = value;
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_speed = 0.01f;

		public float speed {
			get => _speed;
			set
			{
				if (value != _speed)
				{
					_preset = FOG_PRESET.Custom;
					_speed = value;
					if (!Application.isPlaying)
					{
						UpdateWindSpeedQuick();
					}

					isDirty = true;
				}
			}
		}

		[SerializeField] private Vector3
			_windDirection = new Vector3(-1, 0, 0);

		public Vector3 windDirection {
			get => _windDirection;
			set
			{
				if (value != _windDirection)
				{
					_preset = FOG_PRESET.Custom;
					_windDirection = value.normalized;
					if (!Application.isPlaying)
					{
						UpdateWindSpeedQuick();
					}

					isDirty = true;
				}
			}
		}


		[SerializeField] private bool
			_useRealTime = false;

		public bool useRealTime {
			get => _useRealTime;
			set
			{
				if (value != _useRealTime)
				{
					_useRealTime = value;
					isDirty = true;
				}
			}
		}

		#endregion

		#region Sky Haze settings

		[SerializeField] private Color
			_skyColor = new Color(0.89f, 0.89f, 0.89f, 1);

		public Color skyColor {
			get => _skyColor;
			set
			{
				if (value != _skyColor)
				{
					_preset = FOG_PRESET.Custom;
					_skyColor = value;
					ComputeLightColor();
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_skyHaze = 50.0f;

		public float skyHaze {
			get => _skyHaze;
			set
			{
				if (value != _skyHaze)
				{
					_preset = FOG_PRESET.Custom;
					_skyHaze = value;
					if (!Application.isPlaying)
					{
						UpdateWindSpeedQuick();
					}

					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_skyNoiseScale = 1.5f;

		public float skyNoiseScale {
			get => _skyNoiseScale;
			set
			{
				if (value != _skyNoiseScale)
				{
					_skyNoiseScale = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] [Range(0, 1f)] private float
			_skySpeed = 0.3f;

		public float skySpeed {
			get => _skySpeed;
			set
			{
				if (value != _skySpeed)
				{
					_preset = FOG_PRESET.Custom;
					_skySpeed = value;
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_skyNoiseStrength = 0.1f;

		public float skyNoiseStrength {
			get => _skyNoiseStrength;
			set
			{
				if (value != _skyNoiseStrength)
				{
					_preset = FOG_PRESET.Custom;
					_skyNoiseStrength = value;
					if (!Application.isPlaying)
					{
						UpdateWindSpeedQuick();
					}

					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_skyAlpha = 1.0f;

		public float skyAlpha {
			get => _skyAlpha;
			set
			{
				if (value != _skyAlpha)
				{
					_preset = FOG_PRESET.Custom;
					_skyAlpha = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 0.999f)] private float
			_skyDepth = 0.999f;

		public float skyDepth {
			get => _skyDepth;
			set
			{
				if (value != _skyDepth)
				{
					_skyDepth = value;
					if (!Application.isPlaying)
					{
						UpdateWindSpeedQuick();
					}

					isDirty = true;
				}
			}
		}

		#endregion

		#region Fog Void settings

		[SerializeField] private GameObject
			_character;

		public GameObject character {
			get => _character;
			set
			{
				if (value != _character)
				{
					_character = value;
					isDirty = true;
				}
			}
		}


		[SerializeField] private FOG_VOID_TOPOLOGY
			_fogVoidTopology = FOG_VOID_TOPOLOGY.Sphere;

		public FOG_VOID_TOPOLOGY fogVoidTopology {
			get => _fogVoidTopology;
			set
			{
				if (value != _fogVoidTopology)
				{
					_fogVoidTopology = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 10f)] private float
			_fogVoidFallOff = 1.0f;

		public float fogVoidFallOff {
			get => _fogVoidFallOff;
			set
			{
				if (value != _fogVoidFallOff)
				{
					_preset = FOG_PRESET.Custom;
					_fogVoidFallOff = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_fogVoidRadius = 0.0f;

		public float fogVoidRadius {
			get => _fogVoidRadius;
			set
			{
				if (value != _fogVoidRadius)
				{
					_preset = FOG_PRESET.Custom;
					_fogVoidRadius = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private Vector3
			_fogVoidPosition = Vector3.zero;

		public Vector3 fogVoidPosition {
			get => _fogVoidPosition;
			set
			{
				if (value != _fogVoidPosition)
				{
					_preset = FOG_PRESET.Custom;
					_fogVoidPosition = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_fogVoidDepth = 0.0f;

		public float fogVoidDepth {
			get => _fogVoidDepth;
			set
			{
				if (value != _fogVoidDepth)
				{
					_preset = FOG_PRESET.Custom;
					_fogVoidDepth = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_fogVoidHeight = 0.0f;

		public float fogVoidHeight {
			get => _fogVoidHeight;
			set
			{
				if (value != _fogVoidHeight)
				{
					_preset = FOG_PRESET.Custom;
					_fogVoidHeight = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private bool
			_fogVoidInverted = false;

		[Obsolete("Fog Void inverted is now deprecated. Use Fog Area settings.")]
		public bool fogVoidInverted {
			get => _fogVoidInverted;
			set => _fogVoidInverted = value;
		}

		#endregion


		#region Fog Area settings

		[SerializeField] private bool
			_fogAreaShowGizmos = true;

		public bool fogAreaShowGizmos {
			get => _fogAreaShowGizmos;
			set
			{
				if (value != _fogAreaShowGizmos)
				{
					_fogAreaShowGizmos = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] private GameObject
			_fogAreaCenter;

		public GameObject fogAreaCenter {
			get => _fogAreaCenter;
			set
			{
				if (value != _fogAreaCenter)
				{
					_fogAreaCenter = value;
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0.001f, 10f)] private float
			_fogAreaFallOff = 1.0f;

		public float fogAreaFallOff {
			get => _fogAreaFallOff;
			set
			{
				if (value != _fogAreaFallOff)
				{
					_fogAreaFallOff = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private FOG_AREA_FOLLOW_MODE
			_fogAreaFollowMode = FOG_AREA_FOLLOW_MODE.FullXYZ;

		public FOG_AREA_FOLLOW_MODE fogAreaFollowMode {
			get => _fogAreaFollowMode;
			set
			{
				if (value != _fogAreaFollowMode)
				{
					_fogAreaFollowMode = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] private FOG_AREA_TOPOLOGY
			_fogAreaTopology = FOG_AREA_TOPOLOGY.Sphere;

		public FOG_AREA_TOPOLOGY fogAreaTopology {
			get => _fogAreaTopology;
			set
			{
				if (value != _fogAreaTopology)
				{
					_fogAreaTopology = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] private float
			_fogAreaRadius = 0.0f;

		public float fogAreaRadius {
			get => _fogAreaRadius;
			set
			{
				if (value != _fogAreaRadius)
				{
					_fogAreaRadius = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private Vector3
			_fogAreaPosition = Vector3.zero;

		public Vector3 fogAreaPosition {
			get => _fogAreaPosition;
			set
			{
				if (value != _fogAreaPosition)
				{
					_fogAreaPosition = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_fogAreaDepth = 0.0f;

		public float fogAreaDepth {
			get => _fogAreaDepth;
			set
			{
				if (value != _fogAreaDepth)
				{
					_fogAreaDepth = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_fogAreaHeight = 0.0f;

		public float fogAreaHeight {
			get => _fogAreaHeight;
			set
			{
				if (value != _fogAreaHeight)
				{
					_fogAreaHeight = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] private FOG_AREA_SORTING_MODE
			_fogAreaSortingMode = FOG_AREA_SORTING_MODE.DistanceToCamera;

		public FOG_AREA_SORTING_MODE fogAreaSortingMode {
			get => _fogAreaSortingMode;
			set
			{
				if (value != _fogAreaSortingMode)
				{
					_fogAreaSortingMode = value;
					lastTimeSortInstances = 0;
					isDirty = true;
				}
			}
		}

		[SerializeField] private int
			_fogAreaRenderOrder = 1;

		public int fogAreaRenderOrder {
			get => _fogAreaRenderOrder;
			set
			{
				if (value != _fogAreaRenderOrder)
				{
					_fogAreaRenderOrder = value;
					lastTimeSortInstances = 0;
					isDirty = true;
				}
			}
		}

		#endregion


		#region Point Light settings

		[Serializable]
		public struct PointLightParams
		{
			public Light light;
			[HideInInspector] public VolumetricFogLightParams lightParams;
			public float range;
			public float rangeMultiplier;
			public float intensity;
			public float intensityMultiplier;
			public Vector3 position;
			public Color color;
		}

		public PointLightParams[] pointLightParams;

		[SerializeField] private bool pointLightDataMigrated;


		private Vector4[] pointLightColorBuffer;
		private Vector4[] pointLightPositionBuffer;

		[SerializeField] private GameObject[]
			_pointLights = new GameObject[6];

		[SerializeField] private float[]
			_pointLightRanges = new float[6];

		[SerializeField] private float[]
			_pointLightIntensities = new float[6]
			{
				1.0f,
				1.0f,
				1.0f,
				1.0f,
				1.0f,
				1.0f
			};

		[SerializeField] private float[]
			_pointLightIntensitiesMultiplier = new float[6]
			{
				1.0f,
				1.0f,
				1.0f,
				1.0f,
				1.0f,
				1.0f
			};

		[SerializeField] private Vector3[]
			_pointLightPositions = new Vector3[6];

		[SerializeField] private Color[]
			_pointLightColors = new Color[6]
			{
				new Color(1, 1, 0, 1),
				new Color(1, 1, 0, 1),
				new Color(1, 1, 0, 1),
				new Color(1, 1, 0, 1),
				new Color(1, 1, 0, 1),
				new Color(1, 1, 0, 1)
			};

		[SerializeField] private bool
			_pointLightTrackingAuto = false;

		public bool pointLightTrackAuto {
			get => _pointLightTrackingAuto;
			set
			{
				if (value != _pointLightTrackingAuto)
				{
					_pointLightTrackingAuto = value;
					TrackPointLights();
					isDirty = true;
				}
			}
		}

		[SerializeField] private Transform
			_pointLightTrackingPivot;

		public Transform pointLightTrackingPivot {
			get => _pointLightTrackingPivot;
			set
			{
				if (value != _pointLightTrackingPivot)
				{
					_pointLightTrackingPivot = value;
					TrackPointLights();
					isDirty = true;
				}
			}
		}


		[SerializeField] private int
			_pointLightTrackingCount = 0;

		public int pointLightTrackingCount {
			get => _pointLightTrackingCount;
			set
			{
				if (value != _pointLightTrackingCount)
				{
					_pointLightTrackingCount = Mathf.Clamp(value, 0, MAX_POINT_LIGHTS);
					CheckPointLightData();
					TrackPointLights();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 5f)] private float
			_pointLightTrackingCheckInterval = 1f;

		public float pointLightTrackingCheckInterval {
			get => _pointLightTrackingCheckInterval;
			set
			{
				if (value != _pointLightTrackingCheckInterval)
				{
					_pointLightTrackingCheckInterval = value;
					TrackPointLights();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_pointLightTrackingNewLightsCheckInterval = 3f;

		public float pointLightTrackingNewLightsCheckInterval {
			get => _pointLightTrackingNewLightsCheckInterval;
			set
			{
				if (value != _pointLightTrackingNewLightsCheckInterval)
				{
					_pointLightTrackingNewLightsCheckInterval = value;
					TrackPointLights();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_pointLightInscattering = 1f;

		public float pointLightInscattering {
			get => _pointLightInscattering;
			set
			{
				if (value != _pointLightInscattering)
				{
					_pointLightInscattering = value;
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_pointLightIntensity = 1f;

		public float pointLightIntensity {
			get => _pointLightIntensity;
			set
			{
				if (value != _pointLightIntensity)
				{
					_pointLightIntensity = value;
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_pointLightInsideAtten = 0f;

		public float pointLightInsideAtten {
			get => _pointLightInsideAtten;
			set
			{
				if (value != _pointLightInsideAtten)
				{
					_pointLightInsideAtten = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		#endregion

		#region Optimization settings

		[SerializeField] [Range(1, 8)] private int
			_downsampling = 1;

		public int downsampling {
			get => _downsampling;
			set
			{
				if (value != _downsampling)
				{
					_preset = FOG_PRESET.Custom;
					_downsampling = value;
					isDirty = true;
				}
			}
		}


		[SerializeField] private bool
			_forceComposition = false;

		public bool forceComposition {
			get => _forceComposition;
			set
			{
				if (value != _forceComposition)
				{
					_preset = FOG_PRESET.Custom;
					_forceComposition = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] private bool
			_edgeImprove = false;

		public bool edgeImprove {
			get => _edgeImprove;
			set
			{
				if (value != _edgeImprove)
				{
					_preset = FOG_PRESET.Custom;
					_edgeImprove = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0.00001f, 0.005f)]
		private float
			_edgeThreshold = 0.0005f;

		public float edgeThreshold {
			get => _edgeThreshold;
			set
			{
				if (value != _edgeThreshold)
				{
					_preset = FOG_PRESET.Custom;
					_edgeThreshold = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(1, 20)] private float
			_stepping = 12.0f;

		public float stepping {
			get => _stepping;
			set
			{
				if (value != _stepping)
				{
					_preset = FOG_PRESET.Custom;
					_stepping = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 50)] private float
			_steppingNear = 1f;

		public float steppingNear {
			get => _steppingNear;
			set
			{
				if (value != _steppingNear)
				{
					_preset = FOG_PRESET.Custom;
					_steppingNear = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private bool
			_dithering = false;

		public bool dithering {
			get => _dithering;
			set
			{
				if (value != _dithering)
				{
					_dithering = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0.1f, 5f)] private float
			_ditherStrength = 0.75f;

		public float ditherStrength {
			get => _ditherStrength;
			set
			{
				if (value != _ditherStrength)
				{
					_ditherStrength = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] [Range(0, 2f)] private float
			_jitterStrength = 0.5f;

		public float jitterStrength {
			get => _jitterStrength;
			set
			{
				if (value != _jitterStrength)
				{
					_jitterStrength = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		#endregion

		#region Shafts settings

		[SerializeField] private bool
			_lightScatteringEnabled = false;

		public bool lightScatteringEnabled {
			get => _lightScatteringEnabled;
			set
			{
				if (value != _lightScatteringEnabled)
				{
					_lightScatteringEnabled = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_lightScatteringDiffusion = 0.7f;

		public float lightScatteringDiffusion {
			get => _lightScatteringDiffusion;
			set
			{
				if (value != _lightScatteringDiffusion)
				{
					_lightScatteringDiffusion = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] [Range(0, 1f)] private float
			_lightScatteringSpread = 0.686f;

		public float lightScatteringSpread {
			get => _lightScatteringSpread;
			set
			{
				if (value != _lightScatteringSpread)
				{
					_lightScatteringSpread = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(4, 64)] private int
			_lightScatteringSamples = 16;

		public int lightScatteringSamples {
			get => _lightScatteringSamples;
			set
			{
				if (value != _lightScatteringSamples)
				{
					_lightScatteringSamples = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 50f)] private float
			_lightScatteringWeight = 1.9f;

		public float lightScatteringWeight {
			get => _lightScatteringWeight;
			set
			{
				if (value != _lightScatteringWeight)
				{
					_lightScatteringWeight = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 50f)] private float
			_lightScatteringIllumination = 18f;

		public float lightScatteringIllumination {
			get => _lightScatteringIllumination;
			set
			{
				if (value != _lightScatteringIllumination)
				{
					_lightScatteringIllumination = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0.9f, 1.1f)] private float
			_lightScatteringDecay = 0.986f;

		public float lightScatteringDecay {
			get => _lightScatteringDecay;
			set
			{
				if (value != _lightScatteringDecay)
				{
					_lightScatteringDecay = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 0.2f)] private float
			_lightScatteringExposure = 0;

		public float lightScatteringExposure {
			get => _lightScatteringExposure;
			set
			{
				if (value != _lightScatteringExposure)
				{
					_lightScatteringExposure = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_lightScatteringJittering = 0.5f;

		public float lightScatteringJittering {
			get => _lightScatteringJittering;
			set
			{
				if (value != _lightScatteringJittering)
				{
					_lightScatteringJittering = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] [Range(1, 4f)] private int
			_lightScatteringBlurDownscale = 1;

		public int lightScatteringBlurDownscale {
			get => _lightScatteringBlurDownscale;
			set
			{
				if (value != _lightScatteringBlurDownscale)
				{
					_lightScatteringBlurDownscale = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		#endregion

		#region Fog Blur

		[SerializeField] private bool
			_fogBlur = false;

		public bool fogBlur {
			get => _fogBlur;
			set
			{
				if (value != _fogBlur)
				{
					_fogBlur = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_fogBlurDepth = 0.05f;

		public float fogBlurDepth {
			get => _fogBlurDepth;
			set
			{
				if (value != _fogBlurDepth)
				{
					_fogBlurDepth = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		#endregion

		#region Sun Shadows

		[SerializeField] private bool
			_sunShadows = false;

		public bool sunShadows {
			get => _sunShadows;
			set
			{
				if (value != _sunShadows)
				{
					_sunShadows = value;
					CleanUpTextureDepthSun();

					if (_sunShadows)
					{
						needUpdateDepthSunTexture = true;
					}
					else
					{
						DestroySunShadowsDependencies();
					}

					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] private LayerMask
			_sunShadowsLayerMask = -1;

		public LayerMask sunShadowsLayerMask {
			get => _sunShadowsLayerMask;
			set
			{
				if (_sunShadowsLayerMask != value)
				{
					_sunShadowsLayerMask = value;
					isDirty = true;
				}
			}
		}


		[SerializeField] [Range(0, 1f)] private float
			_sunShadowsStrength = 0.5f;

		public float sunShadowsStrength {
			get => _sunShadowsStrength;
			set
			{
				if (value != _sunShadowsStrength)
				{
					_sunShadowsStrength = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}


		[SerializeField] private float
			_sunShadowsBias = 0.1f;

		public float sunShadowsBias {
			get => _sunShadowsBias;
			set
			{
				if (value != _sunShadowsBias)
				{
					_sunShadowsBias = value;
					needUpdateDepthSunTexture = true;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 0.5f)] private float
			_sunShadowsJitterStrength = 0.1f;

		public float sunShadowsJitterStrength {
			get => _sunShadowsJitterStrength;
			set
			{
				if (value != _sunShadowsJitterStrength)
				{
					_sunShadowsJitterStrength = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 4)] private int
			_sunShadowsResolution = 2;
		// default = 2^(9+2) = 2048 texture size

		public int sunShadowsResolution {
			get => _sunShadowsResolution;
			set
			{
				if (value != _sunShadowsResolution)
				{
					_sunShadowsResolution = value;
					needUpdateDepthSunTexture = true;
					CleanUpTextureDepthSun();

					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(50f, 2000f)] private float
			_sunShadowsMaxDistance = 200f;

		public float sunShadowsMaxDistance {
			get => _sunShadowsMaxDistance;
			set
			{
				if (value != _sunShadowsMaxDistance)
				{
					_sunShadowsMaxDistance = value;
					needUpdateDepthSunTexture = true;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private SUN_SHADOWS_BAKE_MODE
			_sunShadowsBakeMode = SUN_SHADOWS_BAKE_MODE.Discrete;

		public SUN_SHADOWS_BAKE_MODE sunShadowsBakeMode {
			get => _sunShadowsBakeMode;
			set
			{
				if (value != _sunShadowsBakeMode)
				{
					_sunShadowsBakeMode = value;
					needUpdateDepthSunTexture = true;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private float
			_sunShadowsRefreshInterval = 0;
		// 0 = no update unless Sun changes position

		public float sunShadowsRefreshInterval {
			get => _sunShadowsRefreshInterval;
			set
			{
				if (value != _sunShadowsRefreshInterval)
				{
					_sunShadowsRefreshInterval = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(0, 1f)] private float
			_sunShadowsCancellation = 0f;

		public float sunShadowsCancellation {
			get => _sunShadowsCancellation;
			set
			{
				if (value != _sunShadowsCancellation)
				{
					_sunShadowsCancellation = value;
					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		#endregion

		#region Turbulence

		[SerializeField] [Range(0, 10f)] private float
			_turbulenceStrength = 0f;

		public float turbulenceStrength {
			get => _turbulenceStrength;
			set
			{
				if (value != _turbulenceStrength)
				{
					_turbulenceStrength = value;
					if (_turbulenceStrength <= 0f)
					{
						UpdateTexture(); // reset texture to normal
					}

					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		#endregion

		#region Other settings

		[SerializeField] private bool
			_useXYPlane = false;

		public bool useXYPlane {
			get => _useXYPlane;
			set
			{
				if (value != _useXYPlane)
				{
					_useXYPlane = value;
					if (_sunShadows)
					{
						needUpdateDepthSunTexture = true;
					}

					UpdateMaterialProperties();
					isDirty = true;
				}
			}
		}

		[SerializeField] private bool
			_useSinglePassStereoRenderingMatrix = false;

		public bool useSinglePassStereoRenderingMatrix {
			get => _useSinglePassStereoRenderingMatrix;
			set
			{
				if (value != _useSinglePassStereoRenderingMatrix)
				{
					_useSinglePassStereoRenderingMatrix = value;
					isDirty = true;
				}
			}
		}

		[SerializeField] private SPSR_BEHAVIOUR _spsrBehaviour = SPSR_BEHAVIOUR.AutoDetectInEditor;

		public SPSR_BEHAVIOUR spsrBehaviour {
			get => _spsrBehaviour;
			set
			{
				if (value != _spsrBehaviour)
				{
					_spsrBehaviour = value;
					isDirty = true;
				}
			}
		}


		[SerializeField] private bool
			_reduceFlickerBigWorlds;

		public bool reduceFlickerBigWorlds {
			get => _reduceFlickerBigWorlds;
			set
			{
				if (value != _reduceFlickerBigWorlds)
				{
					_reduceFlickerBigWorlds = value;
					isDirty = true;
				}
			}
		}

		#endregion


		#region Screen Mask options

		[SerializeField] private bool _enableMask;

		public bool enableMask {
			get => _enableMask;
			set
			{
				if (value != _enableMask)
				{
					_enableMask = value;
					UpdateVolumeMask();
					isDirty = true;
				}
			}
		}


		[SerializeField] private LayerMask _maskLayer = 1 << 23;

		public LayerMask maskLayer {
			get => _maskLayer;
			set
			{
				if (value != _maskLayer)
				{
					_maskLayer = value;
					UpdateVolumeMask();
					isDirty = true;
				}
			}
		}

		[SerializeField] [Range(1, 4)] private int _maskDownsampling = 1;

		public int maskDownsampling {
			get => _maskDownsampling;
			set
			{
				if (value != _maskDownsampling)
				{
					_maskDownsampling = value;
					UpdateVolumeMask();
					isDirty = true;
				}
			}
		}

		#endregion

		#region Internal variables

		private bool isPartOfScene;
		private int noiseTextureSize;

		// transitions
		private float initialFogAlpha, targetFogAlpha;
		private float initialSkyHazeAlpha, targetSkyHazeAlpha;
		private bool transitionAlpha, transitionColor, transitionSpecularColor, transitionLightColor, transitionProfile;
		private bool targetColorActive, targetSpecularColorActive, targetLightColorActive;
		private Color initialFogColor, targetFogColor;
		private Color initialFogSpecularColor, targetFogSpecularColor;
		private Color initialLightColor, targetLightColor;
		private float transitionDuration;
		private float transitionStartTime;
		private float currentFogAlpha, currentSkyHazeAlpha;
		private Color currentFogColor, currentFogSpecularColor, currentLightColor;
		private VolumetricFogProfile initialProfile, targetProfile;

		// fog height related
		private float oldBaselineRelativeCameraY;
		private float currentFogAltitude;

		// sky related
		private float skyHazeSpeedAcum;
		private Color skyHazeLightColor;

		// rendering
		private bool _hasCamera, _hasCameraChecked;

		// if this script is attached to a camera or to another object
		private Camera mainCamera;
		private List<string> shaderKeywords;
		private Material blurMat;
		private RenderBuffer[] mrt;
		private int _renderingInstancesCount;
		private bool shouldUpdateMaterialProperties;
		private int lastFrameCount;

		[NonSerialized] public Material fogMat;

		private RenderTexture depthTexture, depthSunTexture, reducedDestination;

		// point light related
		private Light[] lastFoundLights, lightBuffer;
		private Light[] currentLights;
		private float trackPointAutoLastTime;
		private float trackPointCheckNewLightsLastTime;
		private readonly Vector4 black = new Vector4(0, 0, 0, 1f);

		// transparency support related
		private Shader depthShader, depthShaderAndTrans;
		private GameObject depthCamObj;
		private Camera depthCam;

		// fog texture/lighting/colors related
		private float lastTextureUpdate;
		private Vector3 windSpeedAcum;
		private Texture2D adjustedTexture;
		private Color[] noiseColors, adjustedColors;
		private float sunLightIntensity = 1.0f;
		private bool needUpdateTexture, hasChangeAdjustedColorsAlpha;
		private int updatingTextureSlice;
		private Color updatingTextureLightColor;
		private Color lastRenderSettingsAmbientLight;
		private float lastRenderSettingsAmbientIntensity;
		private int lastFrameAppliedChaos, lastFrameAppliedWind;

		// Sun related
		private Light sunLight;
		private Vector2 oldSunPos;
		private float sunFade = 1f;

		// Sun shadows related
		private GameObject depthSunCamObj;
		private Camera depthSunCam;
		private Shader depthSunShader;

		[NonSerialized] public bool needUpdateDepthSunTexture;

		private float lastShadowUpdateFrame;
		private bool sunShadowsActive;
		private int currentDepthSunTextureRes;
		private Matrix4x4 lightMatrix;

		// turbulence related
		private Texture2D adjustedChaosTexture;
		private Material chaosLerpMat;
		private float turbAcum;
		private float deltaTime, timeOfLastRender;

		// fog instances stuff
		private MeshRenderer mr;
		private float lastTimeSortInstances;
		private const float FOG_INSTANCES_SORT_INTERVAL = 2f;
		private Vector3 lastCamPos;
		private bool needResort;

		// Screen mask
		private CommandBuffer maskCommandBuffer;
		private RenderTextureDescriptor rtMaskDesc;
		private Material maskMaterial;

		#endregion


		#region Game loop events

		private void OnEnable()
		{
			// Note: OnEnable can be called several times when using fog areas

			isPartOfScene = isPartOfScene || IsPartOfScene();
			if (!isPartOfScene)
			{
				return;
			}

			temporaryProperties.color = Color.white;
			temporaryProperties.density = 1f;
			if (_fogVoidInverted)
			{
				// conversion to fog area from fog void from previous versions
				_fogVoidInverted = false;
				_fogAreaCenter = _character;
				_fogAreaDepth = _fogVoidDepth;
				_fogAreaFallOff = _fogVoidFallOff;
				_fogAreaHeight = _fogVoidHeight;
				_fogAreaPosition = _fogVoidPosition;
				_fogAreaRadius = _fogVoidRadius;
				_fogVoidRadius = 0;
				_character = null;
			}

			// Setup fog rendering system
			mainCamera = gameObject.GetComponent<Camera>();
			_hasCamera = mainCamera != null;
			_hasCameraChecked = true;
			if (_hasCamera)
			{
				fogRenderer = this;
				if (mainCamera.depthTextureMode == DepthTextureMode.None)
				{
					mainCamera.depthTextureMode = DepthTextureMode.Depth;
				}

				UpdateVolumeMask();
			}
			else
			{
				if (fogRenderer == null)
				{
					FindMainCamera();
					if (mainCamera == null)
					{
						Debug.LogError("Volumetric Fog: no camera found!");
						return;
					}

					fogRenderer = mainCamera.GetComponent<VolumetricFog>();
					if (fogRenderer == null)
					{
						fogRenderer = mainCamera.gameObject.AddComponent<VolumetricFog>();
						fogRenderer.density = 0;
					}
				}
				else
				{
					mainCamera = fogRenderer.mainCamera;
					if (mainCamera == null)
					{
						mainCamera = fogRenderer.GetComponent<Camera>();
					}
				}
			}

			// Initialize material
			if (fogMat == null)
			{
				InitFogMaterial();
				if (_profile != null && _profileSync)
				{
					_profile.Load(this);
				}
			}
			else
			{
				UpdateMaterialPropertiesNow();
			}

			// Register on other Volumetric Fog & Mist renderer if needed
			RegisterWithRenderers();

#if UNITY_EDITOR
			if (EditorPrefs.GetInt(VFM_BUILD_HINT) == 0)
			{
				EditorPrefs.SetInt(VFM_BUILD_HINT, 1);
				if (EditorPrefs.GetInt(VFM_BUILD_FIRST_INSTALL) == 1)
				{
					EditorUtility.DisplayDialog("Volumetric Fog & Mist updated!",
						"Please review the 'Shader Options' section in Volumetric Fog inspector and make sure you disable unused features to optimize build size and compilation time.\n\nOtherwise when you build the game it will take a long time or even get stuck during shader compilation.",
						"Ok");
				}

				EditorPrefs.SetInt(VFM_BUILD_FIRST_INSTALL, 1);
			}
#endif
			needResort = true;
		}


		private void OnDisable()
		{
			RemoveMaskCommandBuffer();
			RemoveDirectionalLightCommandBuffer();
		}

		private void OnDestroy()
		{
			// Unregister on other Volumetric Fog & Mist renderer if needed
			if (!_hasCamera)
			{
				UnregisterWithRenderers();
			}
			else
			{
				RemoveMaskCommandBuffer();
				UnregisterFogArea(this);
			}

			if (depthCamObj != null)
			{
				DestroyImmediate(depthCamObj);
				depthCamObj = null;
			}

			if (adjustedTexture != null)
			{
				DestroyImmediate(adjustedTexture);
				adjustedTexture = null;
			}

			if (chaosLerpMat != null)
			{
				DestroyImmediate(chaosLerpMat);
				chaosLerpMat = null;
			}

			if (adjustedChaosTexture != null)
			{
				DestroyImmediate(adjustedChaosTexture);
				adjustedChaosTexture = null;
			}

			if (blurMat != null)
			{
				DestroyImmediate(blurMat);
				blurMat = null;
			}

			if (fogMat != null)
			{
				DestroyImmediate(fogMat);
				fogMat = null;
			}

			FogOfWarDestroy();
			CleanUpDepthTexture();
			DestroySunShadowsDependencies();
		}


		public void DestroySelf()
		{
			DestroyRenderComponent<VolumetricFogPreT>();
			DestroyRenderComponent<VolumetricFogPosT>();
			DestroyImmediate(this);
		}

		private void Start()
		{
			currentFogAlpha = _alpha;
			currentSkyHazeAlpha = _skyAlpha;
			lastTextureUpdate = Time.time + _timeBetweenTextureUpdates;
			RegisterWithRenderers(); // ensures it's properly registered
			Update();
		}

		// Check possible alpha transition
		private void Update()
		{
			if (!isPartOfScene || fogRenderer == null)
			{
				return;
			}

			// Updates sun illumination
			if (fogRenderer.sun != null)
			{
				Vector3 lightDir = fogRenderer.sun.transform.forward;
				bool mayUpdateTexture = !Application.isPlaying || updatingTextureSlice < 0 &&
					Time.time - lastTextureUpdate >= _timeBetweenTextureUpdates;
				if (mayUpdateTexture)
				{
					if (lightDir != _lightDirection)
					{
						_lightDirection = lightDir;
						needUpdateTexture = true;
						needUpdateDepthSunTexture = true;
					}

					if (sunLight != null)
					{
						if (_sunCopyColor && sunLight.color != _lightColor)
						{
							_lightColor = sunLight.color;
							currentLightColor = _lightColor;
							needUpdateTexture = true;
						}

						if (sunLightIntensity != sunLight.intensity)
						{
							sunLightIntensity = sunLight.intensity;
							needUpdateTexture = true;
						}
					}
				}
			}

			// Check changes in render settings that affect fog colors
			if (!needUpdateTexture)
			{
				if (_lightingModel == LIGHTING_MODEL.Classic)
				{
					if (lastRenderSettingsAmbientIntensity != RenderSettings.ambientIntensity)
					{
						needUpdateTexture = true;
					}
					else if (lastRenderSettingsAmbientLight != RenderSettings.ambientLight)
					{
						needUpdateTexture = true;
					}
				}
				else if (_lightingModel == LIGHTING_MODEL.Natural)
				{
					if (lastRenderSettingsAmbientLight != RenderSettings.ambientLight)
					{
						needUpdateTexture = true;
					}
				}
			}

			// Check profile transition
			if (transitionProfile)
			{
				float t = (Time.time - transitionStartTime) / transitionDuration;
				if (t > 1)
				{
					t = 1;
				}

				VolumetricFogProfile.Lerp(initialProfile, targetProfile, t, this);
				if (t >= 1f)
				{
					transitionProfile = false;
				}
			}

			// Check alpha transition
			if (transitionAlpha)
			{
				if (targetFogAlpha >= 0 || targetSkyHazeAlpha >= 0)
				{
					if (targetFogAlpha != currentFogAlpha || targetSkyHazeAlpha != currentSkyHazeAlpha)
					{
						if (transitionDuration > 0)
						{
							currentFogAlpha = Mathf.Lerp(initialFogAlpha, targetFogAlpha,
								(Time.time - transitionStartTime) / transitionDuration);
							currentSkyHazeAlpha = Mathf.Lerp(initialSkyHazeAlpha, targetSkyHazeAlpha,
								(Time.time - transitionStartTime) / transitionDuration);
						}
						else
						{
							currentFogAlpha = targetFogAlpha;
							currentSkyHazeAlpha = targetSkyHazeAlpha;
							transitionAlpha = false;
						}

						fogMat.SetFloat("_FogAlpha", currentFogAlpha);
						UpdateSkyColor(currentSkyHazeAlpha);
					}
				}
				else if (currentFogAlpha != _alpha || currentSkyHazeAlpha != _skyAlpha)
				{
					if (transitionDuration > 0)
					{
						currentFogAlpha = Mathf.Lerp(initialFogAlpha, _alpha,
							(Time.time - transitionStartTime) / transitionDuration);
						currentSkyHazeAlpha = Mathf.Lerp(initialSkyHazeAlpha, alpha,
							(Time.time - transitionStartTime) / transitionDuration);
					}
					else
					{
						currentFogAlpha = _alpha;
						currentSkyHazeAlpha = _skyAlpha;
						transitionAlpha = false;
					}

					fogMat.SetFloat("_FogAlpha", currentFogAlpha);
					UpdateSkyColor(currentSkyHazeAlpha);
				}
			}

			// Check color transition
			if (transitionColor)
			{
				if (targetColorActive)
				{
					if (targetFogColor != currentFogColor)
					{
						if (transitionDuration > 0)
						{
							currentFogColor = Color.Lerp(initialFogColor, targetFogColor,
								(Time.time - transitionStartTime) / transitionDuration);
						}
						else
						{
							currentFogColor = targetFogColor;
							transitionColor = false;
						}
					}
				}
				else if (currentFogColor != _color)
				{
					if (transitionDuration > 0)
					{
						currentFogColor = Color.Lerp(initialFogColor, _color,
							(Time.time - transitionStartTime) / transitionDuration);
					}
					else
					{
						currentFogColor = _color;
						transitionColor = false;
					}
				}

				UpdateMaterialFogColor();
			}

			// Check color specular transition
			if (transitionSpecularColor)
			{
				if (targetSpecularColorActive)
				{
					if (targetFogSpecularColor != currentFogSpecularColor)
					{
						if (transitionDuration > 0)
						{
							currentFogSpecularColor = Color.Lerp(initialFogSpecularColor, targetFogSpecularColor,
								(Time.time - transitionStartTime) / transitionDuration);
						}
						else
						{
							currentFogSpecularColor = targetFogSpecularColor;
							transitionSpecularColor = false;
						}

						needUpdateTexture = true;
					}
				}
				else if (currentFogSpecularColor != _specularColor)
				{
					if (transitionDuration > 0)
					{
						currentFogSpecularColor = Color.Lerp(initialFogSpecularColor, _specularColor,
							(Time.time - transitionStartTime) / transitionDuration);
					}
					else
					{
						currentFogSpecularColor = _specularColor;
						transitionSpecularColor = false;
					}

					needUpdateTexture = true;
				}
			}

			// Check color specular transition
			if (transitionLightColor)
			{
				if (targetLightColorActive)
				{
					if (targetLightColor != currentLightColor)
					{
						if (transitionDuration > 0)
						{
							currentLightColor = Color.Lerp(initialLightColor, targetLightColor,
								(Time.time - transitionStartTime) / transitionDuration);
						}
						else
						{
							currentLightColor = targetLightColor;
							transitionLightColor = false;
						}

						needUpdateTexture = true;
					}
				}
				else if (currentLightColor != _lightColor)
				{
					if (transitionDuration > 0)
					{
						currentLightColor = Color.Lerp(initialLightColor, _lightColor,
							(Time.time - transitionStartTime) / transitionDuration);
					}
					else
					{
						currentLightColor = _lightColor;
						transitionLightColor = false;
					}

					needUpdateTexture = true;
				}
			}

			if (_baselineRelativeToCamera)
			{
				UpdateMaterialHeights(mainCamera);
			}
			else if (_character != null)
			{
				_fogVoidPosition = _character.transform.position;
				UpdateMaterialHeights(mainCamera);
			}

#if UNITY_EDITOR
			CheckFogAreaDimensions(); // This is called in OnRenderImage BUT if Game View is not visible, fog area dimensions are not updated and gizmos won't appear with correct size, so we force dimensions check here just in case
#endif
			if (_fogAreaCenter != null)
			{
				if (_fogAreaFollowMode == FOG_AREA_FOLLOW_MODE.FullXYZ)
				{
					_fogAreaPosition = _fogAreaCenter.transform.position;
				}
				else
				{
					_fogAreaPosition.x = _fogAreaCenter.transform.position.x;
					_fogAreaPosition.z = _fogAreaCenter.transform.position.z;
				}

				UpdateMaterialHeights(mainCamera);
			}

			if (_pointLightTrackingAuto)
			{
				if (!Application.isPlaying || Time.time - trackPointAutoLastTime > _pointLightTrackingCheckInterval)
				{
					trackPointAutoLastTime = Time.time;
					TrackPointLights();
				}
			}

			if (updatingTextureSlice >= 0)
			{
				UpdateTextureColors(adjustedColors, false);
			}
			else if (needUpdateTexture)
			{
				UpdateTexture();
			}

			if (_hasCamera)
			{
				// Restores fog of war
				if (_fogOfWarEnabled)
				{
					UpdateFogOfWar();
				}

				// Autoupdate of fog shadows
				if (sunShadowsActive)
				{
					CastSunShadows();
				}

				// Sort fog instances
				int fogInstancesCount = instances.Count;
				if (fogInstancesCount > 1)
				{
					Vector3 camPos = mainCamera.transform.position;
					if (!Application.isPlaying || Time.time - lastTimeSortInstances >= FOG_INSTANCES_SORT_INTERVAL)
					{
						needResort = true;
					}

					if (!needResort)
					{
						float camDist = (camPos.x - lastCamPos.x) * (camPos.x - lastCamPos.x) +
						                (camPos.y - lastCamPos.y) * (camPos.y - lastCamPos.y) +
						                (camPos.z - lastCamPos.z) * (camPos.z - lastCamPos.z);
						if (camDist > 625)
						{
							// forces udpate every 25 meters
							lastCamPos = camPos;
							needResort = true;
						}
					}

					if (needResort)
					{
						needResort = false;
						lastTimeSortInstances = Time.time;
						float camX = camPos.x;
						float camY = camPos.y;
						float camZ = camPos.z;
						for (int k = 0; k < fogInstancesCount; k++)
						{
							VolumetricFog fogInstance = instances[k];
							if (fogInstance != null)
							{
								Vector3 pos = fogInstance.transform.position;
								pos.y = fogInstance.currentFogAltitude;
								float xdx = camX - pos.x;
								float xdy = camY - pos.y;
								float distYAxis = xdy * xdy;
								float xdyh = camY - (pos.y + fogInstance.height);
								float distYAxis2 = xdyh * xdyh;
								fogInstance.distanceToCameraYAxis = distYAxis < distYAxis2 ? distYAxis : distYAxis2;
								float xdz = camZ - pos.z;
								float distSqr = xdx * xdx + xdy * xdy + xdz * xdz;
								fogInstance.distanceToCamera = distSqr;

								Vector3 min = pos - fogInstance.transform.localScale * 0.5f;
								Vector3 max = pos + fogInstance.transform.localScale * 0.5f;
								fogInstance.distanceToCameraMin = mainCamera.WorldToScreenPoint(min).z;
								fogInstance.distanceToCameraMax = mainCamera.WorldToScreenPoint(max).z;
							}
						}

						instances.Sort((x, y) =>
						{
							if (!x || !y)
							{
								return 0;
							}

							if (x._fogAreaSortingMode == FOG_AREA_SORTING_MODE.Fixed ||
							    y._fogAreaSortingMode == FOG_AREA_SORTING_MODE.Fixed)
							{
								if (x._fogAreaRenderOrder < y._fogAreaRenderOrder)
								{
									return -1;
								}

								if (x._fogAreaRenderOrder > y._fogAreaRenderOrder)
								{
									return 1;
								}

								return 0;
							}

							bool overlaps = x.distanceToCameraMin < y.distanceToCameraMin &&
							                x.distanceToCameraMax > y.distanceToCameraMax ||
							                y.distanceToCameraMin < x.distanceToCameraMin &&
							                y.distanceToCameraMax > x.distanceToCameraMax;
							if (overlaps || x._fogAreaSortingMode == FOG_AREA_SORTING_MODE.Altitude ||
							    y._fogAreaSortingMode == FOG_AREA_SORTING_MODE.Altitude)
							{
								if (x.distanceToCameraYAxis < y.distanceToCameraYAxis)
								{
									return 1;
								}

								if (x.distanceToCameraYAxis > y.distanceToCameraYAxis)
								{
									return -1;
								}

								return 0;
							}

							if (x.distanceToCamera < y.distanceToCamera)
							{
								return 1;
							}

							if (x.distanceToCamera > y.distanceToCamera)
							{
								return -1;
							}

							return 0;
						});
					}
				}
			}
		}

		private void OnPreCull()
		{
			if (!enabled || !gameObject.activeSelf || fogMat == null || !_hasCamera || mainCamera == null)
			{
				return;
			}

			if (mainCamera.depthTextureMode == DepthTextureMode.None)
			{
				mainCamera.depthTextureMode = DepthTextureMode.Depth;
			}

			if (_computeDepth)
			{
				GetTransparentDepth();
			}

			// Apply chaos
			if (_hasCamera)
			{
				if (Application.isPlaying)
				{
					int count = renderingInstances.Count;
					for (int k = 0; k < count; k++)
					{
						if (renderingInstances[k] != null && renderingInstances[k].turbulenceStrength > 0)
						{
							renderingInstances[k].ApplyChaos();
						}
					}
				}
			}

			//												if (Application.isPlaying && _turbulenceStrength > 0f)
			//																ApplyChaos ();
		}

		private void OnDidApplyAnimationProperties()
		{
			// support for animating property based fields
			shouldUpdateMaterialProperties = true;
		}

		#endregion


		#region Setting up

		private void FindMainCamera()
		{
			mainCamera = Camera.main;
			if (mainCamera == null)
			{
				Camera[] cams = FindObjectsOfType<Camera>();
				for (int k = 0; k < cams.Length; k++)
				{
					if (cams[k].isActiveAndEnabled)
					{
						mainCamera = cams[k];
						return;
					}
				}
			}
		}

		private bool IsPartOfScene()
		{
			VolumetricFog[] fogs = FindObjectsOfType<VolumetricFog>();
			for (int k = 0; k < fogs.Length; k++)
			{
				if (fogs[k] == this)
				{
					return true;
				}
			}

			return false;
		}

		private void InitFogMaterial()
		{
			targetFogAlpha = -1;
			targetSkyHazeAlpha = -1;
			_skyColor.a = _skyAlpha;
			updatingTextureSlice = -1;
			fogMat = new Material(Shader.Find("VolumetricFogAndMist/VolumetricFog"));
			fogMat.hideFlags = HideFlags.DontSave;
			Texture2D noiseTexture = Resources.Load<Texture2D>("Textures/Noise3");
			noiseTextureSize = noiseTexture.width;
			noiseColors = noiseTexture.GetPixels();
			adjustedColors = new Color[noiseColors.Length];
			adjustedTexture = new Texture2D(noiseTexture.width, noiseTexture.height, TextureFormat.RGBA32, false);
			adjustedTexture.hideFlags = HideFlags.DontSave;
			timeOfLastRender = Time.time;

			// Init & apply settings
			CheckPointLightData();
			if (_pointLightTrackingAuto)
			{
				TrackPointLights();
			}

			FogOfWarInit();

			CopyTransitionValues();

			UpdatePreset();

			oldBaselineRelativeCameraY = mainCamera.transform.position.y;

			if (_sunShadows)
			{
				needUpdateDepthSunTexture = true;
			}
		}


		private void UpdateRenderComponents()
		{
			if (!_hasCamera)
			{
				return;
			}

			if (_renderBeforeTransparent)
			{
				AssignRenderComponent<VolumetricFogPreT>();
				DestroyRenderComponent<VolumetricFogPosT>();
			}
			else if (_transparencyBlendMode == TRANSPARENT_MODE.Blend)
			{
				AssignRenderComponent<VolumetricFogPreT>();
				AssignRenderComponent<VolumetricFogPosT>();
			}
			else
			{
				AssignRenderComponent<VolumetricFogPosT>();
				DestroyRenderComponent<VolumetricFogPreT>();
			}
		}

		private void DestroyRenderComponent<T>() where T : IVolumetricFogRenderComponent
		{
			T[] cc = GetComponentsInChildren<T>(true);
			for (int k = 0; k < cc.Length; k++)
			{
				if (cc[k].fog == this || cc[k].fog == null)
				{
					cc[k].DestroySelf();
				}
			}
		}

		private void AssignRenderComponent<T>() where T : Component, IVolumetricFogRenderComponent
		{
			T[] cc = GetComponentsInChildren<T>(true);
			int freeCC = -1;
			for (int k = 0; k < cc.Length; k++)
			{
				if (cc[k].fog == this)
				{
					return;
				}

				if (cc[k].fog == null)
				{
					freeCC = k;
				}
			}

			if (freeCC < 0)
			{
				gameObject.AddComponent<T>().fog = this;
			}
			else
			{
				cc[freeCC].fog = this;
			}
		}

        /// <summary>
        ///     Used internally to chain multiple fog areas into the main fog renderer.
        /// </summary>
        private void RegisterFogArea(VolumetricFog fog)
		{
			if (instances.Contains(fog))
			{
				return;
			}

			instances.Add(fog);
		}

        /// <summary>
        ///     Used internally to disconnect a non-enabled fog area from the main fog renderer.
        /// </summary>
        private void UnregisterFogArea(VolumetricFog fog)
		{
			if (!instances.Contains(fog))
			{
				return;
			}

			instances.Remove(fog);
		}

		private void RegisterWithRenderers()
		{
			// Get a list of Volumetric Fog & Mist scripts in the scene
			allFogRenderers = FindObjectsOfType<VolumetricFog>();
			if (!_hasCamera && fogRenderer != null)
			{
				if (fogRenderer.enableMultipleCameras)
				{
					for (int k = 0; k < allFogRenderers.Length; k++)
					{
						if (allFogRenderers[k].hasCamera)
						{
							allFogRenderers[k].RegisterFogArea(this);
						}
					}
				}
				else
				{
					// Only register with designated renderer
					fogRenderer.RegisterFogArea(this);
				}
			}
			else
			{
				instances.Clear();
				RegisterFogArea(this);

				// Find all fog areas and attach them to this renderer if enableMultipleCameras is enabled, or only the fog area linked to this fog area
				for (int k = 0; k < allFogRenderers.Length; k++)
				{
					if (!allFogRenderers[k].hasCamera &&
					    (_enableMultipleCameras || allFogRenderers[k].fogRenderer == this))
					{
						RegisterFogArea(allFogRenderers[k]);
					}
				}
			}

			lastTimeSortInstances = 0;
		}

		private void UnregisterWithRenderers()
		{
			if (allFogRenderers != null)
			{
				for (int k = 0; k < allFogRenderers.Length; k++)
				{
					if (allFogRenderers[k] != null && allFogRenderers[k].hasCamera)
					{
						allFogRenderers[k].UnregisterFogArea(this);
					}
				}
			}
		}

		public void UpdateMultiCameraSetup()
		{
			allFogRenderers = FindObjectsOfType<VolumetricFog>();
			for (int k = 0; k < allFogRenderers.Length; k++)
			{
				if (allFogRenderers[k] != null && allFogRenderers[k].hasCamera)
				{
					allFogRenderers[k].SetEnableMultipleCameras(_enableMultipleCameras);
				}
			}

			RegisterWithRenderers();
		}

		private void SetEnableMultipleCameras(bool state)
		{
			_enableMultipleCameras = state;
			RegisterWithRenderers();
		}

		#endregion

		#region Rendering stuff

		internal void DoOnRenderImage(RenderTexture source, RenderTexture destination)
		{
			// Check integrity of fog instances list
			int registeredInstancesCount = instances.Count;
			renderingInstances.Clear();
			Vector3 camPos = Camera.current.transform.position;
			for (int k = 0; k < registeredInstancesCount; k++)
			{
				VolumetricFog fogInstance = instances[k];
				fogInstance.isRendering = false;
				if (fogInstance != null && fogInstance.isActiveAndEnabled && fogInstance.density > 0)
				{
					if (fogInstance._visibilityScope == FOG_VISIBILITY_SCOPE.Global ||
					    fogInstance._visibilityVolume.Contains(camPos))
					{
						renderingInstances.Add(instances[k]);
					}
				}
			}

			_renderingInstancesCount = renderingInstances.Count;

			if (_renderingInstancesCount == 0 || mainCamera == null)
			{
				// No available instances, cancel any rendering
				Graphics.Blit(source, destination);
				return;
			}

#pragma warning disable 0162
			if (USE_DIRECTIONAL_LIGHT_COOKIE)
			{
				if (_sun != null)
				{
					lightMatrix = Matrix4x4.TRS(_sun.transform.position, _sun.transform.rotation, Vector3.one).inverse;
				}
			}
#pragma warning restore 0162

			if (_hasCamera && _density <= 0 && shouldUpdateMaterialProperties)
			{
				// This fog renderer might require updating its material properties (Sun Shadows, ...) but won't render, so we do it here
				UpdateMaterialPropertiesNow(Camera.current);
			}

			if (_renderingInstancesCount == 1)
			{
				// One instance, render directly to destination
				renderingInstances[0].DoOnRenderImageInstance(source, destination);
			}
			else
			{
				RenderTextureDescriptor rtDesc = source.descriptor;
				rtDesc.depthBufferBits = 0;
				rtDesc.msaaSamples = 1;
				//rtDesc.colorFormat = RenderTextureFormat.ARGB32; <-- causes banding issues with Linear Color space
				RenderTexture rt1 = RenderTexture.GetTemporary(rtDesc);
				renderingInstances[0].DoOnRenderImageInstance(source, rt1);
				if (_renderingInstancesCount == 2)
				{
					// Two instance, render to intermediate, then to destination
					renderingInstances[1].DoOnRenderImageInstance(rt1, destination);
				}

				if (_renderingInstancesCount >= 3)
				{
					// 3 or more instances, render them and finally to destinatio
					RenderTexture rt2 = RenderTexture.GetTemporary(rtDesc);
					RenderTexture prev = rt1;
					RenderTexture next = rt2;
					int last = _renderingInstancesCount - 1;
					for (int k = 1; k < last; k++)
					{
						if (k > 1)
						{
							next.DiscardContents();
						}

						renderingInstances[k].DoOnRenderImageInstance(prev, next);
						if (next == rt2)
						{
							prev = rt2;
							next = rt1;
						}
						else
						{
							prev = rt1;
							next = rt2;
						}
					}

					renderingInstances[last].DoOnRenderImageInstance(prev, destination);
					RenderTexture.ReleaseTemporary(rt2);
				}

				RenderTexture.ReleaseTemporary(rt1);
			}
		}


		internal void DoOnRenderImageInstance(RenderTexture source, RenderTexture destination)
		{
			Camera mainCamera = Camera.current;

			if (mainCamera == null || fogMat == null)
			{
				Graphics.Blit(source, destination);
				return;
			}

			isRendering = true;

			if (!_hasCamera)
			{
				CheckFogAreaDimensions();
				if (_sunShadows && !fogRenderer.sunShadows)
				{
					fogRenderer.sunShadows = true; // forces casting shadows on fog renderer
				}
			}

			if (shouldUpdateMaterialProperties)
			{
				UpdateMaterialPropertiesNow(mainCamera);
			}

			if (lastFrameCount != Time.frameCount && Application.isPlaying)
			{
				if (_useRealTime)
				{
					deltaTime = Time.time - timeOfLastRender;
					timeOfLastRender = Time.time;
				}
				else
				{
					deltaTime = Time.deltaTime;
				}

				UpdateWindSpeedQuick();
			}

			if (_hasCamera)
			{
#if UNITY_EDITOR
				if (_spsrBehaviour == SPSR_BEHAVIOUR.AutoDetectInEditor)
				{
					useSinglePassStereoRenderingMatrix =
						PlayerSettings.stereoRenderingPath != StereoRenderingPath.MultiPass;
				}
#endif

				if (_spsrBehaviour == SPSR_BEHAVIOUR.ForcedOn && !_useSinglePassStereoRenderingMatrix)
				{
					useSinglePassStereoRenderingMatrix = true;
				}
				else if (_spsrBehaviour == SPSR_BEHAVIOUR.ForcedOff && _useSinglePassStereoRenderingMatrix)
				{
					useSinglePassStereoRenderingMatrix = false;
				}
			}

			// set light matrix
#pragma warning disable 0162
			if (USE_DIRECTIONAL_LIGHT_COOKIE)
			{
				fogMat.SetMatrix("_VolumetricFogLightMatrix", lightMatrix);
				if (sunLight != null && sunLight.cookieSize > 0)
				{
					fogMat.SetFloat("_VolumetricFogCookieSize", 1f / sunLight.cookieSize);
				}
			}
#pragma warning restore 0162
			bool vrOn = XRSettings.enabled;

			Vector3 camPos = mainCamera.transform.position;
			bool shiftToZero = fogRenderer.reduceFlickerBigWorlds;
			if (shiftToZero)
			{
				fogMat.SetVector("_FlickerFreeCamPos", camPos);
				mainCamera.transform.position = Vector3.zero;
				if (vrOn)
				{
					mainCamera.ResetWorldToCameraMatrix();
				}
			}
			else
			{
				fogMat.SetVector("_FlickerFreeCamPos", Vector3.zero);
			}

			if (mainCamera.orthographic)
			{
				fogMat.SetVector("_ClipDir", mainCamera.transform.forward);
			}

			if (vrOn && fogRenderer.useSinglePassStereoRenderingMatrix)
			{
				fogMat.SetMatrix("_ClipToWorld", mainCamera.cameraToWorldMatrix);
			}
			else
			{
				fogMat.SetMatrix("_ClipToWorld", mainCamera.cameraToWorldMatrix * mainCamera.projectionMatrix.inverse);
			}

			if (shiftToZero)
			{
				mainCamera.transform.position = camPos;
			}

			if (_lightScatteringEnabled && fogRenderer.sun)
			{
				UpdateScatteringData(mainCamera);
			}

			if (lastFrameCount != Time.frameCount || !Application.isPlaying)
			{
				// Updates point light illumination
				if (pointLightParams.Length != MAX_POINT_LIGHTS)
				{
					CheckPointLightData();
				}

				for (int k = 0; k < pointLightParams.Length; k++)
				{
					Light pointLightComponent = pointLightParams[k].light;
					if (pointLightComponent != null)
					{
						if (pointLightParams[k].color != pointLightComponent.color)
						{
							pointLightParams[k].color = pointLightComponent.color;
							isDirty = true;
						}

						if (pointLightParams[k].range != pointLightComponent.range)
						{
							pointLightParams[k].range = pointLightComponent.range;
							isDirty = true;
						}

						if (pointLightParams[k].position != pointLightComponent.transform.position)
						{
							pointLightParams[k].position = pointLightComponent.transform.position;
							isDirty = true;
						}

						if (pointLightParams[k].intensity != pointLightComponent.intensity)
						{
							pointLightParams[k].intensity = pointLightComponent.intensity;
							isDirty = true;
						}

						if (pointLightParams[k].lightParams == null)
						{
							pointLightParams[k].lightParams =
								pointLightParams[k].light.GetComponent<VolumetricFogLightParams>();
							if (pointLightParams[k].lightParams == null)
							{
								pointLightParams[k].lightParams = pointLightParams[k].light.gameObject
									.AddComponent<VolumetricFogLightParams>();
							}
						}

						pointLightParams[k].rangeMultiplier = pointLightParams[k].lightParams.rangeMultiplier;
						pointLightParams[k].intensityMultiplier = pointLightParams[k].lightParams.intensityMultiplier;
					}
				}

				SetPointLightMaterialProperties(mainCamera);
			}

			// Get shaft
			RenderTexture blurTmp2 = null;
			if (LIGHT_SCATTERING_BLUR_ENABLED && _lightScatteringEnabled && _lightScatteringExposure > 0)
			{
				RenderTextureDescriptor rtShaft = source.descriptor;
				rtShaft.msaaSamples = 1;
				RenderTexture shaftDestination = RenderTexture.GetTemporary(rtShaft);
				Graphics.Blit(source, shaftDestination, fogMat, 5);

				RenderTextureDescriptor rtBlur = rtShaft;
				rtBlur.width = GetScaledSize(rtBlur.width, _lightScatteringBlurDownscale);
				rtBlur.height = GetScaledSize(rtBlur.height, _lightScatteringBlurDownscale);
				RenderTexture blurTmp = RenderTexture.GetTemporary(rtBlur);
				blurTmp2 = RenderTexture.GetTemporary(rtBlur);
				Graphics.Blit(shaftDestination, blurTmp, fogMat, 7);
				Graphics.Blit(blurTmp, blurTmp2, fogMat, 6);
				RenderTexture.ReleaseTemporary(blurTmp);
				RenderTexture.ReleaseTemporary(shaftDestination);
				fogMat.SetTexture("_ShaftTex", blurTmp2);
			}

			// Render fog before transparent objects are drawn and only having into account the depth of opaque objects
			if (_downsampling > 1f || _forceComposition)
			{
				int scaledWidth = GetScaledSize(source.width, _downsampling);
				int scaledHeight = GetScaledSize(source.width, _downsampling);
				RenderTextureDescriptor rtReducedDestinationDesc = source.descriptor;
				rtReducedDestinationDesc.width = scaledWidth;
				rtReducedDestinationDesc.height = scaledHeight;
				rtReducedDestinationDesc.msaaSamples = 1;
				reducedDestination =
					RenderTexture.GetTemporary(
						rtReducedDestinationDesc); // scaledWidth, scaledHeight, 0, RenderTextureFormat.ARGB32);
				RenderTextureDescriptor rtReducedDepthDesc = source.descriptor;
				rtReducedDepthDesc.width = scaledWidth;
				rtReducedDepthDesc.height = scaledHeight;
				rtReducedDepthDesc.msaaSamples = 1;

				RenderTextureFormat rtFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat)
					? RenderTextureFormat.RFloat
					: RenderTextureFormat.ARGBFloat;
				rtReducedDestinationDesc.colorFormat = rtFormat;
				RenderTexture
					reducedDepthTexture =
						RenderTexture.GetTemporary(
							rtReducedDestinationDesc); // scaledWidth, scaledHeight, 0, rtFormat);

				if (_fogBlur)
				{
					SetBlurTexture(source, rtReducedDestinationDesc);
				}

				if (!_edgeImprove || vrOn || SystemInfo.supportedRenderTargetCount < 2)
				{
					Graphics.Blit(source, reducedDestination, fogMat, 3);
					if (_edgeImprove)
					{
						Graphics.Blit(source, reducedDepthTexture, fogMat, 4);
						fogMat.SetTexture("_DownsampledDepth", reducedDepthTexture);
					}
					else
					{
						fogMat.SetTexture("_DownsampledDepth", null); // required for metal
					}
				}
				else
				{
					fogMat.SetTexture("_MainTex", source);
					if (mrt == null)
					{
						mrt = new RenderBuffer[2];
					}

					mrt[0] = reducedDestination.colorBuffer;
					mrt[1] = reducedDepthTexture.colorBuffer;
					Graphics.SetRenderTarget(mrt, reducedDestination.depthBuffer);
					Graphics.Blit(null, fogMat, 1);
					fogMat.SetTexture("_DownsampledDepth", reducedDepthTexture);
				}

				fogMat.SetTexture("_FogDownsampled", reducedDestination);
				Graphics.Blit(source, destination, fogMat, 2);

				RenderTexture.ReleaseTemporary(reducedDepthTexture);
				RenderTexture.ReleaseTemporary(reducedDestination);
			}
			else
			{
				if (_fogBlur)
				{
					RenderTextureDescriptor rtReducedDestinationDesc = source.descriptor;
					rtReducedDestinationDesc.width = 256;
					rtReducedDestinationDesc.height = 256;
					SetBlurTexture(source, rtReducedDestinationDesc);
				}

				Graphics.Blit(source, destination, fogMat, 0);
			}

			if (shiftToZero && vrOn)
			{
				mainCamera.ResetWorldToCameraMatrix();
			}

			if (blurTmp2 != null)
			{
				RenderTexture.ReleaseTemporary(blurTmp2);
			}

			lastFrameCount = Time.frameCount;
		}

		private int GetScaledSize(int size, float factor)
		{
			size = (int) (size / factor);
			size /= 4;
			if (size < 1)
			{
				size = 1;
			}

			return size * 4;
		}

		#endregion


		#region Transparency support

		private void CleanUpDepthTexture()
		{
			if (depthTexture)
			{
				RenderTexture.ReleaseTemporary(depthTexture);
				depthTexture = null;
			}
		}

		private void GetTransparentDepth()
		{
			CleanUpDepthTexture();
			if (depthCam == null)
			{
				if (depthCamObj == null)
				{
					depthCamObj = GameObject.Find(DEPTH_CAM_NAME);
				}

				if (depthCamObj == null)
				{
					depthCamObj = new GameObject(DEPTH_CAM_NAME);
					depthCam = depthCamObj.AddComponent<Camera>();
					depthCam.enabled = false;
					depthCamObj.hideFlags = HideFlags.HideAndDontSave;
				}
				else
				{
					depthCam = depthCamObj.GetComponent<Camera>();
					if (depthCam == null)
					{
						DestroyImmediate(depthCamObj);
						depthCamObj = null;
						return;
					}
				}
			}

			depthCam.CopyFrom(mainCamera);
			depthCam.depthTextureMode = DepthTextureMode.None;
			depthTexture = RenderTexture.GetTemporary(mainCamera.pixelWidth, mainCamera.pixelHeight, 24,
				RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
			depthCam.backgroundColor = new Color(0, 0, 0, 0);
			depthCam.clearFlags = CameraClearFlags.SolidColor;
			depthCam.cullingMask = _transparencyLayerMask;
			depthCam.targetTexture = depthTexture;
			depthCam.renderingPath = RenderingPath.Forward;
			if (depthShader == null)
			{
				depthShader = Shader.Find("VolumetricFogAndMist/CopyDepth");
			}

			if (depthShaderAndTrans == null)
			{
				depthShaderAndTrans = Shader.Find("VolumetricFogAndMist/CopyDepthAndTrans");
			}

			switch (_computeDepthScope)
			{
				case COMPUTE_DEPTH_SCOPE.OnlyTreeBillboards:
					depthCam.RenderWithShader(depthShader, "RenderType");
					break;
				case COMPUTE_DEPTH_SCOPE.TreeBillboardsAndTransparentObjects:
					depthCam.RenderWithShader(depthShaderAndTrans, "RenderType");
					break;
				default:
					depthCam.RenderWithShader(depthShaderAndTrans, null);
					break;
			}

			Shader.SetGlobalTexture("_VolumetricFogDepthTexture", depthTexture);
		}

		#endregion

		#region Shadow support

#pragma warning disable 0429
#pragma warning disable 0162
		private void CastSunShadows()
		{
			if (USE_UNITY_SHADOW_MAP || !enabled || !gameObject.activeSelf || fogMat == null)
			{
				return;
			}

			if (_sunShadowsBakeMode == SUN_SHADOWS_BAKE_MODE.Discrete && _sunShadowsRefreshInterval > 0 &&
			    Time.time > lastShadowUpdateFrame + _sunShadowsRefreshInterval)
			{
				needUpdateDepthSunTexture = true;
			}

			if (!Application.isPlaying || needUpdateDepthSunTexture || depthSunTexture == null ||
			    !depthSunTexture.IsCreated())
			{
				needUpdateDepthSunTexture = false;
				lastShadowUpdateFrame = Time.time;
				GetSunShadows();
			}
		}
#pragma warning restore 0162
#pragma warning restore 0429

		private void GetSunShadows()
		{
			if (_sun == null || !_sunShadows)
			{
				return;
			}

			if (depthSunCam == null)
			{
				if (depthSunCamObj == null)
				{
					depthSunCamObj = GameObject.Find(DEPTH_SUN_CAM_NAME);
				}

				if (depthSunCamObj == null)
				{
					depthSunCamObj = new GameObject(DEPTH_SUN_CAM_NAME);
					depthSunCamObj.hideFlags = HideFlags.HideAndDontSave;
					depthSunCam = depthSunCamObj.AddComponent<Camera>();
				}
				else
				{
					depthSunCam = depthSunCamObj.GetComponent<Camera>();
					if (depthSunCam == null)
					{
						DestroyImmediate(depthSunCamObj);
						depthSunCamObj = null;
						return;
					}
				}

				if (depthSunShader == null)
				{
					depthSunShader = Shader.Find("VolumetricFogAndMist/CopySunDepth");
				}

				depthSunCam.SetReplacementShader(depthSunShader, "RenderType");
				depthSunCam.nearClipPlane = 1f;
				depthSunCam.renderingPath = RenderingPath.Forward;
				depthSunCam.orthographic = true;
				depthSunCam.aspect = 1f;
				depthSunCam.backgroundColor = new Color(0, 0, 0.5f, 0);
				depthSunCam.clearFlags = CameraClearFlags.SolidColor;
				depthSunCam.depthTextureMode = DepthTextureMode.None;
			}

			float shadowOrthoSize = _sunShadowsMaxDistance / 0.95f;
			const float farClip = 2000;
			depthSunCam.transform.position = mainCamera.transform.position - _sun.transform.forward * farClip;
			depthSunCam.transform.rotation = _sun.transform.rotation;
			depthSunCam.farClipPlane = farClip * 2f;
			depthSunCam.orthographicSize = shadowOrthoSize;
			if (sunLight != null)
			{
				depthSunCam.cullingMask = _sunShadowsLayerMask;
			}

			if (depthSunTexture == null || currentDepthSunTextureRes != _sunShadowsResolution)
			{
				currentDepthSunTextureRes = _sunShadowsResolution;
				int shadowTexResolution = (int) Mathf.Pow(2, _sunShadowsResolution + 9);
				depthSunTexture = new RenderTexture(shadowTexResolution, shadowTexResolution, 24,
					RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				depthSunTexture.hideFlags = HideFlags.DontSave;
				depthSunTexture.filterMode = FilterMode.Point;
				depthSunTexture.wrapMode = TextureWrapMode.Clamp;
				depthSunTexture.Create();
			}

			depthSunCam.targetTexture = depthSunTexture;
			Shader.SetGlobalFloat("_VF_ShadowBias", _sunShadowsBias);
			if (Application.isPlaying && _sunShadowsBakeMode == SUN_SHADOWS_BAKE_MODE.Realtime)
			{
				if (!depthSunCam.enabled)
				{
					depthSunCam.enabled = true;
				}
			}
			else
			{
				if (depthSunCam.enabled)
				{
					depthSunCam.enabled = false;
				}

				depthSunCam.Render();
			}

			Shader.SetGlobalMatrix("_VolumetricFogSunProj",
				depthSunCam.projectionMatrix * depthSunCam.worldToCameraMatrix);
			Shader.SetGlobalTexture("_VolumetricFogSunDepthTexture", depthSunTexture);
			Vector4 swp = depthSunCam.transform.position;
			swp.w = Mathf.Min(_sunShadowsMaxDistance, _maxFogLength);
			Shader.SetGlobalVector("_VolumetricFogSunWorldPos", swp);
			UpdateSunShadowsData();
		}

		#endregion

		#region Fog Blur support

		private void SetBlurTexture(RenderTexture source, RenderTextureDescriptor desc)
		{
			if (blurMat == null)
			{
				Shader blurShader = Shader.Find("VolumetricFogAndMist/Blur");
				blurMat = new Material(blurShader);
				blurMat.hideFlags = HideFlags.DontSave;
			}

			if (blurMat == null)
			{
				return;
			}

			blurMat.SetFloat("_BlurDepth", _fogBlurDepth);

			RenderTexture temp1 = RenderTexture.GetTemporary(desc); // source.width, source.height, 0, source.format);
			Graphics.Blit(source, temp1, blurMat, 0);
			RenderTexture temp2 = RenderTexture.GetTemporary(desc); // source.width, source.height, 0, source.format);
			Graphics.Blit(temp1, temp2, blurMat, 1);
			blurMat.SetFloat("_BlurDepth", _fogBlurDepth * 2f);
			temp1.DiscardContents();
			Graphics.Blit(temp2, temp1, blurMat, 0);
			temp2.DiscardContents();
			Graphics.Blit(temp1, temp2, blurMat, 1);
			fogMat.SetTexture("_BlurTex", temp2);
			RenderTexture.ReleaseTemporary(temp2);
			RenderTexture.ReleaseTemporary(temp1);
		}


		private void DestroySunShadowsDependencies()
		{
			if (depthSunCamObj != null)
			{
				DestroyImmediate(depthSunCamObj);
				depthSunCamObj = null;
			}

			CleanUpTextureDepthSun();
		}


		private void CleanUpTextureDepthSun()
		{
			if (depthSunTexture != null)
			{
				depthSunTexture.Release();
				depthSunTexture = null;
			}
		}

		#endregion


		#region Settings area

		public string GetCurrentPresetName()
		{
			return Enum.GetName(typeof(FOG_PRESET), _preset);
		}

		public void UpdatePreset()
		{
			switch (_preset)
			{
				case FOG_PRESET.Clear:
					_density = 0;
					_fogOfWarEnabled = false;
					_fogVoidRadius = 0;
					break;
				case FOG_PRESET.Mist:
					_skySpeed = 0.3f;
					_skyHaze = 15;
					_skyNoiseStrength = 0.1f;
					_skyAlpha = 0.8f;
					_density = 0.3f;
					_noiseStrength = 0.6f;
					_noiseScale = 1;
					_skyNoiseScale = 1;
					_noiseSparse = 0f;
					_distance = 0;
					_distanceFallOff = 0f;
					_maxFogLengthFallOff = 0.5f;
					_height = 6;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 8;
					_steppingNear = 0;
					_alpha = 1;
					_color = new Color(0.89f, 0.89f, 0.89f);
					_skyColor = new Color(0.81f, 0.81f, 0.81f);
					_specularColor = new Color(1, 1, 0.8f, 1);
					_specularIntensity = 0.1f;
					_specularThreshold = 0.6f;
					_lightColor = Color.white;
					_lightIntensity = 0.12f;
					_speed = 0.01f;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.WindyMist:
					_skySpeed = 0.3f;
					_skyHaze = 25;
					_skyNoiseStrength = 0.1f;
					_skyAlpha = 0.85f;
					_density = 0.3f;
					_noiseStrength = 0.5f;
					_noiseScale = 1.15f;
					_skyNoiseScale = 1.15f;
					_noiseSparse = 0f;
					_distance = 0;
					_distanceFallOff = 0f;
					_maxFogLengthFallOff = 0.5f;
					_height = 6.5f;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 10;
					_steppingNear = 0;
					_alpha = 1;
					_color = new Color(0.89f, 0.89f, 0.89f, 1);
					_skyColor = new Color(0.81f, 0.81f, 0.81f);
					_specularColor = new Color(1, 1, 0.8f, 1);
					_specularIntensity = 0.1f;
					_specularThreshold = 0.6f;
					_lightColor = Color.white;
					_lightIntensity = 0;
					_speed = 0.15f;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.GroundFog:
					_skySpeed = 0.3f;
					_skyHaze = 0;
					_skyNoiseStrength = 0.1f;
					_skyAlpha = 0.85f;
					_density = 0.6f;
					_noiseStrength = 0.479f;
					_noiseScale = 1.15f;
					_skyNoiseScale = 1.15f;
					_noiseSparse = 0f;
					_distance = 5;
					_distanceFallOff = 1f;
					_maxFogLengthFallOff = 0.6f;
					_height = 1.5f;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 8;
					_steppingNear = 0;
					_alpha = 0.95f;
					_color = new Color(0.89f, 0.89f, 0.89f, 1);
					_skyColor = new Color(0.79f, 0.79f, 0.79f, 1);
					_specularColor = new Color(1, 1, 0.8f, 1);
					_specularIntensity = 0.2f;
					_specularThreshold = 0.6f;
					_lightColor = Color.white;
					_lightIntensity = 0.2f;
					_speed = 0.01f;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.FrostedGround:
					_skySpeed = 0;
					_skyHaze = 0;
					_skyNoiseStrength = 0.729f;
					_skyAlpha = 0.55f;
					_density = 1;
					_noiseStrength = 0.164f;
					_noiseScale = 1.81f;
					_skyNoiseScale = 1.81f;
					_noiseSparse = 0f;
					_distance = 0;
					_distanceFallOff = 0;
					_height = 0.5f;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 20;
					_steppingNear = 50;
					_alpha = 0.97f;
					_color = new Color(0.546f, 0.648f, 0.710f, 1);
					_skyColor = _color;
					_specularColor = new Color(0.792f, 0.792f, 0.792f, 1);
					_specularIntensity = 1;
					_specularThreshold = 0.866f;
					_lightColor = new Color(0.972f, 0.972f, 0.972f, 1);
					_lightIntensity = 0.743f;
					_speed = 0;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.FoggyLake:
					_skySpeed = 0.3f;
					_skyHaze = 40;
					_skyNoiseStrength = 0.574f;
					_skyAlpha = 0.827f;
					_density = 1;
					_noiseStrength = 0.03f;
					_noiseScale = 5.77f;
					_skyNoiseScale = 5.77f;
					_noiseSparse = 0f;
					_distance = 0;
					_distanceFallOff = 0;
					_maxFogLengthFallOff = 0.6f;
					_height = 4;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 6;
					_steppingNear = 14.4f;
					_alpha = 1;
					_color = new Color(0, 0.960f, 1, 1);
					_skyColor = _color;
					_specularColor = Color.white;
					_lightColor = Color.white;
					_specularIntensity = 0.861f;
					_specularThreshold = 0.907f;
					_lightIntensity = 0.126f;
					_speed = 0;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.LowClouds:
					_skySpeed = 0.3f;
					_skyHaze = 60;
					_skyNoiseStrength = 0.97f;
					_skyAlpha = 0.96f;
					_density = 1;
					_noiseStrength = 0.7f;
					_noiseScale = 1;
					_skyNoiseScale = 1f;
					_noiseSparse = 0f;
					_distance = 0;
					_distanceFallOff = 0;
					_maxFogLengthFallOff = 0.6f;
					_height = 4f;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 12;
					_steppingNear = 0;
					_alpha = 0.98f;
					_color = new Color(0.89f, 0.89f, 0.89f, 1);
					_skyColor = new Color(0.79f, 0.79f, 0.79f, 1);
					_specularColor = new Color(1, 1, 0.8f, 1);
					_specularIntensity = 0.15f;
					_specularThreshold = 0.6f;
					_lightColor = Color.white;
					_lightIntensity = 0.15f;
					_speed = 0.008f;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.SeaClouds:
					_skySpeed = 0.3f;
					_skyHaze = 60;
					_skyNoiseStrength = 0.97f;
					_skyAlpha = 0.96f;
					_density = 1;
					_noiseStrength = 1;
					_noiseScale = 1.5f;
					_skyNoiseScale = 1.5f;
					_noiseSparse = 0f;
					_distance = 0;
					_distanceFallOff = 0;
					_maxFogLengthFallOff = 0.7f;
					_deepObscurance = 1f;
					_height = 12.4f;
					_heightFallOff = 0.6f;
					_stepping = 6;
					_alpha = 0.98f;
					_color = new Color(0.89f, 0.89f, 0.89f, 1);
					_skyColor = new Color(0.83f, 0.83f, 0.83f, 1);
					_specularColor = new Color(1, 1, 0.8f, 1);
					_specularIntensity = 0.259f;
					_specularThreshold = 0.6f;
					_lightColor = Color.white;
					_lightIntensity = 0.15f;
					_speed = 0.008f;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.Fog:
					_skySpeed = 0.3f;
					_skyHaze = 144;
					_skyNoiseStrength = 0.7f;
					_skyAlpha = 0.9f;
					_density = 0.35f;
					_noiseStrength = 0.3f;
					_noiseScale = 1;
					_skyNoiseScale = 1f;
					_noiseSparse = 0f;
					_distance = 20;
					_distanceFallOff = 0.7f;
					_maxFogLengthFallOff = 0.5f;
					_height = 8;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 8;
					_steppingNear = 0;
					_alpha = 0.97f;
					_color = new Color(0.89f, 0.89f, 0.89f, 1);
					_skyColor = new Color(0.79f, 0.79f, 0.79f, 1);
					_specularColor = new Color(1, 1, 0.8f, 1);
					_specularIntensity = 0;
					_specularThreshold = 0.6f;
					_lightColor = Color.white;
					_lightIntensity = 0;
					_speed = 0.05f;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.HeavyFog:
					_skySpeed = 0.05f;
					_skyHaze = 500;
					_skyNoiseStrength = 0.826f;
					_skyAlpha = 1;
					_density = 0.35f;
					_noiseStrength = 0.1f;
					_noiseScale = 1;
					_skyNoiseScale = 1f;
					_noiseSparse = 0f;
					_distance = 20;
					_distanceFallOff = 0.8f;
					_deepObscurance = 1f;
					_height = 18;
					_heightFallOff = 0.6f;
					_stepping = 6;
					_steppingNear = 0;
					_alpha = 1;
					_color = new Color(0.91f, 0.91f, 0.91f, 1f);
					_skyColor = new Color(0.79f, 0.79f, 0.79f, 1);
					_specularColor = new Color(1, 1, 0.8f, 1);
					_specularIntensity = 0;
					_specularThreshold = 0.6f;
					_lightColor = Color.white;
					_lightIntensity = 0;
					_speed = 0.015f;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.SandStorm1:
					_skySpeed = 0.35f;
					_skyHaze = 388;
					_skyNoiseStrength = 0.847f;
					_skyAlpha = 1;
					_density = 0.487f;
					_noiseStrength = 0.758f;
					_noiseScale = 1.71f;
					_skyNoiseScale = 1.71f;
					_noiseSparse = 0f;
					_distance = 0;
					_distanceFallOff = 0;
					_height = 16;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 6;
					_steppingNear = 0;
					_alpha = 1;
					_color = new Color(0.505f, 0.505f, 0.505f, 1);
					_skyColor = _color;
					_specularColor = new Color(1, 1, 0.8f, 1);
					_specularIntensity = 0;
					_specularThreshold = 0.6f;
					_lightColor = Color.white;
					_lightIntensity = 0;
					_speed = 0.3f;
					_windDirection = Vector3.right;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.Smoke:
					_skySpeed = 0.109f;
					_skyHaze = 10;
					_skyNoiseStrength = 0.119f;
					_skyAlpha = 1;
					_density = 1;
					_noiseStrength = 0.767f;
					_noiseScale = 1.6f;
					_skyNoiseScale = 1.6f;
					_noiseSparse = 0f;
					_distance = 0;
					_distanceFallOff = 0f;
					_maxFogLengthFallOff = 0.7f;
					_height = 8;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 12;
					_steppingNear = 25;
					_alpha = 1;
					_color = new Color(0.125f, 0.125f, 0.125f, 1);
					_skyColor = _color;
					_specularColor = new Color(1, 1, 1, 1);
					_specularIntensity = 0.575f;
					_specularThreshold = 0.6f;
					_lightColor = Color.white;
					_lightIntensity = 1;
					_speed = 0.075f;
					_windDirection = Vector3.right;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_baselineHeight += 8f;
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.ToxicSwamp:
					_skySpeed = 0.062f;
					_skyHaze = 22;
					_skyNoiseStrength = 0.694f;
					_skyAlpha = 1;
					_density = 1;
					_noiseStrength = 1;
					_noiseScale = 1;
					_skyNoiseScale = 1;
					_noiseSparse = 0f;
					_distance = 0;
					_distanceFallOff = 0;
					_height = 2.5f;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 20;
					_steppingNear = 50;
					_alpha = 0.95f;
					_color = new Color(0.0238f, 0.175f, 0.109f, 1);
					_skyColor = _color;
					_specularColor = new Color(0.593f, 0.625f, 0.207f, 1);
					_specularIntensity = 0.735f;
					_specularThreshold = 0.6f;
					_lightColor = new Color(0.730f, 0.746f, 0.511f, 1);
					_lightIntensity = 0.492f;
					_speed = 0.0003f;
					_windDirection = Vector3.right;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.SandStorm2:
					_skySpeed = 0;
					_skyHaze = 0;
					_skyNoiseStrength = 0.729f;
					_skyAlpha = 0.55f;
					_density = 0.545f;
					_noiseStrength = 1;
					_noiseScale = 3;
					_skyNoiseScale = 3;
					_noiseSparse = 0f;
					_distance = 0;
					_distanceFallOff = 0;
					_height = 12;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 5;
					_steppingNear = 19.6f;
					_alpha = 0.96f;
					_color = new Color(0.609f, 0.609f, 0.609f, 1);
					_skyColor = _color;
					_specularColor = new Color(0.589f, 0.621f, 0.207f, 1);
					_specularIntensity = 0.505f;
					_specularThreshold = 0.6f;
					_lightColor = new Color(0.726f, 0.742f, 0.507f, 1);
					_lightIntensity = 0.581f;
					_speed = 0.168f;
					_windDirection = Vector3.right;
					_downsampling = 1;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					_fogVoidRadius = 0;
					CopyTransitionValues();
					break;
				case FOG_PRESET.WorldEdge:
					_skySpeed = 0.3f;
					_skyHaze = 60;
					_skyNoiseStrength = 0.97f;
					_skyAlpha = 0.96f;
					_density = 1;
					_noiseStrength = 1;
					_noiseScale = 3f;
					_skyNoiseScale = 3f;
					_noiseSparse = 0f;
					_distance = 0;
					_distanceFallOff = 0;
					_maxFogLengthFallOff = 1f;
					_height = 20f;
					_heightFallOff = 0.6f;
					_deepObscurance = 1f;
					_stepping = 6;
					_alpha = 0.98f;
					_color = new Color(0.89f, 0.89f, 0.89f, 1);
					_skyColor = _color;
					_specularColor = new Color(1, 1, 0.8f, 1);
					_specularIntensity = 0.259f;
					_specularThreshold = 0.6f;
					_lightColor = Color.white;
					_lightIntensity = 0.15f;
					_speed = 0.03f;
					_downsampling = 2;
					_baselineRelativeToCamera = false;
					CheckWaterLevel(false);
					Terrain terrain = GetActiveTerrain();
					if (terrain != null)
					{
						_fogVoidPosition = terrain.transform.position + terrain.terrainData.size * 0.5f;
						_fogVoidRadius = terrain.terrainData.size.x * 0.45f;
						_fogVoidHeight = terrain.terrainData.size.y;
						_fogVoidDepth = terrain.terrainData.size.z * 0.45f;
						_fogVoidFallOff = 6f;
						_fogAreaRadius = 0;
						_character = null;
						_fogAreaCenter = null;
						float terrainSize = terrain.terrainData.size.x;
						if (mainCamera.farClipPlane < terrainSize)
						{
							mainCamera.farClipPlane = terrainSize;
						}

						if (_maxFogLength < terrainSize * 0.6f)
						{
							_maxFogLength = terrainSize * 0.6f;
						}
					}

					CopyTransitionValues();
					break;
			}

			currentFogAlpha = _alpha;
			currentFogColor = _color;
			currentFogSpecularColor = _specularColor;
			currentLightColor = _lightColor;
			currentSkyHazeAlpha = _skyAlpha;

			UpdateSun();
			FogOfWarUpdateTexture();
			UpdateMaterialProperties(true);
			UpdateRenderComponents();
			UpdateTextureAlpha();
			UpdateTexture();
			if (_sunShadows)
			{
				needUpdateDepthSunTexture = true;
			}
			else
			{
				DestroySunShadowsDependencies();
			}

			if (!Application.isPlaying)
			{
				UpdateWindSpeedQuick();
			}

			TrackPointLights();
			lastTimeSortInstances = 0;
		}

		public void CheckWaterLevel(bool baseZero)
		{
			if (mainCamera == null)
			{
				return;
			}

			if (_baselineHeight > mainCamera.transform.position.y || baseZero)
			{
				_baselineHeight = 0;
			}

#if GAIA_PRESENT
			GaiaSceneInfo sceneInfo = GaiaSceneInfo.GetSceneInfo();
			_baselineHeight = sceneInfo.m_seaLevel;
#else

			// Finds water
			GameObject water = GameObject.Find("Water");
			if (water == null)
			{
				GameObject[] gos = FindObjectsOfType<GameObject>();
				for (int k = 0; k < gos.Length; k++)
				{
					if (gos[k] != null && gos[k].layer == 4)
					{
						water = gos[k];
						break;
					}
				}
			}

			if (water != null)
			{
				_renderBeforeTransparent = false; // adds compatibility with water
				if (_baselineHeight < water.transform.position.y)
				{
					_baselineHeight = water.transform.position.y;
				}
			}
#endif
			UpdateMaterialHeights(mainCamera);
		}


        /// <summary>
        ///     Get the currently active terrain - or any terrain
        /// </summary>
        /// <returns>A terrain if there is one</returns>
        public static Terrain GetActiveTerrain()
		{
			//Grab active terrain if we can
			Terrain terrain = Terrain.activeTerrain;
			if (terrain != null && terrain.isActiveAndEnabled)
			{
				return terrain;
			}

			//Then check rest of terrains
			for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
			{
				terrain = Terrain.activeTerrains[idx];
				if (terrain != null && terrain.isActiveAndEnabled)
				{
					return terrain;
				}
			}

			return null;
		}

		private void UpdateMaterialFogColor()
		{
			Color color = currentFogColor;
			color.r *= 2f * temporaryProperties.color.r;
			color.g *= 2f * temporaryProperties.color.g;
			color.b *= 2f * temporaryProperties.color.b;
			color.a = 1f - _heightFallOff;
			fogMat.SetColor("_Color", color);
		}

		private void UpdateMaterialHeights(Camera mainCamera)
		{
			currentFogAltitude = _baselineHeight;
			Vector3 adjustedFogAreaPosition = _fogAreaPosition;
			if (_fogAreaRadius > 0)
			{
				//if (_fogAreaCenter != null && _fogAreaFollowMode == FOG_AREA_FOLLOW_MODE.FullXYZ) {
				//currentFogAltitude += _fogAreaCenter.transform.position.y;
				//}
				currentFogAltitude += _fogAreaPosition.y;
				if (_useXYPlane)
				{
					adjustedFogAreaPosition.z = 0; // baseHeight;
				}
				else
				{
					adjustedFogAreaPosition.y = 0; // baseHeight;
				}
			}

			if (_baselineRelativeToCamera && !_useXYPlane)
			{
				oldBaselineRelativeCameraY += (mainCamera.transform.position.y - oldBaselineRelativeCameraY) *
				                              Mathf.Clamp01(1.001f - _baselineRelativeToCameraDelay);
				currentFogAltitude += oldBaselineRelativeCameraY - 1f;
			}

			float scale = 0.01f / _noiseScale;
			fogMat.SetVector("_FogData",
				new Vector4(currentFogAltitude, _height, 1.0f / (_density * temporaryProperties.density), scale));
			fogMat.SetFloat("_FogSkyHaze", _skyHaze + currentFogAltitude);
			Vector3 v = _fogVoidPosition - currentFogAltitude * Vector3.up;
			fogMat.SetVector("_FogVoidPosition", v);
			fogMat.SetVector("_FogAreaPosition", adjustedFogAreaPosition);
		}

        /// <summary>
        ///     Updates the material properties.
        /// </summary>
        public void UpdateMaterialProperties(bool forceNow = false)
		{
			if (forceNow || !Application.isPlaying)
			{
				UpdateMaterialPropertiesNow();
			}
			else
			{
				shouldUpdateMaterialProperties = true;
			}
		}

        /// <summary>
        ///     Updates material immediately
        /// </summary>
        public void UpdateMaterialPropertiesNow()
		{
			UpdateMaterialPropertiesNow(mainCamera);
		}


		private void UpdateMaterialPropertiesNow(Camera mainCamera)
		{
			if (fogMat == null || fogRenderer == null)
			{
				return;
			}

			shouldUpdateMaterialProperties = false;

			UpdateSkyColor(_skyAlpha);

			fogMat.SetFloat("_DeepObscurance", _deepObscurance);

			Vector4 fogStepping = new Vector4(1.0f / (_stepping + 1.0f), 1 / (1 + _steppingNear), _edgeThreshold,
				_dithering ? _ditherStrength * 0.01f : 0f);
			fogMat.SetFloat("_Jitter", _jitterStrength);
			if (!_edgeImprove)
			{
				fogStepping.z = 0;
			}

			fogMat.SetVector("_FogStepping", fogStepping);
			fogMat.SetFloat("_FogAlpha", currentFogAlpha);
			UpdateMaterialHeights(mainCamera);
			float scale = 0.01f / _noiseScale;
			if (_maxFogLength < 0)
			{
				_maxFogLength = 0;
			}

			float maxFogLengthFallOff = _maxFogLength - _maxFogLength * (1f - _maxFogLengthFallOff) + 1.0f;
			fogMat.SetVector("_FogDistance",
				new Vector4(scale * scale * _distance * _distance, _distanceFallOff * _distanceFallOff + 0.1f,
					_maxFogLength, maxFogLengthFallOff));
			UpdateMaterialFogColor();

			// enable shader options
			if (shaderKeywords == null)
			{
				shaderKeywords = new List<string>();
			}
			else
			{
				shaderKeywords.Clear();
			}

			if (_distance > 0)
			{
				shaderKeywords.Add(SKW_FOG_DISTANCE_ON);
			}

			if (_fogVoidRadius > 0 && _fogVoidFallOff > 0)
			{
				Vector4 voidData = new Vector4(1.0f / (1.0f + _fogVoidRadius), 1.0f / (1.0f + _fogVoidHeight),
					1.0f / (1.0f + _fogVoidDepth), _fogVoidFallOff);
				if (_fogVoidTopology == FOG_VOID_TOPOLOGY.Box)
				{
					shaderKeywords.Add(SKW_FOG_VOID_BOX);
				}
				else
				{
					shaderKeywords.Add(SKW_FOG_VOID_SPHERE);
				}

				fogMat.SetVector("_FogVoidData", voidData);
			}

			if (_fogAreaRadius > 0 && _fogAreaFallOff > 0)
			{
				Vector4 areaData = new Vector4(1.0f / (0.0001f + _fogAreaRadius), 1.0f / (0.0001f + _fogAreaHeight),
					1.0f / (0.0001f + _fogAreaDepth), _fogAreaFallOff);
				if (_fogAreaTopology == FOG_AREA_TOPOLOGY.Box)
				{
					shaderKeywords.Add(SKW_FOG_AREA_BOX);
				}
				else
				{
					shaderKeywords.Add(SKW_FOG_AREA_SPHERE);
					areaData.y = _fogAreaRadius * _fogAreaRadius;
					areaData.x /= scale;
					areaData.z /= scale;
				}

				fogMat.SetVector("_FogAreaData", areaData);
			}

			if (_skyHaze < 0)
			{
				_skyHaze = 0;
			}

			if (_skyHaze > 0 && _skyAlpha > 0 && !_useXYPlane && hasCamera)
			{
				shaderKeywords.Add(SKW_FOG_HAZE_ON);
			}

			if (_fogOfWarEnabled)
			{
				shaderKeywords.Add(SKW_FOG_OF_WAR_ON);
				fogMat.SetTexture("_FogOfWar", fogOfWarTexture);
				fogMat.SetVector("_FogOfWarCenter", _fogOfWarCenter);
				fogMat.SetVector("_FogOfWarSize", _fogOfWarSize);
				Vector3 ca = _fogOfWarCenter - 0.5f * _fogOfWarSize;
				if (_useXYPlane)
				{
					fogMat.SetVector("_FogOfWarCenterAdjusted",
						new Vector3(ca.x / _fogOfWarSize.x, ca.y / (_fogOfWarSize.y + 0.0001f), 1f));
				}
				else
				{
					fogMat.SetVector("_FogOfWarCenterAdjusted",
						new Vector3(ca.x / _fogOfWarSize.x, 1f, ca.z / (_fogOfWarSize.z + 0.0001f)));
				}
			}


			// Check proper array initialization of point lights
			CheckPointLightData();

			bool usesPointLights = false;
			for (int k = 0; k < pointLightParams.Length; k++)
			{
				if (pointLightParams[k].light != null || pointLightParams[k].range * pointLightParams[k].intensity > 0)
				{
					usesPointLights = true;
					break;
				}
			}

			if (usesPointLights)
			{
				fogMat.SetFloat("_PointLightInsideAtten", _pointLightInsideAtten);
				shaderKeywords.Add(SKW_POINT_LIGHTS);
			}

			sunShadowsActive = false;
			if (fogRenderer.sun)
			{
				UpdateScatteringData(mainCamera);
				if (_lightScatteringEnabled)
				{
					if (_lightScatteringExposure > 0)
					{
						shaderKeywords.Add(SKW_LIGHT_SCATTERING);
					}
				}

				if (_sunShadows)
				{
					sunShadowsActive = true;
					shaderKeywords.Add(SKW_SUN_SHADOWS);
					UpdateSunShadowsData();
					SetupDirectionalLightCommandBuffer();
				}
			}

			if (_fogBlur)
			{
				shaderKeywords.Add(SKW_FOG_BLUR);
				fogMat.SetFloat("_FogBlurDepth", _fogBlurDepth);
			}

			if (_useXYPlane)
			{
				shaderKeywords.Add(SKW_FOG_USE_XY_PLANE);
			}

			if (fogRenderer.computeDepth)
			{
				shaderKeywords.Add(SKW_FOG_COMPUTE_DEPTH);
			}

			fogMat.shaderKeywords = shaderKeywords.ToArray();

			if (_computeDepth && _computeDepthScope == COMPUTE_DEPTH_SCOPE.TreeBillboardsAndTransparentObjects)
			{
				Shader.SetGlobalFloat("_VFM_CutOff", _transparencyCutOff);
			}
		}

        /// <summary>
        ///     Notifies other fog instancies of property changes on this main cam script
        /// </summary>
        public void NotifyChangesToFogInstances()
		{
			if (!hasCamera)
			{
				return;
			}

			int instancesCount = instances != null ? instances.Count : 0;
			for (int k = 0; k < instancesCount; k++)
			{
				VolumetricFog fog = instances[k];
				if (fog != null && fog != this)
				{
					fog.UpdateMaterialProperties();
				}
			}
		}

		private void UpdateSunShadowsData()
		{
			if (_sun == null || !_sunShadows || fogMat == null)
			{
				return;
			}

			float shadowStrength = _sunShadowsStrength * Mathf.Clamp01(-_sun.transform.forward.y * 10f);
			if (shadowStrength < 0)
			{
				shadowStrength = 0;
			}

			if (shadowStrength > 0 && !fogMat.IsKeywordEnabled(SKW_SUN_SHADOWS))
			{
				fogMat.EnableKeyword(SKW_SUN_SHADOWS);
			}
			else if (shadowStrength <= 0 && fogMat.IsKeywordEnabled(SKW_SUN_SHADOWS))
			{
				fogMat.DisableKeyword(SKW_SUN_SHADOWS);
			}

			if (_hasCamera)
			{
				Shader.SetGlobalVector("_VolumetricFogSunShadowsData",
					new Vector4(shadowStrength, _sunShadowsJitterStrength, _sunShadowsCancellation, 0));
#pragma warning disable 0162
#pragma warning disable 0429
				if (USE_DIRECTIONAL_LIGHT_COOKIE && sunLight != null)
				{
					fogMat.SetTexture("_VolumetricFogLightCookie", sunLight.cookie);
				}
#pragma warning restore 0429
#pragma warning restore 0162
			}
		}

#pragma warning disable 0162
#pragma warning disable 0429
		private void SetupDirectionalLightCommandBuffer()
		{
			if (!USE_UNITY_SHADOW_MAP || _sun == null)
			{
				return;
			}

			Light light = _sun.GetComponent<Light>();
			if (light == null || light.type != LightType.Directional)
			{
				Debug.LogError("Sun reference does not have a valid directional light.");
				return;
			}

			if (_sun.GetComponent<ShadowMapCopy>() == null)
			{
				_sun.AddComponent<ShadowMapCopy>();
			}
		}
#pragma warning restore 0429
#pragma warning restore 0162

		private void RemoveDirectionalLightCommandBuffer()
		{
			if (_sun == null)
			{
				return;
			}

			ShadowMapCopy sm = _sun.GetComponent<ShadowMapCopy>();
			if (sm != null)
			{
				DestroyImmediate(sm);
			}
		}

		private void UpdateWindSpeedQuick()
		{
			if (fogMat == null)
			{
				return;
			}

			if (Application.isPlaying && lastFrameAppliedWind == Time.frameCount)
			{
				return;
			}

			lastFrameAppliedWind = Time.frameCount;

			// fog speed
			windSpeedAcum += deltaTime * _windDirection * _speed;
			fogMat.SetVector("_FogWindDir",
				new Vector3(windSpeedAcum.x % noiseTextureSize, 0, windSpeedAcum.z % noiseTextureSize));
			// sky speed
			skyHazeSpeedAcum += deltaTime * _skySpeed / 20f;
			fogMat.SetVector("_FogSkyData",
				new Vector4(_skyHaze, _skyNoiseStrength / (0.0001f + _density * temporaryProperties.density),
					skyHazeSpeedAcum, _skyDepth));
		}


		private void UpdateScatteringData(Camera mainCamera)
		{
			Vector3 sunSkyPos = mainCamera.transform.position + _lightDirection * 1000f;
			Vector3 viewportPos = mainCamera.WorldToViewportPoint(sunSkyPos,
				XRSettings.enabled ? Camera.MonoOrStereoscopicEye.Left : Camera.MonoOrStereoscopicEye.Mono);
			if (viewportPos.z < 0)
			{
				Vector2 screenSunPos = new Vector2(viewportPos.x, viewportPos.y);
				float night = Mathf.Clamp01(1.0f - _lightDirection.y);
				if (screenSunPos != oldSunPos)
				{
					oldSunPos = screenSunPos;
					sunFade = Mathf.SmoothStep(1, 0, (screenSunPos - Vector2.one * 0.5f).magnitude * 0.5f) * night;
				}

				fogMat.SetVector("_SunPosition", screenSunPos);
				if (XRSettings.enabled)
				{
					Vector3 sunScrPosRightEye =
						mainCamera.WorldToViewportPoint(sunSkyPos, Camera.MonoOrStereoscopicEye.Right);
					fogMat.SetVector("_SunPositionRightEye", sunScrPosRightEye);
				}

				if (_lightScatteringEnabled && !fogMat.IsKeywordEnabled(SKW_LIGHT_SCATTERING))
				{
					fogMat.EnableKeyword(SKW_LIGHT_SCATTERING);
				}

				float intensity = _lightScatteringExposure * sunFade;
				fogMat.SetVector("_FogScatteringData",
					new Vector4(_lightScatteringSpread / _lightScatteringSamples,
						intensity > 0 ? _lightScatteringSamples : 0, intensity,
						_lightScatteringWeight / _lightScatteringSamples));
				fogMat.SetVector("_FogScatteringData2",
					new Vector4(_lightScatteringIllumination, _lightScatteringDecay, _lightScatteringJittering,
						_lightScatteringEnabled && LIGHT_DIFFUSION_ENABLED
							? 1.2f * _lightScatteringDiffusion * night * sunLightIntensity
							: 0));
				fogMat.SetVector("_SunDir", -_lightDirection);
				fogMat.SetColor("_SunColor", _lightColor);
			}
			else
			{
				if (fogMat.IsKeywordEnabled(SKW_LIGHT_SCATTERING))
				{
					fogMat.DisableKeyword(SKW_LIGHT_SCATTERING);
				}
			}
		}

		private void UpdateSun()
		{
			if (fogRenderer != null && fogRenderer.sun != null)
			{
				sunLight = fogRenderer.sun.GetComponent<Light>();
			}
			else
			{
				sunLight = null;
			}
		}

		private void UpdateSkyColor(float alpha)
		{
			if (fogMat == null)
			{
				return;
			}

			Color skyColorAdj = skyHazeLightColor;
			skyColorAdj.a = alpha;
			fogMat.SetColor("_FogSkyColor", skyColorAdj);
			fogMat.SetFloat("_FogSkyNoiseScale", 0.01f / _skyNoiseScale);
		}

		#endregion

		#region Noise texture work

		private void UpdateTextureAlpha()
		{
			// Precompute fog height into alpha channel
			if (adjustedColors == null)
			{
				return;
			}

			float fogNoise = Mathf.Clamp(_noiseStrength, 0, 0.95f); // clamped to prevent flat fog on top
			for (int k = 0; k < adjustedColors.Length; k++)
			{
				float t = 1.0f - (_noiseSparse + noiseColors[k].b) * fogNoise;
				t *= _density * temporaryProperties.density * _noiseFinalMultiplier;
				if (t < 0)
				{
					t = 0;
				}
				else if (t > 1)
				{
					t = 1f;
				}

				adjustedColors[k].a = t;
			}

			hasChangeAdjustedColorsAlpha = true;
		}


		private void UpdateTexture()
		{
			if (fogMat == null)
			{
				return;
			}

			// Precompute light color
			ComputeLightColor();

			if (Application.isPlaying)
			{
				// && !hasChangeAdjustedColorsAlpha) {
				updatingTextureSlice = 0;
			}
			else
			{
				updatingTextureSlice = -1;
			}

			UpdateTextureColors(adjustedColors, hasChangeAdjustedColorsAlpha);
			needUpdateTexture = false;

			// Check Sun position
			UpdateSkyColor(_skyAlpha);
		}

		private void ComputeLightColor()
		{
			float fogIntensity = _lightIntensity + sunLightIntensity;
			if (!_useXYPlane)
			{
				fogIntensity *= Mathf.Clamp01(1.0f - _lightDirection.y * 2.0f); // simulates sunset
			}

			switch (_lightingModel)
			{
				default:
					lastRenderSettingsAmbientLight = RenderSettings.ambientLight;
					lastRenderSettingsAmbientIntensity = RenderSettings.ambientIntensity;
					Color ambientMultiplied = lastRenderSettingsAmbientLight * lastRenderSettingsAmbientIntensity;
					updatingTextureLightColor =
						Color.Lerp(ambientMultiplied, currentLightColor * fogIntensity, fogIntensity);
					skyHazeLightColor = Color.Lerp(ambientMultiplied, _skyColor * fogIntensity, fogIntensity);
					break;
				case LIGHTING_MODEL.Natural:
					lastRenderSettingsAmbientLight = RenderSettings.ambientLight;
					lastRenderSettingsAmbientIntensity = RenderSettings.ambientIntensity;
					updatingTextureLightColor = Color.Lerp(lastRenderSettingsAmbientLight,
						currentLightColor * fogIntensity + lastRenderSettingsAmbientLight, _lightIntensity);
					skyHazeLightColor = Color.Lerp(lastRenderSettingsAmbientLight,
						_skyColor * fogIntensity + lastRenderSettingsAmbientLight, _lightIntensity);
					break;
				case LIGHTING_MODEL.SingleLight:
					lastRenderSettingsAmbientLight = Color.black;
					lastRenderSettingsAmbientIntensity = RenderSettings.ambientIntensity;
					updatingTextureLightColor = Color.Lerp(lastRenderSettingsAmbientLight,
						currentLightColor * fogIntensity, _lightIntensity);
					skyHazeLightColor = Color.Lerp(lastRenderSettingsAmbientLight, _skyColor * fogIntensity,
						_lightIntensity);
					break;
			}
		}

		private void UpdateTextureColors(Color[] colors, bool forceUpdateEntireTexture)
		{
			Vector3 nlight;
			int nz, disp;
			float nyspec;
			float spec = 1.0001f - _specularThreshold;
			int tw = adjustedTexture.width;
			nlight = new Vector3(-_lightDirection.x, 0, -_lightDirection.z).normalized * 0.3f;
			nlight.y = _lightDirection.y > 0
				? Mathf.Clamp01(1.0f - _lightDirection.y)
				: 1.0f - Mathf.Clamp01(-_lightDirection.y);
			nz = Mathf.FloorToInt(nlight.z * tw) * tw;
			disp = (int) (nz + nlight.x * tw) + colors.Length;
			nyspec = nlight.y / spec;
			Color specularColor = currentFogSpecularColor * (1.0f + _specularIntensity) * _specularIntensity;
			bool hasChanged = false;
			if (updatingTextureSlice >= 1 || forceUpdateEntireTexture)
			{
				hasChanged = true;
			}

			float lcr = updatingTextureLightColor.r * 0.5f;
			float lcg = updatingTextureLightColor.g * 0.5f;
			float lcb = updatingTextureLightColor.b * 0.5f;
			float scr = specularColor.r * 0.5f;
			float scg = specularColor.g * 0.5f;
			float scb = specularColor.b * 0.5f;

			int count = colors.Length;
			int k0 = 0;
			int k1 = count;
			if (updatingTextureSlice >= 0)
			{
				if (updatingTextureSlice > _updateTextureSpread)
				{
					// detected change of configuration amid texture updates
					updatingTextureSlice = -1;
					needUpdateTexture = true;
					return;
				}

				k0 = count * updatingTextureSlice / _updateTextureSpread;
				k1 = count * (updatingTextureSlice + 1) / _updateTextureSpread;
			}

			int z = 0;
			for (int k = k0; k < k1; k++)
			{
				int indexg = (k + disp) % count;
				float a = colors[k].a;
				float r = (a - colors[indexg].a) * nyspec;
				if (r < 0f)
				{
					r = 0f;
				}
				else if (r > 1f)
				{
					r = 1f;
				}

				float cor = lcr + scr * r;
				float cog = lcg + scg * r;
				float cob = lcb + scb * r;
				if (!hasChanged)
				{
					if (z++ < 100)
					{
						if (cor != colors[k].r || cog != colors[k].g || cob != colors[k].b)
						{
							hasChanged = true;
						}
					}
					else if (!hasChanged)
					{
						break;
					}
				}

				colors[k].r = cor;
				colors[k].g = cog;
				colors[k].b = cob;
			}

			bool hasNewTextureData = forceUpdateEntireTexture;
			if (hasChanged)
			{
				if (updatingTextureSlice >= 0)
				{
					updatingTextureSlice++;
					if (updatingTextureSlice >= _updateTextureSpread)
					{
						updatingTextureSlice = -1;
						hasNewTextureData = true;
					}
				}
				else
				{
					hasNewTextureData = true;
				}
			}
			else
			{
				updatingTextureSlice = -1;
			}

			if (hasNewTextureData)
			{
				if (Application.isPlaying && _turbulenceStrength > 0f && adjustedChaosTexture)
				{
					adjustedChaosTexture.SetPixels(adjustedColors);
					adjustedChaosTexture.Apply();
				}
				else
				{
					adjustedTexture.SetPixels(adjustedColors);
					adjustedTexture.Apply();
					fogMat.SetTexture("_NoiseTex", adjustedTexture);
				}

				lastTextureUpdate = Time.time;
			}
		}

		internal void ApplyChaos()
		{
			if (!adjustedTexture || Application.isPlaying && lastFrameAppliedChaos == Time.frameCount)
			{
				return;
			}

			lastFrameAppliedChaos = Time.frameCount;

			if (chaosLerpMat == null)
			{
				Shader chaosLerp = Shader.Find("VolumetricFogAndMist/Chaos Lerp");
				chaosLerpMat = new Material(chaosLerp);
				chaosLerpMat.hideFlags = HideFlags.DontSave;
			}

			turbAcum += deltaTime * _turbulenceStrength;
			chaosLerpMat.SetFloat("_Amount", turbAcum);

			if (!adjustedChaosTexture)
			{
				adjustedChaosTexture = Instantiate(adjustedTexture);
				adjustedChaosTexture.hideFlags = HideFlags.DontSave;
			}

			RenderTexture rtAdjusted = RenderTexture.GetTemporary(adjustedTexture.width, adjustedTexture.height, 0,
				RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			rtAdjusted.wrapMode = TextureWrapMode.Repeat;
			Graphics.Blit(adjustedChaosTexture, rtAdjusted, chaosLerpMat);
			fogMat.SetTexture("_NoiseTex", rtAdjusted);
			RenderTexture.ReleaseTemporary(rtAdjusted);
		}

		#endregion


		#region Fog Volume

		private void CopyTransitionValues()
		{
			currentFogAlpha = _alpha;
			currentSkyHazeAlpha = _skyAlpha;
			currentFogColor = _color;
			currentFogSpecularColor = _specularColor;
			currentLightColor = _lightColor;
		}


		public void SetTargetProfile(VolumetricFogProfile targetProfile, float duration)
		{
			if (!_useFogVolumes)
			{
				return;
			}

			initialProfile = ScriptableObject.CreateInstance<VolumetricFogProfile>();
			initialProfile.Save(this);
			this.targetProfile = targetProfile;
			transitionDuration = duration;
			transitionStartTime = Time.time;
			transitionProfile = true;
		}

		public void ClearTargetProfile(float duration)
		{
			SetTargetProfile(initialProfile, duration);
		}


		public void SetTargetAlpha(float newFogAlpha, float newSkyHazeAlpha, float duration)
		{
			if (!_useFogVolumes)
			{
				return;
			}

			initialFogAlpha = currentFogAlpha;
			initialSkyHazeAlpha = currentSkyHazeAlpha;
			targetFogAlpha = newFogAlpha;
			targetSkyHazeAlpha = newSkyHazeAlpha;
			transitionDuration = duration;
			transitionStartTime = Time.time;
			transitionAlpha = true;
		}

		public void ClearTargetAlpha(float duration)
		{
			SetTargetAlpha(-1, -1, duration);
		}

		public void SetTargetColor(Color newColor, float duration)
		{
			if (!useFogVolumes)
			{
				return;
			}

			initialFogColor = currentFogColor;
			targetFogColor = newColor;
			transitionDuration = duration;
			transitionStartTime = Time.time;
			transitionColor = true;
			targetColorActive = true;
		}

		public void ClearTargetColor(float duration)
		{
			SetTargetColor(_color, duration);
			targetColorActive = false;
		}

		public void SetTargetSpecularColor(Color newSpecularColor, float duration)
		{
			if (!useFogVolumes)
			{
				return;
			}

			initialFogSpecularColor = currentFogSpecularColor;
			targetFogSpecularColor = newSpecularColor;
			transitionDuration = duration;
			transitionStartTime = Time.time;
			transitionSpecularColor = true;
			targetSpecularColorActive = true;
		}

		public void ClearTargetSpecularColor(float duration)
		{
			SetTargetSpecularColor(_specularColor, duration);
			targetSpecularColorActive = false;
		}


		public void SetTargetLightColor(Color newLightColor, float duration)
		{
			if (!useFogVolumes)
			{
				return;
			}

			_sunCopyColor = false;
			initialLightColor = currentLightColor;
			targetLightColor = newLightColor;
			transitionDuration = duration;
			transitionStartTime = Time.time;
			transitionLightColor = true;
			targetLightColorActive = true;
		}

		public void ClearTargetLightColor(float duration)
		{
			SetTargetLightColor(_lightColor, duration);
			targetLightColorActive = false;
		}

		#endregion


		#region Point Light functions

		public void CheckPointLightData()
		{
			if (_pointLightTrackingPivot == null)
			{
				_pointLightTrackingPivot = transform;
			}

			// migrate old values
			if (!pointLightDataMigrated)
			{
				pointLightParams = new PointLightParams[MAX_POINT_LIGHTS];
				for (int k = 0; k < _pointLightColors.Length; k++)
				{
					pointLightParams[k].color = _pointLightColors[k];
					Light light = null;
					if (_pointLights[k] != null)
					{
						light = _pointLights[k].GetComponent<Light>();
					}

					pointLightParams[k].light = light;
					pointLightParams[k].intensity = _pointLightIntensities[k];
					pointLightParams[k].intensityMultiplier = _pointLightIntensitiesMultiplier[k];
					pointLightParams[k].position = _pointLightPositions[k];
					pointLightParams[k].range = _pointLightRanges[k];
					pointLightParams[k].rangeMultiplier = 1f;
				}

				for (int k = _pointLightColors.Length; k < MAX_POINT_LIGHTS; k++)
				{
					PointLightDataSetDefaults(k);
				}

				pointLightDataMigrated = true;
				isDirty = true;
#if UNITY_EDITOR
				EditorUtility.SetDirty(this);
#endif
			}

			if (_pointLightTrackingCount > MAX_POINT_LIGHTS)
			{
				_pointLightTrackingCount = MAX_POINT_LIGHTS;
				isDirty = true;
#if UNITY_EDITOR
				EditorUtility.SetDirty(this);
#endif
			}

			// Ensure array consistency
			if (pointLightParams != null)
			{
				if (pointLightParams.Length != MAX_POINT_LIGHTS)
				{
					PointLightParams[] newData = new PointLightParams[MAX_POINT_LIGHTS];
					int count = Mathf.Min(newData.Length, pointLightParams.Length);
					Array.Copy(pointLightParams, newData, count);
					pointLightParams = newData;
					for (int k = count; k < newData.Length; k++)
					{
						PointLightDataSetDefaults(k);
					}

					isDirty = true;
#if UNITY_EDITOR
					EditorUtility.SetDirty(this);
#endif
				}

				for (int k = 0; k < pointLightParams.Length; k++)
				{
					if (pointLightParams[k].rangeMultiplier <= 0)
					{
						pointLightParams[k].rangeMultiplier = 1f;
					}
				}
			}
			else
			{
				pointLightParams = new PointLightParams[MAX_POINT_LIGHTS];
				for (int k = 0; k < pointLightParams.Length; k++)
				{
					PointLightDataSetDefaults(k);
				}

				isDirty = true;
#if UNITY_EDITOR
				EditorUtility.SetDirty(this);
#endif
			}

			if (currentLights == null || currentLights.Length != MAX_POINT_LIGHTS)
			{
				currentLights = new Light[MAX_POINT_LIGHTS];
			}
		}

		private void PointLightDataSetDefaults(int k)
		{
			if (k < pointLightParams.Length)
			{
				pointLightParams[k].color = new Color(1, 1, 0, 1);
				pointLightParams[k].intensity = 1f;
				pointLightParams[k].intensityMultiplier = 1f;
				pointLightParams[k].range = 0f;
				pointLightParams[k].rangeMultiplier = 1f;
			}
		}


		private void SetPointLightMaterialProperties(Camera mainCamera)
		{
			int maxLights = pointLightParams.Length;
			if (pointLightColorBuffer == null || pointLightColorBuffer.Length != maxLights)
			{
				pointLightColorBuffer = new Vector4[maxLights];
			}

			if (pointLightPositionBuffer == null || pointLightPositionBuffer.Length != maxLights)
			{
				pointLightPositionBuffer = new Vector4[maxLights];
			}

			Vector3 camPos = mainCamera != null ? mainCamera.transform.position : Vector3.zero;
			for (int k = 0; k < maxLights; k++)
			{
				Vector3 pos = pointLightParams[k].position;
				if (!sunShadowsActive)
				{
					// when sun shadows are enabled, fogCeilingCut is not displaced in the shader
					pos.y -= _baselineHeight;
				}

				float range = pointLightParams[k].range * pointLightParams[k].rangeMultiplier *
					_pointLightInscattering / 25f; // note: 25 comes from Unity point light attenuation equation
				float multiplier = pointLightParams[k].intensity * pointLightParams[k].intensityMultiplier *
				                   _pointLightIntensity;

				if (range > 0 && multiplier > 0)
				{
					// Apply attenuation if light is affected by fog distance & falloff
					if (_distance > 0)
					{
						float scale = 0.01f / _noiseScale;
						float distScaled = _distance * scale;
						Vector2 lpos2 = new Vector2((camPos.x - pos.x) * scale, (camPos.z - pos.z) * scale);
						float atten = Mathf.Max(distScaled * distScaled - lpos2.sqrMagnitude, 0);
						atten *= _distanceFallOff * _distanceFallOff + 0.1f;
						multiplier = multiplier > atten ? multiplier - atten : 0;
					}

					pointLightPositionBuffer[k].x = pos.x;
					pointLightPositionBuffer[k].y = pos.y;
					pointLightPositionBuffer[k].z = pos.z;
					pointLightPositionBuffer[k].w = 0;
					pointLightColorBuffer[k] = new Vector4(pointLightParams[k].color.r * multiplier,
						pointLightParams[k].color.g * multiplier, pointLightParams[k].color.b * multiplier, range);
				}
				else
				{
					pointLightColorBuffer[k] = black;
				}
			}

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (fogMat.HasProperty("_FogPointLightColor"))
				{
					Color[] existingColors = fogMat.GetColorArray("_FogPointLightColor");
					if (existingColors.Length != pointLightColorBuffer.Length)
					{
						InitFogMaterial();
						InternalEditorUtility.RepaintAllViews();
					}
				}
			}
#endif

			fogMat.SetVectorArray("_FogPointLightColor", pointLightColorBuffer);
			fogMat.SetVectorArray("_FogPointLightPosition", pointLightPositionBuffer);
		}

		public Light GetPointLight(int index)
		{
			if (index < 0 || index >= pointLightParams.Length)
			{
				return null;
			}

			return pointLightParams[index].light;
		}

		// Look for new lights
		private void TrackNewLights()
		{
			lastFoundLights = FindObjectsOfType<Light>();
		}

        /// <summary>
        ///     Look for nearest point lights
        /// </summary>
        public void TrackPointLights(bool forceImmediateUpdate = false)
		{
			if (!_pointLightTrackingAuto)
			{
				return;
			}

			if (_pointLightTrackingPivot == null)
			{
				_pointLightTrackingPivot = transform;
			}

			// Look for new lights?
			if (forceImmediateUpdate || lastFoundLights == null || !Application.isPlaying ||
			    _pointLightTrackingNewLightsCheckInterval > 0 && Time.time - trackPointCheckNewLightsLastTime >
			    _pointLightTrackingNewLightsCheckInterval)
			{
				trackPointCheckNewLightsLastTime = Time.time;
				TrackNewLights();
			}

			// Sort nearest lights
			int lightsFoundCount = lastFoundLights.Length;
			if (lightBuffer == null || lightBuffer.Length != lightsFoundCount)
			{
				lightBuffer = new Light[lightsFoundCount];
			}

			for (int k = 0; k < lightsFoundCount; k++)
			{
				lightBuffer[k] = lastFoundLights[k];
			}

			bool changes = false;
			for (int k = 0; k < pointLightParams.Length && k < currentLights.Length; k++)
			{
				Light g = null;
				if (k < _pointLightTrackingCount)
				{
					g = GetNearestLight(lightBuffer);
				}

				pointLightParams[k].light = g;
				if (pointLightParams[k].range != 0 && g == null)
				{
					pointLightParams[k].range = 0; // disables the light in case g is null
				}

				if (currentLights[k] != g)
				{
					currentLights[k] = g;
					changes = true;
				}
			}

			// Update if there's any change
			if (changes)
			{
				UpdateMaterialProperties();
			}
		}

		private Light GetNearestLight(Light[] lights)
		{
			float minDist = float.MaxValue;
			Vector3 camPos = _pointLightTrackingPivot.position;
			Light nearest = null;
			int selected = -1;
			for (int k = 0; k < lights.Length; k++)
			{
				Light light = lights[k];
				if (light == null || !light.isActiveAndEnabled || light.type != LightType.Point)
				{
					continue;
				}

				float dist = (light.transform.position - camPos).sqrMagnitude;
				if (dist < minDist)
				{
					nearest = light;
					minDist = dist;
					selected = k;
				}
			}

			if (selected >= 0)
			{
				lights[selected] = null;
			}

			return nearest;
		}

		#endregion

		#region Fog Area API

		public static VolumetricFog CreateFogArea(Vector3 position, float radius, float height = 16, float fallOff = 1f)
		{
			VolumetricFog fog = CreateFogAreaPlaceholder(true, position, radius, height, radius);
			fog.preset = FOG_PRESET.SeaClouds;
			fog.transform.position = position;
			fog.skyHaze = 0;
			fog.dithering = true;
			return fog;
		}

		public static VolumetricFog CreateFogArea(Vector3 position, Vector3 boxSize)
		{
			VolumetricFog fog =
				CreateFogAreaPlaceholder(false, position, boxSize.x * 0.5f, boxSize.y * 0.5f, boxSize.z * 0.5f);
			fog.preset = FOG_PRESET.SeaClouds;
			fog.transform.position = position;
			fog.height = boxSize.y * 0.98f;
			fog.skyHaze = 0;
			return fog;
		}

		private static VolumetricFog CreateFogAreaPlaceholder(bool spherical, Vector3 position, float radius,
			float height, float depth)
		{
			GameObject prefab = spherical
				? Resources.Load<GameObject>("Prefabs/FogSphereArea")
				: Resources.Load<GameObject>("Prefabs/FogBoxArea");
			GameObject box = Instantiate(prefab);
			box.transform.position = position;
			box.transform.localScale = new Vector3(radius, height, depth);
			return box.GetComponent<VolumetricFog>();
		}

		public static void RemoveAllFogAreas()
		{
			VolumetricFog[] fogs = FindObjectsOfType<VolumetricFog>();
			for (int k = 0; k < fogs.Length; k++)
			{
				if (fogs[k] != null && !fogs[k].hasCamera)
				{
					DestroyImmediate(fogs[k].gameObject);
				}
			}
		}

		private void CheckFogAreaDimensions()
		{
			if (!_hasCamera && mr == null)
			{
				mr = GetComponent<MeshRenderer>();
			}

			if (mr == null)
			{
				return;
			}

			Vector3 size = mr.bounds.extents;
			switch (_fogAreaTopology)
			{
				case FOG_AREA_TOPOLOGY.Box:
					fogAreaRadius = size.x;
					fogAreaHeight = size.y;
					fogAreaDepth = size.z;
					break;
				case FOG_AREA_TOPOLOGY.Sphere:
					fogAreaRadius = size.x;
					if (transform.localScale.z != transform.localScale.x)
					{
						transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y,
							transform.localScale.x);
					}

					break;
			}

			if (_fogAreaCenter != null)
			{
				if (_fogAreaFollowMode == FOG_AREA_FOLLOW_MODE.FullXYZ)
				{
					transform.position = _fogAreaCenter.transform.position;
				}
				else
				{
					transform.position = new Vector3(_fogAreaCenter.transform.position.x, transform.position.y,
						_fogAreaCenter.transform.position.z);
				}
			}

			fogAreaPosition = transform.position;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (_fogAreaRadius > 0 && _fogAreaShowGizmos)
			{
				if (_fogAreaTopology == FOG_AREA_TOPOLOGY.Box)
				{
					Gizmos.DrawWireCube(fogAreaPosition,
						new Vector3(fogAreaRadius * 2f, fogAreaHeight * 2f, fogAreaDepth * 2f));
				}
				else
				{
					Gizmos.DrawWireSphere(fogAreaPosition, fogAreaRadius);
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			// Show the boundary of this fog effect
			if (_visibilityScope == FOG_VISIBILITY_SCOPE.Volume)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireCube(visibilityVolume.center, visibilityVolume.size);
			}

			ShowFoWGizmo();
		}
#endif

		#endregion

		#region Screen Mask

		public void UpdateVolumeMask()
		{
			if (!_hasCamera || mainCamera == null)
			{
				return;
			}

			RemoveMaskCommandBuffer();

			if (_enableMask)
			{
				if (maskCommandBuffer != null)
				{
					maskCommandBuffer.Clear();
				}
				else
				{
					maskCommandBuffer = new CommandBuffer();
					maskCommandBuffer.name = "Volumetric Fog Mask Write";
				}

				if (maskMaterial == null)
				{
					maskMaterial = new Material(Shader.Find("VolumetricFogAndMist/MaskWrite"));
				}

#if UNITY_2017_2_OR_NEWER
				if (XRSettings.enabled)
				{
					rtMaskDesc = XRSettings.eyeTextureDesc;
				}
				else
				{
					rtMaskDesc = new RenderTextureDescriptor(mainCamera.pixelWidth, mainCamera.pixelHeight);
				}
#else
				rtMaskDesc = new RenderTextureDescriptor(mainCamera.pixelWidth, mainCamera.pixelHeight);
#endif
				rtMaskDesc.colorFormat = RenderTextureFormat.Depth;
				rtMaskDesc.depthBufferBits = 24;
				rtMaskDesc.sRGB = false;
				rtMaskDesc.msaaSamples = 1;
				rtMaskDesc.useMipMap = false;
				rtMaskDesc.volumeDepth = 1;
				int downsampling = Mathf.Max(1, _maskDownsampling);
				rtMaskDesc.width /= downsampling;
				rtMaskDesc.height /= downsampling;

				int maskTarget = Shader.PropertyToID("_VolumetricFogScreenMaskTexture");
				maskCommandBuffer.GetTemporaryRT(maskTarget, rtMaskDesc);
				maskCommandBuffer.SetRenderTarget(maskTarget);
				maskCommandBuffer.ClearRenderTarget(true, false, Color.white);

				Renderer[] rr = FindObjectsOfType<Renderer>();
				for (int k = 0; k < rr.Length; k++)
				{
					if (((1 << rr[k].gameObject.layer) & _maskLayer.value) != 0 && rr[k].gameObject.activeSelf)
					{
						if (rr[k].enabled && Application.isPlaying)
						{
							rr[k].enabled = false;
						}

						maskCommandBuffer.DrawRenderer(rr[k], maskMaterial);
					}
				}

				maskCommandBuffer.ReleaseTemporaryRT(maskTarget);
				mainCamera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, maskCommandBuffer);
			}
		}

		public void TogglePreviewMask()
		{
			Renderer[] rr = FindObjectsOfType<Renderer>();
			for (int k = 0; k < rr.Length; k++)
			{
				if (((1 << rr[k].gameObject.layer) & _maskLayer.value) != 0 && rr[k].gameObject.activeSelf)
				{
					rr[k].enabled = !rr[k].enabled;
				}
			}
		}

		private void RemoveMaskCommandBuffer()
		{
			if (maskCommandBuffer != null && mainCamera != null)
			{
				mainCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, maskCommandBuffer);
			}
		}

		#endregion
	}
}