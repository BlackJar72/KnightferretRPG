using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(SatelliteProfile))]
    public class SatelliteProfileEditor : Editor
    {

        SatelliteProfile satelliteProfile;
        VisualElement root;
        public VisualElement DayLinkedContainer => root.Q<VisualElement>("day-linked-container");
        public VisualElement DayUnlinkedContainer => root.Q<VisualElement>("day-unlinked-container");
        public Toggle LinkToDay => root.Q<Toggle>("link-to-day");

        void OnEnable()
        {

            satelliteProfile = (SatelliteProfile)target;

        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Profiles/UXML/satellite-profile-editor.uxml"
            );

            asset.CloneTree(root);
            LinkToDay.RegisterValueChangedCallback((evt) => { ToggleContainers(serializedObject.FindProperty("linkToDay").boolValue); });
            ToggleContainers(serializedObject.FindProperty("linkToDay").boolValue);

            return root;
        }

        public void ToggleContainers(bool link)
        {
            DayLinkedContainer.style.display = link ? DisplayStyle.Flex : DisplayStyle.None;
            DayUnlinkedContainer.style.display = link ? DisplayStyle.None : DisplayStyle.Flex;
        }


    }
}