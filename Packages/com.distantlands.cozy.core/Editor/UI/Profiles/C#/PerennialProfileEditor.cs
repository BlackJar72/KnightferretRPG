using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(PerennialProfile))]
    public class PerennialProfileEditor : Editor
    {

        SerializedProperty modulateTimeSpeed;
        SerializedProperty startTime;
        SerializedProperty pauseTime;
        SerializedProperty timeSpeedMultiplier;
        SerializedProperty useRealisticYear;
        SerializedProperty useLeapYear;
        SerializedProperty daysPerYear;
        SerializedProperty standardYear;
        SerializedProperty leapYear;
        PerennialProfile prof;
        VisualElement root;

        public VisualElement MovementContainer => root.Q<VisualElement>("movement-container");
        public VisualElement YearContainer => root.Q<VisualElement>("year-container");

        void OnEnable()
        {

            modulateTimeSpeed = serializedObject.FindProperty("modulateTimeSpeed");
            timeSpeedMultiplier = serializedObject.FindProperty("timeSpeedMultiplier");
            standardYear = serializedObject.FindProperty("standardYear");
            leapYear = serializedObject.FindProperty("leapYear");
            startTime = serializedObject.FindProperty("startTime");
            pauseTime = serializedObject.FindProperty("pauseTime");
            useRealisticYear = serializedObject.FindProperty("realisticYear");
            useLeapYear = serializedObject.FindProperty("useLeapYear");
            daysPerYear = serializedObject.FindProperty("daysPerYear");
            prof = (PerennialProfile)target;

        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Profiles/UXML/perennial-profile-editor.uxml"
            );

            asset.CloneTree(root);

            Toggle pauseTimeElement = new Toggle("Pause Time");

            VisualElement pauseTimeContainer = new VisualElement();
            pauseTimeElement.BindProperty(pauseTime);
            pauseTimeElement.AddToClassList("unity-base-field__aligned");
            pauseTimeContainer.SetEnabled(!pauseTimeElement.value);
            MovementContainer.Add(pauseTimeElement);
            pauseTimeElement.RegisterCallback<ClickEvent>((ClickEvent) => { pauseTimeContainer.SetEnabled(!pauseTimeContainer.enabledSelf); });

            pauseTimeContainer.SetEnabled(!serializedObject.FindProperty("pauseTime").boolValue);
            pauseTimeElement.AddToClassList("mb-md");

            PropertyField timeMovementSpeed = new PropertyField();
            timeMovementSpeed.BindProperty(serializedObject.FindProperty("timeMovementSpeed"));
            pauseTimeContainer.Add(timeMovementSpeed);

            PropertyField modulateTimeSpeedField = new PropertyField();
            modulateTimeSpeedField.BindProperty(modulateTimeSpeed);
            pauseTimeContainer.Add(modulateTimeSpeedField);

            PropertyField timeSpeedMultiplierField = new PropertyField();
            timeSpeedMultiplierField.BindProperty(timeSpeedMultiplier);
            timeSpeedMultiplierField.AddToClassList("pl-4");
            timeSpeedMultiplierField.style.display = modulateTimeSpeed.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            modulateTimeSpeedField.RegisterCallback<ClickEvent>((ClickEvent) => { timeSpeedMultiplierField.style.display = modulateTimeSpeed.boolValue ? DisplayStyle.Flex : DisplayStyle.None; });
            timeSpeedMultiplierField.AddToClassList("mb-md");
            pauseTimeContainer.Add(timeSpeedMultiplierField);

            PropertyField resetTimeOnStart = new PropertyField();
            resetTimeOnStart.BindProperty(serializedObject.FindProperty("resetTimeOnStart"));
            pauseTimeContainer.Add(resetTimeOnStart);

            PropertyField startTimeField = new PropertyField();
            startTimeField.style.display = serializedObject.FindProperty("resetTimeOnStart").boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            startTimeField.BindProperty(startTime);
            startTimeField.AddToClassList("pl-4");
            pauseTimeContainer.Add(startTimeField);
            resetTimeOnStart.RegisterCallback<ClickEvent>((ClickEvent) => { startTimeField.style.display = serializedObject.FindProperty("resetTimeOnStart").boolValue ? DisplayStyle.Flex : DisplayStyle.None; });

            MovementContainer.Add(pauseTimeContainer);

            Toggle realisticYear = new Toggle()
            {
                label = "Use Realistic Year"
            };
            realisticYear.BindProperty(useRealisticYear);
            realisticYear.AddToClassList("unity-base-field__aligned");
            YearContainer.Add(realisticYear);

            PropertyField daysPerYearElement = new PropertyField();
            daysPerYearElement.BindProperty(daysPerYear);
            daysPerYearElement.style.display = useRealisticYear.boolValue ? DisplayStyle.None : DisplayStyle.Flex;
            YearContainer.Add(daysPerYearElement);

            VisualElement realisticYearVarsElement = new VisualElement();
            realisticYearVarsElement.style.display = useRealisticYear.boolValue ? DisplayStyle.Flex : DisplayStyle.None;

            realisticYear.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                realisticYearVarsElement.style.display = useRealisticYear.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
                daysPerYearElement.style.display = useRealisticYear.boolValue ? DisplayStyle.None : DisplayStyle.Flex;
            });

            Toggle useLeapYearElement = new Toggle()
            {
                label = "Use Leap Year"
            };
            useLeapYearElement.AddToClassList("unity-base-field__aligned");
            useLeapYearElement.BindProperty(useLeapYear);
            useLeapYearElement.AddToClassList("mb-md");
            realisticYearVarsElement.Add(useLeapYearElement);

            PropertyField standardYearElement = new PropertyField();
            standardYearElement.BindProperty(standardYear);
            realisticYearVarsElement.Add(standardYearElement);


            PropertyField leapYearElement = new PropertyField();
            leapYearElement.BindProperty(leapYear);
            leapYearElement.SetEnabled(useLeapYear.boolValue);
            useLeapYearElement.RegisterCallback<ClickEvent>((ClickEvent) => { leapYearElement.SetEnabled(useLeapYear.boolValue); });
            realisticYearVarsElement.Add(leapYearElement);

            YearContainer.Add(realisticYearVarsElement);

            return root;
        }

    }
}