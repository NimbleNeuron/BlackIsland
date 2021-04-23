using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public static class WorldCharacterSorter
	{
		
		private static float CalcRemainHpRate(WorldCharacter character)
		{
			return (float)character.Status.Hp / (float)character.Stat.MaxHp;
		}

		
		public static IEnumerable<WorldCharacter> ObjectType(this IEnumerable<WorldCharacter> objects, ObjectType objectType)
		{
			return from x in objects
			where x.ObjectType == objectType
			select x;
		}

		
		public static IOrderedEnumerable<T> Nearest<T>(this IEnumerable<T> objects, Vector3 position) where T : WorldObject
		{
			return from x in objects
			orderby Vector3.Distance(x.GetPosition(), position)
			select x;
		}

		
		public static IOrderedEnumerable<SkillAgent> Nearest(this IEnumerable<SkillAgent> objects, Vector3 position)
		{
			return from x in objects
			orderby Vector3.Distance(x.Position, position)
			select x;
		}

		
		public static SkillAgent NearestOne(this List<SkillAgent> objects, Vector3 position)
		{
			SkillAgent result = null;
			float num = float.MaxValue;
			foreach (SkillAgent skillAgent in objects)
			{
				float num2 = Vector3.Distance(skillAgent.Position, position);
				if (num2 <= num)
				{
					num = num2;
					result = skillAgent;
				}
			}
			return result;
		}

		
		public static T NearestOne<T>(this List<T> objects, Vector3 position) where T : WorldObject
		{
			T result = default(T);
			float num = float.MaxValue;
			foreach (T t in objects)
			{
				float num2 = Vector3.Distance(t.GetPosition(), position);
				if (num2 <= num)
				{
					num = num2;
					result = t;
				}
			}
			return result;
		}

		
		public static IOrderedEnumerable<WorldCharacter> MostHP(this IEnumerable<WorldCharacter> objects)
		{
			return from x in objects
			orderby WorldCharacterSorter.CalcRemainHpRate(x) descending
			select x;
		}

		
		public static IOrderedEnumerable<WorldCharacter> MostHP(this IOrderedEnumerable<WorldCharacter> objects)
		{
			return objects.ThenByDescending((WorldCharacter x) => WorldCharacterSorter.CalcRemainHpRate(x));
		}

		
		public static IOrderedEnumerable<WorldCharacter> LeastHP(this IEnumerable<WorldCharacter> objects)
		{
			return from x in objects
			orderby WorldCharacterSorter.CalcRemainHpRate(x)
			select x;
		}

		
		public static IOrderedEnumerable<WorldCharacter> LeastHP(this IOrderedEnumerable<WorldCharacter> objects)
		{
			return objects.ThenBy((WorldCharacter x) => WorldCharacterSorter.CalcRemainHpRate(x));
		}
	}
}
