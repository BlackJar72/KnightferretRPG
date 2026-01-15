//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using DistantLands.Cozy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CozyBiome : CozySystem
    {

        public float transitionTime = 5;
        public float transitionDistance = 5;
        public Collider trigger;
        public List<ICozyBiomeModule> modules = new List<ICozyBiomeModule>();
        public CozyWeather weatherSphere
        {
            get
            {
                if (!cachedWeatherSphere)
                {
                    cachedWeatherSphere = CozyWeather.instance;
                }

                return cachedWeatherSphere;
            }
        }

        public CozyWeather cachedWeatherSphere;
        
        
        [Range(0, 1)]
        public float maxWeight = 1;
        public enum BiomeMode
        {
            Global,
            Local
        }
        public BiomeMode mode;
        public enum TransitionMode
        {
            Distance,
            Time
        }
        public TransitionMode transitionMode;

        // Start is called before the first frame update
        void Start()
        {
            if (Application.isPlaying)
            {
                weatherSphere.systems.Add(this);
            }
        }

        void Update()
        {
            if (modules.Count == 0)
                modules = GetComponents<ICozyBiomeModule>().ToList();

            if (mode == BiomeMode.Global)
            {
                targetWeight = maxWeight;
            }
            else
            {
                if (transitionMode == TransitionMode.Distance)
                    SetWeightByDistance();
                else
                    SetWeightByTime();
            }

            foreach (ICozyBiomeModule module in modules)
            {
                module.UpdateBiomeModule();
            }
        }

        public void SetWeightByDistance()
        {
            if (!weatherSphere)
                targetWeight = 0;


            if (mode == BiomeMode.Global)
            {
                targetWeight = maxWeight;
            }
            else
            {
                if (!trigger)
                {
                    targetWeight = 0; 
                    trigger = GetComponent<Collider>();
                }

                var weatherSpherePoint = weatherSphere.transform.position;
                var closestPoint = trigger.ClosestPoint(weatherSpherePoint);
                var distToClosestPoint = Vector3.Distance(weatherSpherePoint, closestPoint);

                targetWeight = distToClosestPoint <= transitionDistance
                    ? CozyUtilities.Remap(0, transitionDistance, maxWeight, 0, distToClosestPoint)
                    : 0f;
            }
        }
        
        public void SetWeightByTime()
        {
            if (!weatherSphere)
                targetWeight = 0;


            if (mode == BiomeMode.Global)
            {
                targetWeight = maxWeight;
            }
            else
            {
                if (!trigger)
                    targetWeight = 0;

                Vector3 closestPoint = trigger.ClosestPoint(weatherSphere.transform.position);

                if (transitionTime > 0)
                {
                    if (weatherSphere.transform.position == closestPoint)
                    {
                        targetWeight = Mathf.Clamp01(targetWeight + (1 / transitionTime * Time.deltaTime));
                    }
                    else
                        targetWeight = Mathf.Clamp01(targetWeight - (1 / transitionTime * Time.deltaTime));
                }
                else
                {
                    targetWeight = maxWeight;
                }
            }
        }

        public CozyModule GetModule(Type type)
        {

            foreach (ICozyBiomeModule j in modules)
                if (j.GetType() == type)
                    return (CozyModule)j;

            return null;

        }

        public T GetModule<T>() where T : CozyModule
        {

            Type type = typeof(T);

            foreach (CozyModule j in modules)
                if (j.GetType() == type)
                    return j as T;

            return null;

        }
        public void InitializeModule(Type module)
        {

            if (GetModule(module))
                return;

            ICozyBiomeModule mod = (ICozyBiomeModule)gameObject.AddComponent(module);
            if (!mod.CheckBiome())
            {
                DestroyImmediate((CozyModule)mod);
                return;
            }
            // ((CozyModule)mod).system = this;
            mod.AddBiome();
            modules.Add(mod);

        }
        public void RemoveModule(Type module)
        {

            if (!GetModule(module))
                return;

            ICozyBiomeModule mod = modules.Find(x => x.GetType() == module);
            mod.RemoveBiome();
            modules.Remove(mod);

        }

    }

// #if UNITY_EDITOR
//     [CanEditMultipleObjects]
//     [CustomEditor(typeof(CozyBiome))]
//     public class CozyBiomeEditor : Editor
//     {

//         protected static bool managementFoldout;
//         protected static bool weatherFoldout;
//         protected static bool biomeFoldout;
//         protected static bool boundsFoldout;
//         protected static bool infoFoldout;
//         public CozyBiome biome;
//         public CozyWeather weatherSphere;
//         public List<Type> mods;
//         public List<E_BiomeModule> editors = new List<E_BiomeModule>();
//         SerializedProperty mode;
//         SerializedProperty transitionMode;
//         SerializedProperty trigger;
//         SerializedProperty weight;
//         SerializedProperty priority;
//         SerializedProperty transitionDistance;
//         SerializedProperty transitionTime;


//         public void OnEnable()
//         {

//             biome = (CozyBiome)target;
//             weatherSphere = CozyWeather.instance;
//             mode = serializedObject.FindProperty("mode");
//             transitionMode = serializedObject.FindProperty("transitionMode");
//             trigger = serializedObject.FindProperty("trigger");
//             weight = serializedObject.FindProperty("maxWeight");
//             priority = serializedObject.FindProperty("priority");
//             transitionDistance = serializedObject.FindProperty("transitionDistance");
//             transitionTime = serializedObject.FindProperty("transitionTime");

//         }

//         public void CacheEditors()
//         {
//             editors.Clear();

//             foreach (ICozyBiomeModule module in biome.modules)
//                 editors.Add(CreateEditor((CozyModule)module) as E_BiomeModule);

//         }

//         public override void OnInspectorGUI()
//         {
//             serializedObject.Update();

//             if (editors.Count != biome.modules.Count)
//                 CacheEditors();

//             EditorGUI.indentLevel = 0;

//             managementFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(managementFoldout, "   Management Settings", EditorUtilities.FoldoutStyle);
//             EditorGUILayout.EndFoldoutHeaderGroup();

//             if (managementFoldout)
//             {

//                 EditorGUI.indentLevel++;
//                 EditorGUILayout.PropertyField(mode);
//                 EditorGUILayout.PropertyField(transitionMode);
//                 if (mode.intValue == 1)
//                 {
//                     if (!biome.trigger)
//                     {
//                         biome.trigger = biome.GetComponent<Collider>();
//                         EditorGUILayout.HelpBox("Local biomes require a collider! Please add a collider to this game object or manually assign a collider.", MessageType.Warning);
//                     }
//                     else if (!biome.trigger.isTrigger)
//                         EditorGUILayout.HelpBox("You will likely want to change your collider's trigger mode to on.", MessageType.Warning);

//                     EditorGUILayout.PropertyField(trigger);
//                     if (transitionMode.intValue == 0)
//                         EditorGUILayout.PropertyField(transitionDistance, new GUIContent("Transition Distance"));
//                     else
//                         EditorGUILayout.PropertyField(transitionTime, new GUIContent("Transition Time"));

//                 }
//                 EditorGUILayout.PropertyField(weight, new GUIContent("Weight"));
//                 EditorGUILayout.PropertyField(priority, new GUIContent("Priority"));

//                 EditorGUI.indentLevel--;

//             }

//             EditorGUI.indentLevel += 2;
//             foreach (E_BiomeModule module in editors)
//             {
//                 if (module == null) continue;
//                 ((E_CozyModule)module).DisplayToolar(false);
//                 module.DrawInlineBiomeUI();
//             }
//             EditorGUI.indentLevel -= 2;

//             infoFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(infoFoldout, "   Current Information", EditorUtilities.FoldoutStyle);
//             EditorGUILayout.EndFoldoutHeaderGroup();

//             if (infoFoldout)
//             {
//                 EditorGUI.indentLevel++;

//                 //                 EditorGUILayout.HelpBox("Currently it is " + Mathf.Round(biome.currentTemperature) + "F or " + Mathf.Round(biome.currentTemperatureCelsius) + "C with a precipitation chance of " + Mathf.Round(biome.currentPrecipitation) + "%.\n" +
//                 // "Temperatures will " + (biome.currentTemperature > biome.GetTemperature(false, weatherSphere.perennialProfile.ticksPerDay) ? "drop" : "rise") + " tomorrow, bringing the temprature to " + Mathf.Round(biome.GetTemperature(false, weatherSphere.perennialProfile.ticksPerDay)) + "F", MessageType.None);
//                 //                 EditorGUILayout.Space();


//                 // if (biome.currentForecast.Count == 0)
//                 // {
//                 //     EditorGUILayout.HelpBox("No forecast information yet!", MessageType.None);

//                 // }
//                 // else
//                 // {
//                 //     EditorGUILayout.HelpBox("Currently it is " + biome.weatherSphere.currentWeather.name, MessageType.None);


//                 //     for (int i = 0; i < biome.currentForecast.Count; i++)
//                 //     {

//                 //         EditorGUILayout.HelpBox("Starting at " + biome.weatherSphere.perennialProfile.FormatTime(false, biome.currentForecast[i].startTicks) + " the weather will change to " +
//                 //             biome.currentForecast[i].profile.name + " for " + Mathf.Round(biome.currentForecast[i].weatherProfileDuration) +
//                 //             " ticks or unitl " + biome.weatherSphere.perennialProfile.FormatTime(false, biome.currentForecast[i].endTicks) + ".", MessageType.None, true);

//                 //         EditorGUILayout.Space(2);

//                 //     }
//                 // }

//                 foreach (E_BiomeModule module in editors)
//                     if (module != null)
//                         module.DrawBiomeReports();

//                 EditorGUI.indentLevel--;

//             }


//             if (GUILayout.Button("Add Modules"))
//             {
//                 if (mods == null)
//                     mods = EditorUtilities.ResetBiomeModulesList();

//                 if (mods.Contains(typeof(ICozyBiomeModule)))
//                     mods.Remove(typeof(ICozyBiomeModule));

//                 if (mods.Contains(typeof(CozyBiomeModuleBase<>)))
//                     mods.Remove(typeof(CozyBiomeModuleBase<>));

//                 foreach (ICozyBiomeModule a in biome.modules)
//                     if (mods.Contains(a.GetType()))
//                         mods.Remove(a.GetType());

//                 // BiomeModulesSearchProvider provider = CreateInstance<BiomeModulesSearchProvider>();
//                 // provider.modules = mods;
//                 // provider.biome = biome;
//                 // SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);


//             }

//             serializedObject.ApplyModifiedProperties();


//         }
//     }
// #endif
}