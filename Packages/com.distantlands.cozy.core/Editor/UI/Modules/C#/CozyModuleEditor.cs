using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyModule))]
    public class CozyModuleEditor : Editor
    {

        public virtual string ModuleTitle => "Custom Control";
        public virtual string ModuleTooltip => "Extend this class to create your own custom modules";
        public virtual string ModuleSubtitle => "User Generated COZY Module";
        public Texture2D ModuleIcon => Resources.Load<Texture2D>($"Icons/Modules/{ModuleTitle}");
        public Texture2D BannerBackground => Resources.Load<Texture2D>($"Banners/{ModuleTitle}");

        public CozyWeatherEditor weatherEditor;

        public enum ModuleCategory
        {
            atmosphere,
            time,
            ecosystem,
            utility,
            survival,
            integration,
            other
        }

        public virtual ModuleCategory Category => ModuleCategory.other;


        public virtual void GetDebugInformation()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.NextVisible(true);

            while (true)
            {
                if (iterator == null)
                    break;

                EditorGUILayout.PropertyField(iterator, true);
                if (iterator.hasChildren)
                {
                    if (!iterator.NextVisible(false))
                        break;
                    continue;
                }

                if (!iterator.NextVisible(true))
                    break;

            }

            serializedObject.ApplyModifiedProperties();

        }

        public virtual void GetReportsInformation()
        {



        }

        public void RemoveModule()
        {
            CozyWeather.instance.DeintitializeModule((CozyModule)target);
            weatherEditor.RepaintUI();
        }

        public void ResetModule()
        {
            CozyWeather.instance.ResetModule((CozyModule)target);
        }

        public void EditScript()
        {
            MonoScript script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
            AssetDatabase.OpenAsset(script, 1);
        }

        public void OpenContextMenu()
        {

            GenericMenu menu = new GenericMenu();
            AddContextMenuItems(menu);
            menu.AddItem(new GUIContent("Documentation"), false, OpenDocumentationURL);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Reset"), false, ResetModule);
            menu.AddItem(new GUIContent("Remove Module"), false, RemoveModule);
            menu.AddItem(new GUIContent("Edit Script"), false, EditScript);

            menu.ShowAsContext();

        }

        public virtual void AddContextMenuItems(GenericMenu menu)
        {

        }

        public virtual void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/");
        }

        public override void OnInspectorGUI()
        {

        }

        public virtual Button DisplayWidget()
        {
            return SmallWidget();
        }

        public Button SmallWidget()
        {
            VisualElement root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/small-widget.uxml"
            );

            asset.CloneTree(root);

            Button widget = root.Q<Button>("widget-button");
            widget.AddToClassList("module-widget");
            widget.RegisterCallback<ClickEvent>(OpenModuleUI);
            widget.RegisterCallback<ContextClickEvent>((ContextClickEvent) => { OpenContextMenu(); });
            widget.tooltip = ModuleTooltip;

            Image icon = new Image
            {
                image = ModuleIcon,
                name = "icon"
            };

            widget.Q("icon").Add(icon);

            Label title = widget.Q<Label>("title");
            title.text = ModuleTitle;
            title.tooltip = ModuleTooltip;

            return widget;
        }

        public Button LargeWidget()
        {
            VisualElement root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/large-widget.uxml"
            );

            asset.CloneTree(root);

            Button widget = root.Q<Button>("widget-button");
            widget.AddToClassList("module-widget");
            widget.RegisterCallback<ClickEvent>(OpenModuleUI);
            widget.RegisterCallback<ContextClickEvent>((ContextClickEvent) => { OpenContextMenu(); });
            widget.tooltip = ModuleTooltip;

            Image icon = new Image
            {
                image = ModuleIcon,
                name = "icon"
            };

            widget.Q("icon").Add(icon);

            Label title = widget.Q<Label>("title");
            title.text = ModuleTitle;
            title.tooltip = ModuleTooltip;

            return widget;
        }

        public virtual VisualElement DisplayUI()
        {
            VisualElement root = new VisualElement();


            return root;

        }

        public void OpenModuleUI(ClickEvent evt)
        {

        }

    }
}