//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

// using System.Collections.Generic;
// using UnityEngine;
// using System.Collections;
// #if UNITY_EDITOR
// using UnityEditor;
// #endif



// namespace DistantLands.Cozy.Data
// {
//     [System.Serializable]
//     [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Custom FX", order = 361)]
//     public class CustomFXExample : FXProfile
//     {


//         public override void PlayEffect()
//         {

//             //This is called when the FX profile is played.

//         }

//         public override void PlayEffect(float intensity)
//         {

//             //This will play your effect at a certain intensity. Used for transitioning weather primarily.

//         }

//         public override void StopEffect()
//         {

//             //This is called when your effect is stopped.

//         }





//         public override bool InitializeEffect(VFXModule VFX)
//         {
//             if (VFX == null)
//             {
//                 if (CozyWeather.instance.vfxModule)
//                     VFX = CozyWeather.instance.vfxModule;
//                 else
//                     return false;
//             }
            
//             VFXMod = VFX;

//             if (!VFX.particleManager.isEnabled)
//             {

//                 return false;

//             }

//             //This ensures that the VFX module is setup correctly.

//             return true;

//         }

//         public override void DeinitializeEffect()
//         {

//         }

//     }

// #if UNITY_EDITOR
//     [CustomEditor(typeof(CustomFXExample))]
//     [CanEditMultipleObjects]
//     public class E_CustomFX : E_FXProfile
//     {


//         void OnEnable()
//         {

//         }

//         public override void OnInspectorGUI()
//         {
//             serializedObject.Update();

//             //Add your custom properties as property fields here:
//             //EditorGUILayout.PropertyField(serializedObject.FindProperty("myCustomProperty1"));
//             //EditorGUILayout.PropertyField(serializedObject.FindProperty("myCustomProperty2"));
//             //EditorGUILayout.PropertyField(serializedObject.FindProperty("myCustomProperty3"));

//             serializedObject.ApplyModifiedProperties();

//         }

//         public override void RenderInWindow(Rect pos)
//         {

//             //Render your properties inline in a weather or ambience profile like this:

//             // float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
//             // var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);
//             // var propPosB = new Rect(pos.x, pos.y + space * 2, pos.width, EditorGUIUtility.singleLineHeight);
//             // var propPosC = new Rect(pos.x, pos.y + space * 3, pos.width, EditorGUIUtility.singleLineHeight);

//             // serializedObject.Update();

//             // EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("myCustomProperty1"));
//             // EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("myCustomProperty2"));
//             // EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("myCustomProperty3"));

//             // serializedObject.ApplyModifiedProperties();
//         }

//         public override float GetLineHeight()
//         {
//             //This sets the number of lines a property will take.
//             return 3;

//         }

//     }
// #endif
// }