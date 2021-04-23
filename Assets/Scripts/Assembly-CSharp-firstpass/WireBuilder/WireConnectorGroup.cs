using System.Collections.Generic;
using UnityEngine;

namespace WireBuilder
{
	[ExecuteInEditMode]
	[AddComponentMenu("Wire Network/Connector group")]
	[HelpURL("http://staggart.xyz/unity/wire-builder/wb-docs/?section=components")]
	public class WireConnectorGroup : MonoBehaviour
	{
		public float handleSize = 1f;


		[SerializeField] private List<WireConnector> _connectors = new List<WireConnector>();


		
		public List<WireConnector> connectors {
			get => _connectors;
			set => _connectors = value;
		}


		private void OnEnable()
		{
			WireManager.AddConnectorGroup(this);
		}


		private void OnDisable()
		{
			WireManager.RemoveConnectorGroup(this);
		}


		[ContextMenu("Refresh connectors")]
		private void UpdateConnectors()
		{
			foreach (WireConnector wireConnector in connectors)
			{
				wireConnector.group = this;
			}
		}


		private float TryGetMinBoundsSize()
		{
			MeshFilter component = GetComponent<MeshFilter>();
			if (!component)
			{
				return handleSize;
			}

			Bounds bounds = component.sharedMesh.bounds;
			float num = 999f;
			if (bounds.size.x < num)
			{
				num = bounds.size.x;
			}

			if (bounds.size.y < num)
			{
				num = bounds.size.y;
			}

			if (bounds.size.z < num)
			{
				num = bounds.size.z;
			}

			return num * 3f;
		}
	}
}