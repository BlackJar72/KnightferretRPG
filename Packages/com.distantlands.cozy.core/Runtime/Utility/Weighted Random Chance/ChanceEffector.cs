using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy
{
    [Serializable]
    public class ChanceEffector
    {

        public enum LimitType { Temperature, Precipitation, YearPercentage, Time, AccumulatedWetness, AccumulatedSnow, Custom };
        public LimitType limitType;
        public AnimationCurve curve;
        public CustomCozyChanceEffector customChanceEffector;


        public float GetChance(float test)
        {
            return curve.Evaluate(test);
        }

        public float GetChance(CozyWeather weather)
        {
            switch (limitType)
            {
                case LimitType.Temperature:
                    if (weather.climateModule != null)
                        return curve.Evaluate(weather.climateModule.currentTemperature / 100);
                    else
                        return 1;
                case LimitType.Precipitation:
                    if (weather.climateModule != null)
                        return curve.Evaluate(weather.climateModule.currentPrecipitation / 100);
                    else
                        return 1;
                case LimitType.YearPercentage:
                    return curve.Evaluate(weather.timeModule.yearPercentage);
                case LimitType.Time:
                    return curve.Evaluate(weather.timeModule.currentTime);
                case LimitType.AccumulatedSnow:
                    if (weather.climateModule)
                        return curve.Evaluate(weather.climateModule.snowAmount);
                    else
                        return 1;
                case LimitType.AccumulatedWetness:
                    if (weather.climateModule)
                        return curve.Evaluate(weather.climateModule.groundwaterAmount);
                    else
                        return 1;
                case LimitType.Custom:
                    return customChanceEffector.GetChance();
                default:
                    return 1;
            }
        }

        public float GetChanceAtTime(CozyWeather weather, float time)
        {
            switch (limitType)
            {
                case LimitType.Temperature:
                    if (weather.climateModule != null)
                        return curve.Evaluate(weather.climateModule.GetTemperature(time) / 100);
                    else
                        return 1;
                case LimitType.Precipitation:
                    if (weather.climateModule != null)
                        return curve.Evaluate(weather.climateModule.GetHumidity(time) / 100);
                    else
                        return 1;
                case LimitType.YearPercentage:
                    return curve.Evaluate(time / weather.timeModule.DaysPerYear % 1);
                case LimitType.Time:
                    return curve.Evaluate(time % 1);
                case LimitType.AccumulatedSnow:
                    if (weather.climateModule)
                        return curve.Evaluate(weather.climateModule.groundwaterAmount);
                    else
                        return 1;
                case LimitType.AccumulatedWetness:
                    if (weather.climateModule)
                        return curve.Evaluate(weather.climateModule.snowAmount);
                    else
                        return 1;
                default:
                    return 1;
            }
        }
    }

    public class CustomCozyChanceEffector : ScriptableObject
    {
        public virtual float GetChance()
        {
            return 1;
        }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ChanceEffectorAttribute))]
    [UnityEditor.CustomPropertyDrawer(typeof(ChanceEffector))]
    public class ChanceEffectorDrawer : PropertyDrawer
    {

        ChanceEffectorAttribute _attribute;
        public static Type[] customChanceEffectors;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {


            _attribute = (ChanceEffectorAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var titleRect = new Rect(position.x, position.y, 100, position.height);
            var unitRect = new Rect(position.x + 107, position.y, position.width - 135, position.height);
            var dropdown = new Rect(position.x + (position.width - 20), position.y, 20, position.height);

            if (property.FindPropertyRelative("limitType").intValue == 6)
            {

                titleRect = new Rect(position.x, position.y, 100, position.height);
                unitRect = new Rect(position.x + 107, position.y, position.width - 135, position.height);
                dropdown = new Rect(position.x + (position.width - 20), position.y, 20, position.height);

                EditorGUI.PropertyField(titleRect, property.FindPropertyRelative("limitType"), GUIContent.none);
                EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("customChanceEffector"), GUIContent.none);

                EditorGUI.indentLevel = indent;
                EditorGUI.EndProperty();
                return;

            }

            int preset = -1;
            List<AnimationCurve> presets = new List<AnimationCurve>();
            List<GUIContent> presetNames = new List<GUIContent>();

            switch (property.FindPropertyRelative("limitType").intValue)
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



            EditorGUI.PropertyField(titleRect, property.FindPropertyRelative("limitType"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("curve"), GUIContent.none);


            preset = EditorGUI.Popup(dropdown, GUIContent.none, -1, presetNames.ToArray());

            if (preset != -1)
                property.FindPropertyRelative("curve").animationCurveValue = presets[preset];

            preset = -1;

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }

#endif

    public class ChanceEffectorAttribute : PropertyAttribute
    {


        public ChanceEffectorAttribute()
        {


        }

    }

}