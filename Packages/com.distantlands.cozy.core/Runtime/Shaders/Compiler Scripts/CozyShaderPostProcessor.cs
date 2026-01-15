//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace DistantLands.Cozy.ShaderUtility
{
    public class CozyShaderPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            RegisterShaders(importedAssets);

        }
        static void RegisterShaders(string[] paths)
        {
            foreach (var assetPath in paths)
            {
                if (!assetPath.EndsWith("cozyshader", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var mainObj = AssetDatabase.LoadMainAssetAtPath(assetPath) as Shader;

                if (mainObj != null)
                {
                    ShaderUtil.ClearShaderMessages(mainObj);
                    if (!ShaderUtil.ShaderHasError(mainObj))
                    {
                        ShaderUtil.RegisterShader(mainObj);
                    }
                }

                foreach (var obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath))
                {
                    if (obj is Shader)
                    {
                        Shader s = obj as Shader;
                        ShaderUtil.ClearShaderMessages(s);
                        if (!ShaderUtil.ShaderHasError(s))
                        {
                            ShaderUtil.RegisterShader((Shader)obj);
                        }
                    }
                }
            }
        }
    }
}
#endif