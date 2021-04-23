using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blis.Client
{
	[Serializable]
	public class LnDB : MonoBehaviour
	{
		public const string path = "LocalizationData";


		[HideInInspector] public List<LnSentence> rawDataList;
	}
}