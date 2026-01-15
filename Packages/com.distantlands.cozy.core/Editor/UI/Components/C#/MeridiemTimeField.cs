using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;
using System.Collections.Generic;
using UnityEngine;

namespace DistantLands.Cozy.EditorScripts
{

    [CustomPropertyDrawer(typeof(MeridiemTime))]
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



            MeridiemTime time = new MeridiemTime(property.FindPropertyRelative("hours").intValue, property.FindPropertyRelative("minutes").intValue,
                                                 property.FindPropertyRelative("seconds").intValue, property.FindPropertyRelative("milliseconds").intValue);

            float timeAsFloat = time;

            EditorGUI.LabelField(colonRect, ":");
            EditorGUI.BeginChangeCheck();
            int hours = Mathf.Clamp(EditorGUI.IntField(hoursRect, GUIContent.none, Mathf.FloorToInt(time.hours)), 0, 23);
            int minutes = Mathf.Clamp(EditorGUI.IntField(minutesRect, GUIContent.none, Mathf.FloorToInt(time.minutes)), 0, 59);

            if (EditorGUI.EndChangeCheck())
            {
                property.FindPropertyRelative("hours").intValue = hours;
                time.hours = hours;
                property.FindPropertyRelative("minutes").intValue = minutes;
                time.minutes = minutes;
                timeAsFloat = time;
            }

            if (div > 55)
            {
                EditorGUI.BeginChangeCheck();
                timeAsFloat = GUI.HorizontalSlider(sliderRect, timeAsFloat, 0, 1);

                if (EditorGUI.EndChangeCheck())
                {
                    time = timeAsFloat;
                    property.FindPropertyRelative("hours").intValue = time.hours;
                    property.FindPropertyRelative("minutes").intValue = time.minutes;
                    property.FindPropertyRelative("seconds").intValue = time.seconds;
                    property.FindPropertyRelative("milliseconds").intValue = time.milliseconds;
                }
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }


}