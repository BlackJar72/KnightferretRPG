using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using DistantLands.Cozy.Data;
using System.Collections.Generic;

namespace DistantLands.Cozy.EditorScripts
{

    [CustomPropertyDrawer(typeof(ChanceEffector))]
    public class ChanceEffectorField : PropertyDrawer
    {

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/chance-effector-field.uxml"
            );
            asset.CloneTree(root);

            root.Q<EnumField>().BindProperty(property.FindPropertyRelative("limitType"));
            PropertyField propertyField = root.Q<PropertyField>();
            propertyField.BindProperty(property.FindPropertyRelative("customChanceEffector"));
            CurveField curveField = root.Q<CurveField>();
            curveField.BindProperty(property.FindPropertyRelative("curve"));
            VisualElement curveContainer = root.Q<VisualElement>("curve");
            propertyField.label = "";

            List<AnimationCurve> presets = new List<AnimationCurve>();
            List<string> presetNames = new List<string>();


            root.Q<EnumField>().RegisterCallback((ChangeEvent<string> evt) =>
            {
                if (evt.newValue == "Custom")
                {
                    propertyField.style.display = DisplayStyle.Flex;
                    curveContainer.style.display = DisplayStyle.None;
                }
                else
                {
                    propertyField.style.display = DisplayStyle.None;
                    curveContainer.style.display = DisplayStyle.Flex;
                }

                presetNames.Clear();
                presets.Clear();

                switch (evt.newValue)
                {

                    case ("Temperature"):
                        presets.Add(new AnimationCurve(new Keyframe(0.3f, 1), new Keyframe(0.34f, 0), new Keyframe(0, 1), new Keyframe(1, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(0.3f, 0), new Keyframe(0.34f, 1), new Keyframe(0, 0), new Keyframe(1, 1)));
                        presets.Add(new AnimationCurve(new Keyframe(0.8f, 0), new Keyframe(1, 1), new Keyframe(0, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(1, 1), new Keyframe(0, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(1, 0.5f), new Keyframe(0.5f, 0), new Keyframe(0.8f, 1)));
                        presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 0), new Keyframe(0.5f, 1)));
                        presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1)));
                        presetNames.Add("Only below freezing");
                        presetNames.Add("Only above freezing");
                        presetNames.Add("Only above 80F");
                        presetNames.Add("More likely at hot tempratures");
                        presetNames.Add("More likely at warm tempratures");
                        presetNames.Add("More likely at cool tempratures");
                        presetNames.Add("More likely at freezing tempratures");
                        break;
                    case ("Precipitation"):
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1, 3, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1)));
                        presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1, -3, -3)));
                        presetNames.Add("More likely during high precipitation");
                        presetNames.Add("Most likely during high precipitation");
                        presetNames.Add("More likely during low precipitation");
                        presetNames.Add("Most likely during low precipitation");
                        break;
                    case ("YearPercentage"):
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0), new Keyframe(0.1f, 0), new Keyframe(0.2f, 1), new Keyframe(0.35f, 1), new Keyframe(0.45f, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0), new Keyframe(0.35f, 0), new Keyframe(0.45f, 1), new Keyframe(0.6f, 1), new Keyframe(0.7f, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0), new Keyframe(0.6f, 0), new Keyframe(0.7f, 1), new Keyframe(0.85f, 1), new Keyframe(0.95f, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.1f, 0), new Keyframe(0.95f, 1), new Keyframe(1f, 1), new Keyframe(0.85f, 0)));
                        presetNames.Add("More likely during spring");
                        presetNames.Add("Most likely during summer");
                        presetNames.Add("More likely during fall");
                        presetNames.Add("Most likely during winter");
                        break;
                    case ("Time"):
                        presets.Add(new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.2f, 1), new Keyframe(0.25f, 0), new Keyframe(0.75f, 0), new Keyframe(0.8f, 1), new Keyframe(1, 1)));
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.2f, 0), new Keyframe(0.25f, 1), new Keyframe(0.75f, 1), new Keyframe(0.8f, 0), new Keyframe(1, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.18f, 0), new Keyframe(0.25f, 1), new Keyframe(0.35f, 0), new Keyframe(0.7f, 0), new Keyframe(0.75f, 1), new Keyframe(0.85f, 0), new Keyframe(1, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.70f, 0), new Keyframe(0.8f, 1), new Keyframe(0.85f, 0), new Keyframe(1, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.18f, 0), new Keyframe(0.22f, 1), new Keyframe(0.3f, 0), new Keyframe(1, 0)));
                        presetNames.Add("More likely at night");
                        presetNames.Add("Most likely during the day");
                        presetNames.Add("More likely in the evening & morning");
                        presetNames.Add("More likely in the evening");
                        presetNames.Add("Most likely in the morning");
                        break;
                    case ("AccumulatedWetness"):
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1, 3, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1)));
                        presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1, -3, -3)));
                        presetNames.Add("More likely while wet");
                        presetNames.Add("Most likely while wet");
                        presetNames.Add("More likely while dry");
                        presetNames.Add("Most likely while dry");
                        break;
                    case ("AccumulatedSnow"):
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));
                        presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1, 3, 0)));
                        presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1)));
                        presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1, -3, -3)));
                        presetNames.Add("More likely while snowy");
                        presetNames.Add("Most likely while snowy");
                        presetNames.Add("More likely while not snowy");
                        presetNames.Add("Most likely while not snowy");
                        break;

                }

            });

            if (property.FindPropertyRelative("limitType").intValue == 6)
            {
                propertyField.style.display = DisplayStyle.Flex;
                curveContainer.style.display = DisplayStyle.None;
            }
            else
            {
                propertyField.style.display = DisplayStyle.None;
                curveContainer.style.display = DisplayStyle.Flex;
            }


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
                    presetNames.Add("Only below freezing");
                    presetNames.Add("Only above freezing");
                    presetNames.Add("Only above 80F");
                    presetNames.Add("More likely at hot tempratures");
                    presetNames.Add("More likely at warm tempratures");
                    presetNames.Add("More likely at cool tempratures");
                    presetNames.Add("More likely at freezing tempratures");
                    break;
                case (1):
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1, 3, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1)));
                    presets.Add(new AnimationCurve(new Keyframe(1, 0), new Keyframe(0, 1, -3, -3)));
                    presetNames.Add("More likely during high precipitation");
                    presetNames.Add("Most likely during high precipitation");
                    presetNames.Add("More likely during low precipitation");
                    presetNames.Add("Most likely during low precipitation");
                    break;
                case (2):
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0), new Keyframe(0.1f, 0), new Keyframe(0.2f, 1), new Keyframe(0.35f, 1), new Keyframe(0.45f, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0), new Keyframe(0.35f, 0), new Keyframe(0.45f, 1), new Keyframe(0.6f, 1), new Keyframe(0.7f, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0), new Keyframe(0.6f, 0), new Keyframe(0.7f, 1), new Keyframe(0.85f, 1), new Keyframe(0.95f, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.1f, 0), new Keyframe(0.95f, 1), new Keyframe(1f, 1), new Keyframe(0.85f, 0)));
                    presetNames.Add("More likely during spring");
                    presetNames.Add("Most likely during summer");
                    presetNames.Add("More likely during fall");
                    presetNames.Add("Most likely during winter");
                    break;
                case (3):
                    presets.Add(new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.2f, 1), new Keyframe(0.25f, 0), new Keyframe(0.75f, 0), new Keyframe(0.8f, 1), new Keyframe(1, 1)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.2f, 0), new Keyframe(0.25f, 1), new Keyframe(0.75f, 1), new Keyframe(0.8f, 0), new Keyframe(1, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.18f, 0), new Keyframe(0.25f, 1), new Keyframe(0.35f, 0), new Keyframe(0.7f, 0), new Keyframe(0.75f, 1), new Keyframe(0.85f, 0), new Keyframe(1, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.70f, 0), new Keyframe(0.8f, 1), new Keyframe(0.85f, 0), new Keyframe(1, 0)));
                    presets.Add(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.18f, 0), new Keyframe(0.22f, 1), new Keyframe(0.3f, 0), new Keyframe(1, 0)));
                    presetNames.Add("More likely at night");
                    presetNames.Add("Most likely during the day");
                    presetNames.Add("More likely in the evening & morning");
                    presetNames.Add("More likely in the evening");
                    presetNames.Add("Most likely in the morning");
                    break;

            }


            DropdownField presetButton = root.Q<DropdownField>();
            presetButton.choices = presetNames;
            presetButton.labelElement.style.display = DisplayStyle.None;
            presetButton.RegisterCallback((ChangeEvent<string> evt) =>
            {
                curveField.value = presets[presetNames.IndexOf(evt.newValue)];
            });


            return root;

        }

    }

}