//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

#if UNITY_EDITOR
using UnityEditor;

namespace DistantLands.Cozy.EditorScripts
{
    public class EmptyShaderGUI : MaterialEditor
    {

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Material generated at runtime by COZY: Stylized Weather 3. Edit material properties within COZY's editor", MessageType.Info);
        }
    }
}
#endif