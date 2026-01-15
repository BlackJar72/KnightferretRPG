using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{
    public class CozyProfileField<T> : VisualElement where T : CozyProfile
    {
        public static string path;

        public CozyProfileField(SerializedProperty property) 
        {

            AddToClassList("cozy-profile");

            ObjectField profileField = new ObjectField
            {
                objectType = typeof(T)
            };
            profileField.BindProperty(property);

            Button cloneProfile = new Button
            {
                name = "clone-profile",
                tooltip = "Clone Profile"
            };
            cloneProfile.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                path = EditorUtility.SaveFolderPanel("Duplicate Profile", path, "");

                if (string.IsNullOrEmpty(path))
                {
                    GUIUtility.ExitGUI();
                    return;
                }

                path = "Assets" + path.Substring(Application.dataPath.Length);

                if (typeof(ScriptableObject).IsAssignableFrom(typeof(T)))
                {

                    ScriptableObject newInstance = ScriptableObject.CreateInstance(typeof(T));

                    string duplication = JsonUtility.ToJson(property.objectReferenceValue);
                    JsonUtility.FromJsonOverwrite(duplication, newInstance);

                    path = $"{path}/{property.objectReferenceValue.name} Clone.asset";
                    AssetDatabase.CreateAsset(newInstance, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    property.objectReferenceValue = newInstance;
                    property.serializedObject.ApplyModifiedProperties();
                    property.serializedObject.Update();

                }
                else
                {
                    Debug.LogError("Type is not a scriptable object.");
                }

                GUIUtility.ExitGUI();

            });
            cloneProfile.Add(new Image
            {
                image = EditorGUIUtility.IconContent("d_SaveAs").image
            });


            Button newProfile = new Button
            {
                name = "new-profile",
                tooltip = "New Profile"
            };
            newProfile.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                path = EditorUtility.SaveFolderPanel("Create a New Profile", path, "");

                if (string.IsNullOrEmpty(path))
                {
                    GUIUtility.ExitGUI();
                    return;
                }

                path = "Assets" + path.Substring(Application.dataPath.Length);

                if (typeof(ScriptableObject).IsAssignableFrom(typeof(T)))
                {
                    ScriptableObject newInstance = ScriptableObject.CreateInstance(typeof(T));

                    path = $"{path}/New {typeof(T).Name}.asset";
                    AssetDatabase.CreateAsset(newInstance, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    property.objectReferenceValue = newInstance;
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError("Type is not a scriptable object.");
                }

                GUIUtility.ExitGUI();
            });
            newProfile.Add(new Image
            {
                image = EditorGUIUtility.IconContent("CreateAddNew").image
            });


            Add(profileField);
            Add(cloneProfile);
            Add(newProfile);

        }
    }

}