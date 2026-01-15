using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{

    public class VariablePropertyField : VisualElement
    {
        private Toggle Override => this.Q<Toggle>();
        public Label Label => this.Q<Label>();
        private VisualElement FieldContainer => this.Q<VisualElement>("field");
        private VisualElement Mode => this.Q<VisualElement>("mode-selector");

        public VariablePropertyField(SerializedProperty property, bool colorField)
        {

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/variable-property-field.uxml"
            );

            asset.CloneTree(this);
            this.tooltip = property.tooltip;

            Override.BindProperty(property.FindPropertyRelative("overrideValue"));
            Label.SetEnabled(Override.value);
            FieldContainer.SetEnabled(Override.value);
            Mode.SetEnabled(Override.value);

            Override.RegisterCallback<ChangeEvent<bool>>((ChangeEvent<bool> evt) =>
            {
                Label.SetEnabled(evt.newValue);
                FieldContainer.SetEnabled(evt.newValue);
                Mode.SetEnabled(evt.newValue);
            });


            Label.text = property.displayName;
            SerializedProperty modeProperty = property.FindPropertyRelative("mode");
            RefreshProperty(property, colorField, modeProperty.boolValue);

            Mode.RegisterCallback<ClickEvent>((ClickEvent evt) =>
            {
                modeProperty.boolValue = !modeProperty.boolValue;
                RefreshProperty(property, colorField, modeProperty.boolValue);
                property.serializedObject.ApplyModifiedProperties();
            });


        }

        public VariablePropertyField(SerializedProperty property, float min, float max)
        {

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/variable-property-field.uxml"
            );

            asset.CloneTree(this);
            this.tooltip = property.tooltip;

            Override.BindProperty(property.FindPropertyRelative("overrideValue"));
            Label.SetEnabled(Override.value);
            FieldContainer.SetEnabled(Override.value);
            Mode.SetEnabled(Override.value);

            Override.RegisterCallback<ChangeEvent<bool>>((ChangeEvent<bool> evt) =>
            {
                Label.SetEnabled(evt.newValue);
                FieldContainer.SetEnabled(evt.newValue);
                Mode.SetEnabled(evt.newValue);
            });


            Label.text = property.displayName;
            SerializedProperty modeProperty = property.FindPropertyRelative("mode");
            RefreshProperty(property, modeProperty.boolValue, min, max);

            Mode.RegisterCallback<ClickEvent>((ClickEvent evt) =>
            {
                modeProperty.boolValue = !modeProperty.boolValue;
                RefreshProperty(property, modeProperty.boolValue, min, max);
                property.serializedObject.ApplyModifiedProperties();
            });


        }

        public VariablePropertyField(SerializedProperty property, float min, float max, Action action)
        {

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/variable-property-field.uxml"
            );

            asset.CloneTree(this);
            this.tooltip = property.tooltip;

            Override.BindProperty(property.FindPropertyRelative("overrideValue"));
            Label.SetEnabled(Override.value);
            FieldContainer.SetEnabled(Override.value);
            Mode.SetEnabled(Override.value);

            Override.RegisterCallback<ChangeEvent<bool>>((ChangeEvent<bool> evt) =>
            {
                Label.SetEnabled(evt.newValue);
                FieldContainer.SetEnabled(evt.newValue);
                Mode.SetEnabled(evt.newValue);
            });


            Label.text = property.displayName;
            SerializedProperty modeProperty = property.FindPropertyRelative("mode");
            RefreshProperty(property, modeProperty.boolValue, min, max, action);

            Mode.RegisterCallback<ClickEvent>((ClickEvent evt) =>
            {
                modeProperty.boolValue = !modeProperty.boolValue;
                RefreshProperty(property, modeProperty.boolValue, min, max, action);
                property.serializedObject.ApplyModifiedProperties();
            });


        }

        public void RefreshProperty(SerializedProperty property, bool constant, float min, float max)
        {
            FieldContainer.Clear();

            if (constant)
            {
                VisualElement combinedFields = new VisualElement();
                combinedFields.style.flexGrow = 1;
                combinedFields.style.flexDirection = FlexDirection.Row;

                Slider prop = new Slider(min, max);
                prop.BindProperty(property.FindPropertyRelative("floatVal"));
                prop.style.flexGrow = 1;
                combinedFields.Add(prop);

                FloatField input = new FloatField();
                input.BindProperty(property.FindPropertyRelative("floatVal"));
                input.style.width = new Length(50, LengthUnit.Pixel);
                combinedFields.Add(input);

                FieldContainer.Add(combinedFields);
            }
            else
            {
                CurveField prop = new CurveField();
                prop.BindProperty(property.FindPropertyRelative("curveVal"));
                prop.AddToClassList("m-0");
                prop.AddToClassList("h-full");
                FieldContainer.Add(prop);
            }


        }

        public void RefreshProperty(SerializedProperty property, bool constant, float min, float max, Action callback)
        {
            FieldContainer.Clear();

            if (constant)
            {
                VisualElement combinedFields = new VisualElement();
                combinedFields.style.flexGrow = 1;
                combinedFields.style.flexDirection = FlexDirection.Row;

                Slider prop = new Slider(min, max);
                prop.BindProperty(property.FindPropertyRelative("floatVal"));
                prop.style.flexGrow = 1;
                prop.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => { callback.Invoke(); });
                combinedFields.Add(prop);

                FloatField input = new FloatField();
                input.BindProperty(property.FindPropertyRelative("floatVal"));
                input.style.width = new Length(50, LengthUnit.Pixel);
                input.RegisterCallback<ChangeEvent<float>>((ChangeEvent<float> evt) => { callback.Invoke(); });
                combinedFields.Add(input);

                FieldContainer.Add(combinedFields);
            }
            else
            {
                CurveField prop = new CurveField();
                prop.BindProperty(property.FindPropertyRelative("curveVal"));
                prop.AddToClassList("m-0");
                prop.AddToClassList("h-full");
                FieldContainer.Add(prop);
            }


        }

        public void RefreshProperty(SerializedProperty property, bool colorField, bool constant)
        {
            FieldContainer.Clear();

            if (colorField)
            {
                if (constant)
                {
                    ColorField prop = new ColorField();
                    prop.hdr = true;
                    prop.BindProperty(property.FindPropertyRelative("colorVal"));
                    prop.AddToClassList("m-0");
                    prop.AddToClassList("h-full");
                    FieldContainer.Add(prop);
                }
                else
                {
                    GradientField prop = new GradientField();
                    prop.hdr = true;
                    prop.BindProperty(property.FindPropertyRelative("gradientVal"));
                    prop.AddToClassList("m-0");
                    prop.AddToClassList("h-full");
                    FieldContainer.Add(prop);
                }
            }
            else
            {
                if (constant)
                {
                    FloatField prop = new FloatField();
                    prop.BindProperty(property.FindPropertyRelative("floatVal"));
                    prop.AddToClassList("m-0");
                    prop.AddToClassList("h-full");
                    FieldContainer.Add(prop);
                }
                else
                {
                    CurveField prop = new CurveField();
                    prop.BindProperty(property.FindPropertyRelative("curveVal"));
                    prop.AddToClassList("m-0");
                    prop.AddToClassList("h-full");
                    FieldContainer.Add(prop);
                }
            }

        }
    }

}