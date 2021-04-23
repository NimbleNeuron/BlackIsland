using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;
using Random = System.Random;

namespace Blis.Common
{
	public static class GameUtil
	{
		public enum NumberOfDigits
		{
			One,

			Two
		}


		private static readonly ThreadLocal<StringBuilder> StringBuilderThreadLocal =
			new ThreadLocal<StringBuilder>(() => new StringBuilder());


		private static readonly Random rng = new Random();


		private static readonly Dictionary<int, string> intToStringMapOne = new Dictionary<int, string>();


		private static readonly Dictionary<int, string> intToStringMapTwo = new Dictionary<int, string>();


		public static StringBuilder StringBuilder { get; } = new StringBuilder();


		public static StringBuilder StringBuilder_2 { get; } = new StringBuilder();


		public static StringBuilder StringBuilderSafety => StringBuilderThreadLocal.Value;


		public static RaycastHitDistanceComparer raycastHitDistanceComparer { get; } = new RaycastHitDistanceComparer();


		public static T Bind<T>(GameObject parent, ref T target) where T : Component
		{
			if (target != null)
			{
				return target;
			}

			target = TryGetComponent<T>(parent);
			return target;
		}


		public static void BindOrAdd<T>(GameObject parent, ref T target) where T : Component
		{
			if (target != null)
			{
				return;
			}

			target = TryGetOrAddComponent<T>(parent);
		}


		public static T TryGetComponent<T>(GameObject parent) where T : Component
		{
			T component = parent.GetComponent<T>();
			if (component != null)
			{
				return component;
			}

			Debug.LogError(string.Format("Failed to find a component [{0}] [{1}]", typeof(T).Name, parent));
			return default;
		}


		public static T TryGetOrAddComponent<T>(GameObject parent) where T : Component
		{
			T t = parent.GetComponent<T>();
			if (t != null)
			{
				return t;
			}

			t = parent.AddComponent<T>();
			if (t == null)
			{
				Debug.LogError("Why added component is null?");
			}

			return t;
		}


		public static T Bind<T>(GameObject parent, string path) where T : Component
		{
			Transform transform = parent.transform.Find(path);
			if (transform == null)
			{
				throw new Exception("Failed to find a gameObject in path(" + path + ")");
			}

			T component = transform.GetComponent<T>();
			if (component == null)
			{
				throw new Exception(string.Concat("Failed to find a component(", typeof(T).Name, ") from GameObject(",
					path, ")"));
			}

			return component;
		}


		public static Transform FindRecursively(this Transform Transform, string Name, bool includeInactive = true)
		{
			if (Transform.name == Name)
			{
				if (includeInactive)
				{
					return Transform;
				}

				if (!includeInactive && Transform.gameObject.activeInHierarchy)
				{
					return Transform;
				}
			}

			foreach (object obj in Transform)
			{
				Transform transform = ((Transform) obj).FindRecursively(Name, includeInactive);
				if (transform != null)
				{
					return transform;
				}
			}

			return null;
		}


		public static Transform FindParentRecursively(this Transform Transform, string Name)
		{
			if (Transform == null)
			{
				return null;
			}

			if (Transform.name == Name)
			{
				return Transform;
			}

			Transform transform = Transform.parent.FindParentRecursively(Name);
			if (transform != null)
			{
				return transform;
			}

			return null;
		}


		public static DateTime ConvertFromUnixTimestamp(long timestamp)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return dateTime.AddSeconds(timestamp);
		}


		public static long ConvertToUnixTimestamp(DateTime date)
		{
			DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return (long) Math.Floor((date - d).TotalSeconds);
		}


		public static DateTime ConvertTimeFromUtc(long timestamp, TimeZoneInfo timezoneInfo)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(ConvertFromUnixTimestamp(timestamp), timezoneInfo);
		}


		public static DateTime ConvertTimeFromUtc(DateTime dateTime, TimeZoneInfo timezoneInfo)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timezoneInfo);
		}


		public static T CloneComponent<T>(T original, GameObject destination) where T : Component
		{
			Type type = original.GetType();
			Component component = destination.AddComponent(type);
			foreach (FieldInfo fieldInfo in type.GetFields())
			{
				fieldInfo.SetValue(component, fieldInfo.GetValue(original));
			}

			return component as T;
		}


		public static string ReadProgramArg(string arg)
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (commandLineArgs[i].Length > 1 && commandLineArgs[i].StartsWith("-"))
				{
					string b = commandLineArgs[i].Substring(1);
					if (arg == b && i + 1 < commandLineArgs.Length)
					{
						return commandLineArgs[i + 1];
					}
				}
			}

			return "";
		}


		public static void Shuffle<T>(this IList<T> list)
		{
			int i = list.Count;
			while (i > 1)
			{
				i--;
				int index = rng.Next(i + 1);
				T value = list[index];
				list[index] = list[i];
				list[i] = value;
			}
		}


		public static float DistanceOnPlane(Vector3 src, Vector3 dest)
		{
			src.y = 0f;
			dest.y = 0f;
			return (dest - src).magnitude;
		}


		public static float Distance(Vector3 src, Vector3 dest)
		{
			return (dest - src).magnitude;
		}


		public static Vector3 DirectionOnPlane(Vector3 src, Vector3 dest)
		{
			src.y = 0f;
			dest.y = 0f;
			return (dest - src).normalized;
		}


		public static Vector3 Direction(Vector3 src, Vector3 dest)
		{
			return (dest - src).normalized;
		}


		public static Quaternion LookRotation(Vector3 direction)
		{
			if (!direction.Equals(Vector3.zero))
			{
				return Quaternion.LookRotation(direction);
			}

			return Quaternion.identity;
		}


		public static Quaternion LookRotation(Vector3 direction, Vector3 upwards)
		{
			if (!direction.Equals(Vector3.zero))
			{
				return Quaternion.LookRotation(direction, upwards);
			}

			return Quaternion.identity;
		}


		public static float GetCooldown(float cooldown, float statCooldownReduction)
		{
			return cooldown * GetCooldowncooldownReduction(statCooldownReduction);
		}


		public static float GetCooldowncooldownReduction(float statCooldownReduction)
		{
			return 1f - statCooldownReduction;
		}


		public static int GetRandomElementalIndex(List<int> rateList)
		{
			if (rateList.Count == 0)
			{
				return -1;
			}

			int num = rateList.Sum();
			int num2 = UnityEngine.Random.Range(0, num + 1);
			int num3 = 0;
			for (int i = 0; i < rateList.Count; i++)
			{
				num3 += rateList[i];
				if (num2 <= num3)
				{
					return i;
				}
			}

			return -1;
		}


		public static float CalculateLinearBezierPoint(float t, Vector3 p0, Vector3 p1)
		{
			return p0.y + t * (p1.y - p0.y);
		}


		public static float CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
		{
			float num = 1f - t;
			float num2 = t * t;
			return num * num * p0.y + 2f * num * t * p1.y + num2 * p2.y;
		}


		public static float CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			float num = 1f - t;
			float num2 = t * t;
			float num3 = num * num;
			float num4 = num3 * num;
			float num5 = num2 * t;
			return num4 * p0.y + 3f * num3 * t * p1.y + 3f * num * num2 * p2.y + num5 * p3.y;
		}


		public static bool IsEmpty<T>(this IEnumerable<T> collection)
		{
			return !collection.Any<T>();
		}


		public static Vector2 ConvertPositionOnScreenResolution(float x, float y)
		{
			return new Vector2(x * (Screen.width > 1920 ? 1920f / Screen.width : Screen.width / 1920f),
				y * (Screen.height > 1080 ? 1080f / Screen.height : Screen.height / 1080f));
		}


		public static void ExitGame()
		{
			Application.Quit();
		}


		public static float GetDirectionAngle(Vector3 dir_1, Vector3 dir_2)
		{
			dir_1.y = 0f;
			dir_2.y = 0f;
			Vector3 normalized = dir_1.normalized;
			Vector3 normalized2 = dir_2.normalized;
			float x = normalized.x * normalized2.x + normalized.z * normalized2.z;
			return Mathf.Atan2(normalized.z * normalized2.x - normalized.x * normalized2.z, x) * 57.29578f;
		}


		public static float GetAngle(Vector2 pos1, Vector2 pos2)
		{
			Vector2 normalized = (pos2 - pos1).normalized;
			float num = Mathf.Atan2(normalized.x, normalized.y) * 180f / 3.1415927f;
			if (num < 0f)
			{
				num += 360f;
			}

			return 360f - num;
		}


		public static Quaternion GetRotation(Vector2 fromPos, Vector2 toPos)
		{
			return Quaternion.Euler(0f, 0f, GetAngle(toPos, fromPos));
		}


		public static bool Approximately(Vector3 a, Vector3 b, float tolerance)
		{
			return (b - a).sqrMagnitude < tolerance;
		}


		public static bool ApproximatelyToPlane(Vector3 a, Vector3 b, float tolerance)
		{
			b.y = 0f;
			a.y = 0f;
			return (b - a).sqrMagnitude < tolerance;
		}


		public static string IntToString(int value, NumberOfDigits numberOfDigits)
		{
			if (numberOfDigits == NumberOfDigits.One)
			{
				if (value >= 10)
				{
					return value.ToString();
				}

				if (!intToStringMapOne.ContainsKey(value))
				{
					intToStringMapOne.Add(value, value.ToString());
				}

				return intToStringMapOne[value];
			}

			if (numberOfDigits != NumberOfDigits.Two)
			{
				return value.ToString();
			}

			if (value >= 100)
			{
				return value.ToString();
			}

			if (!intToStringMapTwo.ContainsKey(value))
			{
				intToStringMapTwo.Add(value, string.Format("{0:00}", value));
			}

			return intToStringMapTwo[value];
		}
	}
}