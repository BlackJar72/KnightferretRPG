using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace DistantLands.Cozy.EditorScripts
{
    public class ModulesSearchProvider : ScriptableObject, ISearchWindowProvider
    {

        public List<Type> modules;
        public CozyWeather weather;
        public CozyWeatherEditor cozyWeatherEditor;



        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Select a Module"), 0),
                new SearchTreeGroupEntry(new GUIContent("Atmosphere", CozyModuleEditor.ModuleCategory.atmosphere.ToString()), 1),
                new SearchTreeGroupEntry(new GUIContent("Time", CozyModuleEditor.ModuleCategory.time.ToString()), 1),
                new SearchTreeGroupEntry(new GUIContent("Ecosystem", CozyModuleEditor.ModuleCategory.ecosystem.ToString()), 1),
                new SearchTreeGroupEntry(new GUIContent("Integration", CozyModuleEditor.ModuleCategory.integration.ToString()), 1),
                new SearchTreeGroupEntry(new GUIContent("Utility", CozyModuleEditor.ModuleCategory.utility.ToString()), 1),
                new SearchTreeGroupEntry(new GUIContent("Other", CozyModuleEditor.ModuleCategory.other.ToString()), 1),
            };
            for (int index = modules.Count - 1; index >= 0; index--)
            {
                string level = "";
                Type i = modules[index];

                SearchTreeEntry entry = GetSearchTreeEntry(i.Name, i, out level);
                entries.Insert(entries.FindIndex(x => x.content.text == level) + 1, entry);

            }
            // SearchTreeEntry customModuleEntry = new SearchTreeEntry(new GUIContent("Create New Module", EditorGUIUtility.IconContent("Toolbar Plus").image));
            // customModuleEntry.level = 1;
            // customModuleEntry.userData = typeof(CreateCozyModule);
            // entries.Add(customModuleEntry);
            return entries;
        }

        public SearchTreeEntry GetSearchTreeEntry(string name, Type type, out string category)
        {

            GUIContent content = new GUIContent();
            category = "Other";

            switch (name)
            {
                case "CozyAmbienceModule":
                    content = new GUIContent(" Ambience Module", (Texture)Resources.Load("Icons/Modules/Ambience"));
                    category = "Ecosystem";
                    break;
                case "CozyEventModule":
                    content = new GUIContent(" Events Module", (Texture)Resources.Load("Icons/Modules/Events"));
                    category = "Utility";
                    break;
                case "CozyInteractionsModule":
                    content = new GUIContent(" Interactions Module", (Texture)Resources.Load("Icons/Modules/Interactions"));
                    category = "Ecosystem";
                    break;
                case "CozyMicrosplatModule":
                    content = new GUIContent(" Microsplat Integration", (Texture)Resources.Load("Icons/Modules/Integration"));
                    category = "Integration";
                    break;
                case "CozyReflectionsModule":
                    content = new GUIContent(" Reflection Module", (Texture)Resources.Load("Icons/Modules/Reflection"));
                    category = "Atmosphere";
                    break;
                case "CozyReportsModule":
                    content = new GUIContent(" Reports Module", (Texture)Resources.Load("Icons/Modules/Reports"));
                    category = "Utility";
                    break;
                case "CozyDebugModule":
                    content = new GUIContent(" Debug Module", (Texture)Resources.Load("Icons/Modules/Debug"));
                    category = "Utility";
                    break;
                case "CozySatelliteModule":
                    content = new GUIContent(" Satellite Module", (Texture)Resources.Load("Icons/Modules/Satellite"));
                    category = "Atmosphere";
                    break;
                case "CozySaveLoadModule":
                    content = new GUIContent(" Save/Load Module", (Texture)Resources.Load("Icons/Modules/Save & Load"));
                    category = "Utility";
                    break;
                case "CozyTVEModule":
                    content = new GUIContent(" The Vegetation Engine Integration", (Texture)Resources.Load("Icons/Modules/TVE"));
                    category = "Integration";
                    break;
                case "CozyButoModule":
                    content = new GUIContent(" Buto Integration", (Texture)Resources.Load("Icons/Modules/Buto"));
                    category = "Integration";
                    break;
                case "BlocksModule":
                    content = new GUIContent(" Blocks Module", (Texture)Resources.Load("Icons/Modules/Blocks"));
                    category = "Atmosphere";
                    break;
                case "PlumeModule":
                    content = new GUIContent(" Plume Module", (Texture)Resources.Load("Icons/Modules/Plume"));
                    category = "Atmosphere";
                    break;
                case "CataclysmModule":
                    content = new GUIContent(" Cataclysm Module", (Texture)Resources.Load("Icons/Modules/Cataclysm"));
                    category = "Ecosystem";
                    break;
                case "LinkModule":
                    content = new GUIContent(" Link Module", (Texture)Resources.Load("Icons/Modules/Link"));
                    category = "Integration";
                    break;
                case "CultivateModule":
                    content = new GUIContent(" Cultivate Module", (Texture)Resources.Load("Icons/Modules/Cultivate"));
                    category = "Ecosystem";
                    break;
                case "CozyHabits":
                    content = new GUIContent(" Habits Module", (Texture)Resources.Load("Icons/Modules/Habits"));
                    category = "Time";
                    break;
                case "CozyTransitModule":
                    content = new GUIContent(" Transit Module", (Texture)Resources.Load("Icons/Modules/Transit"));
                    category = "Time";
                    break;
                case "CozyClimateModule":
                    content = new GUIContent(" Climate Module", (Texture)Resources.Load("Icons/Modules/Climate"));
                    category = "Ecosystem";
                    break;
                case "CozyWeatherModule":
                    content = new GUIContent(" Weather Module", (Texture)Resources.Load("Icons/Modules/Weather"));
                    category = "Ecosystem";
                    break;
                case "CozyTimeModule":
                    content = new GUIContent(" Time Module", (Texture)Resources.Load("Icons/Modules/Time"));
                    category = "Time";
                    break;
                case "SystemTimeModule":
                    content = new GUIContent(" System Time Module", (Texture)Resources.Load("Icons/Modules/System Time"));
                    category = "Time";
                    break;
                case "CozyAtmosphereModule":
                    content = new GUIContent(" Atmosphere Module", (Texture)Resources.Load("Icons/Modules/Atmosphere"));
                    category = "Atmosphere";
                    break;
                case "CozyRadarModule":
                    content = new GUIContent(" Radar Module", (Texture)Resources.Load("Icons/Modules/Radar"));
                    category = "Ecosystem";
                    break;
                case "EclipseModule":
                    content = new GUIContent(" Eclipse Module", (Texture)Resources.Load("Icons/Modules/Eclipse"));
                    category = "Atmosphere";
                    break;
                case "CozyControlPanelModule":
                    content = new GUIContent(" Control Panel Module", (Texture)Resources.Load("Control Panel"));
                    category = "Utility";
                    break;
                case "CozyWindModule":
                    content = new GUIContent(" Wind Module", (Texture)Resources.Load("Icons/Modules/Wind"));
                    category = "Ecosystem";
                    break;
                case "CozyMLSModule":
                    content = new GUIContent(" Magic Lightmap Switcher Integration", (Texture)Resources.Load("Icons/Modules/MLS"));
                    category = "Integration";
                    break;
                case "CozyPureNatureModule":
                    content = new GUIContent(" Pure Nature 2 Integration", (Texture)Resources.Load("Icons/Modules/Pure Nature"));
                    category = "Integration";
                    break;
                case "ReSoundModule":
                    content = new GUIContent(" ReSound Module", (Texture)Resources.Load("Icons/Modules/ReSound"));
                    category = "Utility";
                    break;
                case "CozyHorizonModule":
                    content = new GUIContent(" Horizon Module", (Texture)Resources.Load("Icons/Modules/Horizon"));
                    category = "Atmosphere";
                    break;
                default:
                    content = new GUIContent(name);
                    break;
            }

            SearchTreeEntry entry = new SearchTreeEntry(content);
            entry.userData = type;
            entry.level = 2;
            return entry;

        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            // if ((Type)SearchTreeEntry.userData == typeof(CreateCozyModule))
            // {
            //     CreateCozyModuleWizard.ShowSetupWizard(cozyWeatherEditor);
            //     return true;
            // }
            weather?.InitializeModule((Type)SearchTreeEntry.userData);
            cozyWeatherEditor.ResetModuleEditors();
            cozyWeatherEditor.RepaintUI();
            return true;
        }


    }

    public class BiomeModulesSearchProvider : ScriptableObject, ISearchWindowProvider
    {

        public List<Type> modules;
        public CozyBiome biome;



        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Select a Module"), 0));
            foreach (Type i in modules)
            {

                entries.Add(GetSearchTreeEntry(i.Name, i));

            }
            return entries;
        }

        public SearchTreeEntry GetSearchTreeEntry(string name, Type type)
        {

            GUIContent content = new GUIContent();

            switch (name)
            {
                case "BlocksModule":
                    content = new GUIContent(" Blocks Extension", (Texture)Resources.Load("Blocks"));
                    break;
                case "CozyAmbienceModule":
                    content = new GUIContent(" Ambience Extension", (Texture)Resources.Load("Ambience Profile"));
                    break;
                case "CozyWeatherModule":
                    content = new GUIContent(" Weather Extension", (Texture)Resources.Load("Weather Profile-01"));
                    break;
                case "CozyClimateModule":
                    content = new GUIContent(" Climate Extension", (Texture)Resources.Load("Climate"));
                    break;
                case "CozyAtmosphereModule":
                    content = new GUIContent(" Atmosphere Extension", (Texture)Resources.Load("Atmosphere"));
                    break;
                case "ReSoundModule":
                    content = new GUIContent(" ReSound Extension", (Texture)Resources.Load("ReSound Icon"));
                    break;
                case "CozyEventModule":
                    content = new GUIContent(" Events Extension", (Texture)Resources.Load("Events"));
                    break;
                case "CozyTimeModule":
                    content = new GUIContent(" Time Extension", (Texture)Resources.Load("CozyCalendar"));
                    break;
                default:
                    content = new GUIContent(name);
                    break;
            }

            SearchTreeEntry entry = new SearchTreeEntry(content);
            entry.level = 1;
            entry.userData = type;
            return entry;

        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            biome?.InitializeModule((Type)SearchTreeEntry.userData);
            return true;
        }
    }

}
