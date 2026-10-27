using System;
using System.Collections.Generic;
using System.Linq;
using Dories.Fsm.Runtime.Procedure;
using Dories.Logging;
using GameFramework.Editor.Misc;
using GameFramework.Runtime.GameBase.GameProfile;
using UnityEditor;

namespace GameFramework.Editor.CustomInspector
{
    [CustomEditor(typeof(GameFramework.Runtime.GameBase.GameEntry))]
    public class GameEntryInspector : UnityEditor.Editor
    {
        private SerializedProperty _targetFrameRateProperty;
        private SerializedProperty _timeScaleProperty;
        private SerializedProperty _gameProfileInfoProperty;
        private SerializedProperty _logSinkProperty;
        private SerializedProperty _proceduresProperty;
        private SerializedProperty _entranceProcedureProperty;
        private List<Type> _procedureTypes;
        
        private void OnEnable()
        {
            _gameProfileInfoProperty = serializedObject.FindProperty("_gameProfileInfo");
            _logSinkProperty = serializedObject.FindProperty("_logSink");
            _targetFrameRateProperty = serializedObject.FindProperty("_targetFrameRate");
            _timeScaleProperty = serializedObject.FindProperty("_timeScale");
            _proceduresProperty = serializedObject.FindProperty("_procedures");
            _entranceProcedureProperty = serializedObject.FindProperty("_entranceProcedure");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_targetFrameRateProperty);
            EditorGUILayout.PropertyField(_timeScaleProperty);
            DrawGameProfiles();
            DrawLogSink();
            DrawProcedures();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGameProfiles()
        {
            InspectorTypeSelector.DrawTypeSelector("Game Profile", serializedObject, _gameProfileInfoProperty, typeof(IGameProfileInfo));
        }

        private void DrawLogSink()
        {
            // Log Sink 可选：没有自定义实现时不画，避免选择器报错
            if (InspectorTypeSelector.GetTypesByScanConfig(typeof(ILogSink)).Count > 0)
                InspectorTypeSelector.DrawTypeSelector("Log Sink", serializedObject, _logSinkProperty, typeof(ILogSink));
        }

        private void DrawProcedures()
        {
            _procedureTypes ??= InspectorTypeSelector.GetTypesByScanConfig(typeof(ProcedureBase))
                .OrderBy(t => t.Name)
                .ToList();

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Procedures", EditorStyles.boldLabel);

            var registered = new List<string>();
            for (int i = 0; i < _proceduresProperty.arraySize; i++)
                registered.Add(_proceduresProperty.GetArrayElementAtIndex(i).stringValue);

            bool changed = false;
            foreach (var type in _procedureTypes)
            {
                bool on = registered.Contains(type.FullName);
                if (EditorGUILayout.ToggleLeft(type.Name, on) != on)
                {
                    if (on)
                        registered.Remove(type.FullName);
                    else
                        registered.Add(type.FullName);
                    changed = true;
                }
            }

            // 类型被删或改名后的残留项
            int stale = registered.RemoveAll(n => _procedureTypes.All(t => t.FullName != n));
            if (stale > 0)
                changed = true;

            if (changed)
            {
                _proceduresProperty.arraySize = registered.Count;
                for (int i = 0; i < registered.Count; i++)
                    _proceduresProperty.GetArrayElementAtIndex(i).stringValue = registered[i];
            }

            if (registered.Count == 0)
            {
                EditorGUILayout.HelpBox("未勾选任何流程，启动时不会进入流程。", MessageType.Info);
                return;
            }

            DrawEntrancePopup(registered);
        }

        private void DrawEntrancePopup(List<string> registered)
        {
            var options = registered.Select(n => n.Substring(n.LastIndexOf('.') + 1)).ToArray();
            int current = registered.IndexOf(_entranceProcedureProperty.stringValue);
            if (current < 0)
            {
                EditorGUILayout.HelpBox("请选择入口流程。", MessageType.Warning);
                _entranceProcedureProperty.stringValue = string.Empty;
            }

            int selected = EditorGUILayout.Popup("First Procedure", current, options);
            if (selected != current && selected >= 0)
                _entranceProcedureProperty.stringValue = registered[selected];
        }
    }
}