using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ModuleWidgetGroup : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ModuleWidgetGroup> { }
    public ModuleWidgetGroup()
    {
        Init();
    }

    private VisualElement TabGroup => this.Q<VisualElement>("tab-group");
    private Label Header => this.Q<Label>("widget-group-header");

    public void AddWidget(VisualElement element)
    {
        TabGroup.Add(element);
    }

    public void Init(
    )
    {
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/module-widget-carousel.uxml"
        );

        asset.CloneTree(this);


    }
    public ModuleWidgetGroup(
        string headerTitle
    )
    {
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/module-widget-carousel.uxml"
        );

        asset.CloneTree(this);
        Header.text = headerTitle;

    }

}
