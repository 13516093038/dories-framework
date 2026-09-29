using GameFramework.Editor.Misc;
using GameFramework.Runtime.UI;
using UnityEditor;

namespace GameFramework.Editor.CustomInspector
{
    [CustomEditor(typeof(GameFramework.Runtime.UI.UISystem))]
    public class UISystemInspector : UnityEditor.Editor
    {
        private SerializedProperty _autoInitProperty;
        private SerializedProperty _autoReleaseTimeProperty;
        private SerializedProperty _resourceProviderNameProperty;
        private SerializedProperty _panelAddressResolverNameProperty;
        private SerializedProperty _panelAddressNameProperty;

        private void OnEnable()
        {
            _autoInitProperty = serializedObject.FindProperty("_autoInit");
            _autoReleaseTimeProperty = serializedObject.FindProperty("_autoReleaseTime");
            _resourceProviderNameProperty = serializedObject.FindProperty("_resourceProviderName");
            _panelAddressResolverNameProperty = serializedObject.FindProperty("_panelAddressResolverName");
            _panelAddressNameProperty = serializedObject.FindProperty("_panelAddressName");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_autoInitProperty);
            EditorGUILayout.PropertyField(_autoReleaseTimeProperty);
            DrawResourceProvider();
            DrawPanelAddressResolver();
            DrawPanelAddress();
        }

        private void DrawResourceProvider()
        {
            InspectorTypeSelector.DrawTypeSelector("Resource Provider", serializedObject, _resourceProviderNameProperty, typeof(IResourceProvider));
        }

        private void DrawPanelAddressResolver()
        {
            InspectorTypeSelector.DrawTypeSelector("Panel Address Resolver", serializedObject, _panelAddressResolverNameProperty, typeof(IPanelAddressResolver));
        }

        private void DrawPanelAddress()
        {
            InspectorTypeSelector.DrawTypeSelector("Panel Address", serializedObject, _panelAddressNameProperty, typeof(IPanelAddress));
        }
    }
}