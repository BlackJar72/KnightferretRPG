using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyButoModule))]
    public class CozyButoModuleEditor : CozyModuleEditor
    {

        CozyButoModule butoModule;
        public override ModuleCategory Category => ModuleCategory.integration;
        public override string ModuleTitle => "Buto";
        public override string ModuleSubtitle => "Buto Integration";
        public override string ModuleTooltip => "Directly integrate with Buto by OccaSoftware.";

        public VisualElement Container => root.Q<VisualElement>("settings-container");
        Button widget;
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            butoModule = (CozyButoModule)target;
        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
#if BUTO
            status.text = "Buto recognized";
#else
            status.text = "Buto not installed";
#endif

            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/buto-module-editor.uxml"
            );

            asset.CloneTree(root);
#if BUTO
            if (serializedObject.FindProperty("fog").objectReferenceValue == null)
            {
                HelpBox fogWarning = new HelpBox("Could not find any instance of Buto in your scene! You will have to set the profile manually in the module settings.", HelpBoxMessageType.Warning);
                Container.Add(fogWarning);
            }

            Container.Add(new VisualElement { style = { height = 20 } });

            if (serializedObject.FindProperty("volumeProfile").objectReferenceValue == null)
            {
                PropertyField volumeProfile = new PropertyField();
                volumeProfile.BindProperty(serializedObject.FindProperty("volumeProfile"));
                Container.Add(volumeProfile);
            }

            PropertyField fogBrightnessMultiplier = new PropertyField();
            fogBrightnessMultiplier.BindProperty(serializedObject.FindProperty("fogBrightnessMultiplier"));
            Container.Add(fogBrightnessMultiplier);

            PropertyField fogDensityMultiplier = new PropertyField();
            fogDensityMultiplier.BindProperty(serializedObject.FindProperty("fogDensityMultiplier"));
            Container.Add(fogDensityMultiplier);


#else 
            HelpBox butoWarning = new HelpBox("Buto Volumetric Fog is not currently in this project! Please make sure that it has been properly downloaded before using this module.", HelpBoxMessageType.Warning);
            Container.Add(butoWarning);
#endif

            serializedObject.ApplyModifiedProperties();


            return root;

        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/buto-module");
        }


    }
}