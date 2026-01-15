using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ModuleWidget : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ModuleTabSelector> { }
    public ModuleWidget()
    {
        Init(GUIContent.none, "");
    }

    private Image Image => this.Q<Image>("icon");
    private Label Label => this.Q<Label>("name");
    private Label DynamicStatus => this.Q<Label>("dynamic-status");

    public ModuleWidget(GUIContent content)
    {
        Init(content, "Yep. It works :)");
    }
    public ModuleWidget(GUIContent content, string status)
    {
        Init(content, status);
    }

    public void Init(
        GUIContent module, 
        string dynamicStatus
    )
    {

        

    }

}
