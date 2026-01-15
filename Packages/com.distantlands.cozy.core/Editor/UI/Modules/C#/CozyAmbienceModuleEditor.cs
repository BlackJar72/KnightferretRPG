using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System.Collections.Generic;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyAmbienceModule))]
    public class CozyAmbienceModuleEditor : CozyBiomeModuleEditor
    {

        CozyAmbienceModule ambienceModule;
        public override ModuleCategory Category => ModuleCategory.ecosystem;
        public override string ModuleTitle => "Ambience";
        public override string ModuleSubtitle => "Secondary Weather Module";
        public override string ModuleTooltip => "Controls a secondary weather system that runs parallel to the main system allowing for ambient noises and FX.";

        public VisualElement CurrentInfoContainer => root.Q<VisualElement>("current-information-container");
        public VisualElement DistributionMap => root.Q<VisualElement>("distribution-map");
        public VisualElement DistributionMapKey => root.Q<VisualElement>("distribution-map-key");
        public VisualElement ChancesByVariableChart => root.Q<VisualElement>("chances-by-variable-chart");
        public VisualElement ChancesByVariableKey => root.Q<VisualElement>("chances-by-variable-key");
        public EnumField ChancesByVariableLimit => root.Q<EnumField>("chances-by-variable-limit");

        public ListView AmbienceProfileList => root.Q<ListView>("ambience-profile-list");
        public VisualElement CurrentInformationContainer => root.Q<VisualElement>("current-information-container");
        Button widget;
        VisualElement root;

        public static Gradient distributionGradient = new Gradient()
        {
            colorKeys = new GradientColorKey[5]
        {
            new GradientColorKey(new Color(0.149f, 0.329f, 0.486f, 1f), 0f),   // #26547C
            new GradientColorKey(new Color(0.937f, 0.278f, 0.435f, 1f), 0.25f), // #EF476F
            new GradientColorKey(new Color(1.0f, 0.820f, 0.400f, 1f), 0.5f),   // #FFD166
            new GradientColorKey(new Color(0.024f, 0.839f, 0.627f, 1f), 0.75f), // #06D6A0
            new GradientColorKey(new Color(0.765f, 0.765f, 0.902f, 1f), 1f)    // #C3C3E6
        }
        };

        void OnEnable()
        {
            if (!target)
                return;

            ambienceModule = (CozyAmbienceModule)target;
        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
            status.style.fontSize = 8;
            if (ambienceModule.currentAmbienceProfile)
                status.text = ambienceModule.currentAmbienceProfile.name;
            else
                status.text = "No ambience playing";

            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/ambience-module-editor.uxml"
            );

            asset.CloneTree(root);

            root.RegisterCallback<PointerMoveEvent>((PointerMoveEvent) =>
            {
                UpdateWheel();
            });

            UpdateWheel();

            AmbienceProfileList.BindProperty(serializedObject.FindProperty("ambienceProfiles"));

            PropertyField currentAmbienceProfile = new PropertyField();
            currentAmbienceProfile.BindProperty(serializedObject.FindProperty("currentAmbienceProfile"));
            CurrentInfoContainer.Add(currentAmbienceProfile);

            return root;

        }

        public override VisualElement DisplayBiomeUI()
        {
            root = new VisualElement();
            PropertyField currentAmbienceProfile = new PropertyField();
            currentAmbienceProfile.BindProperty(serializedObject.FindProperty("currentAmbienceProfile"));
            root.Add(currentAmbienceProfile);

            return root;
        }

        public void RefreshChanceGraph()
        {
            ChancesByVariableKey.Clear();

            ChanceEffector.LimitType limitType = (ChanceEffector.LimitType)ChancesByVariableLimit.value;
            List<AmbienceProfile> adjustedProfiles = ambienceModule.ambienceProfiles
                                                        .Where(x => x.chance.HasLimit(limitType))
                                                        .ToList();




            for (int i = 0; i < adjustedProfiles.Count; i++)
            {
                AmbienceProfile profile = adjustedProfiles[i];
                VisualElement container = new VisualElement();
                container.AddToClassList("swatch");
                container.RegisterCallback<ClickEvent>((ClickEvent evt) =>
                {
                    Selection.activeObject = ambienceModule.ambienceProfiles.First(x => x == profile);
                });


                VisualElement swatch = new VisualElement();
                swatch.style.backgroundColor = distributionGradient.Evaluate((float)i / adjustedProfiles.Count);
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

                    AmbienceProfile profile = adjustedProfiles[i];
                    painter.strokeColor = distributionGradient.Evaluate((float)i / adjustedProfiles.Count);

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

        public void UpdateWheel()
        {

            DistributionMap.Clear();
            DistributionMapKey.Clear();

            CozyWeather cozyWeather = CozyWeather.instance;
            List<AmbienceProfile> sortedProfiles = ambienceModule.ambienceProfiles.ToList()
                .Where(x => x.GetChance(cozyWeather) > 0.05f)
                .OrderBy(x => x.GetChance(cozyWeather)).ToList();

            float totalChance = sortedProfiles.Sum(x => x.GetChance(cozyWeather));
            float iteratedChance = 0;

            if (sortedProfiles.Count == 0)
            {
                return;
            }

            VisualElement element = new VisualElement();
            element.AddToClassList("graph-section");

            element.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = element.contentRect.width;
                float height = element.contentRect.height;

                var painter = context.painter2D;

                for (int i = 0; i < sortedProfiles.Count; i++)
                {

                    Color distributionColor = distributionGradient.Evaluate((float)i / sortedProfiles.Count);
                    float chance = sortedProfiles[i].GetChance(cozyWeather);

                    painter.strokeColor = distributionColor;
                    painter.lineWidth = 24;
                    painter.BeginPath();
                    painter.Arc(new Vector2(width / 2f, height / 2f), height / 3f, 360f * (iteratedChance / totalChance) + 1, 360 * (iteratedChance + chance) / totalChance, ArcDirection.Clockwise);
                    painter.Stroke();
                    painter.ClosePath();

                    if (sortedProfiles[i] == ambienceModule.currentAmbienceProfile)
                    {
                        painter.BeginPath();
                        painter.lineWidth = 2;
                        painter.strokeColor = new Color(1, 1, 1, 0.6f);
                        painter.Arc(new Vector2(width / 2f, height / 2f), height / 3f + 17, 360f * (iteratedChance / totalChance), 360 * (iteratedChance + chance) / totalChance, ArcDirection.Clockwise);
                        painter.Stroke();
                        painter.ClosePath();
                    }

                    iteratedChance += chance;

                }
            };


            for (int i = 0; i < sortedProfiles.Count; i++)
            {
                Color distributionColor = distributionGradient.Evaluate((float)i / sortedProfiles.Count);
                AmbienceProfile profile = sortedProfiles[i];
                float chance = profile.GetChance(cozyWeather);

                VisualElement key = new VisualElement();
                key.AddToClassList("swatch");
                key.RegisterCallback<ClickEvent>((ClickEvent evt) =>
                {
                    Selection.activeObject = ambienceModule.ambienceProfiles.First(x => x == profile);
                });

                VisualElement swatch = new VisualElement();
                swatch.style.backgroundColor = distributionColor;
                key.Add(swatch);

                Label label = new Label()
                {
                    text = $"{profile.name} - {Mathf.Round(chance / totalChance * 100)}%"
                };
                key.Add(label);
                DistributionMapKey.Insert(0, key);
            }

            DistributionMap.Add(element);

        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/ambience-module");
        }


    }
}