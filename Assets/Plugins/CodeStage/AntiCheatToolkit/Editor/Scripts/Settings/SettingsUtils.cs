#region copyright
// --------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [http://codestage.net]
// --------------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using System;
	using UnityEditor;

	internal static class SettingsUtils
	{
		public static bool IsIL2CPPEnabled()
		{
			if (PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) ==
			    ScriptingImplementation.IL2CPP)
			{
				return true;
			}

#if ENABLE_IL2CPP
			return true;
#else
			return false;
#endif
		}

		public static bool IsIL2CPPSupported()
		{
			return IsIL2CPPEnabled() || ReflectionTools.IsScriptingImplementationSupported(ScriptingImplementation.IL2CPP, EditorUserBuildSettings.selectedBuildTargetGroup);
		}

		public static SymbolsData GetSymbolsData()
		{
			var result = new SymbolsData();

			var groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
			foreach (var buildTargetGroup in groups)
			{
				if (buildTargetGroup == BuildTargetGroup.Unknown) continue;

				var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

				result.injectionDebug |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.InjectionDebug);
				result.injectionDebugVerbose |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.InjectionDebugVerbose);
				result.injectionDebugParanoid |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.InjectionDebugParanoid);
				result.wallhackDebug |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.WallhackDebug);
				result.detectionBacklogs |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.DetectionBacklogs);
				result.excludeObfuscation |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.ExcludeObfuscation);
				result.preventReadPhoneState |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.PreventReadPhoneState);
				result.preventInternetPermission |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.PreventInternetPermission);
				result.obscuredAutoMigration |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.ObscuredAutoMigration);
				result.exposeThirdPartyIntegration |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.ThirdPartyIntegration);
				result.usExportCompatible |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.UsExportCompatible);
			}

			return result;
		}

		public static bool GetSymbol(string symbols, string symbol)
		{
			var result = false;

			if (symbols == symbol)
			{
				result = true;
			}
			else if (symbols.StartsWith(symbol + ';'))
			{
				result = true;
			}
			else if (symbols.EndsWith(';' + symbol))
			{
				result = true;
			}
			else if (symbols.Contains(';' + symbol + ';'))
			{
				result = true;
			}

			return result;
		}

		public static void SetSymbol(string symbol)
		{
			var names = Enum.GetNames(typeof(BuildTargetGroup));
			foreach (var n in names)
			{
				if (IsBuildTargetGroupNameObsolete(n)) continue;

				var buildTargetGroup = (BuildTargetGroup)Enum.Parse(typeof(BuildTargetGroup), n);
				if (buildTargetGroup == BuildTargetGroup.Unknown) continue;
				

				var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
				if (symbols.Length == 0)
				{
					symbols = symbol;
				}
				else
				{
					if (symbols.EndsWith(";"))
					{
						symbols += symbol;
					}
					else
					{
						symbols += ';' + symbol;
					}
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
			}
		}

		public static void RemoveSymbol(string symbol)
		{
			var names = Enum.GetNames(typeof(BuildTargetGroup));
			foreach (var n in names)
			{
				if (IsBuildTargetGroupNameObsolete(n)) continue;
				var buildTargetGroup = (BuildTargetGroup)Enum.Parse(typeof(BuildTargetGroup), n);
				if (buildTargetGroup == BuildTargetGroup.Unknown) continue;

				var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

				if (symbols == symbol)
				{
					symbols = string.Empty;
				}
				else if (symbols.StartsWith(symbol + ';'))
				{
					symbols = symbols.Remove(0, symbol.Length + 1);
				}
				else if (symbols.EndsWith(';' + symbol))
				{
					symbols = symbols.Remove(symbols.LastIndexOf(';' + symbol, StringComparison.Ordinal), symbol.Length + 1);
				}
				else if (symbols.Contains(';' + symbol + ';'))
				{
					symbols = symbols.Replace(';' + symbol + ';', ";");
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
			}
		}

		private static bool IsBuildTargetGroupNameObsolete(string name)
		{
			var fi = typeof(BuildTargetGroup).GetField(name);
			var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
			return attributes.Length > 0;
		}
	}
}