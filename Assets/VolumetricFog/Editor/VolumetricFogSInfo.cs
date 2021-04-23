﻿using System.Text;
using System.Collections.Generic;

namespace VolumetricFogAndMist {
	public class VolumetricFogSInfo {

		public string name = "";
		public string path = "";
		public List<SCShaderPass> passes = new List<SCShaderPass>();
		public List<SCKeyword> keywords = new List<SCKeyword>();
		public bool pendingChanges;
		public bool editedByShaderControl;
		public int enabledKeywordCount;

		public void Add(SCShaderPass pass) {
			passes.Add(pass);
			UpdateKeywords();
		}

		public void EnableKeywords() {
			keywords.ForEach((SCKeyword keyword) => keyword.enabled = true);
		}

		void UpdateKeywords() {
			passes.ForEach((SCShaderPass pass) => {
					for (int l = 0; l < pass.keywordLines.Count; l++) {
						SCKeywordLine line = pass.keywordLines[l];
						for (int k = 0; k < line.keywords.Count; k++) {
							SCKeyword keyword = line.keywords[k];
							if (!keywords.Contains(keyword)) {
								if (SCKeywordChecker.IsValid(keyword.name)) {
									keywords.Add(keyword);
								}
							}
						}
					}
				});

			enabledKeywordCount = 0;
			keywords.ForEach((SCKeyword kw) => {
					if (kw.enabled)
						enabledKeywordCount++;
				});
		}

		public SCKeyword GetKeyword(string name) {
			int kCount = keywords.Count;
			for (int k = 0; k < kCount; k++) {
				SCKeyword keyword = keywords[k];
				if (keyword.name.Equals(name))
					return keyword;
			}
			return new SCKeyword(name);
		}

	}

	public class SCShaderPass {
		public int pass;
		public List<SCKeywordLine> keywordLines = new List<SCKeywordLine>();
		public int keywordCount;

		public void Add(SCKeywordLine keywordLine) {
			keywordLines.Add(keywordLine);
			UpdateKeywordCount();
		}

		public void Add(List<SCKeywordLine> keywordLines) {
			this.keywordLines.AddRange(keywordLines);
			UpdateKeywordCount();
		}

		void UpdateKeywordCount() {
			keywordCount = 0;
			keywordLines.ForEach((SCKeywordLine obj) => keywordCount += obj.keywordCount);
		}

		public void Clear() {
			keywordCount = 0;
			keywordLines.Clear();
		}
	}

	public class SCKeywordLine {
		public List<SCKeyword> keywords = new List<SCKeyword>();
		public bool hasUnderscoreVariant;

		public SCKeyword GetKeyword(string name) {
			int kc = keywords.Count;
			for (int k = 0; k < kc; k++) {
				SCKeyword keyword = keywords[k];
				if (keyword.name.Equals(name)) {
					return keyword;
				}
			}
			return null;
		}

		public void Add(SCKeyword keyword) {
			if (GetKeyword(keyword.name) != null)
				return;
			// ignore underscore keywords
			bool goodKeyword = false;
			for (int k = 0; k < keyword.name.Length; k++) {
				if (keyword.name[k] != '_') {
					goodKeyword = true;
					break;
				}
			}
			if (goodKeyword) {
				keywords.Add(keyword);
			} else {
				keyword.isUnderscoreKeyword = true;
				hasUnderscoreVariant = true;
			}
		}

		public void DisableKeywords() {
			keywords.ForEach((SCKeyword obj) => obj.enabled = false);
		}

		public void Clear() {
			keywords.Clear();
		}

		public int keywordCount {
			get { 
				return keywords.Count;
			}
		}

		public int keywordsEnabledCount {
			get {
				int kCount = keywords.Count;
				int enabledCount = 0;
				for (int k = 0; k < kCount; k++) {
					if (keywords[k].enabled)
						enabledCount++;
				}
				return enabledCount;
			}
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			for (int k = 0; k < keywords.Count; k++) {
				if (k > 0)
					sb.Append(" ");
				sb.Append(keywords[k].name);
			}
			return sb.ToString();
		}

	}

	public class SCKeyword {
		public string name;
		public bool enabled;
		public bool isUnderscoreKeyword;

		public SCKeyword(string name) {
			this.name = name;
			enabled = true;
		}

		public override bool Equals(object obj) {
			if (obj is SCKeyword) {
				SCKeyword other = (SCKeyword)obj;
				return name.Equals(other.name);
			}
			return false;
		}

		public override int GetHashCode() {
			return name.GetHashCode();
		}

		public override string ToString() {
			return name;
		}
	}

	static class SCKeywordChecker {

		static Dictionary<string, string> dict = new Dictionary<string, string>();

		static SCKeywordChecker() {
			string[][] kw = new string[][]
			{
				new string[] { VolumetricFog.SKW_FOG_HAZE_ON, "Sky Haze" },
				new string[] { VolumetricFog.SKW_FOG_OF_WAR_ON, "Fog of War" },
				new string[] { VolumetricFog.SKW_LIGHT_SCATTERING, "Light Scattering" },
				new string[] { VolumetricFog.SKW_SUN_SHADOWS, "Sun Shadows" },
				new string[] { VolumetricFog.SKW_FOG_VOID_BOX, "Fog Void (Box shape)" },
				new string[]
				{
					VolumetricFog.SKW_FOG_VOID_SPHERE,
					"Fog Void (Sphere shape)"
				},
				new string[] { VolumetricFog.SKW_FOG_AREA_BOX, "Fog Area (Box shape)" },
				new string[]
				{
					VolumetricFog.SKW_FOG_AREA_SPHERE,
					"Fog Area (Sphere shape)"
				},
				new string[] { VolumetricFog.SKW_FOG_BLUR, "Depth Blur" }, 
				new string[] { VolumetricFog.SKW_FOG_USE_XY_PLANE, "Use XY Plane" },
				new string[]
				{
					VolumetricFog.SKW_FOG_COMPUTE_DEPTH,
					"Compute Depth"
				}
			};
			for (int k = 0; k < kw.Length; k++) dict[kw[k][0]] = kw[k][1];
		}

		public static bool IsValid(string name) {
			return dict.ContainsKey(name);
		}

		public static string Translate(string name) {
			if (dict.ContainsKey(name)) {
				return dict[name];
			} else {
				return name;
			}
		}


		
	}
	
	
}