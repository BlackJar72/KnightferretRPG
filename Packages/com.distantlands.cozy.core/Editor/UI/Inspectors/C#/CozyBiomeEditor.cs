using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using System;
using UnityEditor.Experimental.GraphView;
using DistantLands.Cozy.Data;


namespace DistantLands.Cozy.EditorScripts
{

    [CustomEditor(typeof(CozyBiome))]
    public class CozyBiomeEditor : Editor
    {

        CozyBiome biome;

        public List<CozyModuleEditor> moduleEditors = new List<CozyModuleEditor>();
        public VisualElement root;
        public VisualElement ModuleContainer => root.Q<VisualElement>("module-container");
        public VisualElement LocalSettingsContainer => root.Q<VisualElement>("local-settings-container");
        public VisualElement DistanceContainer => root.Q<VisualElement>("distance");
        public VisualElement TimeContainer => root.Q<VisualElement>("time");
        public EnumField ModeSelector => root.Q<EnumField>("mode-selector");
        public EnumField TransitionSelector => root.Q<EnumField>("transition-selector");
        public Button AddModule => root.Q<Button>("add-module-button");


        List<Type> mods;

        public static CozyWeatherEditor instance;



        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            if (!target)
                return;

            biome = (CozyBiome)target;
        }

        public void ResetModuleEditors()
        {
            ModuleContainer.Clear();
            moduleEditors.Clear();

            for (int i = 0; i < biome.modules.Count; i++)
            {
                CozyBiomeModuleEditor moduleEditor = (CozyBiomeModuleEditor)CreateEditor(biome.modules[i] as UnityEngine.Object);
                if (moduleEditor == null)
                    continue;

                moduleEditors.Add(moduleEditor);
            }

            moduleEditors.RemoveAll(x => x == null);

            foreach (CozyBiomeModuleEditor editor in moduleEditors)
            {
                Label heading = new Label(editor.ModuleTitle);
                heading.AddToClassList("h2");
                ModuleContainer.Add(heading);
                VisualElement ui = editor.DisplayBiomeUI();
                ui.AddToClassList("section-bg");
                ModuleContainer.Add(ui);

            }


        }

        public override VisualElement CreateInspectorGUI()
        {

            root = new VisualElement();

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Inspectors/UXML/cozy-biome.uxml"
            );

            asset.CloneTree(root);
            ResetModuleEditors();

            SetVisible(LocalSettingsContainer, biome.mode == CozyBiome.BiomeMode.Local);
            ModeSelector.RegisterValueChangedCallback((evt) =>
            {
                SetVisible(LocalSettingsContainer, biome.mode == CozyBiome.BiomeMode.Local);
            });

            SetVisible(DistanceContainer, biome.transitionMode == CozyBiome.TransitionMode.Distance);
            SetVisible(TimeContainer, biome.transitionMode == CozyBiome.TransitionMode.Time);
            TransitionSelector.RegisterValueChangedCallback((evt) =>
            {
                SetVisible(DistanceContainer, biome.transitionMode == CozyBiome.TransitionMode.Distance);
                SetVisible(TimeContainer, biome.transitionMode == CozyBiome.TransitionMode.Time);
            });


            AddModule.RegisterCallback<ClickEvent>((evt) =>
            {

                mods = EditorUtilities.ResetBiomeModulesList();

                if (mods.Contains(typeof(ICozyBiomeModule)))
                    mods.Remove(typeof(ICozyBiomeModule));

                if (mods.Contains(typeof(CozyBiomeModuleBase<>)))
                    mods.Remove(typeof(CozyBiomeModuleBase<>));

                foreach (ICozyBiomeModule a in biome.GetComponents<ICozyBiomeModule>())
                    if (mods.Contains(a.GetType()))
                        mods.Remove(a.GetType());

                BiomeModulesSearchProvider provider = CreateInstance<BiomeModulesSearchProvider>();
                provider.modules = mods;
                provider.biome = biome;
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);

            });

            return root;
        }

        public void SetVisible(VisualElement element, bool condition)
        {
            element.style.display = condition ? DisplayStyle.Flex : DisplayStyle.None;
        }

    }
}