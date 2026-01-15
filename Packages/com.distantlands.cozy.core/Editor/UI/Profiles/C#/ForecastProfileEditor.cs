using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;
using System.Linq;
using System.Collections.Generic;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(ForecastProfile))]
    public class ForecastProfileEditor : Editor
    {

        ForecastProfile forecastProfile;
        VisualElement root;

        public VisualElement ChancesByVariableChart => root.Q<VisualElement>("chances-by-variable-chart");
        public VisualElement ChancesByVariableKey => root.Q<VisualElement>("chances-by-variable-key");
        public EnumField ChancesByVariableLimit => root.Q<EnumField>("chances-by-variable-limit");
        public EnumField StartWith => root.Q<EnumField>("start-with");
        public ObjectField InitialWeather => root.Q<ObjectField>("initial-weather");
        public ListView InitialForecast => root.Q<ListView>("initial-forecast");

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
        void OnEnable()
        {
            forecastProfile = (ForecastProfile)target;

        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Profiles/UXML/forecast-profile-editor.uxml"
            );

            asset.CloneTree(root);

            switch (forecastProfile.startWeatherWith)
                {
                    case ForecastProfile.StartWeatherWith.InitialForecast:
                        InitialForecast.style.display = DisplayStyle.Flex;
                        InitialWeather.style.display = DisplayStyle.None;
                        break;  
                    case ForecastProfile.StartWeatherWith.InitialProfile:
                        InitialForecast.style.display = DisplayStyle.None;
                        InitialWeather.style.display = DisplayStyle.Flex;
                        break;  
                    case ForecastProfile.StartWeatherWith.Random:
                        InitialForecast.style.display = DisplayStyle.None;
                        InitialWeather.style.display = DisplayStyle.None;
                        break;  
                }

            StartWith.RegisterCallback((ChangeEvent<string> evt) =>
            {
                switch (evt.newValue)
                {
                    case "initialForecast":
                        InitialForecast.style.display = DisplayStyle.Flex;
                        InitialWeather.style.display = DisplayStyle.None;
                        break;  
                    case "initialProfile":
                        InitialForecast.style.display = DisplayStyle.None;
                        InitialWeather.style.display = DisplayStyle.Flex;
                        break;  
                    case "random":
                        InitialForecast.style.display = DisplayStyle.None;
                        InitialWeather.style.display = DisplayStyle.None;
                        break;  
                }
            });

            RefreshChanceGraph();

            root.RegisterCallback((PointerMoveEvent evt) =>
            {
                RefreshChanceGraph();
            });

            return root;
        }

        public void RefreshChanceGraph()
        {
            ChancesByVariableKey.Clear();

            ChanceEffector.LimitType limitType = (ChanceEffector.LimitType)ChancesByVariableLimit.value;
            List<WeatherProfile> adjustedProfiles = forecastProfile.profilesToForecast
                                                        .Where(x => x.chance.HasLimit(limitType))
                                                        .ToList();




            for (int i = 0; i < adjustedProfiles.Count; i++)
            {
                WeatherProfile profile = adjustedProfiles[i];
                VisualElement container = new VisualElement();
                container.AddToClassList("swatch");
                container.RegisterCallback<ClickEvent>((ClickEvent evt) =>
                {
                    Selection.activeObject = forecastProfile.profilesToForecast.First(x => x == profile);
                });


                VisualElement swatch = new VisualElement();
                swatch.style.backgroundColor = WeatherKeyColors.Evaluate((float)i / adjustedProfiles.Count);
                container.Add(swatch);

                Label timeLabel = new Label
                {
                    text = adjustedProfiles[i].name
                };
                timeLabel.AddToClassList("font-bold");
                container.Add(timeLabel);

                ChancesByVariableKey.Add(container);

            }

            ChancesByVariableChart.Clear();

            VisualElement graphHolder = new VisualElement();
            graphHolder.AddToClassList("graph-section");

            graphHolder.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = graphHolder.contentRect.width;
                float height = graphHolder.contentRect.height;


                var painter = context.painter2D;

                painter.lineWidth = 2;
                painter.strokeColor = Branding.lightGreyAccent;
                painter.BeginPath();
                painter.MoveTo(new Vector2(0, height));
                painter.LineTo(new Vector2(width, height));
                painter.MoveTo(new Vector2(0, height));
                painter.LineTo(new Vector2(0, 0));
                painter.MoveTo(new Vector2(width / 2f, height));
                painter.LineTo(new Vector2(width / 2f, 0));
                painter.MoveTo(new Vector2(width * 3f / 4f, height));
                painter.LineTo(new Vector2(width * 3f / 4f, 0));
                painter.MoveTo(new Vector2(width * 1f / 4f, height));
                painter.LineTo(new Vector2(width * 1f / 4f, 0));
                painter.MoveTo(new Vector2(width, height));
                painter.LineTo(new Vector2(width, 0));
                painter.MoveTo(new Vector2(0, 0));
                painter.LineTo(new Vector2(width, 0));
                painter.Stroke();


                for (int i = 0; i < adjustedProfiles.Count; i++)
                {

                    WeatherProfile profile = adjustedProfiles[i];
                    painter.strokeColor = WeatherKeyColors.Evaluate((float)i / adjustedProfiles.Count);

                    int vertex = 40;

                    painter.BeginPath();
                    painter.MoveTo(new Vector2(0, height * (1 - profile.chance.GetChance(limitType, 0f))));

                    for (int j = 1; j <= vertex; j++)
                    {
                        painter.LineTo(new Vector2(width * (float)j / vertex, height * (1 - profile.chance.GetChance((ChanceEffector.LimitType)ChancesByVariableLimit.value, (float)j / vertex))));
                    }

                    painter.Stroke();
                }
            };

            ChancesByVariableChart.Add(graphHolder);

        }


    }
}