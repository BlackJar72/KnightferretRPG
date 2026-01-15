using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyAtmosphereModule))]
    public class CozyAtmosphereModuleEditor : CozyBiomeModuleEditor
    {

        CozyAtmosphereModule atmosphereModule;
        public override ModuleCategory Category => ModuleCategory.atmosphere;
        public override string ModuleTitle => "Atmosphere";
        public override string ModuleSubtitle => "Skydome Module";
        public override string ModuleTooltip => "Manage skydome, fog, and lighting settings.";


        public VisualElement ProfileContainer => root.Q<VisualElement>("profile-container");
        VisualElement root;


        void OnEnable()
        {
            if (target && target.GetType() == typeof(CozyAtmosphereModule))
                atmosphereModule = (CozyAtmosphereModule)target;
        }

        public override Button DisplayWidget()
        {
            Button widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
            status.Bind(serializedObject);
            if (atmosphereModule.atmosphereProfile)
                status.text = atmosphereModule.atmosphereProfile.name;
            else
                status.text = "Please set an atmosphere";
            widget.Q<Label>("dynamic-status").style.fontSize = 8;
            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/atmosphere-module-editor.uxml"
            );

            asset.CloneTree(root);

            CozyProfileField<AtmosphereProfile> atmosphereProfile = new CozyProfileField<AtmosphereProfile>(serializedObject.FindProperty("atmosphereProfile"));
            ProfileContainer.Add(atmosphereProfile);

            InspectorElement inspector = new InspectorElement(atmosphereModule.atmosphereProfile);
            inspector.AddToClassList("p-0");
            root.Add(inspector);

            return root;

        }

        public override VisualElement DisplayBiomeUI()
        {
            root = new VisualElement();

            CozyProfileField<AtmosphereProfile> atmosphereProfile = new CozyProfileField<AtmosphereProfile>(serializedObject.FindProperty("atmosphereProfile"));
            root.Add(atmosphereProfile);

            return root;

        }


        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/atmosphere-module");
        }


    }
}