using DistantLands.Cozy;
using DistantLands.Cozy.EditorScripts;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace DistantLands.Cozy.EditorScripts
{
    public class CozyTimeOverlay : PopupWindowContent
    {

        Slider CurrentTime => root.Q<Slider>("time");
        IntegerField CurrentDay => root.Q<IntegerField>("day");
        IntegerField CurrentYear => root.Q<IntegerField>("year");

        VisualElement root;

        public VisualElement CreatePanelContent()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Window/UXML/cozy-time-tools.uxml"
            );

            asset.CloneTree(root);

            CurrentTime.value = CozyWeather.instance.dayPercentage;
            CurrentTime.RegisterCallback((ChangeEvent<float> evt) =>
            {
                CozyWeather.instance.timeModule.currentTime = evt.newValue;
            });

            CurrentDay.value = CozyWeather.instance.timeModule.currentDay;
            CurrentDay.RegisterCallback((ChangeEvent<int> evt) =>
            {
                CozyWeather.instance.timeModule.currentDay = evt.newValue;
            });

            CurrentYear.value = CozyWeather.instance.timeModule.currentYear;
            CurrentYear.RegisterCallback((ChangeEvent<int> evt) =>
            {
                CozyWeather.instance.timeModule.currentYear = evt.newValue;
            });

            return root;

        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 150);
        }

        public override void OnGUI(Rect rect)
        {
            // Intentionally left empty
        }

        public override void OnOpen()
        {
            editorWindow.rootVisualElement.Add(CreatePanelContent());
        }

        public override void OnClose()
        {

        }

    }

    public class CozySceneToolsOverlay : PopupWindowContent
    {

        VisualElement root;

        public VisualElement CreatePanelContent()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Window/UXML/cozy-scene-tools.uxml"
            );

            asset.CloneTree(root);

            root.Q<Button>("local-biome").RegisterCallback((ClickEvent evt) =>
            {
                CozySceneTools.CreateLocalBiome();
            });
            root.Q<Button>("global-biome").RegisterCallback((ClickEvent evt) =>
            {
                CozySceneTools.CreateGlobalBiome();
            });

            root.Q<Button>("fog-culling").RegisterCallback((ClickEvent evt) =>
            {
                CozySceneTools.CreateFogCullingZone();
            });
            root.Q<Button>("fx-block").RegisterCallback((ClickEvent evt) =>
            {
                CozySceneTools.CreateFXBlockZone();
            });

            
            root.Q<Button>("volume").RegisterCallback((ClickEvent evt) =>
            {
                CozySceneTools.CreateVolume();
            });

            return root;

        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 250);
        }

        public override void OnGUI(Rect rect)
        {
            // Intentionally left empty
        }

        public override void OnOpen()
        {
            editorWindow.rootVisualElement.Add(CreatePanelContent());
        }

        public override void OnClose()
        {

        }

    }


}

[EditorToolbarElement(id, typeof(SceneView))]
class TimeDropdown : EditorToolbarDropdown
{
    public const string id = "COZY Tools/Time";

    public TimeDropdown()
    {
        icon = (Texture2D)Resources.Load("Icons/Modules/Time");
        clicked += ShowDropdown;
    }

    void ShowDropdown()
    {
        PopupWindow.Show(worldBound, new CozyTimeOverlay());

    }
}

[EditorToolbarElement(id, typeof(SceneView))]
class SceneToolsDropdown : EditorToolbarDropdown
{
    public const string id = "COZY Tools/Scene Generation";

    public SceneToolsDropdown()
    {
        icon = (Texture2D)EditorGUIUtility.IconContent("SceneViewTools").image;
        clicked += ShowDropdown;
    }

    void ShowDropdown()
    {
        PopupWindow.Show(worldBound, new CozySceneToolsOverlay());

    }
}

[EditorToolbarElement(id, typeof(SceneView))]
class ToggleGizmos : EditorToolbarToggle
{
    public const string id = "COZY Tools/Toggle Gizmos";
    public ToggleGizmos()
    {
        icon = (Texture2D)EditorGUIUtility.IconContent("d_GizmosToggle On").image;
        this.RegisterValueChangedCallback(Test);
        value = CozyWeather.DisplayGizmos;
    }

    void Test(ChangeEvent<bool> evt)
    {
        CozyWeather.DisplayGizmos = evt.newValue;
    }
}

[EditorToolbarElement(id, typeof(SceneView))]
class ToggleFog : EditorToolbarToggle
{
    public const string id = "COZY Tools/Show Fog";
    public ToggleFog()
    {
        icon = (Texture2D)EditorGUIUtility.IconContent("d_SceneViewFx").image;
        this.RegisterValueChangedCallback(Test);
        value = CozyWeather.SceneFogRendering;
    }

    void Test(ChangeEvent<bool> evt)
    {
        CozyWeather.SceneFogRendering = evt.newValue;
    }
}

[EditorToolbarElement(id, typeof(SceneView))]
class ToggleFollowSceneCamera : EditorToolbarToggle
{
    public const string id = "COZY Tools/Follow Scene Camera";
    public ToggleFollowSceneCamera()
    {
        icon = (Texture2D)EditorGUIUtility.IconContent("d_SceneViewCamera").image;
        this.RegisterValueChangedCallback(Test);
        value = CozyWeather.FollowEditorCamera;
    }

    void Test(ChangeEvent<bool> evt)
    {
        CozyWeather.FollowEditorCamera = evt.newValue;
    }
}




[Overlay(typeof(SceneView), "COZY Tools")]
[Icon("Packages/com.distantlands.cozy.core/Editor/Resources/Promo/Distant Lands Watermark.png")]
public class EditorToolbarExample : ToolbarOverlay
{
    EditorToolbarExample() : base(
        SceneToolsDropdown.id,
        TimeDropdown.id
        )
    { }
}
