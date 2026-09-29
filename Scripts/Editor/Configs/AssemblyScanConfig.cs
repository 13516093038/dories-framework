using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor.Configs
{
    [CreateAssetMenu(fileName = "AssemblyScanConfig", menuName = "Dories/AssemblyScanConfig")]
    public class AssemblyScanConfig : ScriptableObject
    {
        private const string DefaultAssetPath = "Assets/AssemblyScanConfig.asset";

        [SerializeField] private string[] _assemblyNames = { "Assembly-CSharp" };

        public string[] AssemblyNames => _assemblyNames;

        private static AssemblyScanConfig _instance;

        public static AssemblyScanConfig GetConfig()
        {
            if (_instance != null)
                return _instance;
            
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(AssemblyScanConfig)}");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _instance = AssetDatabase.LoadAssetAtPath<AssemblyScanConfig>(path);
                if (_instance != null)
                    return _instance;
            }
            _instance = CreateInstance<AssemblyScanConfig>();
            AssetDatabase.CreateAsset(_instance, DefaultAssetPath);
            AssetDatabase.SaveAssets();
            return _instance;
        }
    }
}