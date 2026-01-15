using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyReflectionsModule))]
    public class CozyReflectionsModuleEditor : CozyModuleEditor
    {

        CozyReflectionsModule module;
        public override ModuleCategory Category => ModuleCategory.atmosphere;
        public override string ModuleTitle => "Reflection";
        public override string ModuleSubtitle => "Probe Management Module";
        public override string ModuleTooltip => "Sets up a cubemap for reflections with COZY.";


        public VisualElement UpdateContainer => root.Q<VisualElement>("update");
        public VisualElement RenderingContainer => root.Q<VisualElement>("rendering");
        Button widget;
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            module = (CozyReflectionsModule)target;
        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
            switch (module.updateFrequency)
            {
                case CozyReflectionsModule.UpdateFrequency.everyFrame:
                    status.text = "Every Frame";
                    break;
                case CozyReflectionsModule.UpdateFrequency.onAwake:
                    status.text = "On Awake";
                    break;
                case CozyReflectionsModule.UpdateFrequency.onHour:
                    status.text = "Every Hour";
                    break;
                case CozyReflectionsModule.UpdateFrequency.viaScripting:
                    status.text = "Stand by";
                    break;
            }

            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/reflections-module-editor.uxml"
            );

            asset.CloneTree(root);

            PropertyField framesBetweenRenders = new PropertyField();

            PropertyField updateFrequency = new PropertyField();
            updateFrequency.BindProperty(serializedObject.FindProperty("updateFrequency"));
            updateFrequency.RegisterCallback<ChangeEvent<CozyReflectionsModule.UpdateFrequency>>((ChangeEvent<CozyReflectionsModule.UpdateFrequency> evt) =>
            {
                if (evt.newValue == CozyReflectionsModule.UpdateFrequency.everyFrame)
                    framesBetweenRenders.SetEnabled(true);
                else
                    framesBetweenRenders.SetEnabled(false);
            });
            UpdateContainer.Add(updateFrequency);

            framesBetweenRenders.AddToClassList("pl-4");
            framesBetweenRenders.BindProperty(serializedObject.FindProperty("framesBetweenRenders"));
            UpdateContainer.Add(framesBetweenRenders);

            PropertyField refreshOnSceneChange = new PropertyField();
            refreshOnSceneChange.BindProperty(serializedObject.FindProperty("refreshOnSceneChange"));
            UpdateContainer.Add(refreshOnSceneChange);


            PropertyField reflectionCubemap = new PropertyField();
            reflectionCubemap.BindProperty(serializedObject.FindProperty("reflectionCubemap"));
            reflectionCubemap.AddToClassList("mb-md");
            RenderingContainer.Add(reflectionCubemap);

            PropertyField layerMask = new PropertyField();
            layerMask.BindProperty(serializedObject.FindProperty("layerMask"));
            RenderingContainer.Add(layerMask);

            PropertyField automaticallySetLayer = new PropertyField();
            automaticallySetLayer.BindProperty(serializedObject.FindProperty("automaticallySetLayer"));
            automaticallySetLayer.AddToClassList("mb-md");
            RenderingContainer.Add(automaticallySetLayer);

#if COZY_URP
            PropertyField rendererOverride = new PropertyField();
            rendererOverride.BindProperty(serializedObject.FindProperty("rendererOverride"));
            RenderingContainer.Add(rendererOverride);
#endif



            PopupField<string> qLevel = new PopupField<string>("Minimum Quality Level", QualitySettings.names.ToList(), module.minimumQualityLevel);
            qLevel.RegisterValueChangedCallback((ChangeEvent<string> evt) =>
            {
                module.minimumQualityLevel = QualitySettings.names.ToList().IndexOf(evt.newValue);
            });
            qLevel.AddToClassList("unity-base-field__aligned");
            RenderingContainer.Add(qLevel);

            return root;

        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/reflections-module");
        }


    }
}