using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyTransitModule))]
    public class CozyTransitModuleEditor : CozyModuleEditor
    {

        CozyTransitModule transitModule;
        public override ModuleCategory Category => ModuleCategory.time;
        public override string ModuleTitle => "Transit";
        public override string ModuleSubtitle => "Sun Movement Module";
        public override string ModuleTooltip => "Control the sun movement through the sky";


        public VisualElement Graph => root.Q<VisualElement>("current-curve-graph");
        public VisualElement TransitGraph => root.Q<VisualElement>("transit-wheel-graph");
        public VisualElement TransitGraphInfo => root.Q<VisualElement>("transit-wheel-graph-info");
        public VisualElement BlocksGraph => root.Q<VisualElement>("time-blocks-graph");
        public VisualElement BlocksGraphContext => root.Q<VisualElement>("time-blocks-graph-context");
        public VisualElement SunTransitContainer => root.Q<VisualElement>("sun-transit-container");
        public VisualElement SeasonalVariationContainer => root.Q<VisualElement>("seasonal-variation-container");
        public VisualElement TimeBlocksContainer => root.Q<VisualElement>("time-blocks-container");

        public VisualElement Night1 => BlocksGraph.Q<VisualElement>("night1");
        public VisualElement Dawn => BlocksGraph.Q<VisualElement>("dawn");
        public VisualElement Morning => BlocksGraph.Q<VisualElement>("morning");
        public VisualElement Day => BlocksGraph.Q<VisualElement>("day");
        public VisualElement Afternoon => BlocksGraph.Q<VisualElement>("afternoon");
        public VisualElement Evening => BlocksGraph.Q<VisualElement>("evening");
        public VisualElement Twilight => BlocksGraph.Q<VisualElement>("twilight");
        public VisualElement Night2 => BlocksGraph.Q<VisualElement>("night2");

        static Gradient TransitDayColors = new Gradient()
        {
            colorKeys = new GradientColorKey[5] {
                new GradientColorKey(Branding.charcoal,0),
                new GradientColorKey(Branding.red,0.25f),
                new GradientColorKey(Branding.blue,0.5f),
                new GradientColorKey(Branding.orange,0.75f),
                new GradientColorKey(Branding.charcoal,1)
            }
        };

        Button widget;
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            transitModule = (CozyTransitModule)target;
            transitModule.GetModifiedDayPercent();
        }

        public override Button DisplayWidget()
        {
            widget = LargeWidget();
            Label status = widget.Q<Label>("dynamic-status");

            switch (transitModule.GetTimeBlock())
            {
                case CozyTransitModule.TimeBlockName.dawn:
                    status.text = "Dawn";
                    break;
                case CozyTransitModule.TimeBlockName.morning:
                    status.text = "Morning";
                    break;
                case CozyTransitModule.TimeBlockName.day:
                    status.text = "Day";
                    break;
                case CozyTransitModule.TimeBlockName.afternoon:
                    status.text = "Afternoon";
                    break;
                case CozyTransitModule.TimeBlockName.evening:
                    status.text = "Evening";
                    break;
                case CozyTransitModule.TimeBlockName.twilight:
                    status.text = "Twilight";
                    break;
                case CozyTransitModule.TimeBlockName.night:
                    status.text = "Night";
                    break;
            }

            VisualElement lowerContainer = widget.Q<VisualElement>("lower-container");

            lowerContainer.Add(new TransitGraph(transitModule.weatherSphere.modifiedDayPercentage));

            return widget;

        }

        public void DisplayBlockEditor(string blockName)
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(blockName).FindPropertyRelative("start"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(blockName).FindPropertyRelative("end"));
            serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement DisplayUI()
        {
            transitModule.GetModifiedDayPercent();

            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/transit-module-editor.uxml"
            );

            asset.CloneTree(root);

            root.RegisterCallback<PointerMoveEvent>((PointerMoveEvent evt) =>
            {
                RefreshTransitWheelGraph();
                RefreshTransitGraph();
                RefreshTimeBlocksGraph();
                transitModule.GetModifiedDayPercent();
            });

            RefreshTransitWheelGraph();
            RefreshTransitGraph();
            RefreshTimeBlocksGraph();

            PropertyField timeCurveSettings = new PropertyField();
            timeCurveSettings.BindProperty(serializedObject.FindProperty("timeCurveSettings"));
            SunTransitContainer.Add(timeCurveSettings);


            SunTransitContainer.Add(TimeCurveVertex(serializedObject.FindProperty("sunriseWeight"), "Sunrise"));
            SunTransitContainer.Add(TimeCurveVertex(serializedObject.FindProperty("dayWeight"), "Day"));
            SunTransitContainer.Add(TimeCurveVertex(serializedObject.FindProperty("sunsetWeight"), "Sunset"));
            SunTransitContainer.Add(TimeCurveVertex(serializedObject.FindProperty("nightWeight"), "Night"));

            PropertyField springDayLengthOffset = new PropertyField();
            springDayLengthOffset.BindProperty(serializedObject.FindProperty("springDayLengthOffset"));
            SeasonalVariationContainer.Add(springDayLengthOffset);

            PropertyField summerDayLengthOffset = new PropertyField();
            summerDayLengthOffset.BindProperty(serializedObject.FindProperty("summerDayLengthOffset"));
            SeasonalVariationContainer.Add(summerDayLengthOffset);

            PropertyField fallDayLengthOffset = new PropertyField();
            fallDayLengthOffset.BindProperty(serializedObject.FindProperty("fallDayLengthOffset"));
            SeasonalVariationContainer.Add(fallDayLengthOffset);

            PropertyField winterDayLengthOffset = new PropertyField();
            winterDayLengthOffset.BindProperty(serializedObject.FindProperty("winterDayLengthOffset"));
            SeasonalVariationContainer.Add(winterDayLengthOffset);


            TimeBlocksContainer.Add(TimeBlockElement(serializedObject.FindProperty("dawnBlock"), "Dawn"));
            TimeBlocksContainer.Add(TimeBlockElement(serializedObject.FindProperty("morningBlock"), "Morning"));
            TimeBlocksContainer.Add(TimeBlockElement(serializedObject.FindProperty("dayBlock"), "Day"));
            TimeBlocksContainer.Add(TimeBlockElement(serializedObject.FindProperty("afternoonBlock"), "Afternoon"));
            TimeBlocksContainer.Add(TimeBlockElement(serializedObject.FindProperty("eveningBlock"), "Evening"));
            TimeBlocksContainer.Add(TimeBlockElement(serializedObject.FindProperty("twilightBlock"), "Twilight"));
            TimeBlocksContainer.Add(TimeBlockElement(serializedObject.FindProperty("nightBlock"), "Night"));

            return root;

        }

        public void RefreshTransitWheelGraph()
        {
            TransitGraphInfo.Clear();

            Label infoLabel = new Label();
            infoLabel.text = "Sun Angle";
            infoLabel.AddToClassList("h1");
            TransitGraphInfo.Add(infoLabel);

            VisualElement infoHolder = new VisualElement();
            infoHolder.AddToClassList("pl-4");
            TransitGraphInfo.Add(infoHolder);

            for (int i = 0; i < 8; i += 1)
            {
                VisualElement container = new VisualElement();
                container.AddToClassList("swatch");

                VisualElement swatch = new VisualElement();
                swatch.style.backgroundColor = TransitDayColors.Evaluate(transitModule.ModifyDayPercentage((i * 2 + 1) / 16f) / 360);
                container.Add(swatch);


                Label timeLabel = new Label
                {
                    text = $"{((MeridiemTime)(i / 8f)).ToString()} - {Mathf.Round(transitModule.sunMovementCurve.Evaluate(i / 8f))}°"
                };
                timeLabel.AddToClassList("font-bold");
                container.Add(timeLabel);

                infoHolder.Add(container);

            }

            VisualElement lastContainer = new VisualElement();
            lastContainer.AddToClassList("swatch");

            VisualElement lastSwatch = new VisualElement();
            lastSwatch.style.backgroundColor = TransitDayColors.Evaluate(1);
            lastContainer.Add(lastSwatch);


            Label lastTimeLabel = new Label
            {
                text = $"{((MeridiemTime)1).ToString()} - {Mathf.Round(transitModule.sunMovementCurve.Evaluate(1))}°"
            };
            lastTimeLabel.AddToClassList("font-bold");
            lastContainer.Add(lastTimeLabel);

            infoHolder.Add(lastContainer);


            TransitGraph.Clear();

            VisualElement graphHolder = new VisualElement();
            graphHolder.AddToClassList("graph-section");

            graphHolder.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = graphHolder.contentRect.width;
                float height = graphHolder.contentRect.height;

                var painter = context.painter2D;
                painter.lineWidth = 24;

                int spokes = 120;


                for (int i = 0; i < spokes; i++)
                {
                    painter.strokeColor = TransitDayColors.Evaluate(transitModule.ModifyDayPercentage(i / (float)spokes) / 360);
                    float start = 90 + (i / (float)spokes) * 360;

                    painter.BeginPath();
                    painter.Arc(new Vector2(width / 2, height / 2), height / 3, start, start + 2f, ArcDirection.Clockwise);
                    painter.Stroke();
                }

                painter.lineWidth = 3;
                painter.strokeColor = Branding.white;
                painter.BeginPath();
                painter.Arc(new Vector2(width / 2, height / 2), height / 3 + 18, 90 + transitModule.weatherSphere.dayPercentage * 360, 90 + transitModule.weatherSphere.dayPercentage * 360 + 3f, ArcDirection.Clockwise);
                painter.Stroke();
            };

            Label label = new Label();
            label.text = transitModule.weatherSphere.timeModule.currentTime.ToString();
            label.AddToClassList("h1");
            graphHolder.Add(label);

            Label block = new Label();
            block.text = transitModule.GetTimeBlock().ToString();
            block.AddToClassList("h2");
            graphHolder.Add(block);

            TransitGraph.Add(graphHolder);

        }

        public void RefreshTransitGraph()
        {
            Graph.Clear();

            VisualElement graphHolder = new VisualElement();
            graphHolder.AddToClassList("graph-section");

            graphHolder.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = graphHolder.contentRect.width;
                float height = graphHolder.contentRect.height;

                var painter = context.painter2D;

                for (int i = 0; i < 23; i++)
                {
                    if (i % 6 == 0)
                        painter.strokeColor = Branding.whiteAccent;
                    else
                        painter.strokeColor = Branding.lightGreyAccent;

                    painter.BeginPath();
                    painter.MoveTo(new Vector2(width * i / 24, 0));
                    painter.LineTo(new Vector2(width * i / 24, height));
                    painter.Stroke();
                }

                painter.strokeColor = Branding.whiteAccent;
                painter.BeginPath();
                painter.MoveTo(new Vector2(0, height / 2));
                painter.LineTo(new Vector2(width, height / 2));
                painter.Stroke();



                float offset = transitModule.yearWeightsCurve.Evaluate(transitModule.weatherSphere.timeModule.yearPercentage) / 5;

                painter.strokeColor = Branding.white;
                painter.lineWidth = 2;
                painter.BeginPath();
                painter.MoveTo(new Vector2(0, height));
                painter.BezierCurveTo(
                    new Vector2(width * transitModule.nightWeight.weight * 0.25f, height),
                    new Vector2((width * (transitModule.sunriseWeight.time - offset)) - width * transitModule.sunriseWeight.weight * 0.25f, height * (1 - transitModule.sunriseWeight.sunHeight / 180)),
                    new Vector2(width * (transitModule.sunriseWeight.time - offset), height * (1 - transitModule.sunriseWeight.sunHeight / 180))
                    );
                painter.BezierCurveTo(
                    new Vector2((width * (transitModule.sunriseWeight.time - offset)) + width * transitModule.sunriseWeight.weight * 0.25f, height * (1 - transitModule.sunriseWeight.sunHeight / 180)),
                    new Vector2((width * transitModule.dayWeight.time) - width * transitModule.dayWeight.weight * 0.25f, height * (1 - transitModule.dayWeight.sunHeight / 180)),
                    new Vector2(width * transitModule.dayWeight.time, height * (1 - transitModule.dayWeight.sunHeight / 180))
                    );
                painter.BezierCurveTo(
                    new Vector2((width * transitModule.dayWeight.time) + width * transitModule.dayWeight.weight * 0.25f, height * (1 - transitModule.dayWeight.sunHeight / 180)),
                    new Vector2((width * (transitModule.sunsetWeight.time + offset)) - width * transitModule.sunsetWeight.weight * 0.25f, height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180)),
                    new Vector2(width * (transitModule.sunsetWeight.time + offset), height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180))
                    );
                painter.BezierCurveTo(
                    new Vector2((width * (transitModule.sunsetWeight.time + offset)) + width * transitModule.sunsetWeight.weight * 0.25f, height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180)),
                    new Vector2(width - width * transitModule.nightWeight.weight * 0.25f, height),
                    new Vector2(width, height)
                    );
                painter.Stroke();

                float springOffset = transitModule.springDayLengthOffset / 5;

                if (springOffset != 0)
                {
                    painter.strokeColor = Branding.yellow;
                    painter.lineWidth = 1;
                    painter.BeginPath();
                    painter.MoveTo(new Vector2(0, height));
                    painter.BezierCurveTo(
                        new Vector2(width * transitModule.nightWeight.weight * 0.25f, height),
                        new Vector2((width * (transitModule.sunriseWeight.time - springOffset)) - width * transitModule.sunriseWeight.weight * 0.25f, height * (1 - transitModule.sunriseWeight.sunHeight / 180)),
                        new Vector2(width * (transitModule.sunriseWeight.time - springOffset), height * (1 - transitModule.sunriseWeight.sunHeight / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * (transitModule.sunriseWeight.time - springOffset)) + width * transitModule.sunriseWeight.weight * 0.25f, height * (1 - transitModule.sunriseWeight.sunHeight / 180)),
                        new Vector2((width * transitModule.dayWeight.time) - width * transitModule.dayWeight.weight * 0.25f, height * (1 - transitModule.dayWeight.sunHeight / 180)),
                        new Vector2(width * transitModule.dayWeight.time, height * (1 - transitModule.dayWeight.sunHeight / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * transitModule.dayWeight.time) + width * transitModule.dayWeight.weight * 0.25f, height * (1 - transitModule.dayWeight.sunHeight / 180)),
                        new Vector2((width * (transitModule.sunsetWeight.time + springOffset)) - width * transitModule.sunsetWeight.weight * 0.25f, height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180)),
                        new Vector2(width * (transitModule.sunsetWeight.time + springOffset), height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * (transitModule.sunsetWeight.time + springOffset)) + width * transitModule.sunsetWeight.weight * 0.25f, height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180)),
                        new Vector2(width - width * transitModule.nightWeight.weight * 0.25f, height),
                        new Vector2(width, height)
                        );
                    painter.Stroke();
                }

                float summerOffset = transitModule.summerDayLengthOffset / 5;

                if (summerOffset != 0)
                {
                    painter.strokeColor = Branding.orange;
                    painter.lineWidth = 1;
                    painter.BeginPath();
                    painter.MoveTo(new Vector2(0, height));
                    painter.BezierCurveTo(
                        new Vector2(width * transitModule.nightWeight.weight * 0.25f, height),
                        new Vector2((width * (transitModule.sunriseWeight.time - summerOffset)) - width * transitModule.sunriseWeight.weight * 0.25f, height * (1 - transitModule.sunriseWeight.sunHeight / 180)),
                        new Vector2(width * (transitModule.sunriseWeight.time - summerOffset), height * (1 - transitModule.sunriseWeight.sunHeight / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * (transitModule.sunriseWeight.time - summerOffset)) + width * transitModule.sunriseWeight.weight * 0.25f, height * (1 - transitModule.sunriseWeight.sunHeight / 180)),
                        new Vector2((width * transitModule.dayWeight.time) - width * transitModule.dayWeight.weight * 0.25f, height * (1 - transitModule.dayWeight.sunHeight / 180)),
                        new Vector2(width * transitModule.dayWeight.time, height * (1 - transitModule.dayWeight.sunHeight / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * transitModule.dayWeight.time) + width * transitModule.dayWeight.weight * 0.25f, height * (1 - transitModule.dayWeight.sunHeight / 180)),
                        new Vector2((width * (transitModule.sunsetWeight.time + summerOffset)) - width * transitModule.sunsetWeight.weight * 0.25f, height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180)),
                        new Vector2(width * (transitModule.sunsetWeight.time + summerOffset), height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * (transitModule.sunsetWeight.time + summerOffset)) + width * transitModule.sunsetWeight.weight * 0.25f, height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180)),
                        new Vector2(width - width * transitModule.nightWeight.weight * 0.25f, height),
                        new Vector2(width, height)
                        );
                    painter.Stroke();
                }

                float fallOffset = transitModule.fallDayLengthOffset / 5;

                if (fallOffset != 0)
                {
                    painter.strokeColor = Branding.red;
                    painter.lineWidth = 1;
                    painter.BeginPath();
                    painter.MoveTo(new Vector2(0, height));
                    painter.BezierCurveTo(
                        new Vector2(width * transitModule.nightWeight.weight * 0.25f, height),
                        new Vector2((width * (transitModule.sunriseWeight.time - fallOffset)) - width * transitModule.sunriseWeight.weight * 0.25f, height * (1 - transitModule.sunriseWeight.sunHeight / 180)),
                        new Vector2(width * (transitModule.sunriseWeight.time - fallOffset), height * (1 - transitModule.sunriseWeight.sunHeight / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * (transitModule.sunriseWeight.time - fallOffset)) + width * transitModule.sunriseWeight.weight * 0.25f, height * (1 - transitModule.sunriseWeight.sunHeight / 180)),
                        new Vector2((width * transitModule.dayWeight.time) - width * transitModule.dayWeight.weight * 0.25f, height * (1 - transitModule.dayWeight.sunHeight / 180)),
                        new Vector2(width * transitModule.dayWeight.time, height * (1 - transitModule.dayWeight.sunHeight / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * transitModule.dayWeight.time) + width * transitModule.dayWeight.weight * 0.25f, height * (1 - transitModule.dayWeight.sunHeight / 180)),
                        new Vector2((width * (transitModule.sunsetWeight.time + fallOffset)) - width * transitModule.sunsetWeight.weight * 0.25f, height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180)),
                        new Vector2(width * (transitModule.sunsetWeight.time + fallOffset), height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * (transitModule.sunsetWeight.time + fallOffset)) + width * transitModule.sunsetWeight.weight * 0.25f, height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180)),
                        new Vector2(width - width * transitModule.nightWeight.weight * 0.25f, height),
                        new Vector2(width, height)
                        );
                    painter.Stroke();
                }

                float winterOffset = transitModule.winterDayLengthOffset / 5;

                if (winterOffset != 0)
                {
                    painter.strokeColor = Branding.blue;
                    painter.lineWidth = 1;
                    painter.BeginPath();
                    painter.MoveTo(new Vector2(0, height));
                    painter.BezierCurveTo(
                        new Vector2(width * transitModule.nightWeight.weight * 0.25f, height),
                        new Vector2((width * (transitModule.sunriseWeight.time - winterOffset)) - width * transitModule.sunriseWeight.weight * 0.25f, height * (1 - transitModule.sunriseWeight.sunHeight / 180)),
                        new Vector2(width * (transitModule.sunriseWeight.time - winterOffset), height * (1 - transitModule.sunriseWeight.sunHeight / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * (transitModule.sunriseWeight.time - winterOffset)) + width * transitModule.sunriseWeight.weight * 0.25f, height * (1 - transitModule.sunriseWeight.sunHeight / 180)),
                        new Vector2((width * transitModule.dayWeight.time) - width * transitModule.dayWeight.weight * 0.25f, height * (1 - transitModule.dayWeight.sunHeight / 180)),
                        new Vector2(width * transitModule.dayWeight.time, height * (1 - transitModule.dayWeight.sunHeight / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * transitModule.dayWeight.time) + width * transitModule.dayWeight.weight * 0.25f, height * (1 - transitModule.dayWeight.sunHeight / 180)),
                        new Vector2((width * (transitModule.sunsetWeight.time + winterOffset)) - width * transitModule.sunsetWeight.weight * 0.25f, height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180)),
                        new Vector2(width * (transitModule.sunsetWeight.time + winterOffset), height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180))
                        );
                    painter.BezierCurveTo(
                        new Vector2((width * (transitModule.sunsetWeight.time + winterOffset)) + width * transitModule.sunsetWeight.weight * 0.25f, height * (1 - (transitModule.sunsetWeight.sunHeight > 180 ? 360 - transitModule.sunsetWeight.sunHeight : transitModule.sunsetWeight.sunHeight) / 180)),
                        new Vector2(width - width * transitModule.nightWeight.weight * 0.25f, height),
                        new Vector2(width, height)
                        );
                    painter.Stroke();
                }
            };

            Graph.Add(graphHolder);

        }

        public void RefreshTimeBlocksGraph()
        {

            BlocksGraphContext.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = BlocksGraphContext.contentRect.width;
                float height = BlocksGraphContext.contentRect.height;
                var painter = context.painter2D;

                painter.fillColor = Branding.whiteAccent;
                painter.strokeColor = Branding.whiteAccent;

                painter.BeginPath();
                painter.MoveTo(new Vector2(width * transitModule.weatherSphere.dayPercentage, height));
                painter.LineTo(new Vector2(width * transitModule.weatherSphere.dayPercentage - 5, 0));
                painter.LineTo(new Vector2(width * transitModule.weatherSphere.dayPercentage + 5, 0));
                painter.ClosePath();
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
            };

            Night1.style.backgroundColor = new StyleColor(Branding.charcoal);
            Night1.style.flexGrow = (float)transitModule.dawnBlock.start;

            Dawn.style.backgroundColor = new StyleColor(Branding.purple);
            Dawn.style.flexGrow = transitModule.morningBlock.start - transitModule.dawnBlock.start;
            Dawn.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = Dawn.contentRect.width;
                float height = Dawn.contentRect.height;
                var painter = context.painter2D;

                painter.strokeColor = Branding.white;
                float percentage = (transitModule.dawnBlock.end - transitModule.dawnBlock.start) / (transitModule.morningBlock.start - transitModule.dawnBlock.start);
                painter.BeginPath();
                painter.MoveTo(new Vector2(width * percentage, 0));
                painter.LineTo(new Vector2(width * percentage, height));
                painter.Stroke();
            };

            Morning.style.backgroundColor = new StyleColor(Branding.red);
            Morning.style.flexGrow = transitModule.dayBlock.start - transitModule.morningBlock.start;
            Morning.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = Morning.contentRect.width;
                float height = Morning.contentRect.height;
                var painter = context.painter2D;

                painter.strokeColor = Branding.white;
                float percentage = (transitModule.morningBlock.end - transitModule.morningBlock.start) / (transitModule.dayBlock.start - transitModule.morningBlock.start);
                painter.BeginPath();
                painter.MoveTo(new Vector2(width * percentage, 0));
                painter.LineTo(new Vector2(width * percentage, height));
                painter.Stroke();
            };

            Day.style.backgroundColor = new StyleColor(Branding.blue);
            Day.style.flexGrow = transitModule.afternoonBlock.start - transitModule.dayBlock.start;
            Day.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = Morning.contentRect.width;
                float height = Morning.contentRect.height;
                var painter = context.painter2D;

                painter.strokeColor = Branding.white;
                float percentage = (transitModule.dayBlock.end - transitModule.dayBlock.start) / (transitModule.afternoonBlock.start - transitModule.dayBlock.start);
                painter.BeginPath();
                painter.MoveTo(new Vector2(width * percentage, 0));
                painter.LineTo(new Vector2(width * percentage, height));
                painter.Stroke();
            };

            Afternoon.style.backgroundColor = new StyleColor(Branding.green);
            Afternoon.style.flexGrow = transitModule.eveningBlock.start - transitModule.afternoonBlock.start;
            Afternoon.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = Morning.contentRect.width;
                float height = Morning.contentRect.height;
                var painter = context.painter2D;

                painter.strokeColor = Branding.white;
                float percentage = (transitModule.afternoonBlock.end - transitModule.afternoonBlock.start) / (transitModule.eveningBlock.start - transitModule.afternoonBlock.start);
                painter.BeginPath();
                painter.MoveTo(new Vector2(width * percentage, 0));
                painter.LineTo(new Vector2(width * percentage, height));
                painter.Stroke();
            };

            Evening.style.backgroundColor = new StyleColor(Branding.yellow);
            Evening.style.flexGrow = transitModule.twilightBlock.start - transitModule.eveningBlock.start;
            Evening.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = Morning.contentRect.width;
                float height = Morning.contentRect.height;
                var painter = context.painter2D;

                painter.strokeColor = Branding.white;
                float percentage = (transitModule.eveningBlock.end - transitModule.eveningBlock.start) / (transitModule.twilightBlock.start - transitModule.eveningBlock.start);
                painter.BeginPath();
                painter.MoveTo(new Vector2(width * percentage, 0));
                painter.LineTo(new Vector2(width * percentage, height));
                painter.Stroke();
            };

            Twilight.style.backgroundColor = new StyleColor(Branding.orange);
            Twilight.style.flexGrow = transitModule.nightBlock.start - transitModule.twilightBlock.start;
            Twilight.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = Morning.contentRect.width;
                float height = Morning.contentRect.height;
                var painter = context.painter2D;

                painter.strokeColor = Branding.white;
                float percentage = (transitModule.twilightBlock.end - transitModule.twilightBlock.start) / (transitModule.nightBlock.start - transitModule.twilightBlock.start);
                painter.BeginPath();
                painter.MoveTo(new Vector2(width * percentage, 0));
                painter.LineTo(new Vector2(width * percentage, height));
                painter.Stroke();
            };

            Night2.style.backgroundColor = new StyleColor(Branding.charcoal);
            Night2.style.flexGrow = 1 - transitModule.nightBlock.start;
            Night2.generateVisualContent += (MeshGenerationContext context) =>
            {
                float width = Morning.contentRect.width;
                float height = Morning.contentRect.height;
                var painter = context.painter2D;

                painter.strokeColor = Branding.white;
                float percentage = (transitModule.nightBlock.end - transitModule.nightBlock.start) / (1 - transitModule.nightBlock.start);
                painter.BeginPath();
                painter.MoveTo(new Vector2(width * percentage, 0));
                painter.LineTo(new Vector2(width * percentage, height));
                painter.Stroke();
            };

        }

        public VisualElement TimeCurveVertex(SerializedProperty property, string title)
        {
            VisualElement container = new VisualElement();

            Label label = new Label();
            label.text = title;
            label.AddToClassList("h2");
            container.Add(label);

            VisualElement indentedContainer = new VisualElement();
            indentedContainer.AddToClassList("pl-4");

            PropertyField time = new PropertyField();
            time.BindProperty(property.FindPropertyRelative("time"));
            indentedContainer.Add(time);

            PropertyField sunHeight = new PropertyField();
            sunHeight.BindProperty(property.FindPropertyRelative("sunHeight"));
            indentedContainer.Add(sunHeight);

            PropertyField weight = new PropertyField();
            weight.BindProperty(property.FindPropertyRelative("weight"));
            indentedContainer.Add(weight);

            container.Add(indentedContainer);

            return container;
        }

        public VisualElement TimeBlockElement(SerializedProperty property, string title)
        {
            VisualElement container = new VisualElement();

            Label label = new Label();
            label.text = title;
            label.AddToClassList("h2");
            container.Add(label);

            VisualElement indentedContainer = new VisualElement();
            indentedContainer.AddToClassList("pl-4");

            PropertyField start = new PropertyField();
            start.BindProperty(property.FindPropertyRelative("start"));
            indentedContainer.Add(start);

            PropertyField end = new PropertyField();
            end.BindProperty(property.FindPropertyRelative("end"));
            indentedContainer.Add(end);

            container.Add(indentedContainer);

            return container;
        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/transit-module");
        }


    }
}