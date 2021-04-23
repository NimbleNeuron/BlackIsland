using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ara
{
	[ExecuteInEditMode]
	public class AraTrail : MonoBehaviour
	{
		public enum TextureMode
		{
			Stretch,


			Tile
		}


		public enum Timescale
		{
			Normal,


			Unscaled
		}


		public enum TrailAlignment
		{
			View,


			Velocity,


			Local
		}


		public enum TrailSorting
		{
			OlderOnTop,


			NewerOnTop
		}


		public const float epsilon = 1E-05f;


		[Header("Overall")]
		[Tooltip(
			"Trail cross-section asset, determines the shape of the emitted trail. If no asset is specified, the trail will be a simple strip.")]
		public TrailSection section;


		[Tooltip("Whether to use world or local space to generate and simulate the trail.")]
		public Space space;


		[Tooltip("Whether to use regular time.")]
		public Timescale timescale;


		[Tooltip(
			"How to align the trail geometry: facing the camera (view) of using the transform's rotation (local).")]
		public TrailAlignment alignment;


		[Tooltip("Determines the order in which trail points will be rendered.")]
		public TrailSorting sorting;


		[Tooltip("Thickness multiplier, in meters.")]
		public float thickness = 0.1f;


		[Tooltip("Amount of smoothing iterations applied to the trail shape.")] [Range(1f, 8f)]
		public int smoothness = 1;


		[Tooltip("Calculate accurate thickness at sharp corners.")]
		public bool highQualityCorners;


		[Range(0f, 12f)] public int cornerRoundness = 5;


		[Header("Lenght")]
		[Tooltip(
			"How should the thickness of the curve evolve over its lenght. The horizontal axis is normalized lenght (in the [0,1] range) and the vertical axis is a thickness multiplier.")]
		public AnimationCurve thicknessOverLenght = AnimationCurve.Linear(0f, 1f, 0f, 1f);


		[Tooltip("How should vertex color evolve over the trail's length.")]
		public Gradient colorOverLenght = new Gradient();


		[Header("Time")]
		[Tooltip(
			"How should the thickness of the curve evolve with its lifetime. The horizontal axis is normalized lifetime (in the [0,1] range) and the vertical axis is a thickness multiplier.")]
		public AnimationCurve thicknessOverTime = AnimationCurve.Linear(0f, 1f, 0f, 1f);


		[Tooltip("How should vertex color evolve over the trail's lifetime.")]
		public Gradient colorOverTime = new Gradient();


		[Header("Emission")] public bool emit = true;


		[Tooltip("Initial thickness of trail points when they are first spawned.")]
		public float initialThickness = 1f;


		[Tooltip("Initial color of trail points when they are first spawned.")]
		public Color initialColor = Color.white;


		[Tooltip("Initial velocity of trail points when they are first spawned.")]
		public Vector3 initialVelocity = Vector3.zero;


		[Tooltip("Minimum amount of time (in seconds) that must pass before spawning a new point.")]
		public float timeInterval = 0.025f;


		[Tooltip("Minimum distance (in meters) that must be left between consecutive points in the trail.")]
		public float minDistance = 0.025f;


		[Tooltip("Duration of the trail (in seconds).")]
		public float time = 2f;


		[Header("Physics")] [Tooltip("Toggles trail physics.")]
		public bool enablePhysics;


		[Tooltip(
			"Amount of seconds pre-simulated before the trail appears. Useful when you want a trail to be already simulating when the game starts.")]
		public float warmup;


		[Tooltip("Gravity affecting the trail.")]
		public Vector3 gravity = Vector3.zero;


		[Tooltip(
			"Amount of speed transferred from the transform to the trail. 0 means no velocity is transferred, 1 means 100% of the velocity is transferred.")]
		[Range(0f, 1f)]
		public float inertia;


		[Tooltip("Amount of temporal smoothing applied to the velocity transferred from the transform to the trail.")]
		[Range(0f, 1f)]
		public float velocitySmoothing = 0.75f;


		[Tooltip(
			"Amount of damping applied to the trail's velocity. Larger values will slow down the trail more as time passes.")]
		[Range(0f, 1f)]
		public float damping = 0.75f;


		[Header("Rendering")] public Material[] materials = new Material[1];


		public ShadowCastingMode castShadows = ShadowCastingMode.On;


		public bool receiveShadows = true;


		public bool useLightProbes = true;


		[Header("Texture")]
		[Tooltip("How to apply the texture over the trail: stretch it all over its lenght, or tile it.")]
		public TextureMode textureMode;


		[Tooltip("Defines how many times are U coords repeated across the length of the trail.")]
		public float uvFactor = 1f;


		[Tooltip("Defines how many times are V coords repeated trough the width of the trail.")]
		public float uvWidthFactor = 1f;


		[Tooltip(
			"When the texture mode is set to 'Tile', defines where to begin tiling from: 0 means the start of the trail, 1 means the end.")]
		[Range(0f, 1f)]
		public float tileAnchor = 1f;


		private readonly List<int> discontinuities = new List<int>();


		private readonly List<Vector3> normals = new List<Vector3>();


		private readonly List<Point> renderablePoints = new List<Point>();


		private readonly List<Vector4> tangents = new List<Vector4>();


		private readonly List<int> tris = new List<int>();


		private readonly List<Vector2> uvs = new List<Vector2>();


		private readonly List<Color> vertColors = new List<Color>();


		private readonly List<Vector3> vertices = new List<Vector3>();


		private float accumTime;


		private Mesh mesh_;


		[HideInInspector] public List<Point> points = new List<Point>();


		private Vector3 prevPosition;


		private Vector3 velocity = Vector3.zero;


		public Vector3 Velocity => velocity;


		private float DeltaTime {
			get
			{
				if (timescale != Timescale.Unscaled)
				{
					return Time.deltaTime;
				}

				return Time.unscaledDeltaTime;
			}
		}


		private float FixedDeltaTime {
			get
			{
				if (timescale != Timescale.Unscaled)
				{
					return Time.fixedDeltaTime;
				}

				return Time.fixedUnscaledDeltaTime;
			}
		}


		public Mesh mesh => mesh_;


		public void Awake()
		{
			Warmup();
		}


		private void FixedUpdate()
		{
			if (!enablePhysics)
			{
				return;
			}

			PhysicsStep(FixedDeltaTime);
		}


		private void LateUpdate()
		{
			UpdateVelocity();
			EmissionStep(DeltaTime);
			SnapLastPointToTransform();
			UpdatePointsLifecycle();
			if (onUpdatePoints != null)
			{
				onUpdatePoints();
			}
		}


		private void OnEnable()
		{
			prevPosition = transform.position;
			velocity = Vector3.zero;
			mesh_ = new Mesh();
			mesh_.name = "ara_trail_mesh";
			mesh_.MarkDynamic();
			Camera.onPreCull =
				(Camera.CameraCallback) Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(UpdateTrailMesh));
		}


		private void OnDisable()
		{
			DestroyImmediate(mesh_);
			Camera.onPreCull =
				(Camera.CameraCallback) Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(UpdateTrailMesh));
		}


		public void OnValidate()
		{
			time = Mathf.Max(time, 1E-05f);
			warmup = Mathf.Max(0f, warmup);
		}


		
		
		public event Action onUpdatePoints;


		public void Clear()
		{
			points.Clear();
		}


		private void UpdateVelocity()
		{
			if (DeltaTime > 0f)
			{
				velocity = Vector3.Lerp((transform.position - prevPosition) / DeltaTime, velocity, velocitySmoothing);
			}

			prevPosition = transform.position;
		}


		private void EmissionStep(float time)
		{
			accumTime += time;
			if (accumTime >= timeInterval && emit)
			{
				Vector3 vector = space == Space.Self ? transform.localPosition : transform.position;
				if (points.Count <= 1 || Vector3.Distance(vector, points[points.Count - 2].position) >= minDistance)
				{
					EmitPoint(vector);
					accumTime = 0f;
				}
			}
		}


		private void Warmup()
		{
			if (!Application.isPlaying || !enablePhysics)
			{
				return;
			}

			for (float num = warmup; num > FixedDeltaTime; num -= FixedDeltaTime)
			{
				PhysicsStep(FixedDeltaTime);
				EmissionStep(FixedDeltaTime);
				SnapLastPointToTransform();
				UpdatePointsLifecycle();
				if (onUpdatePoints != null)
				{
					onUpdatePoints();
				}
			}
		}


		private void PhysicsStep(float timestep)
		{
			float d = Mathf.Pow(1f - Mathf.Clamp01(damping), timestep);
			for (int i = 0; i < points.Count; i++)
			{
				Point point = points[i];
				point.velocity += gravity * timestep;
				point.velocity *= d;
				point.position += point.velocity * timestep;
				points[i] = point;
			}
		}


		public void EmitPoint(Vector3 position)
		{
			points.Add(new Point(position, initialVelocity + velocity * inertia, transform.right, transform.forward,
				initialColor, initialThickness, time));
		}


		private void SnapLastPointToTransform()
		{
			if (points.Count > 0)
			{
				Point point = points[points.Count - 1];
				if (!emit)
				{
					point.discontinuous = true;
				}

				if (!point.discontinuous)
				{
					point.position = space == Space.Self ? transform.localPosition : transform.position;
					point.normal = transform.forward;
					point.tangent = transform.right;
				}

				points[points.Count - 1] = point;
			}
		}


		private void UpdatePointsLifecycle()
		{
			for (int i = points.Count - 1; i >= 0; i--)
			{
				Point point = points[i];
				point.life -= DeltaTime;
				points[i] = point;
				if (point.life <= 0f)
				{
					if (smoothness <= 1)
					{
						points.RemoveAt(i);
					}
					else if (points[Mathf.Min(i + 1, points.Count - 1)].life <= 0f &&
					         points[Mathf.Min(i + 2, points.Count - 1)].life <= 0f)
					{
						points.RemoveAt(i);
					}
				}
			}
		}


		private void ClearMeshData()
		{
			mesh_.Clear();
			vertices.Clear();
			normals.Clear();
			tangents.Clear();
			uvs.Clear();
			vertColors.Clear();
			tris.Clear();
		}


		private void CommitMeshData()
		{
			mesh_.SetVertices(vertices);
			mesh_.SetNormals(normals);
			mesh_.SetTangents(tangents);
			mesh_.SetColors(vertColors);
			mesh_.SetUVs(0, uvs);
			mesh_.SetTriangles(tris, 0, true);
		}


		private void RenderMesh(Camera cam)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				Graphics.DrawMesh(mesh_,
					space == Space.Self && transform.parent != null
						? transform.parent.localToWorldMatrix
						: Matrix4x4.identity, materials[i], gameObject.layer, cam, 0, null, castShadows, receiveShadows,
					null, useLightProbes);
			}
		}


		public float GetLenght(List<Point> input)
		{
			float num = 0f;
			for (int i = 0; i < input.Count - 1; i++)
			{
				num += Vector3.Distance(input[i].position, input[i + 1].position);
			}

			return num;
		}


		private List<Point> GetRenderablePoints(int start, int end)
		{
			renderablePoints.Clear();
			if (smoothness <= 1)
			{
				for (int i = start; i <= end; i++)
				{
					renderablePoints.Add(points[i]);
				}

				return renderablePoints;
			}

			float num = 1f / smoothness;
			for (int j = start; j < end; j++)
			{
				Point a = j == start ? points[start] + (points[start] - points[j + 1]) : points[j - 1];
				Point d = j == end - 1 ? points[end] + (points[end] - points[end - 1]) : points[j + 2];
				for (int k = 0; k < smoothness; k++)
				{
					float t = k * num;
					Point point = Point.Interpolate(a, points[j], points[j + 1], d, t);
					if (point.life > 0f)
					{
						renderablePoints.Add(point);
					}
				}
			}

			if (points[end].life > 0f)
			{
				renderablePoints.Add(points[end]);
			}

			return renderablePoints;
		}


		private CurveFrame InitializeCurveFrame(Vector3 point, Vector3 nextPoint)
		{
			Vector3 vector = nextPoint - point;
			if (Mathf.Approximately(Mathf.Abs(Vector3.Dot(vector.normalized, transform.forward)), 1f))
			{
				vector += transform.right * 0.01f;
			}

			return new CurveFrame(point, transform.forward, transform.up, vector);
		}


		private void UpdateTrailMesh(Camera cam)
		{
			if ((cam.cullingMask & (1 << gameObject.layer)) == 0)
			{
				return;
			}

			ClearMeshData();
			if (points.Count > 1)
			{
				Vector3 localCamPosition = space == Space.Self && transform.parent != null
					? transform.parent.InverseTransformPoint(cam.transform.position)
					: cam.transform.position;
				discontinuities.Clear();
				for (int i = 0; i < points.Count; i++)
				{
					if (points[i].discontinuous || i == points.Count - 1)
					{
						discontinuities.Add(i);
					}
				}

				int start = 0;
				for (int j = 0; j < discontinuities.Count; j++)
				{
					UpdateSegmentMesh(start, discontinuities[j], localCamPosition);
					start = discontinuities[j] + 1;
				}

				CommitMeshData();
				RenderMesh(cam);
			}
		}


		private void UpdateSegmentMesh(int start, int end, Vector3 localCamPosition)
		{
			List<Point> list = GetRenderablePoints(start, end);
			if (sorting == TrailSorting.NewerOnTop)
			{
				list.Reverse();
			}

			if (list.Count > 1)
			{
				float num = Mathf.Max(GetLenght(list), 1E-05f);
				float num2 = 0f;
				float num3 = textureMode == TextureMode.Stretch ? 0f : -uvFactor * num * tileAnchor;
				if (sorting == TrailSorting.NewerOnTop)
				{
					num3 = 1f - num3;
				}

				Vector4 item = Vector4.zero;
				Vector2 zero = Vector2.zero;
				bool flag = highQualityCorners && alignment != TrailAlignment.Local;
				CurveFrame curveFrame =
					InitializeCurveFrame(list[list.Count - 1].position, list[list.Count - 2].position);
				int item2 = 1;
				int item3 = 0;
				for (int i = list.Count - 1; i >= 0; i--)
				{
					int num4 = Mathf.Max(i - 1, 0);
					int index = Mathf.Min(i + 1, list.Count - 1);
					Point point = list[i];
					Vector3 vector = list[num4].position - point.position;
					Vector3 vector2 = point.position - list[index].position;
					float num5 = num4 == i ? vector2.magnitude : vector.magnitude;
					vector.Normalize();
					vector2.Normalize();
					Vector3 vector3 = alignment == TrailAlignment.Local
						? point.tangent.normalized
						: (vector + vector2) * 0.5f;
					Vector3 vector4 = point.normal;
					if (alignment != TrailAlignment.Local)
					{
						vector4 = alignment == TrailAlignment.View
							? localCamPosition - point.position
							: curveFrame.Transport(vector3, point.position);
					}

					vector4.Normalize();
					Vector3 vector5 = alignment == TrailAlignment.Velocity
						? curveFrame.bitangent
						: Vector3.Cross(vector3, vector4);
					vector5.Normalize();
					float num6 = sorting == TrailSorting.OlderOnTop ? num2 / num : (num - num2) / num;
					float num7 = Mathf.Clamp01(1f - point.life / time);
					num2 += num5;
					Color item4 = point.color * colorOverTime.Evaluate(num7) * colorOverLenght.Evaluate(num6);
					float num8 = thickness * point.thickness * thicknessOverTime.Evaluate(num7) *
					             thicknessOverLenght.Evaluate(num6);
					if (section != null)
					{
						int segments = section.Segments;
						int num9 = segments + 1;
						for (int j = 0; j <= segments; j++)
						{
							vertices.Add(point.position +
							             (section.vertices[j].x * vector5 + section.vertices[j].y * vector3) * num8);
							normals.Add(vertices[vertices.Count - 1] - point.position);
							item = -Vector3.Cross(normals[normals.Count - 1], curveFrame.tangent);
							item.w = 1f;
							tangents.Add(item);
							vertColors.Add(item4);
							zero.Set(j / (float) segments * uvWidthFactor, num3);
							uvs.Add(zero);
							if (j < segments && i < list.Count - 1)
							{
								tris.Add(i * num9 + j);
								tris.Add(i * num9 + j + 1);
								tris.Add((i + 1) * num9 + j);
								tris.Add(i * num9 + j + 1);
								tris.Add((i + 1) * num9 + j + 1);
								tris.Add((i + 1) * num9 + j);
							}
						}
					}
					else
					{
						Quaternion rotation = Quaternion.identity;
						Vector3 vector6 = Vector3.zero;
						float num10 = 0f;
						float d = num8;
						Vector3 vector7 = vector5;
						if (flag)
						{
							Vector3 vector8 = i == 0
								? vector5
								: Vector3.Cross(vector, Vector3.Cross(vector5, vector3)).normalized;
							if (cornerRoundness > 0)
							{
								vector7 = i == list.Count - 1
									? -vector5
									: Vector3.Cross(vector2, Vector3.Cross(vector5, vector3)).normalized;
								num10 = i == 0 || i == list.Count - 1 ? 1f : Mathf.Sign(Vector3.Dot(vector, -vector7));
								float num11 = i == 0 || i == list.Count - 1
									? 3.1415927f
									: Mathf.Acos(Mathf.Clamp(Vector3.Dot(vector8, vector7), -1f, 1f));
								rotation = Quaternion.AngleAxis(57.29578f * num11 / cornerRoundness, vector4 * num10);
								vector6 = vector7 * num8 * num10;
							}

							if (vector8.sqrMagnitude > 0.1f)
							{
								d = num8 / Mathf.Max(Vector3.Dot(vector5, vector8), 0.15f);
							}
						}

						if (flag && cornerRoundness > 0)
						{
							if (num10 > 0f)
							{
								vertices.Add(point.position + vector7 * num8);
								vertices.Add(point.position - vector5 * d);
							}
							else
							{
								vertices.Add(point.position + vector5 * d);
								vertices.Add(point.position - vector7 * num8);
							}
						}
						else
						{
							vertices.Add(point.position + vector5 * d);
							vertices.Add(point.position - vector5 * d);
						}

						normals.Add(-vector4);
						normals.Add(-vector4);
						item = -vector5;
						item.w = 1f;
						tangents.Add(item);
						tangents.Add(item);
						vertColors.Add(item4);
						vertColors.Add(item4);
						zero.Set(num3, sorting == TrailSorting.NewerOnTop ? uvWidthFactor : 0f);
						uvs.Add(zero);
						zero.Set(num3, sorting == TrailSorting.NewerOnTop ? 0f : uvWidthFactor);
						uvs.Add(zero);
						if (i < list.Count - 1)
						{
							int num12 = vertices.Count - 1;
							tris.Add(num12);
							tris.Add(item2);
							tris.Add(item3);
							tris.Add(item3);
							tris.Add(num12 - 1);
							tris.Add(num12);
						}

						item2 = vertices.Count - 1;
						item3 = vertices.Count - 2;
						if (flag && cornerRoundness > 0)
						{
							for (int k = 0; k <= cornerRoundness; k++)
							{
								vertices.Add(point.position + vector6);
								normals.Add(-vector4);
								tangents.Add(item);
								vertColors.Add(item4);
								zero.Set(num3, num10 > 0f ? 0 : 1);
								uvs.Add(zero);
								int num13 = vertices.Count - 1;
								tris.Add(num13);
								tris.Add(item2);
								tris.Add(item3);
								if (num10 > 0f)
								{
									item3 = num13;
								}
								else
								{
									item2 = num13;
								}

								vector6 = rotation * vector6;
							}
						}
					}

					float num14 = textureMode == TextureMode.Stretch ? num5 / num : num5;
					num3 += uvFactor * (sorting == TrailSorting.NewerOnTop ? -num14 : num14);
				}
			}
		}


		public struct CurveFrame
		{
			public CurveFrame(Vector3 position, Vector3 normal, Vector3 bitangent, Vector3 tangent)
			{
				this.position = position;
				this.normal = normal;
				this.bitangent = bitangent;
				this.tangent = tangent;
			}


			public Vector3 Transport(Vector3 newTangent, Vector3 newPosition)
			{
				Vector3 vector = newPosition - position;
				float num = Vector3.Dot(vector, vector);
				Vector3 vector2 = normal - 2f / (num + 1E-05f) * Vector3.Dot(vector, normal) * vector;
				Vector3 b = tangent - 2f / (num + 1E-05f) * Vector3.Dot(vector, tangent) * vector;
				Vector3 vector3 = newTangent - b;
				float num2 = Vector3.Dot(vector3, vector3);
				Vector3 rhs = vector2 - 2f / (num2 + 1E-05f) * Vector3.Dot(vector3, vector2) * vector3;
				Vector3 vector4 = Vector3.Cross(newTangent, rhs);
				normal = rhs;
				bitangent = vector4;
				tangent = newTangent;
				position = newPosition;
				return normal;
			}


			public Vector3 position;


			public Vector3 normal;


			public Vector3 bitangent;


			public Vector3 tangent;
		}


		public struct Point
		{
			public Point(Vector3 position, Vector3 velocity, Vector3 tangent, Vector3 normal, Color color,
				float thickness, float lifetime)
			{
				this.position = position;
				this.velocity = velocity;
				this.tangent = tangent;
				this.normal = normal;
				this.color = color;
				this.thickness = thickness;
				life = lifetime;
				discontinuous = false;
			}


			private static float CatmullRom(float p0, float p1, float p2, float p3, float t)
			{
				float num = t * t;
				return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num +
				               (-p0 + 3f * p1 - 3f * p2 + p3) * num * t);
			}


			private static Color CatmullRomColor(Color p0, Color p1, Color p2, Color p3, float t)
			{
				return new Color(CatmullRom(p0[0], p1[0], p2[0], p3[0], t), CatmullRom(p0[1], p1[1], p2[1], p3[1], t),
					CatmullRom(p0[2], p1[2], p2[2], p3[2], t), CatmullRom(p0[3], p1[3], p2[3], p3[3], t));
			}


			private static Vector3 CatmullRom3D(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
			{
				return new Vector3(CatmullRom(p0[0], p1[0], p2[0], p3[0], t), CatmullRom(p0[1], p1[1], p2[1], p3[1], t),
					CatmullRom(p0[2], p1[2], p2[2], p3[2], t));
			}


			public static Point Interpolate(Point a, Point b, Point c, Point d, float t)
			{
				return new Point(CatmullRom3D(a.position, b.position, c.position, d.position, t),
					CatmullRom3D(a.velocity, b.velocity, c.velocity, d.velocity, t),
					CatmullRom3D(a.tangent, b.tangent, c.tangent, d.tangent, t),
					CatmullRom3D(a.normal, b.normal, c.normal, d.normal, t),
					CatmullRomColor(a.color, b.color, c.color, d.color, t),
					CatmullRom(a.thickness, b.thickness, c.thickness, d.thickness, t),
					CatmullRom(a.life, b.life, c.life, d.life, t));
			}


			public static Point operator +(Point p1, Point p2)
			{
				return new Point(p1.position + p2.position, p1.velocity + p2.velocity, p1.tangent + p2.tangent,
					p1.normal + p2.normal, p1.color + p2.color, p1.thickness + p2.thickness, p1.life + p2.life);
			}


			public static Point operator -(Point p1, Point p2)
			{
				return new Point(p1.position - p2.position, p1.velocity - p2.velocity, p1.tangent - p2.tangent,
					p1.normal - p2.normal, p1.color - p2.color, p1.thickness - p2.thickness, p1.life - p2.life);
			}


			public Vector3 position;


			public Vector3 velocity;


			public Vector3 tangent;


			public Vector3 normal;


			public Color color;


			public float thickness;


			public float life;


			public bool discontinuous;
		}
	}
}