using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StatusMessage : VisualElement
{
    public new class UxmlFactory : UxmlFactory<StatusMessage> { }
    public StatusMessage()
    {
        Init(0);
    }

    private Image Image => this.Q<Image>("status-icon");

    public StatusMessage(int messageType)
    {
        Init(messageType);
    }

    public void UpdateStatus(bool goodStatus)
    {
        if (goodStatus)
            Image.image = EditorGUIUtility.IconContent("Installed").image;
        else
            Image.image = EditorGUIUtility.IconContent("Warning").image;
    }

    public void Init(
        int version
    )
    {
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/status-message.uxml"
        );

        asset.CloneTree(this);
        Image.image = EditorGUIUtility.IconContent("Warning").image;

    }

}
