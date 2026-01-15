using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozySaveLoadModule))]
    public class CozySaveLoadModuleEditor : CozyModuleEditor
    {

        CozySaveLoadModule saveLoadModule;
        public override ModuleCategory Category => ModuleCategory.utility;
        public override string ModuleTitle => "Save & Load";
        public override string ModuleSubtitle => "Data Management Module";
        public override string ModuleTooltip => "Manage save and load commands within the COZY system.";

        public VisualElement Container => root.Q<VisualElement>("current-settings-container");
        Button widget;
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            saveLoadModule = (CozySaveLoadModule)target;
        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
            status.text = "";

            return widget;

        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Save"), false, saveLoadModule.Save);
            menu.AddItem(new GUIContent("Load"), false, saveLoadModule.Load);
            menu.AddSeparator("");
        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            Label label = new Label("Commands");
            label.AddToClassList("h1");
            root.Add(label);

            VisualElement container = new VisualElement();
            container.AddToClassList("section-bg");
            PopupField<int> popup = new PopupField<int>("Save Slot", new List<int>() { 0, 1, 2, 3, 4 }, 0);
            container.Add(popup);

            VisualElement buttonHolder = new VisualElement();
            // buttonHolder.AddToClassList("flex-row");
            Button saveButton = new Button();
            saveButton.text = "Save";
            saveButton.RegisterCallback<ClickEvent>((ClickEvent) => { 
                saveLoadModule.Save(popup.value);
             });
            Button loadButton = new Button();
            loadButton.text = "Load";
            loadButton.RegisterCallback<ClickEvent>((ClickEvent) => { 
                saveLoadModule.Load(popup.value);
             });
            buttonHolder.Add(saveButton);
            buttonHolder.Add(loadButton);
            container.Add(buttonHolder);

            root.Add(container);


            return root;

        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/save-and-load-module");
        }


    }
}