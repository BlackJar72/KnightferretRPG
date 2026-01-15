using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ModuleTabSelector : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ModuleTabSelector> { }
    public ModuleTabSelector()
    {
        Init(GUIContent.none, "");
    }

    private Image Image => this.Q<Image>("icon");
    private Label Label => this.Q<Label>("module-name");
    private Label DynamicStatus => this.Q<Label>("dynamic-status");

    public ModuleTabSelector(GUIContent content)
    {
        Init(content, "Yep. It works :)");
    }
    public ModuleTabSelector(GUIContent content, string status)
    {
        Init(content, status);
    }

    public void Init(
        GUIContent module, 
        string dynamicStatus
    )
    {
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/module-tab-selector.uxml"
        );

        asset.CloneTree(this);
        Image.image = module.image;
        Label.text = module.text;
        DynamicStatus.text = dynamicStatus;

        this.tooltip = module.tooltip;

    }

}
