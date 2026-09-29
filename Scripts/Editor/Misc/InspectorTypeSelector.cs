using System;
using System.Collections.Generic;
using System.Reflection;
using GameFramework.Editor.Configs;
using UnityEditor;

namespace GameFramework.Editor.Misc
{
    public static class InspectorTypeSelector
    {
        public static void DrawTypeSelector(string title,SerializedObject serializedObject, SerializedProperty property, Type type)
        {
            List<Type> types = GetTypesByScanConfig(type);
            if (types.Count == 0)
            {
                EditorGUILayout.HelpBox("未找到 " + type.Name + " 的实现类。", MessageType.Error);
                property.stringValue = null;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.Popup(title, 0, new string[] { "Null" });
                EditorGUI.EndChangeCheck();
                return;
            }
            bool found = false;
            int currentIndex = 0;
            string[] options = new string[types.Count];
            for (int i = 0; i < types.Count; i++)
            {
                options[i] = types[i].Name;
                if (types[i].FullName == property.stringValue)
                {
                    currentIndex = i;
                    found = true;
                }
            }

            if (!found)
            {
                EditorGUILayout.HelpBox("请选择 " + type.Name + " 的实现类。", MessageType.Warning);
                currentIndex = -1;
                property.stringValue = null;
            }
            EditorGUI.BeginChangeCheck();
            int nextIndex = EditorGUILayout.Popup(title, currentIndex, options);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = types[nextIndex].FullName;
            }
            serializedObject.ApplyModifiedProperties();
        }

        public static List<Type> GetTypesByScanConfig(Type type)
        {
            var typeNames = AssemblyScanConfig.GetConfig().AssemblyNames;
            List<Type> gameProfiles = new List<Type>();

            for (int i = 0; i < typeNames.Length; i++)
            {
                Assembly assembly = Assembly.Load(typeNames[i]);
                Type[] types = assembly.GetTypes();

                for (int j = 0; j < types.Length; j++)
                {
                    if (type.IsAssignableFrom(types[j]) && !types[j].IsAbstract && !types[j].IsInterface)
                    {
                        gameProfiles.Add(assembly.GetTypes()[j]);
                    }
                }
            }
            return gameProfiles;
        }
    }
}