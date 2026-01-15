using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SetupCheck : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ModuleTabSelector> { }
    public SetupCheck()
    {
        Init();
    }

    public SetupCheck(string _text, string _fixButtonText, Func<bool> _status, Action _fix)
    {
        text = _text;
        status = _status;
        fix = _fix;
        fixButtonText = _fixButtonText;

        Init();
    }
    public SetupCheck(string _text, string _installButtonText, bool _package, Func<bool> _status, Action _fix)
    {
        text = _text;
        status = _status;
        fix = _fix;
        fixButtonText = _installButtonText;
        package = _package;

        Init();
    }

    public string text = "";
    public string fixButtonText = "Fix";
    public string reinstallButtonText = "Reinstall";
    public bool package = false;
    public Func<bool> status = () => { return true; };
    public Action fix = () => { Debug.Log("Configure fixes!"); };

    private Label SetupLabel => this.Q<Label>("setup-label");
    private VisualElement StatusIcon => this.Q<VisualElement>("status-icon");
    private Button FixButton => this.Q<Button>("fix-button");

    public void Init()
    {
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/setup-check.uxml"
        );

        asset.CloneTree(this);
        bool currentStatus = status.Invoke();

        SetupLabel.text = text;
        StatusIcon.style.display = !package || currentStatus ? DisplayStyle.Flex : DisplayStyle.None;
        StatusIcon.style.backgroundImage = currentStatus ?
            (StyleBackground)EditorGUIUtility.IconContent("Installed").image :
            (StyleBackground)EditorGUIUtility.IconContent("Warning").image;
        FixButton.style.display = package || !currentStatus ? DisplayStyle.Flex : DisplayStyle.None;

        FixButton.text = package && currentStatus ? reinstallButtonText : fixButtonText;
        FixButton.RegisterCallback((ClickEvent evt) =>
        {
            fix.Invoke();
        });

    }

}
