using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy
{
    public static class EditorUtilities
    {

        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);


            }

            return a;
#else
            return null;
#endif

        }

#if UNITY_EDITOR
        public static GUIStyle FoldoutStyle => new GUIStyle(EditorStyles.toolbarButton)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 12,
            padding = new RectOffset(15, 0, 0, 0),
            alignment = TextAnchor.MiddleLeft,
            margin = new RectOffset(5, 10, 5, 5),
            fixedHeight = 30,
            stretchWidth = true
            // normal = new GUIStyleState()
            // {
            //     scaledBackgrounds = EditorStyles.toolbarButton.onNormal.scaledBackgrounds
            // }

        };

#endif
        public static List<Type> ResetModuleList()
        {
            List<Type> listOfMods = (
          from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
          from type in domainAssembly.GetTypes()
          where typeof(CozyModule).IsAssignableFrom(type)
          select type).ToList();

            return listOfMods;

        }
        public static List<Type> ResetBiomeModulesList()
        {
            List<Type> listOfMods = (
          from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
          from type in domainAssembly.GetTypes()
          where typeof(CozyModule).IsAssignableFrom(type) && type.GetInterfaces().Any(i => i == typeof(ICozyBiomeModule))
          select type).ToList();

            return listOfMods;

        }


    }
}