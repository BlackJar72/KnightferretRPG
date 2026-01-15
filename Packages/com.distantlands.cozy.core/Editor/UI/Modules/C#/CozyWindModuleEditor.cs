using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyWindModule))]
    public class CozyWindModuleEditor : CozyModuleEditor
    {

        CozyWindModule windModule;
        public override ModuleCategory Category => ModuleCategory.ecosystem;
        public override string ModuleTitle => "Wind";
        public override string ModuleSubtitle => "Wind Zone Module";
        public override string ModuleTooltip => "Control wind within the COZY system.";

        public VisualElement GraphContainer => root.Q<VisualElement>("graph-container");
        public VisualElement GraphInformationContainer => root.Q<VisualElement>("graph-information-container");
        public VisualElement SelectionContainer => root.Q<VisualElement>("selection-container");
        public VisualElement GlobalSettingsContainer => root.Q<VisualElement>("global-settings-container");

        public Label Direction => root.Q<Label>("direction");
        public Label MainWind => root.Q<Label>("main-wind");
        public Label PulseMagnitude => root.Q<Label>("pulse-magnitude");
        public Label PulseFrequency => root.Q<Label>("pulse-frequency");
        Button widget;
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            windModule = (CozyWindModule)target;
        }

        public override Button DisplayWidget()
        {
            widget = LargeWidget();
            Label status = widget.Q<Label>("dynamic-status");

            Vector3 north = Vector3.Cross(windModule.weatherSphere.sunTransform.parent.forward, Vector3.up);
            Vector3 west = windModule.weatherSphere.sunTransform.parent.forward;
            Vector3 windDirInCardinalDirection = north * windModule.WindDirection.x + west * windModule.WindDirection.y;
            string compassDirection;
            if (Mathf.Abs(windDirInCardinalDirection.x) > Mathf.Abs(windDirInCardinalDirection.z))
                compassDirection = windDirInCardinalDirection.x > 0 ? "N" : "S";
            else
                compassDirection = windDirInCardinalDirection.z > 0 ? "E" : "W";

            status.text = $"Wind: {Mathf.Round(windModule.WindSpeedInKnots * 10f) / 10f} {compassDirection}";

            VisualElement lowerContainer = widget.Q<VisualElement>("lower-container");
            lowerContainer.Add(Weathervane());

            return widget;

        }

        public VisualElement Weathervane()
        {
            VisualElement element = new VisualElement();
            element.AddToClassList("half-graph-section");

            element.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = element.contentRect.width;
                float height = element.contentRect.height;

                var painter = context.painter2D;

                painter.lineWidth = 2;
                painter.strokeColor = new Color(1, 1, 1, 0.25f);
                painter.BeginPath();
                painter.Arc(new Vector2(width / 2, height / 2), width / 3, 0, 360f, ArcDirection.Clockwise);
                painter.Stroke();
                painter.ClosePath();

                painter.fillColor = new Color(1, 1, 1, 1);
                painter.BeginPath();
                painter.MoveTo(new Vector2(width / 2 - (width / 3 - 6), height / 2));
                painter.LineTo(new Vector2(width / 2 - (width / 3), height / 2));
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
                painter.ClosePath();

                painter.BeginPath();
                painter.MoveTo(new Vector2(width / 2, height / 2 + (width / 3 - 6)));
                painter.LineTo(new Vector2(width / 2, height / 2 + (width / 3)));
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
                painter.ClosePath();

                painter.BeginPath();
                painter.MoveTo(new Vector2(width / 2 + (width / 3 - 6), height / 2));
                painter.LineTo(new Vector2(width / 2 + (width / 3), height / 2));
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
                painter.ClosePath();

                painter.BeginPath();
                painter.MoveTo(new Vector2(width / 2, height / 2 - (width / 3 - 6)));
                painter.LineTo(new Vector2(width / 2, height / 2 - (width / 3)));
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
                painter.ClosePath();

                Vector2 compassWindDir = new Vector2(windModule.WindDirection.x, -windModule.WindDirection.z);

                painter.strokeColor = Color.red;
                painter.BeginPath();
                painter.MoveTo(new Vector2(width / 2, height / 2) + compassWindDir.normalized * (width / 3));
                painter.LineTo(new Vector2(width / 2, height / 2) + compassWindDir.normalized * (width / 4));
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
                painter.ClosePath();

            };

            Label label = new Label();
            label.text = $"{Mathf.Round(windModule.WindSpeedInKnots * 10f) / 10f} kn";
            label.AddToClassList("h2");

            element.Add(label);

            return element;
        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/wind-module-editor.uxml"
            );

            asset.CloneTree(root);
            DrawGraph();

            root.RegisterCallback<PointerMoveEvent>((PointerMoveEvent evt) =>
            {
                DrawGraph();
            });


            PropertyField defaultWindProfile = new PropertyField();
            defaultWindProfile.BindProperty(serializedObject.FindProperty("defaultWindProfile"));
            SelectionContainer.Add(defaultWindProfile);
            PropertyField windZone = new PropertyField();
            windZone.BindProperty(serializedObject.FindProperty("windZone"));
            SelectionContainer.Add(windZone);

            PropertyField windMultiplier = new PropertyField();
            windMultiplier.BindProperty(serializedObject.FindProperty("windMultiplier"));
            GlobalSettingsContainer.Add(windMultiplier);
            PropertyField useWindzone = new PropertyField();
            useWindzone.BindProperty(serializedObject.FindProperty("useWindzone"));
            GlobalSettingsContainer.Add(useWindzone);
            PropertyField useShaderWind = new PropertyField();
            useShaderWind.BindProperty(serializedObject.FindProperty("useShaderWind"));
            GlobalSettingsContainer.Add(useShaderWind);

            return root;

        }

        public void DrawGraph()
        {
            GraphContainer.Clear();

            if (windModule.windZone)
            {
                Direction.text = $"Wind Direction: {windModule.windZone.transform.forward}";
                MainWind.text = $"Main Wind Amount: {windModule.windZone.windMain}";
                PulseMagnitude.text = $"Pulse Magnitude: {windModule.windZone.windPulseMagnitude}";
                PulseFrequency.text = $"Pulse Frequency: {windModule.windZone.windPulseFrequency}";
            }
            else
            {
                Direction.text = "No WindZone Detected";
                MainWind.text = "--";
                PulseMagnitude.text = "--";
                PulseFrequency.text = "--";
            }

            VisualElement element = new VisualElement();
            element.AddToClassList("half-graph-section");

            element.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = element.contentRect.width;
                float height = element.contentRect.height;

                var painter = context.painter2D;

                painter.lineWidth = 2;
                painter.strokeColor = new Color(1, 1, 1, 0.25f);
                painter.BeginPath();
                painter.Arc(new Vector2(width / 2, height / 2), height / 2, 0, 360f, ArcDirection.Clockwise);
                painter.Stroke();
                painter.ClosePath();

                painter.fillColor = new Color(1, 1, 1, 1);
                painter.BeginPath();
                painter.MoveTo(new Vector2(width / 2 - (height / 2 - 6), height / 2));
                painter.LineTo(new Vector2(width / 2 - (height / 2), height / 2));
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
                painter.ClosePath();

                painter.BeginPath();
                painter.MoveTo(new Vector2(width / 2, height / 2 + (height / 2 - 6)));
                painter.LineTo(new Vector2(width / 2, height / 2 + (height / 2)));
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
                painter.ClosePath();

                painter.BeginPath();
                painter.MoveTo(new Vector2(width / 2 + (height / 2 - 6), height / 2));
                painter.LineTo(new Vector2(width / 2 + (height / 2), height / 2));
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
                painter.ClosePath();

                painter.BeginPath();
                painter.MoveTo(new Vector2(width / 2, height / 2 - (height / 2 - 6)));
                painter.LineTo(new Vector2(width / 2, height / 2 - (height / 2)));
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
                painter.ClosePath();

                Vector2 compassWindDir = new Vector2(windModule.WindDirection.x, -windModule.WindDirection.z);

                painter.strokeColor = Color.red;
                painter.BeginPath();
                painter.MoveTo(new Vector2(width / 2, height / 2) + compassWindDir.normalized * (height / 2));
                painter.LineTo(new Vector2(width / 2, height / 2) + compassWindDir.normalized * (height / 3));
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
                painter.ClosePath();

            };

            Label label = new Label();
            label.text = $"{Mathf.Round(windModule.windSpeed * windModule.windMultiplier * 10f) / 10f} - {Mathf.Round(windModule.windSpeed * windModule.windMultiplier * 10f) / 10f + windModule.windGusting} kn";
            label.AddToClassList("h1");

            element.Add(label);

            GraphContainer.Add(element);

        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/wind-module");
        }


    }
}