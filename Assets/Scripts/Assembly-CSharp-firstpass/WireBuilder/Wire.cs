using UnityEngine;

namespace WireBuilder
{
	[ExecuteInEditMode]
	public class Wire : MonoBehaviour
	{
		public WireType wireType;


		public WireConnector startConnection;


		public WireConnector endConnection;


		public Vector3[] points;


		public Vector3 startPos;


		public Vector3 endPos;


		[Range(0f, 10f)] public float sagOffset;


		public float length;


		public float sagDepth;


		public float tension;


		public float weight;


		public Gradient windData;


		public LineRenderer lineRenderer;


		public MeshRenderer meshRenderer;


		public MeshFilter meshFilter;


		public Mesh mesh;


		private void OnEnable()
		{
			WireManager.AddWire(this);
		}


		private void OnDisable()
		{
			WireManager.RemoveWire(this);
		}


		private void OnValidate()
		{
			transform.hideFlags = HideFlags.NotEditable;
		}


		public void UpdateWire(bool updateWind)
		{
			WireGenerator.Update(this, wireType, updateWind);
		}


		public void UpdateVegetationMask(WireType wireType) { }
	}
}