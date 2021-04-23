using System.Collections.Generic;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class BlisLookAtConstraint : MonoBehaviour
	{
		public enum axisType
		{
			X,

			Y,

			Z
		}


		public enum lookType
		{
			Camera,

			Nodes
		}


		public enum upCtrType
		{
			LootAt,

			AxisAlignment
		}


		public enum upType
		{
			Camera,

			Node,

			World
		}


		public lookType lookAtType;


		public List<Transform> lookAtNodeList = new List<Transform>();


		public axisType lookAtAxis = axisType.Z;


		public bool lookAtFilp;


		public upType upAxisType = upType.World;


		public Transform upNode;


		public upCtrType upControl = upCtrType.AxisAlignment;


		public axisType sourceAxis = axisType.Y;


		public bool sourceFilp;


		public axisType alignedToUpnodeAxis = axisType.Y;


		private void Update()
		{
			Vector3 upVector = GetUpVector();
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			Vector3 vector3 = Vector3.zero;
			lookType lookType = lookAtType;
			if (lookType != lookType.Camera)
			{
				if (lookType == lookType.Nodes)
				{
					Vector3 a = Vector3.zero;
					int num = 0;
					foreach (Transform transform in lookAtNodeList)
					{
						if (transform != null)
						{
							num++;
							a += transform.position;
						}
					}

					a /= (float) num;
					vector = Vector3.Normalize(a - this.transform.position);
					vector2 = Vector3.Normalize(Vector3.Cross(upVector, vector));
					vector3 = Vector3.Cross(vector, vector2);
				}
			}
			else
			{
				vector = Vector3.Normalize(Camera.main.transform.position - transform.position);
				vector2 = Vector3.Normalize(Vector3.Cross(upVector, vector));
				vector3 = Vector3.Cross(vector, vector2);
			}

			Vector3 vector4 = Vector3.zero;
			Vector3 vector5 = Vector3.zero;
			Vector3 vector6 = Vector3.zero;
			if (lookAtFilp)
			{
				vector = -vector;
				vector2 = -vector2;
			}

			switch (lookAtAxis)
			{
				case axisType.X:
					vector4 = vector;
					if (sourceAxis == axisType.Y)
					{
						vector5 = vector3;
						vector6 = -vector2;
					}
					else if (sourceAxis == axisType.Z)
					{
						vector5 = vector2;
						vector6 = vector3;
					}

					if (sourceFilp)
					{
						vector5 = -vector5;
						vector6 = -vector6;
					}

					break;
				case axisType.Y:
					vector5 = vector;
					if (sourceAxis == axisType.X)
					{
						vector4 = vector3;
						vector6 = vector2;
					}
					else if (sourceAxis == axisType.Z)
					{
						vector4 = -vector2;
						vector6 = vector3;
					}

					if (sourceFilp)
					{
						vector4 = -vector4;
						vector6 = -vector6;
					}

					break;
				case axisType.Z:
					vector6 = vector;
					if (sourceAxis == axisType.X)
					{
						vector4 = vector3;
						vector5 = -vector2;
					}
					else if (sourceAxis == axisType.Y)
					{
						vector4 = vector2;
						vector5 = vector3;
					}

					if (sourceFilp)
					{
						vector4 = -vector4;
						vector5 = -vector5;
					}

					break;
			}

			LookAtQuat(vector4, vector5, vector6);
		}

		private Vector3 GetUpVector()
		{
			Vector3 result = Vector3.zero;
			switch (upAxisType)
			{
				case upType.Camera:
					if (MonoBehaviourInstance<MobaCamera>.inst != null)
					{
						result = MonoBehaviourInstance<MobaCamera>.inst.GetTargetCameraPosition - transform.position;
					}

					break;
				case upType.Node:
					if (upControl == upCtrType.LootAt)
					{
						result = upNode.transform.position - transform.position;
					}
					else if (!(upNode == null))
					{
						switch (alignedToUpnodeAxis)
						{
							case axisType.X:
								result = upNode.right;
								break;
							case axisType.Y:
								result = upNode.up;
								break;
							case axisType.Z:
								result = upNode.forward;
								break;
						}
					}

					break;
				case upType.World:
					switch (alignedToUpnodeAxis)
					{
						case axisType.X:
							result = Vector3.right;
							break;
						case axisType.Y:
							result = Vector3.up;
							break;
						case axisType.Z:
							result = Vector3.forward;
							break;
					}

					break;
			}

			return result;
		}


		private void LookAtQuat(Vector3 xvec, Vector3 yvec, Vector3 zvec)
		{
			float num = 1f + xvec.x + yvec.y + zvec.z;
			if (num == 0f)
			{
				return;
			}

			float num2 = Mathf.Sqrt(num) / 2f;
			float num3 = 4f * num2;
			float x = (yvec.z - zvec.y) / num3;
			float y = (zvec.x - xvec.z) / num3;
			float z = (xvec.y - yvec.x) / num3;
			Quaternion rotation = new Quaternion(x, y, z, num2);
			transform.rotation = rotation;
		}
	}
}