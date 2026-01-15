using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(ClimateProfile))]
    public class ClimateProfileEditor : Editor
    {

        SerializedProperty temperatureOverYear;
        SerializedProperty temperatureOverDay;
        SerializedProperty temperatureFilter;
        SerializedProperty humidityOverYear;
        SerializedProperty humidityOverDay;
        SerializedProperty humidityFilter;
        ClimateProfile climateProfile;
        VisualElement root;
        public VisualElement TemperatureGraph => root.Q<VisualElement>("temperature-graph");
        public VisualElement HumidityGraph => root.Q<VisualElement>("humidity-graph");

        public VisualElement TemperatureContainer => root.Q<VisualElement>("temperature-container");
        public VisualElement HumidityContainer => root.Q<VisualElement>("humidity-container");

        public static Gradient temperatureGradient = new Gradient()
        {
            colorKeys = new GradientColorKey[5] {
            new GradientColorKey(Branding.deepRed, 1f),  
            new GradientColorKey(Branding.orange, 0.75f), 
            new GradientColorKey(Branding.green, 0.5f),  
            new GradientColorKey(Branding.blue, 0.25f),
            new GradientColorKey(Branding.deepBlue, 0f)
            }
        };

        public static Gradient humidityGradient = new Gradient()
        {
            colorKeys = new GradientColorKey[2] {
                new GradientColorKey(Branding.deepBlue, 1),
                new GradientColorKey(Branding.green, 0)
            }
        };

        void OnEnable()
        {

            temperatureOverYear = serializedObject.FindProperty("temperatureOverYear");
            temperatureOverDay = serializedObject.FindProperty("temperatureOverDay");
            temperatureFilter = serializedObject.FindProperty("temperatureFilter");
            humidityOverYear = serializedObject.FindProperty("humidityOverYear");
            humidityOverDay = serializedObject.FindProperty("humidityOverDay");
            humidityFilter = serializedObject.FindProperty("humidityFilter");
            climateProfile = (ClimateProfile)target;

        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Profiles/UXML/climate-profile-editor.uxml"
            );

            asset.CloneTree(root);

            TemperatureGraph.Add(DrawTemperatureGraph());
            HumidityGraph.Add(DrawHumidityGraph());

            PropertyField temperatureOverYearField = new PropertyField();
            temperatureOverYearField.BindProperty(temperatureOverYear);
            TemperatureContainer.Add(temperatureOverYearField);

            PropertyField temperatureOverDayField = new PropertyField();
            temperatureOverDayField.BindProperty(temperatureOverDay);
            TemperatureContainer.Add(temperatureOverDayField);

            PropertyField temperatureFilterField = new PropertyField();
            temperatureFilterField.BindProperty(temperatureFilter);
            TemperatureContainer.Add(temperatureFilterField);

            TemperatureContainer.RegisterCallback((PointerMoveEvent evt) =>
            {
                TemperatureGraph.Clear();
                TemperatureGraph.Add(DrawTemperatureGraph());
            });


            PropertyField humidityOverYearField = new PropertyField();
            humidityOverYearField.BindProperty(humidityOverYear);
            HumidityContainer.Add(humidityOverYearField);

            PropertyField humidityOverDayField = new PropertyField();
            humidityOverDayField.BindProperty(humidityOverDay);
            HumidityContainer.Add(humidityOverDayField);

            PropertyField humidityFilterField = new PropertyField();
            humidityFilterField.BindProperty(humidityFilter);
            HumidityContainer.Add(humidityFilterField);

            HumidityContainer.RegisterCallback((PointerMoveEvent evt) =>
            {
                HumidityGraph.Clear();
                HumidityGraph.Add(DrawHumidityGraph());
            });


            return root;
        }

        public VisualElement DrawHumidityGraph()
        {
            VisualElement element = new VisualElement();
            element.AddToClassList("graph-section");

            element.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = element.contentRect.width;
                float height = element.contentRect.height;

                var painter = context.painter2D;

                painter.lineWidth = 1;
                painter.strokeColor = new Color(1, 1, 1, 0.2f);
                painter.BeginPath();
                painter.MoveTo(new Vector2(0, height * (1 - 0.25f)));
                painter.LineTo(new Vector2(width, height * (1 - 0.25f)));
                painter.Stroke();
                painter.ClosePath();
                painter.BeginPath();
                painter.MoveTo(new Vector2(0, height * (1 - 0.75f)));
                painter.LineTo(new Vector2(width, height * (1 - 0.75f)));
                painter.Stroke();
                painter.ClosePath();
                painter.BeginPath();
                painter.MoveTo(new Vector2(0, height * (1 - 0.5f)));
                painter.LineTo(new Vector2(width, height * (1 - 0.5f)));
                painter.Stroke();
                painter.ClosePath();

                float stems = 24f;
                painter.lineWidth = 5;
                painter.lineCap = LineCap.Round;
                for (int i = 0; i < stems; i++)
                {

                    float evaluatedHumidity = climateProfile.humidityOverYear.Evaluate(i / stems) + climateProfile.humidityFilter;
                    float remappedHumidity = 0 + (evaluatedHumidity - -15) * (1 - 0) / (115 - -15);

                    painter.strokeGradient = new Gradient()
                    {
                        colorKeys = new GradientColorKey[2] {
                            new GradientColorKey(humidityGradient.Evaluate(evaluatedHumidity / 100) * 0.5f, 0f),
                            new GradientColorKey(humidityGradient.Evaluate(evaluatedHumidity / 100), 1f),
                        }
                    };
                    painter.BeginPath();
                    painter.MoveTo(new Vector2(width * i / stems + width / stems / 2, height));
                    painter.LineTo(new Vector2(width * i / stems + width / stems / 2, (1 - Mathf.Clamp(remappedHumidity, 0.1f, 1)) * height));
                    painter.Stroke();
                    painter.ClosePath();
                }


                painter.strokeColor = new Color(1, 1, 1, 0.5f);
                painter.lineWidth = 2;
                painter.BeginPath();
                painter.MoveTo(new Vector2(width * CozyWeather.instance.yearPercentage, height));
                painter.LineTo(new Vector2(width * CozyWeather.instance.yearPercentage, 0));
                painter.Stroke();
                painter.ClosePath();

            };

            return element;
        }

        public VisualElement DrawTemperatureGraph()
        {
            VisualElement element = new VisualElement();
            element.AddToClassList("graph-section");

            element.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = element.contentRect.width;
                float height = element.contentRect.height;

                var painter = context.painter2D;

                painter.lineWidth = 1;
                painter.strokeColor = new Color(1, 1, 1, 0.2f);
                painter.BeginPath();
                painter.MoveTo(new Vector2(0, height * (1 - 0.32f)));
                painter.LineTo(new Vector2(width, height * (1 - 0.32f)));
                painter.Stroke();
                painter.ClosePath();
                painter.BeginPath();
                painter.MoveTo(new Vector2(0, height * (1 - 0.75f)));
                painter.LineTo(new Vector2(width, height * (1 - 0.75f)));
                painter.Stroke();
                painter.ClosePath();


                float stems = 24f;
                painter.lineWidth = 5;
                painter.lineCap = LineCap.Round;
                for (int i = 0; i < stems; i++)
                {

                    float evaluatedTemp = climateProfile.temperatureOverYear.Evaluate(i / stems) + climateProfile.temperatureFilter;
                    float remappedTemp = 0 + (evaluatedTemp - -30) * (1 - 0) / (130 - -30);

                    painter.strokeGradient = new Gradient()
                    {
                        colorKeys = new GradientColorKey[2] {
                            new GradientColorKey(temperatureGradient.Evaluate(evaluatedTemp / 100) * 0.5f, 0f),
                            new GradientColorKey(temperatureGradient.Evaluate(evaluatedTemp / 100), 1f),
                        }
                    };
                    painter.BeginPath();
                    painter.MoveTo(new Vector2(width * i / stems + width / stems / 2, height));
                    painter.LineTo(new Vector2(width * i / stems + width / stems / 2, (1 - Mathf.Clamp(remappedTemp, 0.1f, 1)) * height));
                    painter.Stroke();
                    painter.ClosePath();
                }

                painter.strokeColor = new Color(1, 1, 1, 0.5f);
                painter.lineWidth = 2;
                painter.BeginPath();
                painter.MoveTo(new Vector2(width * CozyWeather.instance.yearPercentage, height));
                painter.LineTo(new Vector2(width * CozyWeather.instance.yearPercentage, 0));
                painter.Stroke();
                painter.ClosePath();

            };

            return element;
        }


    }
}