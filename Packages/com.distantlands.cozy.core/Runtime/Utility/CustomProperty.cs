using System;
using UnityEngine;
using System.Collections.Generic;
using DistantLands.Cozy.Data;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy
{
    [Serializable]
    public class VariableProperty
    {

        public bool overrideValue = true;
        public enum Mode { interpolate, constant }
        public Mode mode = Mode.constant;

        [ColorUsage(true, true)]
        public Color colorVal = Color.white;
        [GradientUsage(true)]
        public Gradient gradientVal;
        public float floatVal = 1;
        public AnimationCurve curveVal = new AnimationCurve() { keys = new Keyframe[2] { new Keyframe(0, 1), new Keyframe(1, 1) } };


        public static implicit operator bool(VariableProperty data)
        {
            return data.overrideValue;
        }

        public void GetValue(out Color color, float time)
        {

            color = mode == Mode.constant ? colorVal : gradientVal.Evaluate(time);

        }
        public void GetValue(out float value, float time)
        {

            value = mode == Mode.constant ? floatVal : curveVal.Evaluate(time);

        }

        public Color GetColorValue(float time)
        {

            return mode == Mode.constant ? colorVal : gradientVal.Evaluate(time);

        }
        public float GetFloatValue(float time)
        {

            return mode == Mode.constant ? floatVal : curveVal.Evaluate(time);

        }
    }

    public class ProperyRelation
    {


        public VariableProperty property;


    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CozyPropertyTypeAttribute))]
    public class CustomAtmospherePropertyDrawer : PropertyDrawer
    {

        bool color;
        float min;
        float max;
        CozyPropertyTypeAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            _attribute = (CozyPropertyTypeAttribute)attribute;
            if (_attribute.min != _attribute.max)
            {
                min = _attribute.min;
                max = _attribute.max;
            }
            color = _attribute.color;

            EditorGUI.BeginProperty(position, label, property);


            var toggleRect = new Rect(position.x, position.y, 20, position.height);
            var labelRect = new Rect(position.x + 22, position.y, (position.width - 45) / 2, position.height);
            var unitRect = new Rect(position.x + 22 + ((position.width - 45) / 2), position.y, (position.width - 45) / 2, position.height);
            var dropdown = new Rect(position.x + (position.width - 35), position.y, 35, position.height);

            var toggle = property.FindPropertyRelative("overrideValue");
            var mode = property.FindPropertyRelative("mode");
            var floatVal = property.FindPropertyRelative("floatVal");

            EditorGUI.PropertyField(toggleRect, toggle, GUIContent.none);
            EditorGUI.PrefixLabel(labelRect, label);
            EditorGUI.BeginDisabledGroup(!toggle.boolValue);

            if (color)
            {
                if (mode.intValue == 0)
                    EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("gradientVal"), GUIContent.none);
                if (mode.intValue == 1)
                    EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("colorVal"), GUIContent.none);
            }
            else
            {
                if (mode.intValue == 0)
                    EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("curveVal"), GUIContent.none);
                if (mode.intValue == 1)
                    if (_attribute.min != _attribute.max)
                        EditorGUI.Slider(unitRect, floatVal, min, max, GUIContent.none);
                    else
                        EditorGUI.PropertyField(unitRect, floatVal, GUIContent.none);
            }
            EditorGUI.PropertyField(dropdown, mode, GUIContent.none);

            EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
        }
    }


    [UnityEditor.CustomPropertyDrawer(typeof(MonthListAttribute))]
    public class MonthListDrawer : PropertyDrawer
    {


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {


            EditorGUI.BeginProperty(position, label, property);


            var rect1 = new Rect(position.x, position.y, 40, EditorGUIUtility.singleLineHeight);
            var rect2 = new Rect(position.x + 50, position.y, position.width / 2 - 54, EditorGUIUtility.singleLineHeight);
            var rect3 = new Rect(position.x + position.width / 2, position.y, position.width / 2 - 4, EditorGUIUtility.singleLineHeight);
            var rect4 = new Rect(position.x + position.width / 2 - 4, position.y, position.width / 2 - 4, EditorGUIUtility.singleLineHeight);

            var name = property.FindPropertyRelative("name");
            var days = property.FindPropertyRelative("days");

            EditorGUI.LabelField(rect1, "Month Name");
            EditorGUI.PropertyField(rect2, name, GUIContent.none);
            EditorGUI.PropertyField(rect3, days, new GUIContent(days.displayName));



            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineCount = 1.15f;

            return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * 2f * (lineCount - 1);
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(MeridiemTimeAttribute))]
    public class MeridiemTimeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            float div = position.width - 50;

            var hoursRect = new Rect(position.x + div, position.y, 22, position.height);
            var colonRect = new Rect(position.x + div + 22, position.y, 5, position.height);
            var minutesRect = new Rect(position.x + div + 28, position.y, 22, position.height);
            var sliderRect = new Rect(position.x, position.y, div - 5, position.height);

            float percentage = property.floatValue;
            MeridiemTime time = percentage;

            EditorGUI.LabelField(colonRect, ":");
            EditorGUI.BeginChangeCheck();
            int hours = Mathf.Clamp(EditorGUI.IntField(hoursRect, GUIContent.none, Mathf.FloorToInt(time.hours)), 0, 24);
            int minutes = Mathf.Clamp(EditorGUI.IntField(minutesRect, GUIContent.none, Mathf.FloorToInt(time.minutes)), 0, 60);
            if (EditorGUI.EndChangeCheck())
                property.floatValue = new MeridiemTime(hours, minutes);
            if (div > 55)
                property.floatValue = GUI.HorizontalSlider(sliderRect, property.floatValue, 0, 1);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }


    [UnityEditor.CustomPropertyDrawer(typeof(FXAttribute))]
    public class FXDrawer : PropertyDrawer
    {

        string title;
        FXAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            _attribute = (FXAttribute)attribute;
            title = _attribute.title;

            float height = EditorGUIUtility.singleLineHeight;
            var unitARect = new Rect(position.x, position.y, position.width, height);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(unitARect, property, GUIContent.none);

            position = new Rect(position.x + 30, position.y, position.width - 30, position.height);

            if (property.objectReferenceValue != null)
            {
                FXProfile profile = (FXProfile)property.objectReferenceValue;
                (Editor.CreateEditor(profile) as E_FXProfile).RenderInWindow(position);

            }


            // if (title != "")
            //     EditorGUI.PropertyField(position, property, GUIContent.none);
            // else
            //     EditorGUI.PropertyField(position, property, new GUIContent(title));


            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineCount = 1;

            if (property.objectReferenceValue != null)
            {
                FXProfile profile = (FXProfile)property.objectReferenceValue;
                lineCount += 0.5f + (Editor.CreateEditor(profile) as E_FXProfile).GetLineHeight();

            }
            return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
        }

    }

    [UnityEditor.CustomPropertyDrawer(typeof(HideTitleAttribute))]
    public class HideTitleDrawer : PropertyDrawer
    {

        string title;
        HideTitleAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            _attribute = (HideTitleAttribute)attribute;
            title = _attribute.title;
            EditorGUI.BeginProperty(position, label, property);

            if (title != "")
                EditorGUI.PropertyField(position, property, GUIContent.none);
            else
                EditorGUI.PropertyField(position, property, new GUIContent(title));


            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _attribute = (HideTitleAttribute)attribute;
            return EditorGUIUtility.singleLineHeight * _attribute.lines;
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(OverrideRangeAttribute))]
    public class OverrideRangeDrawer : PropertyDrawer
    {

        OverrideRangeAttribute _attribute;
        SerializedProperty value;
        SerializedProperty useOverride;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            useOverride = property.FindPropertyRelative("overrideValue");
            _attribute = (OverrideRangeAttribute)attribute;
            EditorGUI.BeginProperty(position, label, property);
            Rect posA = new Rect(position.x, position.y, position.height, position.height);
            Rect posB = new Rect(position.x + position.height + 5, position.y, position.width - (position.height + 5), position.height);

            EditorGUI.PropertyField(posA, useOverride, GUIContent.none);
            EditorGUI.BeginDisabledGroup(!useOverride.boolValue);
            property.FindPropertyRelative("value").floatValue = EditorGUI.Slider(posB, label, property.FindPropertyRelative("value").floatValue, _attribute.MinValue, _attribute.MaxValue);
            EditorGUI.EndDisabledGroup();


            EditorGUI.EndProperty();
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(Overridable<float>))]
    public class OverrideDrawer : PropertyDrawer
    {
        SerializedProperty useOverride;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            useOverride = property.FindPropertyRelative("overrideValue");
            EditorGUI.BeginProperty(position, label, property);
            Rect posA = new Rect(position.x, position.y, position.height, position.height);
            Rect posB = new Rect(position.x + position.height + 5, position.y, position.width - (position.height + 5), position.height);

            EditorGUI.PropertyField(posA, property.FindPropertyRelative("overrideValue"), GUIContent.none);
            EditorGUI.BeginDisabledGroup(!useOverride.boolValue);
            EditorGUI.PropertyField(posB, property.FindPropertyRelative("value"), label);
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(Overridable<Color>))]
    public class OverrideColorDrawer : PropertyDrawer
    {
        SerializedProperty useOverride;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            useOverride = property.FindPropertyRelative("overrideValue");
            EditorGUI.BeginProperty(position, label, property);
            Rect posA = new Rect(position.x, position.y, position.height, position.height);
            Rect posB = new Rect(position.x + position.height + 5, position.y, position.width - (position.height + 5), position.height);

            EditorGUI.PropertyField(posA, property.FindPropertyRelative("overrideValue"), GUIContent.none);
            EditorGUI.BeginDisabledGroup(!useOverride.boolValue);
            property.FindPropertyRelative("value").colorValue = EditorGUI.ColorField(posB, label, property.FindPropertyRelative("value").colorValue, true, true, true);
            EditorGUI.PropertyField(posB, property.FindPropertyRelative("value"), label);
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(SetHeightAttribute))]
    public class SetHeightDrawer : PropertyDrawer
    {

        int lines;
        SetHeightAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            _attribute = (SetHeightAttribute)attribute;
            lines = _attribute.lines;
            position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight * lines);
            EditorGUI.BeginProperty(position, label, property);


            EditorGUI.PropertyField(position, property, label);


            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _attribute = (SetHeightAttribute)attribute;
            lines = _attribute.lines;

            return EditorGUIUtility.singleLineHeight * lines;
        }

    }

    [UnityEditor.CustomPropertyDrawer(typeof(DisplayHorizontallyAttribute))]
    public class DisplayHorizontallyDrawer : PropertyDrawer
    {

        DisplayHorizontallyAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            _attribute = (DisplayHorizontallyAttribute)attribute;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.LabelField(position, label);


            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (property.CountInProperty() + 2);
        }

    }

    [UnityEditor.CustomPropertyDrawer(typeof(MultiAudioAttribute))]
    public class MultiAudioDrawer : PropertyDrawer
    {

        MultiAudioAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            _attribute = (MultiAudioAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            var titleRect = new Rect(position.x, position.y, position.width / 2, EditorGUIUtility.singleLineHeight);
            var typeRect = new Rect(position.x + (position.width / 2), position.y, position.width / 2, EditorGUIUtility.singleLineHeight);
            var unitRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width - 25, EditorGUIUtility.singleLineHeight);
            var dropdown = new Rect(position.x + (position.width - 20), position.y + EditorGUIUtility.singleLineHeight, 20, EditorGUIUtility.singleLineHeight);


            int preset = -1;
            List<AnimationCurve> presets = new List<AnimationCurve>();
            List<GUIContent> presetNames = new List<GUIContent>();


            switch (property.FindPropertyRelative("intensityCurve").FindPropertyRelative("limitType").intValue)
            {

                case (0):
                    presets.Add(new AnimationCurve(new Keyframe(0.3f, 1), new Keyframe(0.34f, 0), new Keyframe(0, 1), new Keyframe(1, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0.3f, 0), new Keyframe(0.34f, 1), new Keyframe(0, 0), new Keyframe(1, 1)));
                    presets.Add(new AnimationCurve(new Keyframe(0.8f, 0), new Keyframe(1, 1), new Keyframe(0, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(1, 1), new Keyframe(0, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(1, 0.5f), new Keyframe(0.5f, 0), new Keyframe(0.8f, 1)));
                    presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 0), new Keyframe(0.5f, 1)));
                    presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1)));
                    presetNames.Add(new GUIContent("Only below freezing"));
                    presetNames.Add(new GUIContent("Only above freezing"));
                    presetNames.Add(new GUIContent("Only above 80F"));
                    presetNames.Add(new GUIContent("More likely at hot tempratures"));
                    presetNames.Add(new GUIContent("More likely at warm tempratures"));
                    presetNames.Add(new GUIContent("More likely at cool tempratures"));
                    presetNames.Add(new GUIContent("More likely at freezing tempratures"));
                    break;
                case (1):
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1, 3, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1)));
                    presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1, -3, -3)));
                    presetNames.Add(new GUIContent("More likely during high precipitation"));
                    presetNames.Add(new GUIContent("Most likely during high precipitation"));
                    presetNames.Add(new GUIContent("More likely during low precipitation"));
                    presetNames.Add(new GUIContent("Most likely during low precipitation"));
                    break;
                case (2):
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0), new Keyframe(0.1f, 0), new Keyframe(0.2f, 1), new Keyframe(0.35f, 1), new Keyframe(0.45f, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0), new Keyframe(0.35f, 0), new Keyframe(0.45f, 1), new Keyframe(0.6f, 1), new Keyframe(0.7f, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0), new Keyframe(0.6f, 0), new Keyframe(0.7f, 1), new Keyframe(0.85f, 1), new Keyframe(0.95f, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.1f, 0), new Keyframe(0.95f, 1), new Keyframe(1f, 1), new Keyframe(0.85f, 0)));
                    presetNames.Add(new GUIContent("More likely during spring"));
                    presetNames.Add(new GUIContent("Most likely during summer"));
                    presetNames.Add(new GUIContent("More likely during fall"));
                    presetNames.Add(new GUIContent("Most likely during winter"));
                    break;
                case (3):
                    presets.Add(new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.2f, 1), new Keyframe(0.25f, 0), new Keyframe(0.75f, 0), new Keyframe(0.8f, 1), new Keyframe(1, 1)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.2f, 0), new Keyframe(0.25f, 1), new Keyframe(0.75f, 1), new Keyframe(0.8f, 0), new Keyframe(1, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.18f, 0), new Keyframe(0.25f, 1), new Keyframe(0.35f, 0), new Keyframe(0.7f, 0), new Keyframe(0.75f, 1), new Keyframe(0.85f, 0), new Keyframe(1, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.70f, 0), new Keyframe(0.8f, 1), new Keyframe(0.85f, 0), new Keyframe(1, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.18f, 0), new Keyframe(0.22f, 1), new Keyframe(0.3f, 0), new Keyframe(1, 0)));
                    presetNames.Add(new GUIContent("More likely at night"));
                    presetNames.Add(new GUIContent("Most likely during the day"));
                    presetNames.Add(new GUIContent("More likely in the evening & morning"));
                    presetNames.Add(new GUIContent("More likely in the evening"));
                    presetNames.Add(new GUIContent("Most likely in the morning"));
                    break;

            }

            EditorGUI.PropertyField(titleRect, property.FindPropertyRelative("FX"), GUIContent.none);
            EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("intensityCurve").FindPropertyRelative("limitType"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("intensityCurve").FindPropertyRelative("curve"), GUIContent.none);

            preset = EditorGUI.Popup(dropdown, GUIContent.none, -1, presetNames.ToArray());

            if (preset != -1)
                property.FindPropertyRelative("intensityCurve").FindPropertyRelative("curve").animationCurveValue = presets[preset];



            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }

    }

    [UnityEditor.CustomPropertyDrawer(typeof(TransitionTimeAttribute))]
    public class TransitionTimeDrawer : PropertyDrawer
    {

        TransitionTimeAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            _attribute = (TransitionTimeAttribute)attribute;
            int preset = -1;

            EditorGUI.BeginProperty(position, label, property);

            var unitRect = new Rect(position.x, position.y, position.width - 25, position.height);
            var dropdown = new Rect(position.x + (position.width - 20), position.y, 20, position.height);



            List<AnimationCurve> presets = new List<AnimationCurve>()
            {
                new AnimationCurve (new Keyframe(0, 0, 1, 1), new Keyframe (1, 1, 1, 1)),
                new AnimationCurve (new Keyframe(0, 0, 0, 0), new Keyframe (1, 1, 2, -2)),
                new AnimationCurve (new Keyframe(0, 0, 2, 2), new Keyframe (1, 1, 0, 0)),
                new AnimationCurve (new Keyframe(0, 0, 0, 0), new Keyframe (1, 1, 3.25f, -3.25f)),
                new AnimationCurve (new Keyframe(0, 0, 3.25f, 3.25f), new Keyframe (1, 1, 0, 0)),
                new AnimationCurve (new Keyframe(0, 0, 0, 0), new Keyframe (1, 1, 0, 0)),
                new AnimationCurve (new Keyframe(0, 0, 3, 3), new Keyframe (1, 1, 3, 3))
            };

            List<GUIContent> presetNames = new List<GUIContent>()
            {
                new GUIContent("Linear"),
                new GUIContent("Exponential"),
                new GUIContent("Inverse Exponential"),
                new GUIContent("Steep Exponential"),
                new GUIContent("Steep Inverse Exponential"),
                new GUIContent("Smooth"),
                new GUIContent("Slerped"),
            };

            EditorGUI.PropertyField(unitRect, property, label);

            preset = EditorGUI.Popup(dropdown, GUIContent.none, -1, presetNames.ToArray());

            if (preset != -1)
                property.animationCurveValue = presets[preset];



            EditorGUI.EndProperty();
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(WeatherRelationAttribute))]
    public class WeightedWeatherDrawer : PropertyDrawer
    {
        WeatherRelationAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            _attribute = (WeatherRelationAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            var titleRect = new Rect(position.x, position.y, 150, position.height);
            var unitRect = new Rect(position.x + 157, position.y, position.width - 155, position.height);


            EditorGUI.PropertyField(titleRect, property.FindPropertyRelative("profile"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("weight"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }


#endif
    public class CozyPropertyTypeAttribute : PropertyAttribute
    {
        public bool color;
        public float min;
        public float max;


        public CozyPropertyTypeAttribute()
        {

            color = false;

        }

        public CozyPropertyTypeAttribute(bool isColorType)
        {

            color = isColorType;

        }
        public CozyPropertyTypeAttribute(bool isColorType, float min, float max)
        {

            color = isColorType;
            this.min = min;
            this.max = max;

        }
    }

    public class FXAttribute : PropertyAttribute
    {
        public string title;


        public FXAttribute()
        {

            title = "";

        }

        public FXAttribute(string _title)
        {

            title = _title;

        }
    }

    public class OverrideRangeAttribute : PropertyAttribute
    {
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public OverrideRangeAttribute(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }

    public class HideTitleAttribute : PropertyAttribute
    {
        public string title;
        public float lines;


        public HideTitleAttribute()
        {

            title = "";
            lines = 1;

        }
        public HideTitleAttribute(float _lines)
        {

            title = "";
            lines = _lines;

        }

        public HideTitleAttribute(string _title, float _lines)
        {

            title = _title;
            lines = _lines;

        }
    }

    public class DisplayHorizontallyAttribute : PropertyAttribute
    {

        public string key;

        public DisplayHorizontallyAttribute(string _Key)
        {

            key = _Key;

        }
    }

    public class MonthListAttribute : PropertyAttribute
    {
        public MonthListAttribute()
        {


        }
    }

    public class SetHeightAttribute : PropertyAttribute
    {
        public int lines;


        public SetHeightAttribute()
        {

            lines = 1;

        }

        public SetHeightAttribute(int _lines)
        {

            lines = _lines;

        }
    }

    public class FormatTimeAttribute : PropertyAttribute
    {

        public FormatTimeAttribute()
        {


        }

    }

    public class MeridiemTimeAttribute : PropertyAttribute
    {

    }

    public class ModulatedPropertyAttribute : PropertyAttribute
    {
        public ModulatedPropertyAttribute()
        {

        }
    }

    public class TransitionTimeAttribute : PropertyAttribute
    {
        public TransitionTimeAttribute()
        {

        }
    }

    public class WeatherRelationAttribute : PropertyAttribute
    {


        public WeatherRelationAttribute()
        {


        }

    }

    public class MultiAudioAttribute : PropertyAttribute
    {

        public MultiAudioAttribute()
        {


        }

    }


}