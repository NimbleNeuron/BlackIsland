using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OutlineUGUI : Shadow
{
	
	protected OutlineUGUI()
	{
	}

	
	public override void ModifyMesh(VertexHelper vh)
	{
		if (!this.IsActive())
		{
			return;
		}
		List<UIVertex> list = new List<UIVertex>();
		vh.GetUIVertexStream(list);
		if (list == null || list.Count == 0)
		{
			return;
		}
		int num = 0;
		for (float num2 = 0f; num2 <= 6.2831855f; num2 += 3.1415927f / (float)this.mDivideAmoumt)
		{
			int start = num;
			num = list.Count;
			base.ApplyShadow(list, base.effectColor, start, num, base.effectDistance.x * Mathf.Cos(num2), base.effectDistance.y * Mathf.Sin(num2));
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(list);
	}

	
	[Range(2f, 16f)]
	[SerializeField]
	private int mDivideAmoumt = 4;
}
