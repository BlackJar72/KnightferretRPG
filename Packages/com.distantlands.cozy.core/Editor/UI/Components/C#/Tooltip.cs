using DistantLands.Cozy;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

class Tooltip : VisualElement
{

    public new class UxmlFactory : UxmlFactory<Tooltip, UxmlTraits> { }

    private Label MessageBox => this.Q<Label>();

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlStringAttributeDescription message =
            new UxmlStringAttributeDescription { name = "message", defaultValue = "This is a helpful tooltip for the user that explains a particular part of the COZY Ecosystem in an easy to digest manner." };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as Tooltip;

            ate.message = message.GetValueFromBag(bag, cc);
            ate.Init();
        }
    }

    public Tooltip()
    {
        Init();
    }

    public Tooltip(string message)
    {
        Init(message);
    }

    public void Init(string message)
    {
        Clear();

        style.display = CozyWeather.Tooltips ? DisplayStyle.Flex : DisplayStyle.None;

        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/tooltip.uxml"
        );

        asset.CloneTree(this);
        MessageBox.text = message;

    }

    public void Init()
    {
        Clear();

        style.display = CozyWeather.Tooltips ? DisplayStyle.Flex : DisplayStyle.None;

        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/tooltip.uxml"
        );

        asset.CloneTree(this);
        MessageBox.text = message;

    }

    public string message { get; set; }
}