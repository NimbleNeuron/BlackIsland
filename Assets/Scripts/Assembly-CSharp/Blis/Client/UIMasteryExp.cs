using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIMasteryExp : BaseControl
	{
		[SerializeField] private Text category = default;


		[SerializeField] private Text value = default;

		public void SetCategory(string category)
		{
			this.category.text = category;
		}


		public void SetValue(string value)
		{
			this.value.text = value;
		}
	}
}