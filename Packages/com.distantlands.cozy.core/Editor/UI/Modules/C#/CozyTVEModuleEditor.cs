using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyTVEModule))]
    public class CozyTVEModuleEditor : CozyModuleEditor
    {
        CozyTVEModule tveModule;
        public override ModuleCategory Category => ModuleCategory.integration;
        public override string ModuleTitle => "TVE";
        public override string ModuleSubtitle => "The Visual Engine Integration";
        public override string ModuleTooltip => "Directly integrate with The Visual Engine by Boxophobic.";

        public VisualElement Container => root.Q<VisualElement>("current-settings-container");
        Button widget;
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            tveModule = (CozyTVEModule)target;
        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
#if THE_VISUAL_ENGINE
            status.text = "The Visual Engine";
#elif THE_VEGETATION_ENGINE
            status.text = "The Vegetation Engine";
#else
            status.text = "TVE not installed";
#endif
            return widget;
        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/tve-module-editor.uxml"
            );

            asset.CloneTree(root);

#if THE_VEGETATION_ENGINE
            if (!tveModule.globalControl || !tveModule.globalMotion)
            {
                HelpBox globalControlWarning = new HelpBox("Make sure that you have active TVE Global Motion and TVE Global Control objects in your scene!", HelpBoxMessageType.Warning);
                Container.Add(globalControlWarning);
                return root;
            }

            PropertyField updateFrequency = new PropertyField();
            updateFrequency.BindProperty(serializedObject.FindProperty("updateFrequency"));
            Container.Add(updateFrequency);

            // Add Control Toggle Settings
            Container.Add(CreateControlsHeader());

            PropertyField enableMotion = new PropertyField();
            enableMotion.BindProperty(serializedObject.FindProperty("enableMotionControl"));
            Container.Add(enableMotion);

            PropertyField enableSeason = new PropertyField();
            enableSeason.BindProperty(serializedObject.FindProperty("enableSeasonControl"));
            Container.Add(enableSeason);

            PropertyField enableWetness = new PropertyField();
            enableWetness.BindProperty(serializedObject.FindProperty("enableWetnessControl"));
            Container.Add(enableWetness);

            PropertyField enableSnow = new PropertyField();
            enableSnow.BindProperty(serializedObject.FindProperty("enableSnowControl"));
            Container.Add(enableSnow);

            // Add TVE References
            Container.Add(CreateReferencesHeader());

            PropertyField globalControl = new PropertyField();
            globalControl.BindProperty(serializedObject.FindProperty("globalControl"));
            Container.Add(globalControl);

            PropertyField globalMotion = new PropertyField();
            globalMotion.BindProperty(serializedObject.FindProperty("globalMotion"));
            Container.Add(globalMotion);

#elif THE_VISUAL_ENGINE
            if (!tveModule.visualManager)
            {
                HelpBox globalControlWarning = new HelpBox("Make sure that you have active TVE Visual Manager in your scene!", HelpBoxMessageType.Warning);
                Container.Add(globalControlWarning);
                return root;
            }
            
            PropertyField updateFrequency = new PropertyField();
            updateFrequency.BindProperty(serializedObject.FindProperty("updateFrequency"));
            Container.Add(updateFrequency);

            // Add Control Toggle Settings
            Container.Add(CreateControlsHeader());

            PropertyField enableMotion = new PropertyField();
            enableMotion.BindProperty(serializedObject.FindProperty("enableMotionControl"));
            Container.Add(enableMotion);

            PropertyField enableSeason = new PropertyField();
            enableSeason.BindProperty(serializedObject.FindProperty("enableSeasonControl"));
            Container.Add(enableSeason);

            PropertyField enableWetness = new PropertyField();
            enableWetness.BindProperty(serializedObject.FindProperty("enableWetnessControl"));
            Container.Add(enableWetness);

            PropertyField enableSnow = new PropertyField();
            enableSnow.BindProperty(serializedObject.FindProperty("enableSnowControl"));
            Container.Add(enableSnow);

            // Add TVE References
            Container.Add(CreateReferencesHeader());
            
            PropertyField visualManager = new PropertyField();
            visualManager.BindProperty(serializedObject.FindProperty("visualManager"));
            Container.Add(visualManager);

#else
            HelpBox vegetationEngineWarning = new HelpBox("The Visual Engine is not currently in this project! Please make sure that it has been properly downloaded before using this module.", HelpBoxMessageType.Warning);
            Container.Add(vegetationEngineWarning);
#endif

            return root;
        }

        private Label CreateControlsHeader()
        {
            Label header = new Label("Control Settings");
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.marginTop = 10;
            header.style.marginBottom = 5;
            return header;
        }

        private Label CreateReferencesHeader()
        {
            Label header = new Label("TVE References");
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.marginTop = 10;
            header.style.marginBottom = 5;
            return header;
        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/tve-module");
        }
    }
}