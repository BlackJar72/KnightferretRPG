//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula


using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;


namespace DistantLands.Cozy
{

    public abstract class CozyModule : MonoBehaviour
    {

        [HideInInspector]
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

            set
            {
                cachedWeatherSphere = value;
            }
        }

        public CozyWeather cachedWeatherSphere;
        
        
        public CozySystem cachedSystem;
        
        public CozySystem system
        {
            get
            {
                if (!cachedSystem)
                {
                    if (GetComponent<CozyBiome>())
                        cachedSystem = GetComponent<CozyBiome>();
                    else
                        cachedSystem = weatherSphere;
                }

                return cachedSystem;
            }

            set
            {
                cachedSystem = value;
            }
        }
        
        public void OnEnable()
        {
            InitializeModule();
        }
        public virtual void InitializeModule()
        {

            if (!enabled)
                return;

            if (GetComponent<CozyWeather>())
            {
                GetComponent<CozyWeather>().InitializeModule(GetType());
                DestroyImmediate(this);
                return;

            }

            CozyWeather.OnFrameReset += FrameReset;
            CozyWeather.UpdateWeatherWeights += UpdateWeatherWeights;
            CozyWeather.UpdateFXWeights += UpdateFXWeights;
            CozyWeather.PropogateVariables += PropogateVariables;
            CozyWeather.CozyUpdateLoop += CozyUpdateLoop;

        }
        internal virtual bool CheckIfModuleCanBeRemoved(out string warning)
        {
            warning = "";
            return true;
        }
        internal virtual bool CheckIfModuleCanBeAdded(out string warning)
        {
            warning = "";
            return true;
        }
        public virtual void FrameReset()
        {

        }
        public virtual void UpdateWeatherWeights()
        {

        }
        public virtual void UpdateFXWeights()
        {

        }
        public virtual void PropogateVariables()
        {

        }
        public virtual void CozyUpdateLoop()
        {

        }
        
        public virtual void OnSceneLoaded()
        {
            
        }

        public virtual void OnSceneUnloaded()
        {
            
        }
        
        public virtual void SetupModule(Type[] requirements)
        {

            if (!enabled)
                return;

            foreach (Type type in requirements)
            {
                if (!weatherSphere.GetModule(type))
                {
                    weatherSphere.InitializeModule(type);
                    Debug.Log($"{GetType().Name} requires {type.Name} to function. {type.Name} has been automatically added to the weather sphere!");
                }
            }

        }
        public void OnDisable()
        {
            DeinitializeModule();
        }
        public virtual void DeinitializeModule()
        {
            if (GetComponent<CozyBiome>())
                return;

            CozyWeather.OnFrameReset -= FrameReset;
            CozyWeather.UpdateWeatherWeights -= UpdateWeatherWeights;
            CozyWeather.UpdateFXWeights -= UpdateFXWeights;
            CozyWeather.PropogateVariables -= PropogateVariables;
            CozyWeather.CozyUpdateLoop -= CozyUpdateLoop;
        }

    }

#if UNITY_EDITOR
    public class E_CozyModule : Editor
    {

        public virtual GUIContent GetGUIContent()
        {

            return new GUIContent();

        }
        public virtual void GetDebugInformation()
        {
            DisplayToolar(false);
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.NextVisible(true);

            while (true)
            {
                if (iterator == null)
                    break;

                EditorGUILayout.PropertyField(iterator, true);
                if (iterator.hasChildren)
                {
                    if (!iterator.NextVisible(false))
                        break;
                    continue;
                }

                if (!iterator.NextVisible(true))
                    break;

            }

            serializedObject.ApplyModifiedProperties();

        }

        public virtual void GetReportsInformation()
        {



        }
        public void RemoveModule()
        {
            CozyWeather.instance.DeintitializeModule((CozyModule)target);

        }

        public void ResetModule()
        {
            CozyWeather.instance.ResetModule((CozyModule)target);
        }
        public void EditScript()
        {
            MonoScript script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
            AssetDatabase.OpenAsset(script, 1);
        }

        public virtual void OpenContextMenu(Vector2 pos)
        {

            GenericMenu menu = new GenericMenu();
            // menu.AddSeparator("");
            menu.AddItem(new GUIContent("Remove Module"), false, RemoveModule);
            menu.AddItem(new GUIContent("Reset"), false, ResetModule);
            menu.AddItem(new GUIContent("Edit Script"), false, EditScript);

            menu.ShowAsContext();

        }

        public virtual void OpenDocumentationURL()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/");
        }
        public override void OnInspectorGUI()
        {

        }

        public void DisplayToolar(bool menus)
        {

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField(new GUIContent(GetGUIContent()));
            EditorGUILayout.Separator();

            if (menus)
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(EditorGUIUtility.IconContent("_Help"), EditorStyles.toolbarButton))
                {
                    OpenDocumentationURL();
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("_Menu"), EditorStyles.toolbarButton))
                {
                    OpenContextMenu(Event.current.mousePosition);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        public virtual void DisplayInCozyWindow()
        {
            serializedObject.Update();



            serializedObject.ApplyModifiedProperties();


        }

    }
#endif
}