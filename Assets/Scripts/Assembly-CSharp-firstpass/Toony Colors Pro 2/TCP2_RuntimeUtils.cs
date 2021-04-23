using System;
using System.Collections.Generic;
using UnityEngine;

public static class TCP2_RuntimeUtils
{
	private const string BASE_SHADER_PATH = "Toony Colors Pro 2/";


	private const string VARIANT_SHADER_PATH = "Hidden/Toony Colors Pro 2/Variants/";


	private const string BASE_SHADER_NAME = "Desktop";


	private const string BASE_SHADER_NAME_MOB = "Mobile";


	private static readonly List<string[]> ShaderVariants = new List<string[]>
	{
		new[]
		{
			"Specular",
			"TCP2_SPEC"
		},
		new[]
		{
			"Reflection",
			"TCP2_REFLECTION",
			"TCP2_REFLECTION_MASKED"
		},
		new[]
		{
			"Matcap",
			"TCP2_MC"
		},
		new[]
		{
			"Rim",
			"TCP2_RIM"
		},
		new[]
		{
			"RimOutline",
			"TCP2_RIMO"
		},
		new[]
		{
			"Outline",
			"OUTLINES"
		},
		new[]
		{
			"OutlineBlending",
			"OUTLINE_BLENDING"
		}
	};


	public static Shader GetShaderWithKeywords(Material material)
	{
		string text = material.shader != null && material.shader.name.ToLower().Contains("mobile")
			? "Mobile"
			: "Desktop";
		string text2 = text;
		foreach (string[] array in ShaderVariants)
		{
			foreach (string a in material.shaderKeywords)
			{
				for (int j = 1; j < array.Length; j++)
				{
					if (a == array[j])
					{
						text2 = text2 + " " + array[0];
					}
				}
			}
		}

		text2 = text2.TrimEnd(Array.Empty<char>());
		string str = "Toony Colors Pro 2/";
		if (text2 != text)
		{
			str = "Hidden/Toony Colors Pro 2/Variants/";
		}

		return Shader.Find(str + text2);
	}
}