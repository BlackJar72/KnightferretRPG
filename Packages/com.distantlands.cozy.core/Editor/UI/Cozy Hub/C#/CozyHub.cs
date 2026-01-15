using System;
using System.Collections.Generic;
using DistantLands.Cozy;
using DistantLands.Cozy.EditorScripts;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CozyHub : EditorWindow
{

    [MenuItem("Tools/Cozy: Stylized Weather 3/Open Cozy Hub")]
    public static void ShowExample()
    {
        CozyHub wnd = GetWindow<CozyHub>();
        wnd.titleContent = new GUIContent("Cozy Hub");
    }

    public void CreateGUI()
    {
        if (CozyWeather.instance)
        {
            ScrollView scrollView = new ScrollView();
            InspectorElement inspector = new InspectorElement(CozyWeather.instance);

            scrollView.Add(inspector);
            rootVisualElement.Add(scrollView);
        }
        else
        {
            Tooltip tooltip = new Tooltip("No instance of COZY detected! Please set up your scene.");
            rootVisualElement.Add(tooltip);

            Button setupButton = new Button(() => { CozySceneTools.SetupScene(); });
            setupButton.text = "Setup Scene";
            rootVisualElement.Add(setupButton);
        }
    }

}
