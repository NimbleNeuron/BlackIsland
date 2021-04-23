using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Utils
{
	
	public class RandomDistribution<T>
	{
		
		public RandomDistribution(List<T> values, RandomDistribution<T>.PredicateDistribution<T> predicateDistribution)
		{
			int num = 0;
			foreach (T t in values)
			{
				int num2 = predicateDistribution(t);
				num += num2;
				this.accumulatedDistributionTuple.Add(new Tuple<int, T>(num, t));
			}
			this.maxDistributionValue = num;
		}

		
		public T Sample()
		{
			int sampleValue = UnityEngine.Random.Range(0, this.maxDistributionValue);
			return this.accumulatedDistributionTuple.First((Tuple<int, T> t) => t.Item1 > sampleValue).Item2;
		}

		
		private int maxDistributionValue;

		
		private readonly List<Tuple<int, T>> accumulatedDistributionTuple = new List<Tuple<int, T>>();

		
		public delegate int PredicateDistribution<in U>(U value);
	}
}
