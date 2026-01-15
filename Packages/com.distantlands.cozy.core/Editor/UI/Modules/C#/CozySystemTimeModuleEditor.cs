using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(SystemTimeModule))]
    public class CozySystemTimeModuleEditor : CozyModuleEditor
    {

        SystemTimeModule timeModule;
        public override ModuleCategory Category => ModuleCategory.time;
        public override string ModuleTitle => "System Time";
        public override string ModuleSubtitle => "Time Management Module";
        public override string ModuleTooltip => "Manage your in-game time in terms of the users system time.";
        public VisualElement ProfileContainer => root.Q<VisualElement>("profile-container");
        public VisualElement Container => root.Q<VisualElement>("current-settings-container");
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            timeModule = (SystemTimeModule)target;
        }

        public override Button DisplayWidget()
        {
            Button widget = LargeWidget();
            Label status = widget.Q<Label>("dynamic-status");
            status.text = timeModule.currentTime;
            VisualElement lowerContainer = widget.Q<VisualElement>("lower-container");

            lowerContainer.Add(new Label()
            {
                text = $"Currently it is {timeModule.currentTime.ToString()}"
            });
            Label dayYearLabel = new Label()
            {
                text = $"Day {timeModule.currentDay} of year {timeModule.currentYear}"
            };
            lowerContainer.Add(dayYearLabel);

            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Modules/UXML/system-time-module-editor.uxml"
            );

            asset.CloneTree(root);
            root.Bind(serializedObject);

            return root;

        }

        public void PauseTime(object check)
        {
            timeModule.pauseTime = (bool)check;
        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Pause Time"), false, PauseTime, true);
            menu.AddItem(new GUIContent("Unpause Time"), false, PauseTime, false);
            menu.AddSeparator("");
        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/system-time-module");
        }


    }
}