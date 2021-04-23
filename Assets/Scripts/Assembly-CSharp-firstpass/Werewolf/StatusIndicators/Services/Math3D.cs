using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Werewolf.StatusIndicators.Services
{
	public class Math3D
	{
		private static Transform tempChild;


		private static Transform tempParent;


		private static Vector3[] positionRegister;


		private static float[] posTimeRegister;


		private static int positionSamplesTaken;


		private static Quaternion[] rotationRegister;


		private static float[] rotTimeRegister;


		private static int rotationSamplesTaken;


		public static void Init()
		{
			tempChild = new GameObject("Math3d_TempChild").transform;
			tempParent = new GameObject("Math3d_TempParent").transform;
			tempChild.gameObject.hideFlags = HideFlags.HideAndDontSave;
			Object.DontDestroyOnLoad(tempChild.gameObject);
			tempParent.gameObject.hideFlags = HideFlags.HideAndDontSave;
			Object.DontDestroyOnLoad(tempParent.gameObject);
			tempChild.parent = tempParent;
		}


		public static Vector3 AddVectorLength(Vector3 vector, float size)
		{
			float num = Vector3.Magnitude(vector);
			float d = (num + size) / num;
			return vector * d;
		}


		public static Vector3 SetVectorLength(Vector3 vector, float size)
		{
			return Vector3.Normalize(vector) * size;
		}


		public static Quaternion SubtractRotation(Quaternion B, Quaternion A)
		{
			return Quaternion.Inverse(A) * B;
		}


		public static Quaternion AddRotation(Quaternion A, Quaternion B)
		{
			return A * B;
		}


		public static Vector3 TransformDirectionMath(Quaternion rotation, Vector3 vector)
		{
			return rotation * vector;
		}


		public static Vector3 InverseTransformDirectionMath(Quaternion rotation, Vector3 vector)
		{
			return Quaternion.Inverse(rotation) * vector;
		}


		public static Vector3 RotateVectorFromTo(Quaternion from, Quaternion to, Vector3 vector)
		{
			Quaternion rotation = SubtractRotation(to, from);
			Vector3 point = InverseTransformDirectionMath(from, vector);
			Vector3 vector2 = rotation * point;
			return TransformDirectionMath(from, vector2);
		}


		public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal,
			Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
		{
			linePoint = Vector3.zero;
			lineVec = Vector3.zero;
			lineVec = Vector3.Cross(plane1Normal, plane2Normal);
			Vector3 vector = Vector3.Cross(plane2Normal, lineVec);
			float num = Vector3.Dot(plane1Normal, vector);
			if (Mathf.Abs(num) > 0.006f)
			{
				Vector3 rhs = plane1Position - plane2Position;
				float d = Vector3.Dot(plane1Normal, rhs) / num;
				linePoint = plane2Position + d * vector;
				return true;
			}

			return false;
		}


		public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec,
			Vector3 planeNormal, Vector3 planePoint)
		{
			intersection = Vector3.zero;
			float num = Vector3.Dot(planePoint - linePoint, planeNormal);
			float num2 = Vector3.Dot(lineVec, planeNormal);
			if (num2 != 0f)
			{
				float size = num / num2;
				Vector3 b = SetVectorLength(lineVec, size);
				intersection = linePoint + b;
				return true;
			}

			return false;
		}


		public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1,
			Vector3 linePoint2, Vector3 lineVec2)
		{
			Vector3 lhs = linePoint2 - linePoint1;
			Vector3 rhs = Vector3.Cross(lineVec1, lineVec2);
			Vector3 lhs2 = Vector3.Cross(lhs, lineVec2);
			if (Mathf.Abs(Vector3.Dot(lhs, rhs)) < 0.0001f && rhs.sqrMagnitude > 0.0001f)
			{
				float d = Vector3.Dot(lhs2, rhs) / rhs.sqrMagnitude;
				intersection = linePoint1 + lineVec1 * d;
				return true;
			}

			intersection = Vector3.zero;
			return false;
		}


		public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2,
			Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
		{
			closestPointLine1 = Vector3.zero;
			closestPointLine2 = Vector3.zero;
			float num = Vector3.Dot(lineVec1, lineVec1);
			float num2 = Vector3.Dot(lineVec1, lineVec2);
			float num3 = Vector3.Dot(lineVec2, lineVec2);
			float num4 = num * num3 - num2 * num2;
			if (num4 != 0f)
			{
				Vector3 rhs = linePoint1 - linePoint2;
				float num5 = Vector3.Dot(lineVec1, rhs);
				float num6 = Vector3.Dot(lineVec2, rhs);
				float d = (num2 * num6 - num5 * num3) / num4;
				float d2 = (num * num6 - num5 * num2) / num4;
				closestPointLine1 = linePoint1 + lineVec1 * d;
				closestPointLine2 = linePoint2 + lineVec2 * d2;
				return true;
			}

			return false;
		}


		public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
		{
			float d = Vector3.Dot(point - linePoint, lineVec);
			return linePoint + lineVec * d;
		}


		public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
		{
			Vector3 vector = ProjectPointOnLine(linePoint1, (linePoint2 - linePoint1).normalized, point);
			int num = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, vector);
			if (num == 0)
			{
				return vector;
			}

			if (num == 1)
			{
				return linePoint1;
			}

			if (num == 2)
			{
				return linePoint2;
			}

			return Vector3.zero;
		}


		public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
		{
			float num = SignedDistancePlanePoint(planeNormal, planePoint, point);
			num *= -1f;
			Vector3 b = SetVectorLength(planeNormal, num);
			return point + b;
		}


		public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
		{
			return vector - Vector3.Dot(vector, planeNormal) * planeNormal;
		}


		public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
		{
			return Vector3.Dot(planeNormal, point - planePoint);
		}


		public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
		{
			return Vector3.Dot(Vector3.Cross(normal, vectorA), vectorB);
		}


		public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
		{
			Vector3 lhs = Vector3.Cross(normal, referenceVector);
			return Vector3.Angle(referenceVector, otherVector) * Mathf.Sign(Vector3.Dot(lhs, otherVector));
		}


		public static float AngleVectorPlane(Vector3 vector, Vector3 normal)
		{
			float num = (float) Math.Acos(Vector3.Dot(vector, normal));
			return 1.5707964f - num;
		}


		public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
		{
			double num = Vector3.Dot(vec1, vec2);
			if (num < -1.0)
			{
				num = -1.0;
			}

			if (num > 1.0)
			{
				num = 1.0;
			}

			return (float) Math.Acos(num);
		}


		public static void PlaneFrom3Points(out Vector3 planeNormal, out Vector3 planePoint, Vector3 pointA,
			Vector3 pointB, Vector3 pointC)
		{
			planeNormal = Vector3.zero;
			planePoint = Vector3.zero;
			Vector3 vector = pointB - pointA;
			Vector3 vector2 = pointC - pointA;
			planeNormal = Vector3.Normalize(Vector3.Cross(vector, vector2));
			Vector3 vector3 = pointA + vector / 2f;
			Vector3 vector4 = pointA + vector2 / 2f;
			Vector3 lineVec = pointC - vector3;
			Vector3 lineVec2 = pointB - vector4;
			Vector3 vector5;
			ClosestPointsOnTwoLines(out planePoint, out vector5, vector3, lineVec, vector4, lineVec2);
		}


		public static Vector3 GetForwardVector(Quaternion q)
		{
			return q * Vector3.forward;
		}


		public static Vector3 GetUpVector(Quaternion q)
		{
			return q * Vector3.up;
		}


		public static Vector3 GetRightVector(Quaternion q)
		{
			return q * Vector3.right;
		}


		public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
		}


		public static Vector3 PositionFromMatrix(Matrix4x4 m)
		{
			Vector4 column = m.GetColumn(3);
			return new Vector3(column.x, column.y, column.z);
		}


		public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector,
			Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp)
		{
			Quaternion lhs = Quaternion.LookRotation(alignWithVector, alignWithNormal);
			Quaternion rotation = Quaternion.LookRotation(customForward, customUp);
			gameObjectInOut.transform.rotation = lhs * Quaternion.Inverse(rotation);
		}


		public static void TransformWithParent(out Quaternion childRotation, out Vector3 childPosition,
			Quaternion parentRotation, Vector3 parentPosition, Quaternion startParentRotation,
			Vector3 startParentPosition, Quaternion startChildRotation, Vector3 startChildPosition)
		{
			childRotation = Quaternion.identity;
			childPosition = Vector3.zero;
			tempParent.rotation = startParentRotation;
			tempParent.position = startParentPosition;
			tempParent.localScale = Vector3.one;
			tempChild.rotation = startChildRotation;
			tempChild.position = startChildPosition;
			tempChild.localScale = Vector3.one;
			tempParent.rotation = parentRotation;
			tempParent.position = parentPosition;
			childRotation = tempChild.rotation;
			childPosition = tempChild.position;
		}


		public static void PreciseAlign(ref GameObject gameObjectInOut, Vector3 alignWithVector,
			Vector3 alignWithNormal, Vector3 alignWithPosition, Vector3 triangleForward, Vector3 triangleNormal,
			Vector3 trianglePosition)
		{
			LookRotationExtended(ref gameObjectInOut, alignWithVector, alignWithNormal, triangleForward,
				triangleNormal);
			Vector3 b = gameObjectInOut.transform.TransformPoint(trianglePosition);
			Vector3 translation = alignWithPosition - b;
			gameObjectInOut.transform.Translate(translation, Space.World);
		}


		public static void VectorsToTransform(ref GameObject gameObjectInOut, Vector3 positionVector,
			Vector3 directionVector, Vector3 normalVector)
		{
			gameObjectInOut.transform.position = positionVector;
			gameObjectInOut.transform.rotation = Quaternion.LookRotation(directionVector, normalVector);
		}


		public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
		{
			Vector3 rhs = linePoint2 - linePoint1;
			Vector3 lhs = point - linePoint1;
			if (Vector3.Dot(lhs, rhs) <= 0f)
			{
				return 1;
			}

			if (lhs.magnitude <= rhs.magnitude)
			{
				return 0;
			}

			return 2;
		}


		public static float MouseDistanceToLine(Vector3 linePoint1, Vector3 linePoint2)
		{
			Camera main = Camera.main;
			Vector3 mousePosition = Input.mousePosition;
			Vector3 linePoint3 = main.WorldToScreenPoint(linePoint1);
			Vector3 linePoint4 = main.WorldToScreenPoint(linePoint2);
			Vector3 vector = ProjectPointOnLineSegment(linePoint3, linePoint4, mousePosition);
			vector = new Vector3(vector.x, vector.y, 0f);
			return (vector - mousePosition).magnitude;
		}


		public static float MouseDistanceToCircle(Vector3 point, float radius)
		{
			Camera main = Camera.main;
			Vector3 mousePosition = Input.mousePosition;
			Vector3 vector = main.WorldToScreenPoint(point);
			vector = new Vector3(vector.x, vector.y, 0f);
			return (vector - mousePosition).magnitude - radius;
		}


		public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2, Vector3 rectA, Vector3 rectB,
			Vector3 rectC, Vector3 rectD)
		{
			bool flag = false;
			bool flag2 = IsPointInRectangle(linePoint1, rectA, rectC, rectB, rectD);
			if (!flag2)
			{
				flag = IsPointInRectangle(linePoint2, rectA, rectC, rectB, rectD);
			}

			if (!flag2 && !flag)
			{
				bool flag3 = AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);
				bool flag4 = AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);
				bool flag5 = AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);
				bool flag6 = AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);
				return flag3 || flag4 || flag5 || flag6;
			}

			return true;
		}


		public static bool IsPointInRectangle(Vector3 point, Vector3 rectA, Vector3 rectC, Vector3 rectB, Vector3 rectD)
		{
			Vector3 vector = rectC - rectA;
			float size = -(vector.magnitude / 2f);
			vector = AddVectorLength(vector, size);
			Vector3 linePoint = rectA + vector;
			Vector3 vector2 = rectB - rectA;
			float num = vector2.magnitude / 2f;
			Vector3 vector3 = rectD - rectA;
			float num2 = vector3.magnitude / 2f;
			float magnitude = (ProjectPointOnLine(linePoint, vector2.normalized, point) - point).magnitude;
			return (ProjectPointOnLine(linePoint, vector3.normalized, point) - point).magnitude <= num &&
			       magnitude <= num2;
		}


		public static bool AreLineSegmentsCrossing(Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2)
		{
			Vector3 vector = pointA2 - pointA1;
			Vector3 vector2 = pointB2 - pointB1;
			Vector3 point;
			Vector3 point2;
			if (ClosestPointsOnTwoLines(out point, out point2, pointA1, vector.normalized, pointB1, vector2.normalized))
			{
				bool flag = PointOnWhichSideOfLineSegment(pointA1, pointA2, point) != 0;
				int num = PointOnWhichSideOfLineSegment(pointB1, pointB2, point2);
				return !flag && num == 0;
			}

			return false;
		}


		public static bool LinearAcceleration(out Vector3 vector, Vector3 position, int samples)
		{
			Vector3 a = Vector3.zero;
			vector = Vector3.zero;
			if (samples < 3)
			{
				samples = 3;
			}

			if (positionRegister == null)
			{
				positionRegister = new Vector3[samples];
				posTimeRegister = new float[samples];
			}

			for (int i = 0; i < positionRegister.Length - 1; i++)
			{
				positionRegister[i] = positionRegister[i + 1];
				posTimeRegister[i] = posTimeRegister[i + 1];
			}

			positionRegister[positionRegister.Length - 1] = position;
			posTimeRegister[posTimeRegister.Length - 1] = Time.time;
			positionSamplesTaken++;
			if (positionSamplesTaken >= samples)
			{
				for (int j = 0; j < positionRegister.Length - 2; j++)
				{
					Vector3 a2 = positionRegister[j + 1] - positionRegister[j];
					float num = posTimeRegister[j + 1] - posTimeRegister[j];
					if (num == 0f)
					{
						return false;
					}

					Vector3 b = a2 / num;
					a2 = positionRegister[j + 2] - positionRegister[j + 1];
					num = posTimeRegister[j + 2] - posTimeRegister[j + 1];
					if (num == 0f)
					{
						return false;
					}

					Vector3 a3 = a2 / num;
					a += a3 - b;
				}

				a /= (float) (positionRegister.Length - 2);
				float d = posTimeRegister[posTimeRegister.Length - 1] - posTimeRegister[0];
				vector = a / d;
				return true;
			}

			return false;
		}


		public static bool AngularAcceleration(out Vector3 vector, Quaternion rotation, int samples)
		{
			Vector3 a = Vector3.zero;
			vector = Vector3.zero;
			if (samples < 3)
			{
				samples = 3;
			}

			if (rotationRegister == null)
			{
				rotationRegister = new Quaternion[samples];
				rotTimeRegister = new float[samples];
			}

			for (int i = 0; i < rotationRegister.Length - 1; i++)
			{
				rotationRegister[i] = rotationRegister[i + 1];
				rotTimeRegister[i] = rotTimeRegister[i + 1];
			}

			rotationRegister[rotationRegister.Length - 1] = rotation;
			rotTimeRegister[rotTimeRegister.Length - 1] = Time.time;
			rotationSamplesTaken++;
			if (rotationSamplesTaken >= samples)
			{
				for (int j = 0; j < rotationRegister.Length - 2; j++)
				{
					Quaternion rotation2 = SubtractRotation(rotationRegister[j + 1], rotationRegister[j]);
					float num = rotTimeRegister[j + 1] - rotTimeRegister[j];
					if (num == 0f)
					{
						return false;
					}

					Vector3 b = RotDiffToSpeedVec(rotation2, num);
					rotation2 = SubtractRotation(rotationRegister[j + 2], rotationRegister[j + 1]);
					num = rotTimeRegister[j + 2] - rotTimeRegister[j + 1];
					if (num == 0f)
					{
						return false;
					}

					Vector3 a2 = RotDiffToSpeedVec(rotation2, num);
					a += a2 - b;
				}

				a /= (float) (rotationRegister.Length - 2);
				float d = rotTimeRegister[rotTimeRegister.Length - 1] - rotTimeRegister[0];
				vector = a / d;
				return true;
			}

			return false;
		}


		public static float LinearFunction2DBasic(float x, float Qx, float Qy)
		{
			return x * (Qy / Qx);
		}


		public static float LinearFunction2DFull(float x, float Px, float Py, float Qx, float Qy)
		{
			float num = Qy - Py;
			float num2 = Qx - Px;
			float num3 = num / num2;
			return Py + num3 * (x - Px);
		}


		private static Vector3 RotDiffToSpeedVec(Quaternion rotation, float deltaTime)
		{
			float num;
			if (rotation.eulerAngles.x <= 180f)
			{
				num = rotation.eulerAngles.x;
			}
			else
			{
				num = rotation.eulerAngles.x - 360f;
			}

			float num2;
			if (rotation.eulerAngles.y <= 180f)
			{
				num2 = rotation.eulerAngles.y;
			}
			else
			{
				num2 = rotation.eulerAngles.y - 360f;
			}

			float num3;
			if (rotation.eulerAngles.z <= 180f)
			{
				num3 = rotation.eulerAngles.z;
			}
			else
			{
				num3 = rotation.eulerAngles.z - 360f;
			}

			return new Vector3(num / deltaTime, num2 / deltaTime, num3 / deltaTime);
		}
	}
}