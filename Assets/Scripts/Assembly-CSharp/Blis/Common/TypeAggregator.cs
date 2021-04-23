using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Blis.Common
{
	public abstract class TypeAggregator<TKey, TAttr, T> where TAttr : Attribute
	{
		private readonly Dictionary<TKey, Type> typeMap = new Dictionary<TKey, Type>();

		protected Dictionary<Type, TKey> revertTypeMap = new Dictionary<Type, TKey>();


		public TypeAggregator()
		{
			Type[] types = Assembly.GetAssembly(typeof(T)).GetTypes();
			List<Type> list = new List<Type>();
			foreach (Type type in types)
			{
				if (type.Namespace == GetType().Namespace && !type.IsAbstract && type.IsSubclassOf(typeof(T)))
				{
					list.Add(type);
				}
			}

			foreach (Type type2 in list)
			{
				TAttr customAttribute = type2.GetCustomAttribute<TAttr>();
				if (customAttribute == null)
				{
					Debug.LogWarning(type2.Name + " has no attribute");
				}
				else
				{
					TKey aggregateType = GetAggregateType(customAttribute);
					if (typeMap.ContainsKey(aggregateType))
					{
						throw new Exception("Type is duplicated: " + aggregateType);
					}

					typeMap.Add(aggregateType, type2);
					revertTypeMap.Add(type2, aggregateType);
				}
			}
		}


		public T Create(TKey key)
		{
			Type type;
			if (typeMap.TryGetValue(key, out type))
			{
				return (T) Activator.CreateInstance(type);
			}

			throw new Exception("Failed to find type from key " + key);
		}


		protected abstract TKey GetAggregateType(TAttr attr);
	}
}