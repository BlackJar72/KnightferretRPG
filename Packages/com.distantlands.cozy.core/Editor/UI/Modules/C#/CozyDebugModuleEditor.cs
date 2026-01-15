using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyDebugModule))]
    public class CozyDebugModuleEditor : CozyModuleEditor
    {

        CozyDebugModule debugModule;
        public override ModuleCategory Category => ModuleCategory.utility;
        public override string ModuleTitle => "Debug";
        public override string ModuleSubtitle => "System Debug Helper";
        public override string ModuleTooltip => "Aids in debugging and testing the COZY system.";


        public VisualElement Container => root.Q<VisualElement>("current-settings-container");
        Button widget;
        VisualElement root;

        void OnEnable()
        {
            if (!target)
                return;

            debugModule = (CozyDebugModule)target;
        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
            status.text = "Debug all modules";

            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();

            foreach (CozyModuleEditor module in weatherEditor.moduleEditors)
            {
                Label label = new Label(module.ModuleTitle);
                label.AddToClassList("h1");
                Label desc = new Label(module.ModuleSubtitle);
                desc.AddToClassList("h2");
                


                IMGUIContainer container = new IMGUIContainer();
                container.onGUIHandler += () =>
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Space(10);
                    module.GetDebugInformation();
                    EditorGUILayout.Space(10);
                    EditorGUI.indentLevel--;
                };
                container.AddToClassList("section-bg");

                root.Add(label);
                root.Add(desc);
                root.Add(container);
            }



            return root;

        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/debug-module");
        }


    }
}