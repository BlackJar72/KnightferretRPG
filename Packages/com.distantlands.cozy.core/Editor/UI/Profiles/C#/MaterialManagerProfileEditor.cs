using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;
using System.Collections.Generic;
using System.Linq;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(MaterialManagerProfile))]
    public class MaterialManagerProfileEditor : Editor
    {
        VisualElement root;
        MaterialManagerProfile profile;

        public VisualElement LocalSettingsContainer => root.Q<VisualElement>("local-settings-container");

        public VisualElement GlobalSettingsContainer => root.Q<VisualElement>("global-settings-container");

        public static Gradient temperatureGradient = new Gradient()
        {
            colorKeys = new GradientColorKey[5] {
            new GradientColorKey(Branding.deepRed, 1f),
            new GradientColorKey(Branding.orange, 0.75f),
            new GradientColorKey(Branding.green, 0.5f),
            new GradientColorKey(Branding.blue, 0.25f),
            new GradientColorKey(Branding.deepBlue, 0f)
            }
        };


        void OnEnable()
        {

            profile = (MaterialManagerProfile)target;

        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Profiles/UXML/material-manager-profile-editor.uxml"
            );

            asset.CloneTree(root);

            RenderModulatedValues();

            PropertyField snowTexture = new PropertyField();
            snowTexture.BindProperty(serializedObject.FindProperty("snowTexture"));
            GlobalSettingsContainer.Add(snowTexture);
            PropertyField snowNoiseSize = new PropertyField();
            snowNoiseSize.BindProperty(serializedObject.FindProperty("snowNoiseSize"));
            GlobalSettingsContainer.Add(snowNoiseSize);
            PropertyField snowColor = new PropertyField();
            snowColor.BindProperty(serializedObject.FindProperty("snowColor"));
            GlobalSettingsContainer.Add(snowColor);
            PropertyField puddleScale = new PropertyField();
            puddleScale.BindProperty(serializedObject.FindProperty("puddleScale"));
            GlobalSettingsContainer.Add(puddleScale);

            return root;
        }

        public void RenderModulatedValues()
        {
            LocalSettingsContainer.Clear();
            SerializedProperty array = serializedObject.FindProperty("modulatedValues");
            for (int i = 0; i < array.arraySize; i++)
            {
                LocalSettingsContainer.Add(RenderSingleModulatedValue(array.GetArrayElementAtIndex(i), i));
            }

        }

        public VisualElement RenderSingleModulatedValue(SerializedProperty property, int i)
        {

            VisualElement element = new VisualElement();
            element.AddToClassList("mb-md");
            Label label = new Label();

            DropdownField modulationTarget = new DropdownField();
            modulationTarget.choices = new List<string>() { "Terrain Layer Color", "Terrain Layer Tint", "Material Color", "Material Value", "Global Color", "Global Value" };
            modulationTarget.AddToClassList("flex-grow");
            modulationTarget.BindProperty(property.FindPropertyRelative("modulationTarget"));
            modulationTarget.RegisterCallback((ChangeEvent<string> evt) =>
            {
                label.text = GetTitle(property);
                if (evt.previousValue != null)
                    RenderModulatedValues();
            });

            DropdownField modulationSource = new DropdownField();
            modulationSource.choices = new List<string>() { "Day Percent", "Year Percent", "Precipitation", "Temperature", "Snow Amount", "Rain Amount" };
            modulationSource.AddToClassList("flex-grow");
            modulationSource.RegisterCallback((ChangeEvent<string> evt) =>
            {
                label.text = GetTitle(property);
            });
            modulationSource.BindProperty(property.FindPropertyRelative("modulationSource"));

            VisualElement headerContainer = new VisualElement();
            headerContainer.AddToClassList("flex-row");

            label.text = GetTitle(property);
            label.AddToClassList("flex-grow");
            label.AddToClassList("h2");

            Button addNew = new Button();
            addNew.Add(new Image() { image = EditorGUIUtility.IconContent("Toolbar Plus").image });
            addNew.RegisterCallback<ClickEvent>((ClickEvent evt) =>
            {
                serializedObject.FindProperty("modulatedValues").InsertArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                RenderModulatedValues();
            });

            Button deleteElement = new Button();
            deleteElement.Add(new Image() { image = EditorGUIUtility.IconContent("Toolbar Minus").image });
            deleteElement.RegisterCallback<ClickEvent>((ClickEvent evt) =>
            {
                serializedObject.FindProperty("modulatedValues").DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                RenderModulatedValues();
            });

            headerContainer.Add(label);
            headerContainer.Add(addNew);
            headerContainer.Add(deleteElement);

            VisualElement propertiesContainer = new VisualElement();
            propertiesContainer.AddToClassList("section-bg");

            VisualElement targetContainer = new VisualElement();
            targetContainer.AddToClassList("flex-row");

            targetContainer.Add(modulationTarget);
            targetContainer.Add(modulationSource);
            propertiesContainer.Add(targetContainer);
            propertiesContainer.Add(RefreshProperties(property));

            element.Add(headerContainer);
            element.Add(propertiesContainer);


            return element;
        }

        VisualElement RefreshProperties(SerializedProperty property)
        {

            VisualElement parent = new VisualElement();

            switch (property.FindPropertyRelative("modulationTarget").enumValueIndex)
            {
                case (0):
                    {
                        PropertyField gradientField = new PropertyField();
                        gradientField.BindProperty(property.FindPropertyRelative("mappedGradient"));
                        parent.Add(gradientField);
                        PropertyField targetLayer = new PropertyField();
                        targetLayer.BindProperty(property.FindPropertyRelative("targetLayer"));
                        parent.Add(targetLayer);
                    }
                    break;
                case (1):
                    {
                        PropertyField gradientField = new PropertyField();
                        gradientField.BindProperty(property.FindPropertyRelative("mappedGradient"));
                        parent.Add(gradientField);
                        PropertyField targetLayer = new PropertyField();
                        targetLayer.BindProperty(property.FindPropertyRelative("targetLayer"));
                        parent.Add(targetLayer);
                        break;
                    }
                case (2):
                    {
                        PropertyField gradientField = new PropertyField();
                        gradientField.BindProperty(property.FindPropertyRelative("mappedGradient"));
                        parent.Add(gradientField);
                        PropertyField targetLayer = new PropertyField();
                        targetLayer.BindProperty(property.FindPropertyRelative("targetMaterial"));
                        parent.Add(targetLayer);

                        List<string> names = new List<string>();
                        var targetMaterial = property.FindPropertyRelative("targetMaterial");
                        var targetVariableName = property.FindPropertyRelative("targetVariableName");

                        if (targetMaterial.objectReferenceValue)
                        {
                            for (int i = 0; i < (targetMaterial.objectReferenceValue as Material).shader.GetPropertyCount(); i++)
                                if ((targetMaterial.objectReferenceValue as Material).shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Color)
                                    names.Add((targetMaterial.objectReferenceValue as Material).shader.GetPropertyName(i));

                            DropdownField targetVariableNameField = new DropdownField();
                            targetVariableNameField.choices = names;
                            targetVariableNameField.BindProperty(targetVariableName);
                            parent.Add(targetVariableNameField);
                        }
                        else
                        {
                            PropertyField targetVariableNameField = new PropertyField();
                            targetVariableNameField.BindProperty(targetVariableName);
                            parent.Add(targetVariableNameField);
                        }

                        break;
                    }
                case (3):
                    {
                        PropertyField gradientField = new PropertyField();
                        gradientField.BindProperty(property.FindPropertyRelative("mappedCurve"));
                        parent.Add(gradientField);
                        PropertyField targetLayer = new PropertyField();
                        targetLayer.BindProperty(property.FindPropertyRelative("targetMaterial"));
                        parent.Add(targetLayer);
                        
                        List<string> names = new List<string>();
                        var targetMaterial = property.FindPropertyRelative("targetMaterial");
                        var targetVariableName = property.FindPropertyRelative("targetVariableName");

                        if (targetMaterial.objectReferenceValue)
                        {
                            for (int i = 0; i < (targetMaterial.objectReferenceValue as Material).shader.GetPropertyCount(); i++)
                                if ((targetMaterial.objectReferenceValue as Material).shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Float)
                                    names.Add((targetMaterial.objectReferenceValue as Material).shader.GetPropertyName(i));

                            DropdownField targetVariableNameField = new DropdownField();
                            targetVariableNameField.choices = names;
                            targetVariableNameField.BindProperty(targetVariableName);
                            parent.Add(targetVariableNameField);
                        }
                        else
                        {
                            PropertyField targetVariableNameField = new PropertyField();
                            targetVariableNameField.BindProperty(targetVariableName);
                            parent.Add(targetVariableNameField);
                        }
                        break;
                    }
                case (4):
                    {
                        PropertyField gradientField = new PropertyField();
                        gradientField.BindProperty(property.FindPropertyRelative("mappedGradient"));
                        parent.Add(gradientField);
                        PropertyField targetVariableName = new PropertyField();
                        targetVariableName.BindProperty(property.FindPropertyRelative("targetVariableName"));
                        parent.Add(targetVariableName);
                        break;
                    }
                case (5):
                    {
                        PropertyField gradientField = new PropertyField();
                        gradientField.BindProperty(property.FindPropertyRelative("mappedCurve"));
                        parent.Add(gradientField);
                        PropertyField targetVariableName = new PropertyField();
                        targetVariableName.BindProperty(property.FindPropertyRelative("targetVariableName"));
                        parent.Add(targetVariableName);
                        break;
                    }
                default:
                    break;
            }

            return parent;
        }

        public string GetTitle(SerializedProperty property)
        {

            Object targetMaterial = property.FindPropertyRelative("targetMaterial").objectReferenceValue;
            Object targetLayer = property.FindPropertyRelative("targetLayer").objectReferenceValue;

            switch (property.FindPropertyRelative("modulationTarget").enumValueIndex)
            {
                case (0):
                    return $"{(targetLayer != null ? targetLayer.name : "Unset Layer")}";
                case (1):
                    return $"{(targetLayer != null ? targetLayer.name : "Unset Layer")}";
                case (2):
                    return $"{(targetMaterial != null ? targetMaterial.name : "Unset Material")} | {property.FindPropertyRelative("targetVariableName").stringValue}";
                case (3):
                    return $"{(targetMaterial != null ? targetMaterial.name : "Unset Material")} | {property.FindPropertyRelative("targetVariableName").stringValue}";
                case (4):
                    return $"Global Color | {property.FindPropertyRelative("targetVariableName").stringValue}";
                case (5):
                    return $"Global Value | {property.FindPropertyRelative("targetVariableName").stringValue}";
                default:
                    return "Unreferenced";

            }

        }

    }
}