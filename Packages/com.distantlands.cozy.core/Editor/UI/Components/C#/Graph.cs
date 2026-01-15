using DistantLands.Cozy;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

class Graph : VisualElement
{
    public new class UxmlFactory : UxmlFactory<Graph> { }
    public Graph()
    {
        Init();
    }

    public void Init()
    {
        style.display = CozyWeather.Graphs ? DisplayStyle.Flex : DisplayStyle.None;
    }

}