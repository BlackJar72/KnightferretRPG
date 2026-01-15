using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(AmbienceProfile))]
    public class AmbienceProfileEditor : Editor
    {


        AmbienceProfile profile;
        VisualElement root;

        public VisualElement ForecastingContainer => root.Q<VisualElement>("forecasting-container");
        public ListView FXContainer => root.Q<ListView>("fx-list");

        void OnEnable()
        {

            profile = (AmbienceProfile)target;

        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Profiles/UXML/ambience-profile-editor.uxml"
            );

            asset.CloneTree(root);

            // PropertyField chance = new PropertyField();
            // WRCField chance = 
            // chance.BindProperty(serializedObject.FindProperty("chance"));
            ForecastingContainer.Add(new WRCField(serializedObject.FindProperty("chance")));

            PropertyField minTime = new PropertyField();
            minTime.BindProperty(serializedObject.FindProperty("minTime"));
            ForecastingContainer.Add(minTime);
            PropertyField maxTime = new PropertyField();
            maxTime.BindProperty(serializedObject.FindProperty("maxTime"));
            ForecastingContainer.Add(maxTime);
            PropertyField dontPlayDuring = new PropertyField();
            dontPlayDuring.BindProperty(serializedObject.FindProperty("dontPlayDuring"));
            ForecastingContainer.Add(dontPlayDuring);

            FXContainer.BindProperty(serializedObject.FindProperty("FX"));


            return root;
        }
    }
}