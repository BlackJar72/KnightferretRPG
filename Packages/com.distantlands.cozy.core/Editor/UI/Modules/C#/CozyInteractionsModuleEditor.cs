using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyInteractionsModule))]
    public class CozyInteractionsModuleEditor : CozyModuleEditor
    {

        CozyInteractionsModule interactionsModule;
        public override ModuleCategory Category => ModuleCategory.ecosystem;
        public override string ModuleTitle => "Interactions";
        public override string ModuleSubtitle => "Global Modification Module";
        public override string ModuleTooltip => "Modifies and transforms the world based on the COZY system.";

        public VisualElement Container => root.Q<VisualElement>("profile-container");
        public VisualElement ProfileUIContainer => root.Q<VisualElement>("profile-ui");
        SerializedProperty profile;
        Button widget;
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            interactionsModule = (CozyInteractionsModule)target;
            profile = serializedObject.FindProperty("profile");
        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
            status.text = $"{interactionsModule.profile.modulatedValues.Length} Values";

            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/interactions-module-editor.uxml"
            );

            asset.CloneTree(root);

            PropertyField profileField = new PropertyField();
            profileField.BindProperty(profile);
            profileField.RegisterCallback<ChangeEvent<MaterialManagerProfile>>((ChangeEvent<MaterialManagerProfile> evt) =>
            {
                RefreshProfileUI();
            });
            Container.Add(profileField);

            RefreshProfileUI();

            return root;

        }

        public void RefreshProfileUI()
        {
            ProfileUIContainer.Clear();

            if (interactionsModule.profile)
                ProfileUIContainer.Add(new InspectorElement(interactionsModule.profile));

        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/interactions-module");
        }


    }
}