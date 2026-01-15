using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyPureNatureModule))]
    public class CozyPureNatureModuleEditor : CozyModuleEditor
    {

        CozyPureNatureModule module;
        public override ModuleCategory Category => ModuleCategory.integration;
        public override string ModuleTitle => "Pure Nature";
        public override string ModuleSubtitle => "Pure Nature 2 Integration";
        public override string ModuleTooltip => "Directly integrate with Pure Nature 2 by BK.";

        public VisualElement Container => root.Q<VisualElement>("current-settings-container");
        Button widget;
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            module = (CozyPureNatureModule)target;
        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
            status.text = "Integrated";

            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/pure-nature-module-editor.uxml"
            );

            asset.CloneTree(root);
            
            root.Bind(serializedObject);

            return root;

        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/pure-nature-2-module");
        }


    }
}