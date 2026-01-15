using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(WeatherProfile))]
    public class WeatherProfileEditor : Editor
    {


        WeatherProfile profile;
        VisualElement root;

        public VisualElement ForecastingContainer => root.Q<VisualElement>("forecasting-container");
        public ListView FXContainer => root.Q<ListView>("fx-list");

        void OnEnable()
        {

            profile = (WeatherProfile)target;

        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Profiles/UXML/weather-profile-editor.uxml"
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

            PropertyField forecastNext = new PropertyField();
            forecastNext.BindProperty(serializedObject.FindProperty("forecastNext"));

            PropertyField forecastModifierMethod = new PropertyField();
            forecastModifierMethod.BindProperty(serializedObject.FindProperty("forecastModifierMethod"));
            forecastModifierMethod.RegisterValueChangeCallback((evt) =>
            {
                switch ((WeatherProfile.ForecastModifierMethod)serializedObject.FindProperty("forecastModifierMethod").intValue)
                {
                    case WeatherProfile.ForecastModifierMethod.forecastNext:
                        forecastNext.style.display = DisplayStyle.Flex;
                        forecastNext.label = "Forecast Next";
                        break;
                    case WeatherProfile.ForecastModifierMethod.DontForecastNext:
                        forecastNext.style.display = DisplayStyle.Flex;
                        forecastNext.label = "Do Not Forecast Next";
                        break;
                    case WeatherProfile.ForecastModifierMethod.forecastAnyProfileNext:
                        forecastNext.style.display = DisplayStyle.None;
                        break;
                }
            });
            ForecastingContainer.Add(forecastModifierMethod);

            ForecastingContainer.Add(forecastNext);

            FXContainer.BindProperty(serializedObject.FindProperty("FX"));


            return root;
        }
    }
}