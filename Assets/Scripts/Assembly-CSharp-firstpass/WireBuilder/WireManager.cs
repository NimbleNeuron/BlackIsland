using System.Collections.Generic;
using UnityEngine;

namespace WireBuilder
{
	[ExecuteInEditMode]
	public static class WireManager
	{
		[Tooltip(
			"Toggles additional visual gizmos when network objects are selected.\nSuch as the wire points, or connections between connectors.")]
		public static bool debug = false;


		[Tooltip("The connector groups current contained in the network")]
		public static List<WireConnectorGroup> Groups = new List<WireConnectorGroup>();


		[Tooltip("The connectors currently contained in the network")]
		public static List<WireConnector> Connectors = new List<WireConnector>();


		[Tooltip("The wires currently contained in the network")]
		public static List<Wire> Wires = new List<Wire>();


		public static WireConnectorGroup CreateGroupObject(GameObject sourceObject)
		{
			if (sourceObject == null)
			{
				Debug.Log("Failed to create connector group object, source object is null");
			}

			if (sourceObject.GetComponent<WireConnectorGroup>() == null)
			{
				Debug.LogError("Source object has no WireConnectorGroup component", sourceObject);
				return null;
			}

			GameObject gameObject = Object.Instantiate<GameObject>(sourceObject);
			gameObject.name = sourceObject.name;
			gameObject.transform.parent = sourceObject.transform.parent;
			return gameObject.GetComponent<WireConnectorGroup>();
		}


		public static void AddConnectorGroup(WireConnectorGroup group)
		{
			if (!Groups.Contains(group))
			{
				Groups.Add(group);
			}
		}


		public static void RemoveConnectorGroup(WireConnectorGroup group)
		{
			if (Groups.Contains(group))
			{
				Groups.Remove(group);
			}
		}


		public static void UpdateConnectorGroup(WireConnectorGroup group)
		{
			if (!group)
			{
				return;
			}

			foreach (WireConnector wireConnector in group.connectors)
			{
				if (!wireConnector)
				{
					break;
				}

				wireConnector.group = group;
				if (wireConnector.wires != null)
				{
					foreach (Wire wire in wireConnector.wires)
					{
						UpdateWire(wire, wireConnector.wireType);
					}
				}
			}
		}


		public static void DestroyConnectorGroup(WireConnectorGroup group)
		{
			if (!group)
			{
				return;
			}

			foreach (WireConnector wireConnector in group.connectors)
			{
				for (int i = 0; i < wireConnector.wires.Count; i++)
				{
					if (wireConnector.wires[i])
					{
						DestroyWire(wireConnector.wires[i]);
					}
				}
			}

			Groups.Remove(group);
			Object.DestroyImmediate(group.gameObject);
		}


		public static WireConnector CreateConnectorObject()
		{
			return new GameObject
			{
				name = "WireConnector"
			}.AddComponent<WireConnector>();
		}


		public static void AddConnector(WireConnector connector)
		{
			if (!Connectors.Contains(connector))
			{
				Connectors.Add(connector);
			}
		}


		public static void RemoveConnector(WireConnector connector)
		{
			if (Connectors.Contains(connector))
			{
				Connectors.Remove(connector);
			}
		}


		public static void UpdateConnector(WireConnector connector)
		{
			if (connector.wires != null)
			{
				foreach (Wire wire in connector.wires)
				{
					UpdateWire(wire, connector.wireType);
				}
			}
		}


		public static Wire CreateWireObject(WireConnector connector)
		{
			if (!connector.wireType)
			{
				Debug.LogError("Trying to create wire from connect without a wire type");
				return null;
			}

			Wire wire = WireGenerator.New(connector.wireType);
			if (connector.group != null)
			{
				wire.transform.parent = connector.group.transform.parent;
			}
			else
			{
				wire.transform.parent = connector.transform;
			}

			return wire;
		}


		public static Wire CreateWireObject(WireType type)
		{
			return WireGenerator.New(type);
		}


		public static Wire CreateWireObject(WireConnector start, WireConnector end, WireType type)
		{
			Wire wire = WireGenerator.New(type);
			wire.startConnection = start;
			wire.endConnection = end;
			wire.UpdateWire(true);
			return wire;
		}


		public static void AddWire(Wire wire)
		{
			if (!Wires.Contains(wire))
			{
				Wires.Add(wire);
			}
		}


		public static void RemoveWire(Wire wire)
		{
			if (Wires.Contains(wire))
			{
				Wires.Remove(wire);
			}
		}


		public static void UpdateWire(Wire wire, WireType type, bool updateWind = false)
		{
			if (!wire)
			{
				return;
			}

			WireGenerator.Update(wire, type, updateWind);
		}


		public static void DestroyWire(Wire wire)
		{
			if (wire.startConnection != null)
			{
				wire.startConnection.wires.Remove(wire);
			}

			if (wire.endConnection != null)
			{
				wire.endConnection.wires.Remove(wire);
			}

			Object.DestroyImmediate(wire.gameObject);
		}


		public static void UpdateAllWires(bool updateWind = false)
		{
			if (Connectors == null)
			{
				return;
			}

			foreach (WireConnector wireConnector in Connectors)
			{
				if (!wireConnector)
				{
					break;
				}

				foreach (Wire wire in wireConnector.wires)
				{
					wire.wireType = wireConnector.wireType;
					UpdateWire(wire, wireConnector.wireType, updateWind);
				}
			}
		}


		public static void UpdateWireWind()
		{
			UpdateAllWires(true);
		}


		public static void ValidateAll()
		{
			if (Groups == null)
			{
				return;
			}

			Groups.RemoveAll(WireConnectorgroup => WireConnectorgroup == null);
			Connectors.RemoveAll(WireConnector => WireConnector == null);
			Wires.RemoveAll(Wire => Wire == null);
			foreach (WireConnectorGroup wireConnectorGroup in Groups)
			{
				ValidateGroup(wireConnectorGroup);
				if (wireConnectorGroup.connectors != null)
				{
					foreach (WireConnector wireConnector in wireConnectorGroup.connectors)
					{
						ValidateConnector(wireConnector);
						if (wireConnector.wires != null)
						{
							foreach (Wire wire in wireConnector.wires)
							{
								ValidateWire(wire);
							}
						}
					}
				}
			}

			ValidateAllWires();
		}


		public static void ValidateAllGroups()
		{
			Groups.RemoveAll(WireConnectorgroup => WireConnectorgroup == null);
			foreach (WireConnectorGroup wireConnectorGroup in Groups)
			{
				foreach (WireConnector wireConnector in wireConnectorGroup.connectors)
				{
					if (wireConnector == null)
					{
						return;
					}

					foreach (Wire wire in wireConnector.wires)
					{
						if (wire == null)
						{
							wireConnector.wires.Remove(wire);
						}
					}
				}
			}
		}


		private static void ValidateGroup(WireConnectorGroup group)
		{
			group.connectors.RemoveAll(WireConnector => WireConnector == null);
			foreach (WireConnector connector in group.connectors)
			{
				ValidateConnector(connector);
			}
		}


		private static void ValidateAllWires()
		{
			Wires.RemoveAll(Wire => Wire == null);
			foreach (Wire wire in Wires)
			{
				ValidateWire(wire);
			}
		}


		private static void ValidateWire(Wire wire)
		{
			if (!Wires.Contains(wire))
			{
				Wires.Add(wire);
			}

			if (wire.startConnection && !wire.startConnection.wires.Contains(wire))
			{
				wire.startConnection.wires.Add(wire);
			}

			if (wire.endConnection && !wire.endConnection.wires.Contains(wire))
			{
				wire.endConnection.wires.Add(wire);
			}
		}


		private static void ValidateConnector(WireConnector connector)
		{
			if (!Connectors.Contains(connector))
			{
				Connectors.Add(connector);
			}

			if (connector.wires == null)
			{
				return;
			}

			connector.wires.RemoveAll(Wire => Wire == null);
			foreach (Wire wire in connector.wires)
			{
				ValidateWire(wire);
			}
		}
	}
}