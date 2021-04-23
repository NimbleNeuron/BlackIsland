#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;
using System.Collections.Generic;
using NodeCanvas.Framework;

namespace NodeCanvas.Editor
{

    ///Graph Refactoring
    public class GraphRefactor : EditorWindow
    {

        //...
        public static void ShowWindow() {
            GetWindow<GraphRefactor>().Show();
        }

        private Dictionary<string, List<IMissingRecoverable>> recoverablesMap;
        private Dictionary<string, string> recoverableChangesMap;

        private Dictionary<string, List<ISerializedReflectedInfo>> reflectedMap;
        private Dictionary<string, fsData> reflectedChangesMap;

        ///----------------------------------------------------------------------------------------------

        void Flush() {
            recoverablesMap = null;
            reflectedChangesMap = null;
            recoverablesMap = null;
            reflectedChangesMap = null;
        }

        //...
        void Gather() {

            EditorGUIUtility.keyboardControl = 0;
            EditorGUIUtility.hotControl = 0;

            GatherRecoverables();
            GatherReflected();
        }

        //...
        void GatherRecoverables() {
            recoverablesMap = new Dictionary<string, List<IMissingRecoverable>>();
            recoverableChangesMap = new Dictionary<string, string>();

            var graph = GraphEditor.currentGraph;
            var metaGraph = graph.GetFlatMetaGraph();
            var recoverables = metaGraph.GetAllChildrenReferencesOfType<IMissingRecoverable>();
            foreach ( var recoverable in recoverables ) {
                List<IMissingRecoverable> collection;
                if ( !recoverablesMap.TryGetValue(recoverable.missingType, out collection) ) {
                    collection = new List<IMissingRecoverable>();
                    recoverablesMap[recoverable.missingType] = collection;
                    recoverableChangesMap[recoverable.missingType] = recoverable.missingType;
                }
                collection.Add(recoverable);
            }
        }

        //...
        void GatherReflected() {
            reflectedMap = new Dictionary<string, List<ISerializedReflectedInfo>>();
            reflectedChangesMap = new Dictionary<string, fsData>();
            var graph = GraphEditor.currentGraph;
            JSONSerializer.SerializeAndExecuteNoCycles(typeof(NodeCanvas.Framework.Internal.GraphSource), graph.GetGraphSource(), DoCollect);
        }

        //...
        void DoCollect(object o, fsData d) {
            if ( o is ISerializedReflectedInfo ) {
                var reflect = (ISerializedReflectedInfo)o;
                if ( reflect.AsMemberInfo() == null ) {
                    List<ISerializedReflectedInfo> collection;
                    if ( !reflectedMap.TryGetValue(reflect.AsString(), out collection) ) {
                        collection = new List<ISerializedReflectedInfo>();
                        reflectedMap[reflect.AsString()] = collection;
                        reflectedChangesMap[reflect.AsString()] = d;
                    }
                    collection.Add(reflect);
                }
            }
        }

        ///----------------------------------------------------------------------------------------------

        //...
        void Save() {

            if ( recoverableChangesMap.Count > 0 || reflectedChangesMap.Count > 0 ) {

                if ( recoverableChangesMap.Count > 0 ) {
                    SaveRecoverables();
                }
                if ( reflectedChangesMap.Count > 0 ) {
                    SaveReflected();
                }

                GraphEditor.currentGraph.SelfSerialize();
                GraphEditor.currentGraph.SelfDeserialize();
                GraphEditor.currentGraph.Validate();
                Gather();
            }
        }

        //...
        void SaveRecoverables() {
            foreach ( var pair in recoverablesMap ) {
                foreach ( var recoverable in pair.Value ) {
                    recoverable.missingType = recoverableChangesMap[pair.Key];
                }
            }
        }

        //...
        void SaveReflected() {
            foreach ( var pair in reflectedMap ) {
                foreach ( var reflect in pair.Value ) {
                    var data = reflectedChangesMap[pair.Key];
                    JSONSerializer.TryDeserializeOverwrite(reflect, data.ToString(), null);
                }
            }
        }

        ///----------------------------------------------------------------------------------------------

        //...
        void OnEnable() {
            titleContent = new GUIContent("Refactor", StyleSheet.canvasIcon);
            GraphEditor.onCurrentGraphChanged -= OnGraphChanged;
            GraphEditor.onCurrentGraphChanged += OnGraphChanged;
        }

        //...
        void OnDisable() {
            GraphEditor.onCurrentGraphChanged -= OnGraphChanged;
        }

        void OnGraphChanged(Graph graph) { Flush(); Repaint(); }

        //...
        void OnGUI() {

            if ( Application.isPlaying ) {
                ShowNotification(new GUIContent("Refactor only works in editor mode. Please exit play mode."));
                return;
            }

            if ( GraphEditor.current == null || GraphEditor.currentGraph == null ) {
                ShowNotification(new GUIContent("No Graph is currently open in the Graph Editor."));
                return;
            }

            RemoveNotification();

            EditorGUILayout.HelpBox("Batch refactor missing nodes, tasks, types as well as missing reflection based methods, properties, fields and so on references. Note that changes made here are irreversible. Please proceed with caution.\n\n1) Hit Gather to fetch missing elements from the currently viewing graph in the editor.\n2) Rename elements serialization data to their new name (keep the same format).\n3) Hit Save to commit your changes.", MessageType.Info);

            if ( GUILayout.Button("Gather", GUILayout.Height(30)) ) { Gather(); }
            EditorUtils.Separator();

            if ( recoverablesMap == null || reflectedMap == null ) { return; }

            EditorGUI.indentLevel = 1;
            DoRecoverables();

            EditorGUI.indentLevel = 1;
            DoReflected();

            if ( recoverableChangesMap.Count > 0 || reflectedChangesMap.Count > 0 ) {
                if ( GUILayout.Button("Save", GUILayout.Height(30)) ) { Save(); }
            } else {
                GUILayout.Label("It's all looking good :-)");
                EditorUtils.Separator();
            }
        }

        //...
        void DoRecoverables() {

            if ( recoverablesMap.Count == 0 ) {
                GUILayout.Label("No missing recoverable elements found.");
                return;
            }

            foreach ( var pair in recoverablesMap ) {
                var originalName = pair.Key;
                GUILayout.Label(string.Format("<b>{0} occurencies: Type '{1}'</b>", pair.Value.Count, originalName));
                GUILayout.Space(5);
                var typeName = recoverableChangesMap[originalName];
                typeName = EditorGUILayout.TextField("Type Name", typeName);
                recoverableChangesMap[originalName] = typeName;
                EditorUtils.Separator();
            }
        }

        //...
        void DoReflected() {

            if ( reflectedMap.Count == 0 ) {
                GUILayout.Label("No missing reflected references found.");
                return;
            }

            foreach ( var pair in reflectedMap ) {
                var information = pair.Key;
                GUILayout.Label(string.Format("<b>{0} occurencies: '{1}'</b>", pair.Value.Count, information));
                GUILayout.Space(5);
                fsData data = reflectedChangesMap[information];
                var dict = new Dictionary<string, fsData>(data.AsDictionary);
                foreach ( var dataPair in dict ) {
                    var value = dataPair.Value.AsString;
                    var newValue = EditorGUILayout.TextField(dataPair.Key, value);
                    if ( newValue != value ) {
                        data.AsDictionary[dataPair.Key] = new fsData(newValue);
                    }
                }
                reflectedChangesMap[information] = data;
                EditorUtils.Separator();
            }
        }

    }
}
#endif

