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
        
        private void OnEnable()
        {
            _gameProfileInfoProperty = serializedObject.FindProperty("_gameProfileInfo");
            _targetFrameRateProperty = serializedObject.FindProperty("_targetFrameRate");
            _timeScaleProperty = serializedObject.FindProperty("_timeScale");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_targetFrameRateProperty);
            EditorGUILayout.PropertyField(_timeScaleProperty);
            DrawGameProfiles();
        }

        private void DrawGameProfiles()
        {
            InspectorTypeSelector.DrawTypeSelector("Game Profile", serializedObject, _gameProfileInfoProperty, typeof(IGameProfileInfo));
        }
    }
}