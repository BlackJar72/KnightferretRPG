using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DistantLands.Cozy.EditorScripts
{
    [CustomEditor(typeof(CozyEventModule))]
    public class CozyEventModuleEditor : CozyBiomeModuleEditor
    {

        CozyEventModule eventModule;
        public override ModuleCategory Category => ModuleCategory.utility;
        public override string ModuleTitle => "Events";
        public override string ModuleSubtitle => "Correlation Module";
        public override string ModuleTooltip => "Setup Unity events that directly integrate into the COZY system.";

        public VisualElement Container => root.Q<VisualElement>("current-settings-container");
        Button widget;
        VisualElement root;

        SerializedProperty onDawn;
        SerializedProperty onMorning;
        SerializedProperty onDay;
        SerializedProperty onAfternoon;
        SerializedProperty onEvening;
        SerializedProperty onTwilight;
        SerializedProperty onNight;
        SerializedProperty onNewMinute;
        SerializedProperty onNewHour;
        SerializedProperty onNewDay;
        SerializedProperty onNewYear;
        SerializedProperty onWeatherProfileChange;
        SerializedProperty cozyEvents;

        void OnEnable()
        {
            if (!target)
                return;

            eventModule = (CozyEventModule)target;

            onDawn = serializedObject.FindProperty("onDawn");
            onMorning = serializedObject.FindProperty("onMorning");
            onDay = serializedObject.FindProperty("onDay");
            onAfternoon = serializedObject.FindProperty("onAfternoon");
            onEvening = serializedObject.FindProperty("onEvening");
            onTwilight = serializedObject.FindProperty("onTwilight");
            onNight = serializedObject.FindProperty("onNight");
            onNewMinute = serializedObject.FindProperty("onNewMinute");
            onNewHour = serializedObject.FindProperty("onNewHour");
            onNewDay = serializedObject.FindProperty("onNewDay");
            onNewYear = serializedObject.FindProperty("onNewYear");
            onWeatherProfileChange = serializedObject.FindProperty("onWeatherProfileChange");
            cozyEvents = serializedObject.FindProperty("cozyEvents");

        }

        public override Button DisplayWidget()
        {
            widget = SmallWidget();
            Label status = widget.Q<Label>("dynamic-status");
            status.text = "";

            return widget;

        }

        public override VisualElement DisplayUI()
        {
            root = new VisualElement();


            Label timeBlocksTitle = new Label()
            {
                text = "Time Blocks"
            };
            timeBlocksTitle.AddToClassList("h1");
            root.Add(timeBlocksTitle);

            VisualElement timeBlocksCategory = new VisualElement();
            timeBlocksCategory.AddToClassList("section-bg");

            PropertyField dawnProperty = new PropertyField();
            dawnProperty.BindProperty(onDawn);
            timeBlocksCategory.Add(dawnProperty);

            PropertyField morningProperty = new PropertyField();
            morningProperty.BindProperty(onMorning);
            timeBlocksCategory.Add(morningProperty);

            PropertyField dayProperty = new PropertyField();
            dayProperty.BindProperty(onDay);
            timeBlocksCategory.Add(dayProperty);

            PropertyField afternoonProperty = new PropertyField();
            afternoonProperty.BindProperty(onAfternoon);
            timeBlocksCategory.Add(afternoonProperty);

            PropertyField eveningProperty = new PropertyField();
            eveningProperty.BindProperty(onEvening);
            timeBlocksCategory.Add(eveningProperty);

            PropertyField twilightProperty = new PropertyField();
            twilightProperty.BindProperty(onTwilight);
            timeBlocksCategory.Add(twilightProperty);

            PropertyField nightProperty = new PropertyField();
            nightProperty.BindProperty(onNight);
            timeBlocksCategory.Add(nightProperty);

            root.Add(timeBlocksCategory);

            Label timeElapsedTitle = new Label()
            {
                text = "Time Elapsed Events"
            };
            timeElapsedTitle.AddToClassList("h1");
            root.Add(timeElapsedTitle);
            
            VisualElement timeElapsedCategory = new VisualElement();
            timeElapsedCategory.AddToClassList("section-bg");

            PropertyField newMinuteProperty = new PropertyField();
            newMinuteProperty.BindProperty(onNewMinute);
            timeElapsedCategory.Add(newMinuteProperty);

            PropertyField newHourProperty = new PropertyField();
            newHourProperty.BindProperty(onNewHour);
            timeElapsedCategory.Add(newHourProperty);

            PropertyField newDayProperty = new PropertyField();
            newDayProperty.BindProperty(onNewDay);
            timeElapsedCategory.Add(newDayProperty);

            PropertyField newYearProperty = new PropertyField();
            newYearProperty.BindProperty(onNewYear);
            timeElapsedCategory.Add(newYearProperty);

            root.Add(timeElapsedCategory);

            Label weatherTitle = new Label()
            {
                text = "Weather Events"
            };
            weatherTitle.AddToClassList("h1");
            root.Add(weatherTitle);
            
            VisualElement weatherEventsCategory = new VisualElement();
            weatherEventsCategory.AddToClassList("section-bg");


            PropertyField weatherProfileChangeProperty = new PropertyField();
            weatherProfileChangeProperty.BindProperty(onWeatherProfileChange);
            weatherEventsCategory.Add(weatherProfileChangeProperty);

            PropertyField cozyEventsProperty = new PropertyField();
            cozyEventsProperty.BindProperty(cozyEvents);
            weatherEventsCategory.Add(cozyEventsProperty);

            root.Add(weatherEventsCategory);


            return root;

        }
        

        public override VisualElement DisplayBiomeUI()
        {
            root = new VisualElement();
            
            PropertyField onEnterBiome = new PropertyField();
            onEnterBiome.BindProperty(serializedObject.FindProperty("onEnterBiome"));
            root.Add(onEnterBiome);

            PropertyField whileInBiome = new PropertyField();
            whileInBiome.BindProperty(serializedObject.FindProperty("whileInBiome"));
            root.Add(whileInBiome);
            
            PropertyField onExitBiome = new PropertyField();
            onExitBiome.BindProperty(serializedObject.FindProperty("onExitBiome"));
            root.Add(onExitBiome);
            
            return root;
        }

        public override void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/how-it-works/modules/events-module");
        }


    }
}