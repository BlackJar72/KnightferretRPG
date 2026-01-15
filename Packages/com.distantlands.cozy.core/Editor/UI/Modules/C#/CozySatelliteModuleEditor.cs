using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;
using System.Linq;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozySatelliteModule))]
    public class CozySatelliteModuleEditor : CozyModuleEditor
    {

        CozySatelliteModule satelliteModule;
        public override ModuleCategory Category => ModuleCategory.atmosphere;
        public override string ModuleTitle => "Satellite";
        public override string ModuleSubtitle => "Moon Module";
        public override string ModuleTooltip => "Manage satellites and moons within the COZY system.";

        public VisualElement CurrentSatellitesContainer => root.Q<VisualElement>("current-satellites-container");
        public VisualElement SatellitesContainer => root.Q<VisualElement>("satellite-inspector-container");
        public VisualElement SatelliteGraph => root.Q<VisualElement>("satellite-graph");
        public VisualElement OrbitGraph => root.Q<VisualElement>("orbit-graph");
        public VisualElement OrbitGraphKey => root.Q<VisualElement>("orbit-graph-key");
        public VisualElement MoonPhaseGraph => root.Q<VisualElement>("moon-phase-graph");
        public VisualElement PhaseGraph => root.Q<VisualElement>("moon-graph");
        public Label MoonName => root.Q<Label>("moon-name");
        public Label PhaseName => root.Q<Label>("phase-name");
        public Label PhaseTime => root.Q<Label>("phase-time");
        public Label Illumination => root.Q<Label>("illumination");
        public Label Declination => root.Q<Label>("declination");
        Button widget;
        VisualElement root;

        static Gradient OrbitColors = new Gradient()
        {
            colorKeys = new GradientColorKey[5] {
                new GradientColorKey(Branding.deepBlue,0),
                new GradientColorKey(Branding.red,0.25f),
                new GradientColorKey(Branding.blue,0.5f),
                new GradientColorKey(Branding.green,0.75f),
                new GradientColorKey(Branding.charcoal,1)
            }
        };

        void OnEnable()
        {
            if (!target)
                return;

            satelliteModule = (CozySatelliteModule)target;
        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");

            status.text = satelliteModule.GetMoonPhaseName();

            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/satellite-module-editor.uxml"
            );

            asset.CloneTree(root);

            root.RegisterCallback((PointerMoveEvent evt) =>
            {
                RefreshPhaseGraph();
                RefreshOrbitGraph();
            });


            RefreshPhaseGraph();
            RefreshOrbitGraph();
            RefreshSatelliteList();

            PropertyField satellites = new PropertyField();
            satellites.BindProperty(serializedObject.FindProperty("satellites"));
            satellites.RegisterCallback<ChangeEvent<SatelliteProfile[]>>((ChangeEvent<SatelliteProfile[]> evt) =>
            {
                RefreshSatelliteList();
            });
            CurrentSatellitesContainer.Add(satellites);

            return root;

        }

        public void RefreshPhaseGraph()
        {

            if (satelliteModule.satellites.Length == 0)
                return;

            SatelliteProfile satelliteProfile = satelliteModule.satellites[satelliteModule.mainMoon];
            float globalDay = satelliteModule.weatherSphere.timeModule.AbsoluteDay - satelliteProfile.rotationPeriodOffset;
            float cyclePercentage = globalDay % satelliteProfile.rotationPeriod / satelliteProfile.rotationPeriod;
            float phase = (cyclePercentage - .5f) * 2;
            // Debug.Log(phase);


            Shader.SetGlobalFloat("CZY_UI_MOONPHASE", phase);


            PhaseName.text = satelliteProfile.name;
            MoonName.text = satelliteModule.GetMoonPhaseName();
            PhaseTime.text = $"{satelliteProfile.rotationPeriod - (satelliteModule.weatherSphere.timeModule.AbsoluteDay + satelliteProfile.rotationPeriodOffset % satelliteProfile.rotationPeriod)} Days for Next Cycle";
            Illumination.text = $"{Mathf.Round((Vector3.Dot(satelliteModule.weatherSphere.sunTransform.forward, satelliteModule.weatherSphere.moonDirection) + 1) * 50)}% Illumination";

            float dec = satelliteProfile.declination * Mathf.Sin(Mathf.PI * 2 * ((satelliteModule.weatherSphere.modifiedDayPercentage + (float)(satelliteModule.weatherSphere.timeModule.currentDay + satelliteProfile.rotationPeriodOffset + satelliteModule.weatherSphere.timeModule.DaysPerYear * satelliteModule.weatherSphere.timeModule.currentYear) % satelliteProfile.declinationPeriod) / satelliteProfile.declinationPeriod));
            Declination.text = $"Current Declination of {Mathf.Round(dec)}°";



        }

        public void RefreshOrbitGraph()
        {
            if (satelliteModule.satellites.Length == 0)
                return;
                
            OrbitGraphKey.Clear();

            VisualElement infoHolder = new VisualElement();
            infoHolder.AddToClassList("pl-4");
            OrbitGraphKey.Add(infoHolder);

            VisualElement sunContainer = new VisualElement();
            sunContainer.AddToClassList("swatch");

            VisualElement sunSwatch = new VisualElement();
            sunSwatch.style.backgroundColor = Branding.yellow;
            sunContainer.Add(sunSwatch);

            Label sunLabel = new Label
            {
                text = "Sun"
            };
            sunLabel.AddToClassList("font-bold");
            sunContainer.Add(sunLabel);

            infoHolder.Add(sunContainer);

            for (int i = 0; i < satelliteModule.satellites.Length; i++)
            {
                VisualElement container = new VisualElement();
                container.AddToClassList("swatch");

                VisualElement swatch = new VisualElement();
                swatch.style.backgroundColor = OrbitColors.Evaluate((float)i / satelliteModule.satellites.Length);
                container.Add(swatch);

                Label timeLabel = new Label
                {
                    text = satelliteModule.satellites[i].name
                };
                timeLabel.AddToClassList("font-bold");
                container.Add(timeLabel);

                infoHolder.Add(container);

            }

            OrbitGraph.Clear();

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
                painter.MoveTo(new Vector2(0, height / 2));
                painter.LineTo(new Vector2(width, height / 2));
                painter.Stroke();

                painter.lineWidth = 2;
                painter.strokeColor = Branding.yellow;
                float sunAngle = satelliteModule.weatherSphere.modifiedDayPercentage * 360 + 90;
                float sunRadius = height * 0.45f;

                painter.BeginPath();
                painter.Arc(new Vector2(width / 2, height / 2), sunRadius,
                            sunAngle + 7,
                            sunAngle + 353,
                            ArcDirection.Clockwise);
                painter.Stroke();

                painter.BeginPath();
                painter.Arc(new Vector2(width / 2, height / 2) + new Vector2(Mathf.Cos(Mathf.Deg2Rad * sunAngle) * sunRadius, Mathf.Sin(Mathf.Deg2Rad * sunAngle) * sunRadius),
                            (2 * Mathf.PI * sunRadius / 360) * 4.5f, 0, 360, ArcDirection.Clockwise);
                painter.Stroke();


                for (int i = 0; i < satelliteModule.satellites.Length; i++)
                {

                    SatelliteProfile sat = satelliteModule.satellites[i];

                    painter.strokeColor = OrbitColors.Evaluate((float)i / satelliteModule.satellites.Length);
                    float satAngle = 90 + ((360 * satelliteModule.weatherSphere.modifiedDayPercentage) + sat.satelliteRotation + sat.orbitOffset);
                    float satRadius = height * 0.45f - ((i + 1) * 15);

                    painter.BeginPath();
                    painter.Arc(new Vector2(width / 2, height / 2), satRadius,
                                satAngle + 7,
                                satAngle + 353,
                                ArcDirection.Clockwise);
                    painter.Stroke();

                    painter.BeginPath();
                    painter.Arc(new Vector2(width / 2, height / 2) + new Vector2(Mathf.Cos(Mathf.Deg2Rad * satAngle) * satRadius, Mathf.Sin(Mathf.Deg2Rad * satAngle) * satRadius),
                                2 * Mathf.PI * satRadius / 360 * 4.5f, 0, 360, ArcDirection.Clockwise);
                    painter.Stroke();

                }
            };

            OrbitGraph.Add(graphHolder);

        }

        public void RefreshSatelliteList()
        {

            for (int i = 0; i < satelliteModule.satellites.Length; i++)
            {
                Label label = new Label();
                label.text = satelliteModule.satellites[i].name;
                label.AddToClassList("h2");
                SatellitesContainer.Add(label);

                VisualElement container = new VisualElement();
                container.AddToClassList("section-bg");
                SatellitesContainer.Add(container);

                InspectorElement inspector = new InspectorElement(satelliteModule.satellites[i]);
                inspector.AddToClassList("p-0");
                container.Add(inspector);
            }

        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/satellite-module");
        }


    }
}