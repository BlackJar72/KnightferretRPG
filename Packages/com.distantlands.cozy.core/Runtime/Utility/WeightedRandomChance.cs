//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

//  Documentation provided here: https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/weighted-random-values

using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace DistantLands.Cozy
{
    [System.Serializable]
    public class WeightedRandomChance
    {

        [Range(0, 1)]
        public float baseChance = 1;
        [Tooltip("Animation curves that increase or decrease chance based on time, temperature, etc.")]
        public List<ChanceEffector> chanceEffectors = new List<ChanceEffector>();

        public float GetChance() => GetChance(CozyWeather.instance);
        public float GetChance(CozyWeather weather)
        {

            float i = baseChance;

            foreach (ChanceEffector j in chanceEffectors)
                if (j != null)
                    i *= j.GetChance(weather);

            return Mathf.Max(i, 0);

        }
        public float GetChance(CozyWeather weather, float inTime)
        {

            float i = baseChance;

            foreach (ChanceEffector j in chanceEffectors)
                if (j != null)
                    i *= j.GetChanceAtTime(weather, inTime);

            return Mathf.Max(i, 0);

        }

        public bool HasLimit(ChanceEffector.LimitType limit)
        {
            foreach (ChanceEffector effector in chanceEffectors)
            {
                if (effector.limitType == limit)
                    return true;
            }

            return false;
        }

        public float GetChance(ChanceEffector.LimitType limit, float test)
        {
            float i = baseChance;

            foreach (ChanceEffector effector in chanceEffectors)
            {
                i *= effector.limitType == limit ? effector.GetChance(test) : 1;
            }

            return i;
        }

        public static implicit operator float(WeightedRandomChance chance)
        {
            return chance.GetChance();
        }

    }
#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(WeightedRandomChance))]
    public class WeightedChanceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {


            float height = EditorGUIUtility.singleLineHeight;
            var labelRect = new Rect(position.x, position.y, 100, height);
            var unitARect = new Rect(position.x + 100, position.y, position.width - 130, height);
            var unitBRect = new Rect(position.width - 7, position.y, 25, height);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.LabelField(labelRect, label);
            EditorGUI.PropertyField(unitARect, property.FindPropertyRelative("baseChance"), GUIContent.none);
            if (GUI.Button(unitBRect, "..."))
                EditWeightedRandomInWindow.OpenWindow(property);

            EditorGUI.EndProperty();
        }
    }

    public class EditWeightedRandomInWindow : EditorWindow
    {

        public Vector2 scrollPos;
        public SerializedProperty chance;

        public static void OpenWindow(SerializedProperty chance)
        {
            EditWeightedRandomInWindow window = (EditWeightedRandomInWindow)GetWindow(typeof(EditWeightedRandomInWindow), true, $"Adjust Chance Effectors");
            window.chance = chance;
            window.minSize = new Vector2(200, 100);
            window.Show();

        }

        private void OnGUI()
        {
            if (chance == null)
            {
                Close();
                return;
            }
            EditorGUI.indentLevel = 1;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            chance.serializedObject.Update();
            EditorGUILayout.PropertyField(chance.FindPropertyRelative("baseChance"));
            EditorGUILayout.PropertyField(chance.FindPropertyRelative("chanceEffectors"));
            chance.serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Done"))
                Close();
        }
    }

#endif
}