using UnityEngine;

namespace WireBuilder
{
	[CreateAssetMenu(menuName = "Wire type")]
	[HelpURL("http://staggart.xyz/unity/wire-builder/wb-docs/?section=components")]
	public class WireType : ScriptableObject
	{
		public enum GeometryType
		{
			Line,


			Mesh
		}


		[Tooltip("Wires can be generated as Meshes or Lines. Lines are best suited for low fidelity wires or 2D games")]
		public GeometryType geometryType;


		[Range(0.1f, 2f)]
		[Tooltip(
			"Controls the distance between edge loops along the length of the wire. Wires always have a minimum of 5 points")]
		public float pointsPerMeter = 0.3f;


		[Range(3f, 12f)] [Tooltip("Controls the amount of sides on the wire mesh")]
		public int radialSegments = 5;


		[Range(0.1f, 10f)] [Tooltip("Heavy wires will sag lower")]
		public float weight = 5f;


		[Range(0.01f, 0.5f)] [Tooltip("A wire's thickness")]
		public float diameter = 0.1f;


		public Material material;


		public LineTextureMode textureMode = LineTextureMode.Tile;


		[Tooltip(
			"The distance the texture tiles over the length of a wires. A value of one means the texture is stretched over the wires.")]
		public float tiling = 3f;


		public LayerMask layer = 1;


		public string tag = "Untagged";


		[Tooltip("When enabled, a VegetationMaskLine is added to each wire, and removes any trees underneath them")]
		public bool enableTreeMask;


		[Range(1f, 40f)] public float treeMaskWidth = 20f;


		[Tooltip(
			"When enabled, a VegetationMaskLine is added to each wire, and removes any large objects underneath them")]
		public bool enableLargeObjectMask;


		[Range(1f, 40f)] public float largeObjectMaskWidth = 20f;
	}
}