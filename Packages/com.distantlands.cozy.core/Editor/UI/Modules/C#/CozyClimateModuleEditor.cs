using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyClimateModule))]
    public class CozyClimateModuleEditor : CozyBiomeModuleEditor
    {

        CozyClimateModule climateModule;
        public override ModuleCategory Category => ModuleCategory.ecosystem;
        public override string ModuleTitle => "Climate";
        public override string ModuleSubtitle => "Ecosystem Control Module";
        public override string ModuleTooltip => "Control temperature and humidity.";

        public VisualElement ProfileContainer => root.Q<VisualElement>("profile-container");
        public VisualElement PrecipitationContainer => root.Q<VisualElement>("precipitation-container");
        public VisualElement CurrentTemperatureWidget => root.Q<VisualElement>("current-temperature-widget");
        public VisualElement CurrentHumidityWidget => root.Q<VisualElement>("current-humidity-widget");

        VisualElement root;



        void OnEnable()
        {
            if (target)
                climateModule = (CozyClimateModule)target;

        }

        public override Button DisplayWidget()
        {
            Button widget = LargeWidget();

            string fuzzyTemp = "Mild";
            string fuzzyHumidity = "";

            if (climateModule.currentTemperature < 25)
                fuzzyTemp = "Cold";
            else if (climateModule.currentTemperature < 50)
                fuzzyTemp = "Cool";
            else if (climateModule.currentTemperature > 100)
                fuzzyTemp = "Hot";
            else if (climateModule.currentTemperature > 75)
                fuzzyTemp = "Warm";

            if (climateModule.currentPrecipitation < 30)
                fuzzyHumidity = "and dry";
            else if (climateModule.currentPrecipitation > 70)
                fuzzyHumidity = "and wet";

            widget.Bind(serializedObject);

            widget.Q<Label>("dynamic-status").text = $"{fuzzyTemp} {fuzzyHumidity}";
            widget.Q<VisualElement>("lower-container").Add(new Label()
            {
                text = $"Temperature: {Mathf.Round(serializedObject.FindProperty("currentTemperature").floatValue)}°F"
            });
            widget.Q<VisualElement>("lower-container").Add(new Label()
            {
                text = $"Humidity: {Mathf.Round(climateModule.currentPrecipitation)}%"
            });
            widget.Q<VisualElement>("lower-container").Add(new Label()
            {
                text = climateModule.currentTemperature <= 32 ? $"Snow Amount: {Mathf.Round(climateModule.snowAmount * 10) / 10}" : $"Wetness: {Mathf.Round(climateModule.groundwaterAmount * 10) / 10}"
            });

            return widget;

        }

        public override VisualElement DisplayBiomeUI()
        {

            root = new VisualElement();

            CozyProfileField<ClimateProfile> profile = new CozyProfileField<ClimateProfile>(serializedObject.FindProperty("climateProfile"));
            root.Add(profile);

            return root;
        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/climate-module-editor.uxml"
            );

            asset.CloneTree(root);

            UpdateTemperatureWidget();
            UpdateHumidityWidget();

            PropertyField controlMethodElement = new PropertyField();
            controlMethodElement.BindProperty(serializedObject.FindProperty("controlMethod"));
            ProfileContainer.Add(controlMethodElement);

            CozyProfileField<ClimateProfile> profile = new CozyProfileField<ClimateProfile>(serializedObject.FindProperty("climateProfile"));
            profile.style.display = serializedObject.FindProperty("controlMethod").intValue == 0 ? DisplayStyle.None : DisplayStyle.Flex;

            VisualElement nativeSettingsContainer = new VisualElement();
            nativeSettingsContainer.style.display = serializedObject.FindProperty("controlMethod").intValue == 0 ? DisplayStyle.Flex : DisplayStyle.None;

            PropertyField currentTemperatureElement = new PropertyField();
            currentTemperatureElement.BindProperty(serializedObject.FindProperty("currentTemperature"));
            currentTemperatureElement.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                UpdateTemperatureWidget();
            });
            nativeSettingsContainer.Add(currentTemperatureElement);

            PropertyField currentHumidityElement = new PropertyField
            {
                label = "Current Humidity"
            };
            currentHumidityElement.BindProperty(serializedObject.FindProperty("currentPrecipitation"));
            currentHumidityElement.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) =>
            {
                UpdateHumidityWidget();
            });
            nativeSettingsContainer.Add(currentHumidityElement);

            ProfileContainer.Add(profile);
            ProfileContainer.Add(nativeSettingsContainer);


            PropertyField snowAmountElement = new PropertyField();
            snowAmountElement.BindProperty(serializedObject.FindProperty("snowAmount"));
            PrecipitationContainer.Add(snowAmountElement);

            PropertyField snowMeltElement = new PropertyField();
            snowMeltElement.BindProperty(serializedObject.FindProperty("snowMeltSpeed"));
            PrecipitationContainer.Add(snowMeltElement);

            PropertyField groundwaterAmountElement = new PropertyField();
            groundwaterAmountElement.BindProperty(serializedObject.FindProperty("groundwaterAmount"));
            PrecipitationContainer.Add(groundwaterAmountElement);

            PropertyField dryingSpeedElement = new PropertyField();
            dryingSpeedElement.BindProperty(serializedObject.FindProperty("dryingSpeed"));
            PrecipitationContainer.Add(dryingSpeedElement);


            InspectorElement inspector = new InspectorElement(climateModule.climateProfile);
            inspector.style.display = serializedObject.FindProperty("controlMethod").intValue == 0 ? DisplayStyle.None : DisplayStyle.Flex;
            inspector.AddToClassList("p-0");
            root.Add(inspector);
            inspector.RegisterCallback<PointerMoveEvent>((PointerMoveEvent) =>
            {

            });

            controlMethodElement.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> evt) =>
            {
                inspector.style.display = serializedObject.FindProperty("controlMethod").intValue == 0 ? DisplayStyle.None : DisplayStyle.Flex;
                profile.style.display = serializedObject.FindProperty("controlMethod").intValue == 0 ? DisplayStyle.None : DisplayStyle.Flex;
                nativeSettingsContainer.style.display = serializedObject.FindProperty("controlMethod").intValue == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            });

            return root;

        }

        public void UpdateTemperatureWidget()
        {
            CurrentTemperatureWidget.Q<Label>("value").text = $"Temperature: {Mathf.Round(climateModule.currentTemperature)}°F";
            CurrentTemperatureWidget.Q<VisualElement>("graph").Clear();
            CurrentTemperatureWidget.Q<VisualElement>("graph").Add(DrawLineGraph(ClimateProfileEditor.temperatureGradient, Mathf.Clamp01(climateModule.currentTemperature / 100)));
        }
        public void UpdateHumidityWidget()
        {
            CurrentHumidityWidget.Q<Label>("value").text = $"Humidity: {Mathf.Round(climateModule.currentPrecipitation)}%";
            CurrentHumidityWidget.Q<VisualElement>("graph").Clear();
            CurrentHumidityWidget.Q<VisualElement>("graph").Add(DrawLineGraph(ClimateProfileEditor.humidityGradient, Mathf.Clamp01(climateModule.currentPrecipitation / 100)));
        }

        public VisualElement DrawLineGraph(Gradient gradient, float currentValue)
        {
            VisualElement element = new VisualElement();
            element.AddToClassList("graph-section");

            element.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = element.contentRect.width;
                float height = element.contentRect.height;

                var painter = context.painter2D;

                painter.lineWidth = 8;
                painter.strokeGradient = gradient;
                painter.lineCap = LineCap.Round;
                painter.BeginPath();
                painter.MoveTo(new Vector2(0, height * 0.5f));
                painter.LineTo(new Vector2(width, height * 0.5f));
                painter.Stroke();
                painter.ClosePath();

                painter.strokeColor = Color.white;
                painter.BeginPath();
                painter.Arc(new Vector2(width * currentValue, height * 0.5f), 1, 0, 360, ArcDirection.Clockwise);
                painter.Stroke();
                painter.ClosePath();

            };

            return element;
        }
    }
}