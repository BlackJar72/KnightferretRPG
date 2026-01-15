using DistantLands.Cozy.EditorScripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TransitGraph : VisualElement
{
    float time = 0.25f;
    static Gradient DayGradient => new Gradient() {
            colorKeys = new GradientColorKey[6] {
                new GradientColorKey(Branding.charcoal, 0.2f),
                new GradientColorKey(Branding.red, 0.25f),
                new GradientColorKey(Branding.blue, 0.3f),
                new GradientColorKey(Branding.blue, 0.7f),
                new GradientColorKey(Branding.orange, 0.75f),
                new GradientColorKey(Branding.charcoal, 0.8f)
            }
        };
    public new class UxmlFactory : UxmlFactory<VersionSignifier> { }
    public TransitGraph()
    {
        Init();
    }
    public TransitGraph(float _time)
    {
        time = _time;
        Init();
    }

    public void Init(
    )
    {
        AddToClassList("graph-section");
        generateVisualContent += GenerateTransitGraph;

    }

    public void GenerateTransitGraph(MeshGenerationContext context)
    {
        float width = contentRect.width;
        float height = contentRect.height;

        AnimationCurve curve = new AnimationCurve(new Keyframe[3] {
            new Keyframe(0, 0, 0, 0, 0.5f, 0.5f),
            new Keyframe(0.5f, 1, 0, 0, 0.5f, 0.5f),
            new Keyframe(1, 0, 0, 0, 0.5f, 0.5f)
            });

        var painter = context.painter2D;

        painter.lineWidth = 2;
        painter.strokeColor = new Color(0, 0, 0, 0.25f);

        painter.BeginPath();
        painter.MoveTo(new Vector2(0, height * 1 / 3));
        painter.LineTo(new Vector2(width, height * 1 / 3));
        painter.Stroke();
        painter.ClosePath();

        painter.strokeGradient = DayGradient;

        painter.BeginPath();
        painter.MoveTo(new Vector2(0, height * 2 / 3));
        painter.BezierCurveTo(new Vector2(width / 4, height * 2 / 3), new Vector2(width / 4, 0), new Vector2(width / 2, 0));
        painter.BezierCurveTo(new Vector2(width * 3 / 4, 0), new Vector2(width * 3 / 4, height * 2 / 3), new Vector2(width, height * 2 / 3));
        painter.Stroke();
        painter.ClosePath();

        painter.strokeColor = DayGradient.Evaluate(time);
        painter.fillColor = DayGradient.Evaluate(time);
        painter.BeginPath();
        painter.Arc(new Vector2(width * time, (1 - curve.Evaluate(time)) * height * 2 / 3), 5, 0.0f, 360f, ArcDirection.CounterClockwise);
        painter.Fill(FillRule.NonZero);
        painter.Stroke();
        painter.ClosePath();
    }

}