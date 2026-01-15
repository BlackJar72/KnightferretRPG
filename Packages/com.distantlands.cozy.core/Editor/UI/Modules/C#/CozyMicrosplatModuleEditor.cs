using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyMicrosplatModule))]
    public class CozyMicrosplatModuleEditor : CozyModuleEditor
    {

        CozyMicrosplatModule microsplatModule;
        public override ModuleCategory Category => ModuleCategory.integration;
        public override string ModuleTitle => "MicroSplat";
        public override string ModuleSubtitle => "MicroSplat Integration";
        public override string ModuleTooltip => "Directly integrate with MicroSplat by Jason Booth.";

        public VisualElement Container => root.Q<VisualElement>("settings-container");
        public VisualElement UpdateContainer => root.Q<VisualElement>("update-container");
        Button widget;
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            microsplatModule = (CozyMicrosplatModule)target;
        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
            status.text = "";

            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/microsplat-module-editor.uxml"
            );

            asset.CloneTree(root);

            PropertyField updateWetness = new PropertyField();
            updateWetness.BindProperty(serializedObject.FindProperty("updateWetness"));
            Container.Add(updateWetness);

            VisualElement paddedContainer = new VisualElement();
            paddedContainer.AddToClassList("pl-4");

            PropertyField minWetness = new PropertyField();
            minWetness.BindProperty(serializedObject.FindProperty("minWetness"));
            paddedContainer.Add(minWetness);

            PropertyField maxWetness = new PropertyField();
            maxWetness.BindProperty(serializedObject.FindProperty("maxWetness"));
            paddedContainer.Add(maxWetness);

            Container.Add(paddedContainer);

            PropertyField updateRainRipples = new PropertyField();
            updateRainRipples.BindProperty(serializedObject.FindProperty("updateRainRipples"));
            Container.Add(updateRainRipples);

            PropertyField updatePuddles = new PropertyField();
            updatePuddles.BindProperty(serializedObject.FindProperty("updatePuddles"));
            Container.Add(updatePuddles);

            PropertyField updateStreams = new PropertyField();
            updateStreams.BindProperty(serializedObject.FindProperty("updateStreams"));
            Container.Add(updateStreams);

            PropertyField updateSnow = new PropertyField();
            updateSnow.BindProperty(serializedObject.FindProperty("updateSnow"));
            Container.Add(updateSnow);

            PropertyField updateWindStrength = new PropertyField();
            updateWindStrength.BindProperty(serializedObject.FindProperty("updateWindStrength"));
            Container.Add(updateWindStrength);

            PropertyField updateFrequency = new PropertyField();
            updateFrequency.BindProperty(serializedObject.FindProperty("updateFrequency"));
            UpdateContainer.Add(updateFrequency);

            serializedObject.ApplyModifiedProperties();


            return root;

        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/microsplat-module");
        }


    }
}