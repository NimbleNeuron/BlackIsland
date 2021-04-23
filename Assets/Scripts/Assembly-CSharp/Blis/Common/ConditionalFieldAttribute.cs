using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blis.Common
{
	
	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		
		public ConditionalFieldAttribute(string propertyToCheck, object compareValue = null)
		{
			this._propertyToCheck = propertyToCheck;
			this._compareValue = compareValue;
		}

		
		public ConditionalFieldAttribute(string propertyToCheck, params object[] compareValueList)
		{
			this._propertyToCheck = propertyToCheck;
			this._compareValueList = compareValueList.ToList<object>();
		}

		
		private readonly string _propertyToCheck;

		
		private readonly object _compareValue;

		
		private readonly List<object> _compareValueList;
	}
}
