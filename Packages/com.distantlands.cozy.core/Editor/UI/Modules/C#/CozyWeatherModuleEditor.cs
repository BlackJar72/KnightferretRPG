using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using DistantLands.Cozy.Data;
using System.Collections.Generic;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyWeatherModule))]
    public class CozyWeatherModuleEditor : CozyBiomeModuleEditor
    {

        CozyWeatherModule weatherModule;
        public override ModuleCategory Category => ModuleCategory.ecosystem;
        public override string ModuleTitle => "Weather";
        public override string ModuleSubtitle => "Forecast Module";
        public override string ModuleTooltip => "Manage weather, forecast and playback options.";

        public VisualElement SelectionContainer => root.Q<VisualElement>("selection-container");
        public VisualElement DynamicSettings => root.Q<VisualElement>("dynamic-settings");
        public VisualElement SettingsContainer => root.Q<VisualElement>("settings-container");

        static Gradient WeatherKeyColors = new Gradient()
        {
            colorKeys = new GradientColorKey[8] {
                new GradientColorKey(Branding.deepBlue,0f/7f),
                new GradientColorKey(Branding.red,1f/7f),
                new GradientColorKey(Branding.yellow,2f/7f),
                new GradientColorKey(Branding.green,3f/7f),
                new GradientColorKey(Branding.purple,4f/7f),
                new GradientColorKey(Branding.blue,5f/7f),
                new GradientColorKey(Branding.orange,6f/7f),
                new GradientColorKey(Branding.deepBlue,1f)
            }
        };

        Button widget;
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            weatherModule = (CozyWeatherModule)target;
        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
            status.text = weatherModule.ecosystem.currentWeather.name;

            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/weather-module-editor.uxml"
            );

            asset.CloneTree(root);



            PropertyField weatherSelectionMode = new PropertyField();
            weatherSelectionMode.BindProperty(serializedObject.FindProperty("ecosystem").FindPropertyRelative("weatherSelectionMode"));
            weatherSelectionMode.RegisterValueChangeCallback(evt =>
            {
                GetCurrentSettings();
            });
            SelectionContainer.Insert(0, weatherSelectionMode);


            GetCurrentSettings();

            return root;

        }

        public override VisualElement DisplayBiomeUI()
        {
            root = new VisualElement();

            PropertyField weatherSelectionMode = new PropertyField();
            weatherSelectionMode.BindProperty(serializedObject.FindProperty("ecosystem").FindPropertyRelative("weatherSelectionMode"));
            weatherSelectionMode.RegisterValueChangeCallback((evt) =>
            {
                GetBiomeSettings();
            });
            root.Add(weatherSelectionMode);

            VisualElement dynamicSettings = new VisualElement();
            dynamicSettings.name = "dynamic-settings";
            root.Add(dynamicSettings);


            VisualElement settingsContainer = new VisualElement();
            settingsContainer.name = "settings-container";
            root.Add(settingsContainer);
            
            GetCurrentSettings();

            return root;
        }

        public void GetCurrentSettings()
        {
            DynamicSettings.Clear();
            switch ((CozyEcosystem.EcosystemStyle)serializedObject.FindProperty("ecosystem").FindPropertyRelative("weatherSelectionMode").enumValueIndex)
            {
                case CozyEcosystem.EcosystemStyle.automatic:
                    PropertyField currentWeather = new PropertyField();
                    currentWeather.BindProperty(serializedObject.FindProperty("ecosystem").FindPropertyRelative("currentWeather"));
                    currentWeather.RegisterValueChangeCallback(evt =>
                    {
                        RenderSingleWeatherInspector();
                    });
                    DynamicSettings.Add(currentWeather);
                    RenderSingleWeatherInspector();
                    break;
                case CozyEcosystem.EcosystemStyle.manual:
                    SettingsContainer.Clear();
                    PropertyField weightedWeatherProfiles = new PropertyField();
                    weightedWeatherProfiles.BindProperty(serializedObject.FindProperty("ecosystem").FindPropertyRelative("weightedWeatherProfiles"));
                    DynamicSettings.Add(weightedWeatherProfiles);
                    break;
                default:
                    PropertyField previewWeather = new PropertyField();
                    previewWeather.label = "Preview Weather";
                    previewWeather.BindProperty(serializedObject.FindProperty("ecosystem").FindPropertyRelative("currentWeather"));
                    DynamicSettings.Add(previewWeather);
                    PropertyField forecastProfile = new PropertyField();
                    forecastProfile.BindProperty(serializedObject.FindProperty("ecosystem").FindPropertyRelative("forecastProfile"));
                    forecastProfile.RegisterValueChangeCallback(evt =>
                    {
                        RenderForecastInspector();
                    });

                    DynamicSettings.Add(forecastProfile);

                    RenderForecastInspector();
                    break;
            }
        }

        public void GetBiomeSettings()
        {
            DynamicSettings.Clear();
            switch ((CozyEcosystem.EcosystemStyle)serializedObject.FindProperty("ecosystem").FindPropertyRelative("weatherSelectionMode").enumValueIndex)
            {
                case CozyEcosystem.EcosystemStyle.automatic:
                    PropertyField currentWeather = new PropertyField();
                    currentWeather.BindProperty(serializedObject.FindProperty("ecosystem").FindPropertyRelative("currentWeather"));
                    DynamicSettings.Add(currentWeather);
                    break;
                case CozyEcosystem.EcosystemStyle.manual:
                    PropertyField weightedWeatherProfiles = new PropertyField();
                    weightedWeatherProfiles.BindProperty(serializedObject.FindProperty("ecosystem").FindPropertyRelative("weightedWeatherProfiles"));
                    DynamicSettings.Add(weightedWeatherProfiles);
                    break;
                default:
                    PropertyField forecastProfile = new PropertyField();
                    forecastProfile.BindProperty(serializedObject.FindProperty("ecosystem").FindPropertyRelative("forecastProfile"));
                    DynamicSettings.Add(forecastProfile);
                    break;
            }
        }

        public void RenderForecastInspector()
        {
            SettingsContainer.Clear();
            InspectorElement inspector = new InspectorElement(weatherModule.ecosystem.forecastProfile);
            inspector.AddToClassList("p-0");
            SettingsContainer.Add(inspector);
        }

        public void RenderSingleWeatherInspector()
        {
            SettingsContainer.Clear();
            InspectorElement inspector = new InspectorElement(weatherModule.ecosystem.currentWeather);
            inspector.AddToClassList("p-0");
            SettingsContainer.Add(inspector);
        }




        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/weather-module");
        }


    }
}