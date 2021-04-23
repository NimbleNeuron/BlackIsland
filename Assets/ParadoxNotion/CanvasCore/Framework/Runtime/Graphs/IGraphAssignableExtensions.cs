using System.Linq;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Framework
{

    public static class IGraphAssignableExtensions
    {
        ///Checks and possibly makes and returns runtime instance
        public static Graph CheckInstance(this IGraphAssignable assignable) {
            if ( assignable.subGraph == assignable.currentInstance ) {
                return assignable.currentInstance;
            }

            Graph instance = null;
            if ( assignable.instances == null ) { assignable.instances = new System.Collections.Generic.Dictionary<Graph, Graph>(); }
            if ( !assignable.instances.TryGetValue(assignable.subGraph, out instance) ) {
                instance = Graph.Clone(assignable.subGraph, assignable.graph);
                assignable.instances[assignable.subGraph] = instance;
            }

            assignable.subGraph = instance;
            assignable.currentInstance = instance;
            return instance;
        }

        ///Utility to start sub graph (makes instance, writes mapping, starts graph and on stop reads mapping)
        public static bool TryStartSubGraph(this IGraphAssignable assignable, Component agent, System.Action<bool> callback = null) {
            assignable.currentInstance = assignable.CheckInstance();
            if ( assignable.currentInstance != null ) {
                assignable.TryWriteMappedVariables();
                //we always start with the current graphs blackboard parent bb as the subgraphs parent bb
                assignable.currentInstance.StartGraph(agent, assignable.graph.blackboard.parent, Graph.UpdateMode.Manual, (result) =>
                {
                    if ( assignable.status == Status.Running ) { assignable.TryReadMappedVariables(); }
                    if ( callback != null ) { callback(result); }
                });
                return true;
            }
            return false;
        }

        ///Stop subgraph if currentInstance exists
        public static bool TryStopSubGraph(this IGraphAssignable assignable) {
            if ( assignable.currentInstance != null ) {
                assignable.currentInstance.Stop();
                return true;
            }
            return false;
        }

        ///Pause subgraph if currentInstance exists
        public static bool TryPauseSubGraph(this IGraphAssignable assignable) {
            if ( assignable.currentInstance != null ) {
                assignable.currentInstance.Pause();
                return true;
            }
            return false;
        }

        ///Resume subgraph if currentInstance exists
        public static bool TryResumeSubGraph(this IGraphAssignable assignable) {
            if ( assignable.currentInstance != null ) {
                assignable.currentInstance.Resume();
                return true;
            }
            return false;
        }

        ///Update subgraph if currentInstance exists
        public static bool TryUpdateSubGraph(this IGraphAssignable assignable) {
            if ( assignable.currentInstance != null ) {
                if ( assignable.currentInstance.isRunning ) {
                    assignable.currentInstance.UpdateGraph(assignable.graph.deltaTime);
                    return true;
                }
            }
            return false;
        }

        ///Write mapped variables to subgraph (write in)
        public static void TryWriteMappedVariables(this IGraphAssignable assignable) {
            if ( !assignable.currentInstance.allowBlackboardOverrides || assignable.variablesMap == null ) { return; }
            for ( var i = 0; i < assignable.variablesMap.Count; i++ ) {
                var bbParam = assignable.variablesMap[i];
                if ( !bbParam.isNone && bbParam.canWrite ) {
                    var targetSubVariable = assignable.currentInstance.blackboard.GetVariableByID(bbParam.targetSubGraphVariableID);
                    if ( targetSubVariable != null && targetSubVariable.isExposedPublic && !targetSubVariable.isPropertyBound ) {
                        //we could also use onValueChange to make this more dynamic, but is it desirable?
                        targetSubVariable.value = bbParam.value;
                    }
                }
            }
        }

        ///Read mapped variables from subgraph (read out)
        public static void TryReadMappedVariables(this IGraphAssignable assignable) {
            if ( !assignable.currentInstance.allowBlackboardOverrides || assignable.variablesMap == null ) { return; }
            for ( var i = 0; i < assignable.variablesMap.Count; i++ ) {
                var bbParam = assignable.variablesMap[i];
                if ( !bbParam.isNone && bbParam.canRead ) {
                    var targetSubVariable = assignable.currentInstance.blackboard.GetVariableByID(bbParam.targetSubGraphVariableID);
                    if ( targetSubVariable != null && targetSubVariable.isExposedPublic && !targetSubVariable.isPropertyBound ) {
                        //we could also use onValueChange to make this more dynamic, but is it desirable?
                        bbParam.value = targetSubVariable.value;
                    }
                }
            }
        }

        ///----------------------------------------------------------------------------------------------

        ///Validate the variables mapping
        public static void ValidateSubGraphAndParameters(this IGraphAssignable assignable) {
            if ( !ParadoxNotion.Services.Threader.applicationIsPlaying ) {
                if ( assignable.subGraph == null || !assignable.subGraph.allowBlackboardOverrides || assignable.subGraph.blackboard.variables.Count == 0 ) {
                    assignable.variablesMap = null;
                }
            }
        }

        ///Link subgraph variables to parent graph variables matching name and type
        public static void AutoLinkByName(this IGraphAssignable assignable) {
            if ( assignable.subGraph == null || assignable.variablesMap == null ) { return; }
            foreach ( var bbParam in assignable.variablesMap ) {
                var thatVariable = assignable.subGraph.blackboard.GetVariableByID(bbParam.targetSubGraphVariableID);
                if ( thatVariable != null && thatVariable.isExposedPublic && !thatVariable.isPropertyBound ) {
                    var thisVariable = assignable.graph.blackboard.GetVariable(thatVariable.name, thatVariable.varType);
                    if ( thisVariable != null ) {
                        bbParam.SetType(thatVariable.varType);
                        bbParam.name = thatVariable.name;
                    }
                }
            }
        }

        ///----------------------------------------------------------------------------------------------

#if UNITY_EDITOR

        //Shows blackboard variables mapping
        public static void ShowVariablesMappingGUI(this IGraphAssignable assignable) {

            if ( assignable.subGraph == null || !assignable.subGraph.allowBlackboardOverrides ) {
                assignable.variablesMap = null;
                return;
            }

            ParadoxNotion.Design.EditorUtils.Separator();
            ParadoxNotion.Design.EditorUtils.CoolLabel("SubGraph Variables Mapping");

            var subTreeVariables = assignable.subGraph.blackboard.variables.Values;
            if ( subTreeVariables.Count == 0 || !subTreeVariables.Any(v => v.isExposedPublic) ) {
                UnityEditor.EditorGUILayout.HelpBox("SubGraph has no exposed public variables. You can make variables exposed public through the 'gear' menu of a variable.", UnityEditor.MessageType.Info);
                assignable.variablesMap = null;
                return;
            }

            UnityEditor.EditorGUILayout.HelpBox("Map SubGraph exposed variables to this graph variables.\nUse the arrow buttons on the right of each parameter to enable Write (In) and/or Read (Out) and thus override the default value. Write In takes place when the SubGraph starts.\nRead Out takes place when the SubGraph ends.", UnityEditor.MessageType.Info);

            foreach ( var variable in subTreeVariables ) {

                if ( variable is Variable<VariableSeperator> ) { continue; }
                if ( !variable.isExposedPublic || variable.isPropertyBound ) { continue; }

                if ( assignable.variablesMap == null ) {
                    assignable.variablesMap = new System.Collections.Generic.List<BBMappingParameter>();
                }

                var bbParam = assignable.variablesMap.Find(x => x.targetSubGraphVariableID == variable.ID);
                if ( bbParam == null ) {
                    GUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    EditorUtils.DrawEditorFieldDirect(new GUIContent(variable.name), variable.value, variable.varType, default(InspectedFieldInfo));
                    GUI.enabled = true;
                    int tmp = 0;
                    if ( GUILayout.Button(EditorUtils.GetTempContent("▽", null, "Write (In)"), Styles.centerLabel, GUILayout.Width(12)) ) { tmp = 1; }
                    UnityEditor.EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), UnityEditor.MouseCursor.Link);
                    if ( GUILayout.Button(EditorUtils.GetTempContent("△", null, "Read (Out)"), Styles.centerLabel, GUILayout.Width(12)) ) { tmp = -1; }
                    if ( tmp != 0 ) {
                        UndoUtility.RecordObject(assignable.graph, "Override Variable");
                        bbParam = new BBMappingParameter(variable);
                        bbParam.canWrite = tmp == 1;
                        bbParam.canRead = tmp == -1;
                        bbParam.useBlackboard = tmp == -1;
                        bbParam.value = variable.value;
                        bbParam.bb = assignable.graph.blackboard;
                        assignable.variablesMap.Add(bbParam);
                        UndoUtility.SetDirty(assignable.graph);
                    }
                    UnityEditor.EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), UnityEditor.MouseCursor.Link);
                    GUILayout.EndHorizontal();
                    continue;
                }

                if ( bbParam.varType != variable.varType && ( bbParam.canRead || bbParam.canWrite ) ) { bbParam.SetType(variable.varType); }

                GUILayout.BeginHorizontal();

                GUI.enabled = bbParam.canRead || bbParam.canWrite;
                NodeCanvas.Editor.BBParameterEditor.ParameterField(variable.name, bbParam);
                if ( bbParam.canRead && !bbParam.useBlackboard ) { EditorUtils.MarkLastFieldWarning("The parameter is set to Read Out, but is not linked to any Variable."); }
                GUI.enabled = true;

                if ( GUILayout.Button(EditorUtils.GetTempContent(bbParam.canWrite ? "▼" : "▽", null, "Write (In)"), Styles.centerLabel, GUILayout.Width(12)) ) {
                    UndoUtility.RecordObject(assignable.graph, "Set Write In");
                    bbParam.canWrite = !bbParam.canWrite;
                    UndoUtility.SetDirty(assignable.graph);
                }
                UnityEditor.EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), UnityEditor.MouseCursor.Link);
                if ( GUILayout.Button(EditorUtils.GetTempContent(bbParam.canRead ? "▲" : "△", null, "Read (Out)"), Styles.centerLabel, GUILayout.Width(12)) ) {
                    UndoUtility.RecordObject(assignable.graph, "Set Read Out");
                    bbParam.canRead = !bbParam.canRead;
                    UndoUtility.SetDirty(assignable.graph);
                }
                UnityEditor.EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), UnityEditor.MouseCursor.Link);
                if ( !bbParam.canRead && !bbParam.canWrite ) {
                    UndoUtility.RecordObject(assignable.graph, "Remove Override");
                    assignable.variablesMap.Remove(bbParam);
                    UndoUtility.SetDirty(assignable.graph);
                }

                GUILayout.EndHorizontal();
            }

            if ( assignable.variablesMap != null ) {
                for ( var i = assignable.variablesMap.Count; i-- > 0; ) {
                    var bbParam = assignable.variablesMap[i];
                    var variable = assignable.subGraph.blackboard.GetVariableByID(bbParam.targetSubGraphVariableID);
                    if ( variable == null || !variable.isExposedPublic || variable.isPropertyBound ) {
                        assignable.variablesMap.RemoveAt(i);
                        UndoUtility.SetDirty(assignable.graph);
                    }
                }
            }
        }
#endif

        ///----------------------------------------------------------------------------------------------

    }
}